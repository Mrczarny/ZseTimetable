using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TimetableLib.Changes;
using TimetableLib.DataAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.Replacements;
using TimetableLib.Models.ScrapperModels;

namespace ZseTimetable.Services
{
    public class ChangesService : IHostedService, IDisposable
    {
        private readonly ILogger<ChangesService> _logger;
        private readonly HttpClient _client;
        private readonly IConfiguration _config;
        private readonly ChangesAccess _db;
        private ChangesScrapper _scrapper;
        private Timer? _timer;
        private IEnumerable<TimetableServiceOption> TimetablesTypes;

        public ChangesService(ILogger<ChangesService> logger, IDataWrapper db, IConfiguration config,
            IHttpClientFactory client)
        {
            _logger = logger;
            _db = db.ChangesAccess;
            _config = config;
            _client = client.CreateClient("baseHttp");
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Changes Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(3));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Changes Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation("Starting replacements upload...");
            await using (_scrapper = new ChangesScrapper(_config.GetSection(ScrapperOption.Position)
                             .GetSection("Changes")
                             .GetChildren().Select(x => x.Get<ScrapperOption>())))
            {
                await DatabaseUpload(
                    _db.GetDBModels<ReplacementDB>(
                        GetAllReplacements()));
            }

            _logger.LogInformation("Replacements upload completed!");
        }

        private async Task DatabaseUpload(IAsyncEnumerable<ReplacementDB> DbModels)
        {

            await foreach (var dbModel in DbModels)
            {
                var records = _db.GetByDate<ReplacementDB>(dbModel.Date);
                if (records.Any())
                {
                    _logger.LogInformation($"Updating replacement of {dbModel.OgTeacherName} for {dbModel.ClassName}...");
                    CreateMissing(records, dbModel);
                    continue;
                }


                FillNewReplacement(dbModel);
                if (dbModel.LessonId != null)
                {
                    _logger.LogInformation(
                        $"Creating replacement of {dbModel.OgTeacherName} for {dbModel.ClassName}...");
                    _db.Create(dbModel);
                }

            }



        }

        private void CreateMissing(IEnumerable<ReplacementDB> records, ReplacementDB dbModel)
        {
            foreach (var record in records)
            {
                FillNewReplacement(dbModel);
                if (record.LessonId == dbModel.LessonId)
                {
                    try
                    {
                        _db.Update((long) record.Id, dbModel);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"Error while trying to update replacement of {dbModel.OgTeacherName} for {dbModel.ClassName}!");
                    }

                    return;
                }
            }

            try
            {
                FillNewReplacement(dbModel);
                if (dbModel.LessonId != null)
                {
                    _db.Create(dbModel);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Error while trying to create replacement of {dbModel.OgTeacherName} for {dbModel.ClassName}!");
            }
        }

        private void FillNewReplacement(ReplacementDB rp)
        {
            rp.ClassId = _db.GetByName<ClassDB>(rp.ClassName)?.Id;
            rp.ClassroomId = _db.GetByName<ClassroomDB>(rp.ClassroomName)?.Id;
            rp.TeacherId = _db.GetByName<TeacherDB>($"{rp.OgTeacherName[0]}.{rp.OgTeacherName.Split(' ').Last()}")?.Id;
            if (rp.TeacherId != null)
                rp.LessonId = _db.GetLessonId<TeacherDB>((long) rp.TeacherId, rp.LessonNumber, rp.Date.DayOfWeek);
            if (rp.LessonId == null && rp.ClassId != null)
                rp.LessonId = _db.GetLessonId<ClassDB>((long)rp.ClassId, rp.LessonNumber, rp.Date.DayOfWeek, rp.Group);
            //if (rp.LessonId == null && rp.ClassroomId != null)
            //    rp.LessonId = _db.GetLessonId<TeacherDB>((long)rp.ClassroomId, rp.LessonNumber, rp.Date.DayOfWeek);

        }

        private async IAsyncEnumerable<IPersist> GetAllReplacements()
        {
            DayReplacements spChanges ;
            try
            {
                await using var rawChanges = await _client.GetStreamAsync("https://zastepstwa.zse.bydgoszcz.pl");
                spChanges =
                    _scrapper.Scrap(await new StreamReader(rawChanges, Encoding.GetEncoding("iso-8859-2"))
                        .ReadToEndAsync());
            }
            catch (Exception e)
            {
                spChanges = new DayReplacements
                {
                    Replacements = Enumerable.Empty<TeacherReplacements>()
                };
                _logger.LogError(e,
                    $"Error while trying to get replacements from {"https://zastepstwa.zse.bydgoszcz.pl"}");

            }
            foreach (var tReplacement in spChanges.Replacements)
            foreach (var lReplacement in tReplacement.ClassReplacements)
            {
                lReplacement.DayOfReplacement ??= spChanges.Date.Value;
                yield return lReplacement;
            }


        }
    }
}