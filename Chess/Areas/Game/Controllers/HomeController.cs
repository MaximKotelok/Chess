using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Chess.Areas.Game.Controllers
{
	[Area("Game")]
	public class HomeController : Controller   
    {


        public IActionResult Index()
        {
            TempData["Success"] = "Added To Compare List Successfully";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}