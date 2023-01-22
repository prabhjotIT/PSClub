using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PSClubs.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PSClubs.Controllers
{
    // This controler is created itself and This controller is use to display the main page and connection for this string is defualt connection, Actions like home and privacy are there in this controller

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        //This is method is called whenever no specific action is mentioned in the search bar of browser it shows home page
        public IActionResult Index()
        {
            @TempData["Message"] = "This is my test message!";
            return View();
        }
        //This method is called when user clicks on privacy on the website it simply return view for privacy
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
