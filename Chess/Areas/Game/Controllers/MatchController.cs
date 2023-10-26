using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Models.ViewModels;
using System.Linq;
using System.Security.Claims;
using Utils;

namespace Chess.Areas.Game.Controllers
{
    [Area("Game")]
    [Authorize]
    public class MatchController : Controller
	{
        private readonly IHubContext<SignalR> _hubContext;
        private readonly IUnitOfWork _unitOfWork;

        private static Dictionary<string, List<string>> Lobby { get; set; }

        public MatchController(IUnitOfWork unitOfWork, IHubContext<SignalR> hubContext)
		{
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
            Lobby = new Dictionary<string, List<string>>();
        }

		public IActionResult Index()
		{
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var friendsIds = _unitOfWork.UserFriend               
                .GetAll(
                    a => (a.SenderUserId == userId || a.ReceiverUserId == userId)
                )
                .Where(a=>a.IsReceived == true)
                .Select(a => {

                    if (a.SenderUserId != userId)                    
                        return a.SenderUserId;                    
                    else
                        return a.ReceiverUserId;
                }).ToList();

            var friends = friendsIds
                .Select(a => _unitOfWork.User.Get(b => b.Id == a))
                .Select(a=> {
                    string? matchId = null;
                    var matches = _unitOfWork.Session
                    .GetAll(b => (b.BlackId == a.Id && b.WhiteId == userId) ||
                    (b.BlackId == userId && b.WhiteId == a.Id)).ToList();

                    if(matches != null && matches.Count != 0)
                    {
                        var activeMatches = matches.Where(b => b.IsWhiteWin == null).ToList();
                        if (activeMatches != null && activeMatches.Count != 0)
                        {
                            matchId = activeMatches.First().Id;
                        }
                            
                    }

                    



					return new UserWithMatchId { User = a, MatchId = matchId };
                })
                .ToList();

            


            return View(new GameIndexViewModel { Friends=friends, UserId=userId });
		}

        [HttpPost]
        public IActionResult CreateMatch(string oponentId)
        {
            string matchId = StaticFunctions.CreateUniqueId();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _unitOfWork.Session.Add(
                new Session { Id = matchId, BlackId = oponentId, WhiteId = userId, Steps = "", IsWaiting = true }
                );
            _unitOfWork.Save();

            return RedirectToAction(nameof(GameLobby), new { id= matchId });
        }

        public IActionResult GameLobby(string id)
        {

			Response.Cookies.Append("matchId", id);
			return View(nameof(GameLobby),id);
        }

		public IActionResult Play(string id)
		{
			var session = _unitOfWork.Session.Get(a => a.Id == id);
			if(session == null)
                return Content("Error 404");

            if(session != null || session.IsWaiting != true)
            {            
			    Response.Cookies.Append("whiteId", session.WhiteId);
			    Response.Cookies.Append("blackId", session.BlackId);

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Response.Cookies.Append("userId", userId);
				Response.Cookies.Append("matchId", id);
				return View();
            }
            return Problem();
		}
	}
}
