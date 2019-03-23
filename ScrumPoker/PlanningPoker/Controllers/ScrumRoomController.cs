using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PlanningPoker.Controllers
{
    public class ScrumRoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}