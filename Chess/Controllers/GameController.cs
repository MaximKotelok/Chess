using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace Chess.Controllers
{
	public class GameController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public GameController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index(string id)
		{
			var session = _unitOfWork.Session.Get(a => a.Id == id);

			Response.Cookies.Append("whiteId", session.WhiteId);
			Response.Cookies.Append("blackId", session.BlackId);
			Response.Cookies.Append("userId", "1");

			return View();
		}
	}
}
