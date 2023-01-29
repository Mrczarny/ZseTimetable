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
using TimetableLib.DataAccess;
using TimetableLib.Models.DBModels;
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
                await DatabaseUpload(_db.GetDBModels<ReplacementDB>(GetAllReplacements()));
            }

            _logger.LogInformation("Replacements upload completed!");
        }

        private async Task DatabaseUpload(IAsyncEnumerable<ReplacementDB> DbModels)
        {
            await foreach (var dbModel in DbModels)
            {
                var records = _db.GetByDate<ReplacementDB>(DateTime.Today);
                if (records != null)
                {
                    _logger.LogInformation($"Updating replacement of {dbModel.TeacherName} for {dbModel.ClassName}...");
                    CreateMissing(records, dbModel);
                }
                else
                {
                    FillNewReplacement(dbModel);
                    if (dbModel.LessonId != null)
                    {
                        _logger.LogInformation(
                            $"Creating replacement of {dbModel.TeacherName} for {dbModel.ClassName}...");
                        _db.Create(dbModel);
                    }
                }
            }
        }

        private void CreateMissing(IEnumerable<ReplacementDB> records, ReplacementDB dbModel)
        {
            foreach (var record in records)
                if (record.LessonId == dbModel.LessonId)
                {
                    try
                    {
                        _db.Update((long) record.Id, dbModel);
                    }
                    catch (Exception e)
                    {
                    }

                    return;
                }

            try
            {
                FillNewReplacement(dbModel);
                if (dbModel.LessonId != null) _db.Create(dbModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Error while trying to create replacement of {dbModel.TeacherName} for {dbModel.ClassName}!");
            }
        }

        private void FillNewReplacement(ReplacementDB rp)
        {
            rp.ClassId = _db.GetByName<ClassDB>(rp.ClassName)?.Id;
            rp.ClassroomId = _db.GetByName<ClassroomDB>(rp.ClassroomName)?.Id;
            rp.TeacherId = _db.GetByName<TeacherDB>($"{rp.TeacherName[0]}.{rp.TeacherName.Split(' ').Last()}")?.Id;
            if (rp.TeacherId != null)
                rp.LessonId = _db.GetLessonId((long) rp.TeacherId, rp.LessonNumber, DateTime.Today.DayOfWeek);
        }

        private async IAsyncEnumerable<IPersist> GetAllReplacements()
        {
            using (var rawChanges = await _client.GetStreamAsync("https://zastepstwa.zse.bydgoszcz.pl"))
            {
                var spChanges =
                    _scrapper.Scrap(await new StreamReader(rawChanges, Encoding.GetEncoding("iso-8859-2"))
                        .ReadToEndAsync());

                foreach (var tReplacement in spChanges.Replacements)
                foreach (var lReplacement in tReplacement.ClassReplacements)
                    yield return lReplacement;
            }
        }
    }
}