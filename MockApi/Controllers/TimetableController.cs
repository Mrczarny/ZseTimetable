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
    public class TimetableController : ControllerBase
    {

        private readonly ILogger<TimetableController> _logger;

        public TimetableController(ILogger<TimetableController> logger)
        {
            _logger = logger;
        }

        [HttpGet("zmiany")]
        public Stream GetReplacements()
        {
            var html = System.IO.File.Open(@"./Files/ddd.html", FileMode.Open);
            return html;
        }

        [HttpGet("plany")]
        public Stream GetClassTimetable()
        {
            var html = System.IO.File.OpenRead(@"./Files/o1.html");
            return html;
        }
    }
}
