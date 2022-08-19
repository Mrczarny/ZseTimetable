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
using TimetableLib.Models.ScrapperModels;
using TimetableLib.Timetables;

namespace ZseTimetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly ILogger<TimetableController> _logger;
        private readonly HttpClient _client;
        private readonly DataAccess _db;
        private readonly TimetableScrapper _scrapper;

        public TimetableController(IConfiguration config, ILogger<TimetableController> logger )
        {
            _logger = logger;
            _client = new HttpClient();
            //_db = db;
            _scrapper = new TimetableScrapper(
                config.GetSection("Regex").GetSection("Timetable").GetChildren().ToDictionary(x => x.Key,
                    x => new Dictionary<string, int>(x.GetSection("Pattern").Value, int.Parse(x.GetSection("RegexOptions").Value))));
        }

        [HttpGet("{id}")]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<Timetable>> GetNewClassTimetableAsync(int id = 1)
        {
            try
            {
                var rawTimetable = await _client.GetStreamAsync("https://localhost:5005/MockServer/plany");
                var jsonChanges = await _scrapper.Scrap<Teacher>(await new StreamReader(rawTimetable).ReadToEndAsync());
                rawTimetable.Close();
                //_db.Update<Timetable>(id);
                _client.Dispose();
                return jsonChanges;
            }
            catch (HttpRequestException exception)
            {
                return Problem(exception.Message);
            }
        }

        public async Task<ActionResult<Timetable>> GetClassTimetableAsync(int id = 1)
        {
            try
            {
                return _db.Get<Timetable>(id);
            }
            catch (HttpRequestException exception)
            {
                return Problem(exception.Message);
            }
        }


    }
}