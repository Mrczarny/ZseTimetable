using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TimetableLib.DataAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.DTOs;

namespace ZseTimetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangesController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly ILogger<ChangesController> _logger;

        private readonly ChangesAccess _db;
        //private readonly ChangesScrapper _scrapper;

        public ChangesController(IConfiguration config, ILogger<ChangesController> logger, IHttpClientFactory client,
            IDataWrapper db)
        {
            _logger = logger;
            _client = client.CreateClient("baseHttp");
            _db = db.ChangesAccess;
            //_scrapper = new ChangesScrapper(config.GetSection(ScrapperOption.Position)
            //    .GetSection("Changes")
            //    .GetChildren().Select(x => x.Get<ScrapperOption>())
            //);
        }


        [HttpGet]
        [HttpGet("Week")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<ReplacementDTO>>> GetWeekChanges()
        {
            try
            {
                var ts = new TimeSpan((int) DateTime.Today.DayOfWeek, 0,0,0);
                var weekStartDay = DateTime.Today.Subtract(ts);
                var replacements = new List<ReplacementDTO>();
                for (int i = 0; i < 7; i++)
                {
                    var dbReplacements = _db.GetByDate<ReplacementDB>(weekStartDay.AddDays(i));

                    foreach (var rp in dbReplacements)
                    {
                        var lesson = _db.Get<LessonDB>((long)rp.LessonId);
                        lesson.ClassName = _db.GetNameById<ClassDB>((long)lesson.ClassId);
                        lesson.ClassroomName = _db.GetNameById<ClassroomDB>((long)lesson.ClassroomId);
                        lesson.TeacherName = _db.GetNameById<TeacherDB>((long)lesson.TeacherId);

                        replacements.Add(new ReplacementDTO(rp, lesson) { ReplacementDate = weekStartDay.AddDays(i) });
                    }
                }

                return replacements;
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return Problem("Sorry, we seem to have a problem.");
            }
        }

        [HttpGet]
        [HttpGet("Today")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<ReplacementDTO>>> GetChanges()
        {
            try
            {
                var dbReplacements = _db.GetByDate<ReplacementDB>(DateTime.Today);
                var replacements = new List<ReplacementDTO>();
                foreach (var rp in dbReplacements)
                {
                    var lesson = _db.Get<LessonDB>((long)rp.LessonId);
                    lesson.ClassName = _db.GetNameById<ClassDB>((long)lesson.ClassId);
                    lesson.ClassroomName = _db.GetNameById<ClassroomDB>((long)lesson.ClassroomId);
                    lesson.TeacherName = _db.GetNameById<TeacherDB>((long)lesson.TeacherId);

                    replacements.Add(new ReplacementDTO(rp, lesson) { ReplacementDate = DateTime.Now });
                }

                return replacements;
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return Problem("Sorry, we seem to have a problem.");
            }
        }

        [HttpGet("{date:datetime}")]
        public async Task<ActionResult<IEnumerable<ReplacementDTO>>> GetReplacementByDate(DateTime date)
        {
            try
            {
                var dbReplacements = _db.GetByDate<ReplacementDB>(date);
                var replacements = new List<ReplacementDTO>();
                foreach (var rp in dbReplacements)
                {
                    var lesson = _db.Get<LessonDB>((long) rp.LessonId);
                    lesson.ClassName = _db.GetNameById<ClassDB>((long)lesson.ClassId);
                    lesson.ClassroomName = _db.GetNameById<ClassroomDB>((long) lesson.ClassroomId);
                    lesson.TeacherName = _db.GetNameById<TeacherDB>((long) lesson.TeacherId);
                    replacements.Add(new ReplacementDTO(rp, lesson) {ReplacementDate = date});
                }


                return replacements;
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return Problem("Sorry, we seem to have a problem.");
            }
        }
    }
}