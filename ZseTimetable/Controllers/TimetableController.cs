using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimetableLib;

namespace ZseTimetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController : ControllerBase
    {
        private readonly ILogger<TimetableController> _logger;
        private readonly HttpClient _client;

        public TimetableController(ILogger<TimetableController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
        }

        [HttpGet("{id}")]
        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> GetClassTimetableAsync(int id = 1)
        {
            try
            {
                var rawTimetable = await _client.GetStreamAsync("https://localhost:5005/Timetable/plany");
                var jsonChanges = await TimetableScrapper.Scrap(rawTimetable);
                return Ok(jsonChanges);
            }
            catch (HttpRequestException exception)
            {
                return Problem(exception.Message);
            }
        }

    }
}