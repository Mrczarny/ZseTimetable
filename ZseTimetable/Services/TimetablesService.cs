using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TimetableLib.DataAccess;
using TimetableLib.DBAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.ScrapperModels;

namespace ZseTimetable.Services
{
    public class TimetablesService : IHostedService, IDisposable
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TimetablesService> _logger;
        private readonly DateTime _modifiedSince;
        private readonly string baseName = "plany";
        private readonly HttpClient _client;
        private readonly TimetablesAccess _db;
        private TimetableScrapper _scrapper;
        private Timer _timer;
        private readonly IEnumerable<TimetableServiceOption> TimetablesTypes;

        public TimetablesService(ILogger<TimetablesService> logger, IConfiguration config, IDataWrapper db,
            IHttpClientFactory client)
        {
            _logger = logger;
            //_scrapper = new TimetableScrapper(config.GetSection(ScrapperOption.Position)
            //    .GetSection("Timetable")
            //    .GetChildren().Select(x => x.Get<ScrapperOption>())
            //);
            _config = config;
            _db = db.TimetablesAccess;
            TimetablesTypes = config.GetSection(ScrapperOption.Position).GetSection("Types").GetChildren()
                .Select(x => x.Get<TimetableServiceOption>());
            _client = client.CreateClient("baseHttp");
            var year = DateTime.Now.Month > 7 ? DateTime.Now.Year : DateTime.Now.Year - 1;
            _modifiedSince = DateTime.Parse($"01.08.{year}");
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Timetable Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            _logger.LogInformation("Starting timetables upload...");
            //_db.BeginTransaction("Timetables");
            try
            {
                await using (_scrapper = new TimetableScrapper(_config.GetSection(ScrapperOption.Position)
                                 .GetSection("Timetable")
                                 .GetChildren().Select(x => x.Get<ScrapperOption>())))
                {
                    await DatabaseUpload(
                        _db.GetDBModels<ClassDB>(
                            GetTimetable<Class>(
                                TimetablesTypes.First(x => x.type == typeof(Class).Name).letter)));

                    await DatabaseUpload(
                        _db.GetDBModels<ClassroomDB>(
                            GetTimetable<Classroom>(
                                TimetablesTypes.First(x => x.type == typeof(Classroom).Name).letter)));

                    await DatabaseUpload(
                        _db.GetDBModels<TeacherDB>(
                            GetTimetable<Teacher>(
                                TimetablesTypes.First(x => x.type == typeof(Teacher).Name).letter)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //_db.CancelTransaction("Timetables");
                throw;
            }

            _logger.LogInformation("Timetables upload completed!");
            // _db.EndTransaction("Timetables");
        }

        private async Task DatabaseUpload<T>(IAsyncEnumerable<T> dbModels) where T : class, ITimetables, new()
        {
            await foreach (var dbModel in dbModels)
                try
                {
                    var record = _db.GetByName<T>(dbModel.Name);

                    if (record != null)
                    {
                        _logger.LogInformation($"Updating {dbModel.GetType().Name}: {dbModel.Name}...");
                        try
                        {
                            _db.FillITimetablesModel(record);
                            dbModel.TimetableId = record.TimetableId;
                            _db.Update((long) record.Id, dbModel);
                            _db.Update(record.TimetableId, dbModel.Timetable);
                            foreach (var DbDay in dbModel.Timetable.Days)
                            {
                                var day = record.Timetable.Days.Single(x => x.Day == DbDay.Day);
                                DbDay.TimetableId = record.TimetableId;
                                DbDay.Id = day.Id;
                                _db.Update((long) day.Id, DbDay);
                                foreach (var DbLesson in DbDay.Lessons)
                                {
                                    var matchingLesson = day.Lessons?.FirstOrDefault(x =>
                                        x.Number == DbLesson.Number && x.Group == DbLesson.Group);
                                    if (matchingLesson != null)
                                    {
                                        dbModel.SetLessonId(DbLesson);
                                        dbModel.SetLessonName(DbLesson);
                                        //_db.Update((long)matchingLesson.Id, DbLesson);
                                        UpdateLesson((long) matchingLesson.Id, DbLesson);
                                        day.Lessons.Remove(
                                            matchingLesson); //Because of this one line TimetableDay.Lessons has to be list
                                    }
                                    else
                                    {
                                        dbModel.SetLessonId(DbLesson);
                                        dbModel.SetLessonName(DbLesson);

                                        CreateLesson(DbLesson, DbDay);
                                    }
                                }

                                if (day.Lessons != null)
                                    foreach (var lesson in day.Lessons)
                                    {
                                        foreach (var dayLesson in _db.GetAll<TimetableDayLessonDB>()
                                                     .Where(x => x.LessonId == lesson.Id))
                                            _db.Delete<TimetableDayLessonDB>((long) dayLesson.Id);

                                        _db.Delete<LessonDB>((long) lesson.Id);
                                    }
                            }

                            _logger.LogInformation($"{dbModel.GetType().Name}: {dbModel.Name} updated!");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e,
                                $"Error while trying to update {dbModel.GetType().Name}: {dbModel.Name}");
                            if (record.TimetableId != null)
                            {
                                foreach (var day in _db.GetTDaysByTId(record.TimetableId))
                                {
                                    foreach (var lesson in _db.GetLessonsByTDayId((long)day.Id))
                                    {
                                        _db.Delete<LessonDB>((long)lesson.Id);

                                }
                                    _db.Delete<TimetableDayDB>((long)day.Id);
                                }
                                _db.Delete<TimetableDB>(record.TimetableId);
                                _db.Delete<T>((long) record.Id);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"Creating {dbModel.GetType().Name}: {dbModel.Name}...");
                        try
                        {
                            dbModel.TimetableId = _db.Create(dbModel.Timetable);
                            dbModel.Id = _db.Create(dbModel);
                            foreach (var day in dbModel.Timetable.Days)
                            {
                                day.TimetableId = dbModel.TimetableId;
                                day.Id = _db.Create(day);
                                foreach (var lesson in day.Lessons)
                                {
                                    dbModel.SetLessonId(lesson);
                                    dbModel.SetLessonName(lesson);

                                    //lesson.GetType().GetProperty(dbModel.GetType().Name[..^2] + "Name")
                                    //    .SetValue(lesson, dbModel.Name);
                                    //lesson.GetType().GetProperty(dbModel.GetType().Name[..^2] + "Id")
                                    //    .SetValue(lesson, dbModel.Id); 
                                    CreateLesson(lesson, day);
                                }
                            }

                            _logger.LogInformation($"{dbModel.GetType().Name}: {dbModel.Name} created!");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e,
                                $"Error while trying to create {dbModel.GetType().Name}: {dbModel.Name}");
                            if (dbModel.TimetableId != null)
                            {
                                foreach (var day in _db.GetTDaysByTId(dbModel.TimetableId))
                                {
                                    foreach (var lesson in _db.GetLessonsByTDayId((long) day.Id))
                                        _db.Delete<LessonDB>((long) lesson.Id);
                                    _db.Delete<TimetableDayDB>((long) day.Id);
                                }

                                }
                                    _db.Delete<TimetableDayDB>((long)day.Id);
                                }
                                _db.Delete<TimetableDB>(dbModel.TimetableId);
                                //_db.Delete<T>((long) dbModel.Id);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);

                    throw;
                }
        }
        }

        private void UpdateLesson(long oldLessonId, LessonDB newLesson)
        {
            var matchingClassLesson = FindAndFillLesson<ClassDB>(newLesson);
            if (matchingClassLesson != null)
            {
                _db.Update((long) matchingClassLesson.Id, matchingClassLesson);
                //_db.Create(new TimetableDayLessonDB
                //{
                //    LessonId = (long)matchingClassLesson.Id,
                //    TimetableDayId = (long)day.Id
                //});
                return;
            }

            var matchingClassroomLesson = FindAndFillLesson<ClassroomDB>(newLesson);
            if (matchingClassroomLesson != null)
            {
                _db.Update((long) matchingClassroomLesson.Id, matchingClassroomLesson);
                //_db.Create(new TimetableDayLessonDB
                //{
                //    LessonId = (long)matchingClassroomLesson.Id,
                //    TimetableDayId = (long)day.Id
                //});
                return;
            }

            var matchingTeacherLesson = FindAndFillLesson<TeacherDB>(newLesson);
            if (matchingTeacherLesson != null)
            {
                _db.Update((long) matchingTeacherLesson.Id, matchingTeacherLesson);
                //_db.Create(new TimetableDayLessonDB
                //{
                //    LessonId = (long)matchingTeacherLesson.Id,
                //    TimetableDayId = (long)day.Id
                //});
            }
        }

        private LessonDB FindAndFillLesson<T>(LessonDB lesson) where T : class, ITimetables, new()
        {
            var t = new T();
            if (t.GetLessonName(lesson) != null && t.GetLessonLink(lesson) != null)
            {
                var matchingClass = _db.GetByLink<T>(t.GetLessonLink(lesson)) ??
                                    _db.GetByName<T>(t.GetLessonName(lesson));
                if (matchingClass != null)
                {
                    _db.FillITimetablesModel(matchingClass);
                    var matchingLesson = FindMatchingLesson(matchingClass.Timetable.Days, lesson);
                    if (matchingLesson != null)
                    {
                        matchingLesson.ClassId ??= lesson.ClassId;
                        matchingLesson.ClassroomId ??= lesson.ClassroomId;
                        matchingLesson.TeacherId ??= lesson.TeacherId;
                        return matchingLesson;
                    }
                }
            }

            return null;
        }

        private void CreateLesson(LessonDB lesson, TimetableDayDB day) // TODO - DRY :)
        {
            var matchingClassLesson = FindAndFillLesson<ClassDB>(lesson);
            if (matchingClassLesson != null)
            {
                _db.Update((long) matchingClassLesson.Id, matchingClassLesson);
                _db.Create(new TimetableDayLessonDB
                {
                    LessonId = (long) matchingClassLesson.Id,
                    TimetableDayId = (long) day.Id
                });
                return;
            }

            var matchingClassroomLesson = FindAndFillLesson<ClassroomDB>(lesson);
            if (matchingClassroomLesson != null)
            {
                _db.Update((long) matchingClassroomLesson.Id, matchingClassroomLesson);
                _db.Create(new TimetableDayLessonDB
                {
                    LessonId = (long) matchingClassroomLesson.Id,
                    TimetableDayId = (long) day.Id
                });
                return;
            }

            var matchingTeacherLesson = FindAndFillLesson<TeacherDB>(lesson);
            if (matchingTeacherLesson != null)
            {
                _db.Update((long) matchingTeacherLesson.Id, matchingTeacherLesson);
                _db.Create(new TimetableDayLessonDB
                {
                    LessonId = (long) matchingTeacherLesson.Id,
                    TimetableDayId = (long) day.Id
                });
                return;
            }


            var dayLesson = new TimetableDayLessonDB
            {
                LessonId = _db.Create(lesson),
                TimetableDayId = (long) day.Id
            };
            _db.Create(dayLesson);
        }

        private LessonDB FindMatchingLesson(IEnumerable<TimetableDayDB> timetableDays, LessonDB lesson)
        {
            foreach (var day in timetableDays)
            {
                var match = day.Lessons?.Find(x => x.Number == lesson.Number
                                                   && x.Name == lesson.Name && x.Group == lesson.Group);
                if (match != null)
                {
                    return match;
            }

            return null;
        }

        private async IAsyncEnumerable<T> GetTimetable<T>(char letter) where T : IScrappable, new()
        {
            //gets all urls 
            var allUrls = GetAllUrls(letter);
            //gets html files and uses scrapper to scrap them
            await foreach (var url in allUrls)
            {
                var returner = new T(); // ?????
                returner.Link = url[(_client.BaseAddress.AbsoluteUri.Length + baseName.Length + 1)..];
                try
                {
                    using (var rawTimetable = await _client.GetStreamAsync(url))
                    {
                        var html = await new StreamReader(rawTimetable).ReadToEndAsync();
                        returner.Timetable = await _scrapper.Scrap<T>(html);
                    }
                }
                catch (HttpRequestException e)
                {
                    _logger.LogCritical($"{e.Message},{e.InnerException},{e.Source}");
                    throw;
                }

                returner.Name = returner.Timetable.Title;
                yield return returner;
            }
        }

        private async IAsyncEnumerable<string> GetAllUrls(char letter)
        {
            var id = 1;
            while (true)
            {
                var head = new HttpRequestMessage(HttpMethod.Head,
                    $"{_client.BaseAddress}{baseName}/{letter}{id}.html")
                {
                    Headers =
                    {
                        IfModifiedSince = _modifiedSince
                    }
                };

                HttpResponseMessage response;
                try
                {
                    response = await _client.SendAsync(head);
                }
                catch (HttpRequestException e)
                {
                    break;
                }

                if (response.StatusCode == HttpStatusCode.NotFound) break;
                if (response.IsSuccessStatusCode) yield return $"{_client.BaseAddress}{baseName}/{letter}{id}.html";

                id++;
            }
        }
    }
}