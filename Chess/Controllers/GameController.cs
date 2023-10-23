using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Chess.Controllers
{
    /*[ApiController]
    [Route("[controller]")]*/
    public class GameController : Controller   
    {

        private readonly IUnitOfWork _unitOfWork;

        public GameController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            _unitOfWork.User.Add(new Models.User { Password = "password", Username="user1", Reputation=5, Id=Guid.NewGuid().ToString("N")  });
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}