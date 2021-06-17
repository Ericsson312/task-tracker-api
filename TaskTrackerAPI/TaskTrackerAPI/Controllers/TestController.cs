using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Controllers
{
    public class TestController : Controller
    {
        [HttpGet("api/task")]
        public ActionResult Get()
        {
            return Ok(new { task_to_do = "clean the room" });
        }
    }
}
