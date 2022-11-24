using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TimetableLib.DataAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.ScrapperModels;

namespace ZseTimetable.Services
{
    public class TimetablesService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimetablesService> _logger;
        private Timer _timer;
        private DataAccess _db;
        private TimetableScrapper _scrapper;
        private IEnumerable<TimetableServiceOption> TimetablesTypes;
        private readonly string baseAddress = "https://plan.zse.bydgoszcz.pl/plany";

        public TimetablesService(ILogger<TimetablesService> logger, IConfiguration config, DataAccess db)
        {
            _logger = logger;
            _scrapper = new TimetableScrapper(config.GetSection(ScrapperOption.Position)
                .GetSection("Timetable")
                .GetChildren().Select(x => x.Get<ScrapperOption>())
            );
            _db = db;
            TimetablesTypes = config.GetSection(ScrapperOption.Position).GetSection("Types").GetChildren()
                .Select(x => x.Get<TimetableServiceOption>());
        }
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Timetable Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {

            IEnumerable<Class> classes = await GetTimetable<Class>(TimetablesTypes.First(x => x.type == typeof(Class).Name).letter);
            IEnumerable<Classroom> classrooms = await GetTimetable<Classroom>(TimetablesTypes.First(x => x.type == typeof(Classroom).Name).letter);
            IEnumerable<Teacher> teachers = await GetTimetable<Teacher>(TimetablesTypes.First(x => x.type == typeof(Teacher).Name).letter);


            //scrapper models to db models
            var DbClasses = GetDBModels<ClassDB>(classes);
            var DbClassrooms = GetDBModels<ClassroomDB>(classrooms); //TODO - make DoWork better
            var DbTeacher = GetDBModels<TeacherDB>(teachers);

            //updates or creates record 
            DatabaseUpload(DbClasses);
            DatabaseUpload(DbClassrooms);
            DatabaseUpload(DbTeacher);
        }


        private async void DatabaseUpload<T>(IAsyncEnumerable<T> dbModels) where T : ITimetables, new()
        {
            await foreach (var dbModel in dbModels)
            {
                try
                {
                    var record = _db.GetByName<T>(dbModel.Name);

                    if (record != null)
                    {
                        record.Timetable = _db.Get<TimetableDB>(record.TimetableId);
                        record.Timetable.Days =
                            _db.GetAll<TimetableDayDB>().Where(x => x.TimetableId == record.Timetable.Id);
                        foreach (var day in record.Timetable.Days)
                        {
                            day.Lessons = from dayLessonDb in _db.GetAll<TimetableDayLessonDB>()
                                join lessonDb in _db.GetAll<LessonDB>() on dayLessonDb.LessonId equals lessonDb.Id //TODO - ! THIS QUERIES TWO ENTIRE TABLES ! 
                                select lessonDb;
                        }
                        _db.Update(record.TimetableId, dbModel.Timetable);
                        _db.Update((long)record.Id, dbModel);
                        foreach (var DbDay in dbModel.Timetable.Days)
                        {
                            var day = record.Timetable.Days.Single(x => x.Day == DbDay.Day);
                            _db.Update((long)day.Id, DbDay);
                            foreach (var DbLesson in DbDay.Lessons)
                            {
                                if (DbLesson.)
                                {
                                    if (lesson)
                                    {
                                    
                                    }
                                }
                                _db.Update((long)lesson.Id, lesson);
                            }
                        }
                    }
                    else
                    {
                        dbModel.TimetableId = _db.Create(dbModel.Timetable);
                        dbModel.Id = _db.Create(dbModel);
                        foreach (var day in dbModel.Timetable.Days)
                        {
                            day.TimetableId = dbModel.TimetableId;
                            day.Id = _db.Create(day);
                            foreach (var lesson in day.Lessons)
                            {
                                lesson.GetType().GetProperty(dbModel.GetType().Name[..^2] + "Id").SetValue(lesson, dbModel.Id); //TODO - too ambiguous, will couse crash someday
                                var lessonDay = new TimetableDayLessonDB
                                {
                                    LessonId = _db.Create(lesson),
                                    TimetableDayId = (long)day.Id
                                };
                                _db.Create(lessonDay);

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

        private async IAsyncEnumerable<T> GetDBModels<T>(IEnumerable<IScrappable> scrpModels) where T : class, IDBModel
        {
            foreach (var scrpModel in scrpModels)
            {
                yield return scrpModel.GetDBModel<T>();
            }
        }

        private async  Task<IEnumerable<T>> GetTimetable<T>(char letter) where T : IScrappable, new()
        {
            //gets all urls 
            var allUrls = GetAllUrls(letter);
            var allTimetables = new List<T>();
            //gets html files and uses scrapper to scrap them
            await foreach (var url in allUrls)
            {
                T returner = new T(); // ?????
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        var rawTimetable = await client.GetStreamAsync(url);
                        var html = await new StreamReader(rawTimetable).ReadToEndAsync();
                        returner.Timetable = await _scrapper.Scrap<T>(html);
                    }
                    catch (HttpRequestException e)
                    {
                        _logger.LogCritical($"{e.Message},{e.InnerException},{e.Source}");
                        throw;
                    } 
                }

                returner.Name = returner.Timetable.Title;
                allTimetables.Add(returner);
            }

            return allTimetables;
        }

        private async IAsyncEnumerable<string> GetAllUrls(char letter)
        {
            int  id = 1;
            while (true)
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        var head = new HttpRequestMessage(HttpMethod.Head,
                            $"{baseAddress}/{letter}{id}.html")
                            {Headers = { IfModifiedSince = DateTime.Now.Subtract(TimeSpan.FromDays(365)).ToUniversalTime()
                            }};
                        var response = await client.SendAsync(head);
                        if (!response.IsSuccessStatusCode)
                        {
                            break;
                        }
                    }
                    catch (HttpRequestException e)
                    {
                        break;
                    }
                    yield return $"{baseAddress}/{letter}{id}.html"; 
                }
                id++;
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }

}
