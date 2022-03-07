using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ZseTimetable.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtimetableController : ControllerBase
    {
        private readonly ILogger<OtimetableController> _logger;
        private HttpClient _client;

        public OtimetableController(ILogger<OtimetableController> logger)
        {
            _logger = logger;
            _client = new HttpClient();
        }

        [HttpGet]
        public async Task<string> GetAsync(int id = 1)
        {
            var response = await _client.GetStringAsync("https://plan.zse.bydgoszcz.pl/plany/o"+id+ ".html");
            return response;
        }
    }
}
