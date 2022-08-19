using System;
using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimetableLib.DataAccess;
using TimetableLib.Models.Replacements;

namespace ZseTimetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangesController : ControllerBase
    {
        private readonly ILogger<ChangesController> _logger;
        private readonly HttpClient _client;
        private DateTime _lastScrap;
        private DataAccess _db;

        public ChangesController(ILogger<ChangesController> logger, DataAccess db)
        {
            _logger = logger;
            _client = new HttpClient();
            _db = db;
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
                    var jsonChanges = ChangesScrapper.Scrap(await new StreamReader(rawChanges).ReadToEndAsync());
                    rawChanges.Close();
                    _db.Update<DayReplacements>(1);
                    return jsonChanges;
                }
                catch (HttpRequestException exception)
                {
                    return Problem(exception.Message);
                }
            }
            else
            {
                return _db.Get<DayReplacements>();
            }
        }
    }
}
