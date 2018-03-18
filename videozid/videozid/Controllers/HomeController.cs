using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using videozid.Models;

namespace videozid.Controllers
{
    public class HomeController : Controller
    {

        public readonly RPPP15Context ctx;

        public HomeController(RPPP15Context ctx)
        {
            this.ctx = ctx;
        } 
        
        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        
    }
}
