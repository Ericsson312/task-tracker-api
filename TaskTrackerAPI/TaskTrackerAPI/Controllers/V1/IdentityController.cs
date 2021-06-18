using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskTrackerApi.Controllers.V1
{
    public class IdentityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
