using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task<ActionResult> GetClassTimetableAsync(int id = 1)
        {
            try
            {
                var response = await _client.GetStreamAsync("https://plan.zse.bydgoszcz.pl/plany/o" + id + ".html");
                return Ok(response);
            }
            catch (HttpRequestException exception)
            {
                return Problem(exception.Message); //TODO: make response for every bad request
            }
        }

        [HttpGet("changes")]
        public async Task<ActionResult> GetChangesAsync()
        {
            try
            {
                var response = await _client.GetStreamAsync("https://zastepstwa.zse.bydgoszcz.pl/index.html");
                var proc = new Processor();
                 return Ok(proc.ChangesProcessor(response));
            }
            catch (HttpRequestException exception)
            {
                return Problem(exception.Message);
            }
        }

    }
}