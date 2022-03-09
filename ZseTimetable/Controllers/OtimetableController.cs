using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ZseTimetable.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
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

        [Microsoft.AspNetCore.Mvc.HttpGet("{id}")]
        [Microsoft.AspNetCore.Mvc.HttpGet]
        public async Task<ActionResult> GetClassTimetableAsync(int id = 1)
        {
            try
            {
                var response = await _client.GetStringAsync("https://plan.zse.bydgoszcz.pl/plany/o" + id + ".html");
                return Ok(response);
            }
            catch (HttpRequestException exception)
            {
                return Content(exception.Message);
            }
        }


    }
}
