using DataAccess.Migrations;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using System.Security.Claims;

namespace Chess.Areas.Game.Controllers
{
    [Area("Game")]
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HistoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

           
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Response.Cookies.Append("userId", userId);

            HistoryViewModel viewModel = new HistoryViewModel();
            viewModel.history = _unitOfWork.Session.GetAll(x => (x.BlackId == userId || x.WhiteId == userId) && x.IsWhiteWin != null)
                .OrderByDescending(x => x.BeginOfGame)
                .ToList();
            viewModel.userId = userId;

            return View(viewModel);
        }
        public IActionResult Replay(String id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Response.Cookies.Append("userId", userId);


            Session session = _unitOfWork.Session.Get(x => x.Id == id);
            parseSessionHistory(session.Steps, out Dictionary<int, string> white, out Dictionary<int, string> black);

            ReplayViewModel viewModel = new ReplayViewModel();
            viewModel.WhiteMoves = white;
            viewModel.BlackMoves = black;

            return View(viewModel);
        }

        private static void parseSessionHistory(string history, out Dictionary<int, string> white, out Dictionary<int, string> black)
        {
            white = new Dictionary<int, string>();
            black = new Dictionary<int, string>();

            string[] moves = history.Split(' ');

            for (int i = 0; i < moves.Length; i++)
            {
                string[] moveParts = moves[i].Split(':');
                if (moveParts.Length == 2)
                {
                    string[] details = moveParts[1].Split('-');
                    if (i % 2 == 0)
                    {
                        white.Add(i, details[0] + "-" + details[1]);
                    }
                    else
                    {
                        black.Add(i, details[0] + "-" + details[1]);
                    }
                }
            }

        }
    }
}
