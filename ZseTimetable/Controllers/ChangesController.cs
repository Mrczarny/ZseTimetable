using System.IO;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimetableLib.Models.Replacements;

namespace ZseTimetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangesController : ControllerBase
    {
        private readonly ILogger<ChangesController> _logger;
        private readonly HttpClient _client;

        public ChangesController(ILogger<ChangesController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
        }

        [HttpGet]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<DayReplacement>> GetChangesAsync()
        {
            try
            {
                var rawChanges = await _client.GetStreamAsync("https://localhost:5005/MockServer/zmiany");
                var jsonChanges = ChangesScrapper.Scrap(await new StreamReader(rawChanges).ReadToEndAsync());
                rawChanges.Close();
                return jsonChanges;
            }
            catch (HttpRequestException exception)
            {
                return Problem(exception.Message);
            }
        }
    }
}
