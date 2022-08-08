using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MockApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MockServerController : ControllerBase
    {
        
        private readonly ILogger<MockServerController> _logger;

        public MockServerController(ILogger<MockServerController> logger)
        {
            _logger = logger;
        }

        [HttpGet("zmiany")]
        public Stream GetReplacements()
        {
            Random random = new Random();
            var html = System.IO.File.OpenRead($"./Files/changes/z{random.Next(1,7)}.html");
            return html;
        }

        [HttpGet("plany")]
        public Stream GetClassTimetable()
        {
            Random random = new Random();
            var html = System.IO.File.OpenRead($"./Files/timetables/o{random.Next(1,7)}.html");
            return html;
        }
    }
}
