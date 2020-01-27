using Microsoft.AspNetCore.Mvc;
using SQLDBAccess.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SQLDBAccess.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StatisticsController : ControllerBase
    {
        private readonly PlagiatorContext Context;
        public StatisticsController(PlagiatorContext context)
        {
            Context = context;
        }

        //[HttpGet]
        //public async Task<ActionResult> GenerateStatistics()
        //{


        //}
    }
}
