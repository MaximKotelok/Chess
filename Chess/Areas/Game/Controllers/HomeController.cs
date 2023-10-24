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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}