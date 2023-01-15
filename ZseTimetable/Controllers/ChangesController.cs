using System;
using System.Collections.Generic;
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
using TimetableLib.DBAccess;
using TimetableLib.Models.DBModels;
using TimetableLib.Models.DTOs;
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
        private ChangesAccess _db;
        //private readonly ChangesScrapper _scrapper;

        public ChangesController(IConfiguration config, ILogger<ChangesController> logger, IHttpClientFactory client,
            IDataWrapper db)
        {
            _logger = logger;
            _client = client.CreateClient("baseHttp");
            _db = db.ChangesAccess;
            //_scrapper = new ChangesScrapper(config.GetSection(ScrapperOption.Position)
            //    .GetSection("Changes")
            //    .GetChildren().Select(x => x.Get<ScrapperOption>())
            //);
        }


        //[HttpGet]
        //[Produces(MediaTypeNames.Application.Json)]
        //public async Task<ActionResult<IEnumerable<ReplacementDTO>>> GetChangesAsync()
        //{
        //    try
        //    {
        //        DayReplacements scrappedChanges;
        //        using (var rawChanges = await _client.GetStreamAsync("zmiany"))
        //        {
        //            scrappedChanges = _scrapper.Scrap(await new StreamReader(rawChanges).ReadToEndAsync());
        //        }

        //        //_db.Update<ReplacementDB>(jsonChanges.Replacements);
        //        throw new NotImplementedException();

        //    }
        //    catch (HttpRequestException exception)
        //    {
        //        return Problem(exception.Message);
        //    }
        //}
    }
}

