using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeTableProcessor;

namespace ZseTimetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtimetableController : ControllerBase
    {
        private readonly ILogger<OtimetableController> _logger;
        private readonly HttpClient _client;

        public OtimetableController(ILogger<OtimetableController> logger)
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
                var rawTimetable = await _client.GetStreamAsync("https://plan.zse.bydgoszcz.pl/plany/o" + id + ".html");
                var scrapper = new TimetableScrapper();
                var jsonChanges = await scrapper.Scrapper(rawTimetable);
                return Ok(jsonChanges);
            }
            catch (HttpRequestException exception)
            {
                return Problem(exception.Message);
            }
        }

        [HttpGet("changes")]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult> GetChangesAsync()
        {
            try
            {
                var rawChanges = await _client.GetStreamAsync("https://zastepstwa.zse.bydgoszcz.pl/index.html");
                var scrapper = new ChangesScrapper();
                var jsonChanges = await scrapper.Scrapper(rawChanges);
                return Ok(jsonChanges);
            }
            catch (HttpRequestException exception)
            {
                return Problem(exception.Message);
            }
        }

    }
}