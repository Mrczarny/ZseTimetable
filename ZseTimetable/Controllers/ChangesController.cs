using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TimetableLib.DataAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.Replacements;
using TimetableLib.Models.ScrapperModels;

namespace ZseTimetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangesController : ControllerBase
    {
        private readonly ILogger<ChangesController> _logger;
        private readonly HttpClient _client;
        private DateTime _lastScrap;
        //private DataAccess _db;
        private readonly ChangesScrapper _scrapper;

        public ChangesController(IConfiguration config, ILogger<ChangesController> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
            //_db = db;
            _scrapper = new ChangesScrapper(config.GetSection(ScrapperOption.Position)
                .GetSection("Changes")
                .GetChildren().Select(x => x.Get<ScrapperOption>())
            );
        }


        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<DayReplacements>> GetChangesAsync()
        {
            if (DateTime.Now > _lastScrap.AddMinutes(3))
            {
                try
                {
                    var rawChanges = await _client.GetStreamAsync("https://localhost:5005/MockServer/zmiany");
                    _lastScrap = DateTime.Now;
                    var jsonChanges = _scrapper.Scrap(await new StreamReader(rawChanges).ReadToEndAsync());
                    rawChanges.Close();
                    //_db.Update<ReplacementDB>(jsonChanges.Replacements);
                    return jsonChanges;

                }
                catch (HttpRequestException exception)
                {
                    return Problem(exception.Message);
                }
            }
            else
            {
                throw new Exception("Too fast");
                //return _db.Get<DayReplacements>();
            }
        }
    }
}
