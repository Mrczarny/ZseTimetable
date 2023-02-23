using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TimetableLib.DataAccess;
using TimetableLib.DBAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.DTOs;

namespace ZseTimetable.Controllers
{
    /// <summary>
    ///     TimetableController is responsible for handling all requests about timetable
    ///     Default kind of response format is json
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly TimetablesAccess _db;

        private readonly ILogger<TimetableController> _logger;
        //private readonly TimetableScrapper _scrapper;

        public TimetableController(IConfiguration config, ILogger<TimetableController> logger, IDataWrapper db,
            IHttpClientFactory client)
        {
            _logger = logger;
            _client = client.CreateClient();
            _db = db.TimetablesAccess;
            //_scrapper = new TimetableScrapper(config.GetSection(ScrapperOption.Position)
            //    .GetSection("Timetable")
            //    .GetChildren().Select(x => x.Get<ScrapperOption>())
            //);
        }

        //[HttpGet("class/{id:long}")]
        //[Produces(MediaTypeNames.Application.Json)]
        //public async Task<ActionResult<ClassDTO>> GetClassTimetableAsync(long id = 1)
        //{
        //    _logger.LogInformation($"Getting class of id: {id}");
        //    try
        //    {
        //        //Get latest populated(!) class from db
        //        var classLs = _db.Get<ClassDB>(id);
        //        if (classLs != null)
        //        {
        //            _db.FillITimetablesModel(classLs);
        //            //returns DTO of timetable
        //            return new ClassDTO(classLs);
        //        }


        //        //classLs.Timetable.Days = _db.Get<ReplacementDB>(classLs.Id); //adds all needed replacements


        //        return NotFound("Class not found");
        //    }
        //    catch (HttpRequestException exception)
        //    {
        //        return Problem(exception.Message);
        //    }
        //}


        [HttpGet("class/{name}")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ClassDTO>> GetClassTimetableAsync(string name = "1B")
        {
            _logger.LogInformation($"Getting class {name}...");
            try
            {
                //Get latest populated(!) class from db
                var classLs = _db.GetByName<ClassDB>(name);
                if (classLs != null)
                {
                    _db.FillITimetablesModel(classLs);

                    foreach (var day in classLs.Timetable.Days)
                    {
                        foreach ( var dayLesson in day.Lessons)
                        {
                            if (dayLesson.ClassroomId != null && dayLesson.TeacherId != null)
                            {
                                var Classroom = _db.Get<ClassroomDB>((long) dayLesson.ClassroomId);
                                _db.FillITimetablesModel(Classroom);
                                var Teacher = _db.Get<TeacherDB>((long) dayLesson.TeacherId);
                                _db.FillITimetablesModel(Teacher);
                                dayLesson.ClassroomName = Classroom.Name;
                                dayLesson.TeacherName = Teacher.Name;
                                dayLesson.ClassName = classLs.Name;
                            }
                        }
                    }

                    //var replacements =
                    //    _client.GetAsync($"{Request.Scheme}://{Request.Host}/Changes/Today");
                    //if (replacements.IsCompletedSuccessfully && replacements.Result.IsSuccessStatusCode)
                    //{

                    //}
                    //returns DTO of timetable
                    return new ClassDTO(classLs); ;
                }


                //classLs.Timetable.Days = _db.Get<ReplacementDB>(classLs.Id); //adds all needed replacements
                _logger.LogInformation($"class {name} not found!");
                return NotFound("Class not found");
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return Problem("Sorry, we seem to have a problem.");
            }
        }

        [HttpGet("class/allNames")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<List<string>>> GetAllClassNames()
        {
            try
            {
                return _db.GetAll<ClassDB>().Select(x => x.Name).ToList();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Network error getting all names of classes!");
                return Problem("Sorry, we seem to have a problem.");
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return Problem("Sorry, we seem to have a problem.");
            }
        }

        [HttpGet("classroom/{name}")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ClassroomDTO>> GetClassroomTimetableAsync(string name = "1")
        {
            _logger.LogInformation($"Getting class {name}...");
            try
            {
                //Get latest populated(!) classroom from db
                var classroomLs = _db.GetByName<ClassroomDB>(name);
                if (classroomLs != null)
                {
                    _db.FillITimetablesModel(classroomLs);

                    foreach (var day in classroomLs.Timetable.Days)
                    {
                        foreach (var dayLesson in day.Lessons)
                        {
                            if (dayLesson.ClassroomId != null && dayLesson.TeacherId != null)
                            {
                                    var Class = _db.Get<ClassDB>((long)dayLesson.ClassId);
                                _db.FillITimetablesModel(Class);
                                    var Teacher = _db.Get<TeacherDB>((long)dayLesson.TeacherId);
                                _db.FillITimetablesModel(Teacher);
                                dayLesson.ClassName = Class.Name;
                                dayLesson.TeacherName = Teacher.Name;
                                dayLesson.ClassroomName = classroomLs.Name;
                            }
                        }
                    }
                    //returns DTO of timetable
                    return new ClassroomDTO(classroomLs); ;
                }


                //classLs.Timetable.Days = _db.Get<ReplacementDB>(classLs.Id); //adds all needed replacements

                return NotFound("Classroom not found");
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return Problem("Sorry, we seem to have a problem.");
            }
        }

        [HttpGet("classroom/allNames")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<List<string>>> GetAllClassroomNames()
        {
            try
            {
                return _db.GetAll<ClassroomDB>().Select(x => x.Name).ToList();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Network error getting all names of classrooms!");
                return Problem("Sorry, we seem to have a problem.");
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return Problem("Sorry, we seem to have a problem.");
            }
        }

        [HttpGet("teacher/{name}")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<TeacherDTO>> GetTeacherTimetableAsync(string name = "P.Siwka")
        {
            try
            {
                //Get latest populated(!) classroom from db
                var TeacherLs = _db.GetByName<TeacherDB>(name);
                if (TeacherLs != null)
                {
                    _db.FillITimetablesModel(TeacherLs);

                    foreach (var day in TeacherLs.Timetable.Days)
                    {
                        foreach (var dayLesson in day.Lessons)
                        {
                            if (dayLesson.ClassroomId != null && dayLesson.TeacherId != null)
                            {
                                var Class = _db.Get<ClassDB>((long) dayLesson.ClassId);
                                _db.FillITimetablesModel(Class);
                                var Classroom = _db.Get<ClassroomDB>((long) dayLesson.ClassroomId);
                                _db.FillITimetablesModel(Classroom);
                                dayLesson.ClassName = Class.Name;
                                dayLesson.ClassroomName = Classroom.Name;
                                dayLesson.TeacherName = TeacherLs.Name;
                            }
                        }
                    }
                    //returns DTO of timetable
                    return new TeacherDTO(TeacherLs); ;
                }


                //classLs.Timetable.Days = _db.Get<ReplacementDB>(classLs.Id); //adds all needed replacements

                return NotFound("Teacher not found");
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return Problem("Sorry, we seem to have a problem.");
            }
        }

        [HttpGet("teacher/allNames")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<List<string>>> GetAllTeacherNames()
        {
            try
            {
                return _db.GetAll<TeacherDB>().Select(x => x.Name).ToList();
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Network error getting all names of teachers!");
                return Problem("Sorry, we seem to have a problem.");
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return Problem("Sorry, we seem to have a problem.");
            }
        }

        //[HttpGet]
        //[Produces(MediaTypeNames.Application.Json)]
        //public async Task<ActionResult<IEnumerable<ClassDTO>>> GetAllClassTimetableAsync()
        //{
        //    try
        //    {
        //        var classesDb = _db.GetAll<ClassDB>();
        //        foreach (var classDb in classesDb)
        //        {
        //            _db.FillITimetablesModel(classDb);
        //        }
        //        //returns DTOs of timetables
        //        return classesDb.Select(x => new ClassDTO(x)).ToList(); // this conversion is stupid but necessary 
        //    }
        //    catch (HttpRequestException exception)
        //    {
        //        return Problem(exception.Message);
        //    }
        //}
    }
}