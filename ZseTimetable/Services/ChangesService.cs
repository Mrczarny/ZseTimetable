using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private Timer? _timer = null;
        private ChangesAccess _db;
        private IEnumerable<TimetableServiceOption> TimetablesTypes;
        private ChangesScrapper _scrapper;
        private IConfiguration _config;
        private HttpClient _client;

        public ChangesService(ILogger<ChangesService> logger, IDataWrapper db, IConfiguration config, IHttpClientFactory client)
        {
            _logger = logger;
            _db = db.ChangesAccess;
            _config = config;
            _client = client.CreateClient("baseHttp");
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Changes Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(3));

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            await using (_scrapper = new ChangesScrapper(_config.GetSection(ScrapperOption.Position)
                             .GetSection("Changes")
                             .GetChildren().Select(x => x.Get<ScrapperOption>())))
            {
                await DatabaseUpload(_db.GetDBModels<ReplacementDB>(GetAllReplacements()));
            }
        }

        private async Task DatabaseUpload(IAsyncEnumerable<ReplacementDB> DbModels)
        {
            await foreach (var dbModel in DbModels)
            {
                var records = _db.GetByDate<ReplacementDB>(DateTime.Today);
                if (records != null)
                {
                    CreateMissing(records, dbModel);
                }
                else
                {
                    _db.Create(dbModel);
                }
            }
        }

        private void CreateMissing(IEnumerable<ReplacementDB> records, ReplacementDB dbModel)
        {

            foreach (var record in records)
            {
                if (record.LessonId == dbModel.LessonId &&  
                    record.TeacherId == dbModel.TeacherId &&
                    record.ClassroomId == dbModel.ClassroomId &&
                    record.LessonNumber == dbModel.LessonNumber)
                {
                    return;
                }
            }

            _db.Create(dbModel);
        }

        private async IAsyncEnumerable<IPersist> GetAllReplacements()
        {
            using (var rawChanges = await _client.GetStreamAsync($"https://zastepstwa.zse.bydgoszcz.pl"))
            {
                var spChanges = _scrapper.Scrap(await new StreamReader(rawChanges).ReadToEndAsync());
                foreach (var tReplacement in spChanges.Replacements)
                {
                    foreach (var lReplacement in tReplacement.ClassReplacements)
                    {
                        yield return lReplacement;
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Changes Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
