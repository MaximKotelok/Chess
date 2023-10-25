using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

namespace Chess.Areas.Game.Controllers
{
    [Area("Game")]
    [Authorize]
    public class MatchController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public MatchController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index(string id)
		{
			var session = _unitOfWork.Session.Get(a => a.Id == id);

			Response.Cookies.Append("whiteId", session.WhiteId);
			Response.Cookies.Append("blackId", session.BlackId);

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Response.Cookies.Append("userId", userId);

			return View();
		}
	}
}
