using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TimetableLib;
using TimetableLib.DataAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.DTOs;
using TimetableLib.Timetables;

namespace ZseTimetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly ILogger<TimetableController> _logger;
        private readonly HttpClient _client;
        private DataAccess _db;
        //private readonly TimetableScrapper _scrapper;

        public TimetableController(IConfiguration config, ILogger<TimetableController> logger, DataAccess db, HttpClient client)
        {
            _logger = logger;
            _client = client;
            _db = db;
            _client = client;
            //_scrapper = new TimetableScrapper(config.GetSection(ScrapperOption.Position)
            //    .GetSection("Timetable")
            //    .GetChildren().Select(x => x.Get<ScrapperOption>())
            //);

        }

        [HttpGet("{id}")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<ClassDTO>> GetClassTimetableAsync(int id = 1)
        {
            try
            {
                //Get latest populated(!) class from db
                var classLs = _db.Get<ClassDB>(id);
                
                //classLs.Timetable.Days = _db.Get<ReplacementDB>(classLs.Id); //adds all needed replacements



                //returns DTO of timetable
                return new ClassDTO(classLs);
            }
            catch (HttpRequestException exception)
            {
                return Problem(exception.Message);
            }
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<IEnumerable<ClassDTO>>> GetAllClassTimetableAsync()
        {
            try
            {
                //returns DTOs of timetable
                return _db.GetAll<ClassDB>().Select(x => new ClassDTO(x)).ToList(); // this cast is stupid but necessary 
            }
            catch (HttpRequestException exception)
            {
                return Problem(exception.Message);
            }
        }


    }
}