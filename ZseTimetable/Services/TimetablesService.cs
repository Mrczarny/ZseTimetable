using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TimetableLib.DataAccess;
using TimetableLib.DBAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.ScrapperModels;
using Azure;

namespace ZseTimetable.Services
{
    public class TimetablesService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger<TimetablesService> _logger;
        private Timer _timer;
        private TimetablesAccess _db;
        private TimetableScrapper _scrapper;
        private IEnumerable<TimetableServiceOption> TimetablesTypes;
        private readonly string baseAddress = "https://plan.zse.bydgoszcz.pl/plany";

        public TimetablesService(ILogger<TimetablesService> logger, IConfiguration config, TimetablesAccess db)
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

        
        private async void DatabaseUpload<T>(IAsyncEnumerable<T> dbModels) where T : class,ITimetables, new()
        {
            await foreach (var dbModel in dbModels)
            {
                try
                {
                    var record = _db.GetByName<T>(dbModel.Name);

                    if (record != null)
                    {
                        FillDbModel(record);
                        dbModel.TimetableId = record.TimetableId;
                        _db.Update((long)record.Id, dbModel);
                        _db.Update(record.TimetableId, dbModel.Timetable);
                        foreach (var DbDay in dbModel.Timetable.Days)
                        {
                            var day = record.Timetable.Days.Single(x => x.Day == DbDay.Day);
                            DbDay.TimetableId = record.TimetableId;
                            DbDay.Id = day.Id;
                            _db.Update((long)day.Id, DbDay);
                            foreach (var DbLesson in DbDay.Lessons)
                            {
                                var matchingLesson = day.Lessons?.FirstOrDefault(x => x.Number == DbLesson.Number
                                                                 && x.Name == DbLesson.Name && x.Group == DbLesson.Group);
                                if (matchingLesson != null)
                                {
                                    _db.Update((long)matchingLesson.Id, DbLesson);
                                    day.Lessons.Remove(matchingLesson); // TODO - Becouse of this one line TimetableDay.Lessons has to be list
                                }
                                else
                                {
                                    DbLesson.GetType().GetProperty(dbModel.GetType().Name[..^2] + "Name")
                                        .SetValue(DbLesson, dbModel.Name);
                                    DbLesson.GetType().GetProperty(dbModel.GetType().Name[..^2] + "Id")
                                        .SetValue(DbLesson, dbModel.Id); //TODO - too ambiguous, will couse crash someday
                                    CreateLesson(DbLesson, DbDay);
                                }

                                 
                            }

                            if (day.Lessons != null)
                                foreach (var lesson in day.Lessons)
                                {
                                    foreach (var dayLesson in _db.GetAll<TimetableDayLessonDB>()
                                                 .Where(x => x.LessonId == lesson.Id))
                                    {
                                        _db.Delete<TimetableDayLessonDB>((long) dayLesson.Id);
                                    }

                                    _db.Delete<LessonDB>((long) lesson.Id);
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
                                lesson.GetType().GetProperty(dbModel.GetType().Name[..^2] + "Name")
                                    .SetValue(lesson, dbModel.Name);
                                lesson.GetType().GetProperty(dbModel.GetType().Name[..^2] + "Id")
                                    .SetValue(lesson, dbModel.Id); //TODO - too ambiguous, will couse crash someday

                                CreateLesson(lesson, day);
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

        private void FillDbModel<T>(T record) where T : class, ITimetables, new()
        {
            record.Timetable = _db.Get<TimetableDB>(record.TimetableId);
            record.Timetable.Days =
                _db.GetAll<TimetableDayDB>()?.Where(x => x.TimetableId == record.Timetable.Id);
            foreach (var day in record.Timetable.Days)
            {
                var dayLessons = _db.GetAll<TimetableDayLessonDB>()?.Where(x => x.TimetableDayId == day.Id);
                var Lessons = _db.GetAll<LessonDB>();
                if (dayLessons != null && Lessons != null)
                {
                    day.Lessons = (from dayLessonDb in dayLessons
                                   join lessonDb in Lessons on dayLessonDb.LessonId equals
                                       lessonDb.Id //TODO - ! THIS QUERIES TWO ENTIRE TABLES ! 
                                   select lessonDb ).ToList(); 
                }
            }
        }

        private void CreateLesson(LessonDB lesson, TimetableDayDB day) // TODO - DRY :)
        {
            //var matchingDbModel = _db.GetByName<T>(lesson.ClassName);
            //if (matchingDbModel != null)
            //{
            //    FillDbModel(matchingDbModel);
            //    var matchingLesson = matchingDbModel.Timetable.Days
            //        .FirstOrDefault(x => x.Day == day.Day)?.Lessons
            //        .FirstOrDefault(x => x.Number == lesson.Number
            //                             && x.Name == lesson.Name && x.Group == lesson.Group);
            //    if (matchingLesson != null)
            //    {
            //        _db.Create(new TimetableDayLessonDB
            //        {
            //            LessonId = (long)matchingLesson.Id,
            //            TimetableDayId = (long)day.Id
            //        });
            //        return;
            //    }
            //}

            if (lesson.ClassName != null && lesson.ClassLink != null)
            {
                var matchingClass = _db.GetByName<ClassDB>(lesson.ClassName) ?? _db.GetByLink<ClassDB>(lesson.ClassLink) ;
                if (matchingClass != null)
                {
                    FillDbModel(matchingClass);
                    var matchingLesson = matchingClass.Timetable.Days?
                        .FirstOrDefault(x => x.Day == day.Day)?.Lessons?
                        .FirstOrDefault(x => x.Number == lesson.Number
                                             && x.Name == lesson.Name && x.Group == lesson.Group);
                    if (matchingLesson != null)
                    {
                        matchingLesson.ClassroomId ??= lesson.ClassroomId;
                        matchingLesson.TeacherId ??= lesson.TeacherId;
                        _db.Update((long)matchingLesson.Id, matchingLesson);
                        _db.Create(new TimetableDayLessonDB
                        {
                            LessonId = (long)matchingLesson.Id,
                            TimetableDayId = (long)day.Id
                        });
                        return;
                    }
                }
            }

            if (lesson.ClassroomName != null && lesson.ClassroomLink != null)
            {
                var matchingClassroom = _db.GetByName<ClassroomDB>(lesson.ClassroomName) ?? _db.GetByLink<ClassroomDB>(lesson.ClassroomLink);
                if (matchingClassroom != null)
                {
                    FillDbModel(matchingClassroom);
                    var matchingLesson = matchingClassroom.Timetable.Days?
                        .FirstOrDefault(x => x.Day == day.Day)?.Lessons?
                        .FirstOrDefault(x => x.Number == lesson.Number
                                             && x.Name == lesson.Name && x.Group == lesson.Group);
                    if (matchingLesson != null)
                    {
                        matchingLesson.ClassId ??= lesson.ClassId;
                        matchingLesson.TeacherId ??= lesson.TeacherId;
                        _db.Update((long)matchingLesson.Id, lesson);
                        _db.Create(new TimetableDayLessonDB
                        {
                            LessonId = (long)matchingLesson.Id,
                            TimetableDayId = (long)day.Id
                        });
                        return;
                    }
                }
            }

            if (lesson.TeacherName != null && lesson.TeachereLink != null)
            {
                var matchingTeacher = _db.GetByName<TeacherDB>(lesson.TeacherName) ?? _db.GetByLink<TeacherDB>(lesson.TeachereLink);
                if (matchingTeacher != null)
                {
                    FillDbModel(matchingTeacher);
                    var matchingLesson = matchingTeacher.Timetable.Days?
                        .FirstOrDefault(x => x.Day == day.Day)?.Lessons
                        .FirstOrDefault(x => x.Number == lesson.Number
                                             && x.Name == lesson.Name && x.Group == lesson.Group);
                    if (matchingLesson != null)
                    {
                        matchingLesson.ClassId ??= lesson.ClassId;
                        matchingLesson.ClassroomId ??= lesson.ClassroomId;
                        _db.Update((long)matchingLesson.Id, lesson);
                        _db.Create(new TimetableDayLessonDB
                        {
                            LessonId = (long)matchingLesson.Id,
                            TimetableDayId = (long)day.Id
                        });
                        return;
                    }
                }
            }

            var dayLesson = new TimetableDayLessonDB
            {
                LessonId = _db.Create(lesson),
                TimetableDayId = (long)day.Id
            };
            _db.Create(dayLesson);
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
                    var head = new HttpRequestMessage(HttpMethod.Head,
                        $"{baseAddress}/{letter}{id}.html")
                    {
                        Headers = { IfModifiedSince = DateTime.Now.Subtract(TimeSpan.FromDays(365)).ToUniversalTime()
                            }
                    };

                    HttpResponseMessage response;
                    try { response = await client.SendAsync(head); }
                    catch (HttpRequestException e) { break; }

                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        break;
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        yield return $"{baseAddress}/{letter}{id}.html";
                    }

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
