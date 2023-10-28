using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Models.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chess.Areas.Game.Controllers
{
    [ApiController]
    [Route("/api")]
    public class ApiController : Controller
    {
            
        private readonly IUnitOfWork _unitOfWork;
        public ApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


		[HttpGet("GetUsersByUsername")]
		public IActionResult GetUsersByUsername(string username)
		{

			List<UserFriendViewModel> data = new List<UserFriendViewModel>();
			if (!System.String.IsNullOrEmpty(username))
				data = _unitOfWork.User.GetAll(a => a.UserName.StartsWith(username)).Select(a => new UserFriendViewModel { AvatarPath = a.AvatarPath, Id = a.Id, UserName = a.UserName })
					.ToList();

			var result = JsonConvert.SerializeObject(data);

			return Json(result);


		}

		[HttpGet("sessions")]
        public async Task<IActionResult> GetSessionData(string sessionId)
        {
            var session = _unitOfWork.Session.Get(a=>a.Id == sessionId);

            if (session != null)
            {
                string jsonData = session.Steps;
                return new JsonResult(jsonData);
            }

            return Problem();
            
        }

        [HttpPost("setWinner")]
        public async Task<IActionResult> SetWinner([FromBody] WinnerData data)
        {
			var session = _unitOfWork.Session.Get(a => a.Id == data.SessionId);
			if(session.IsWhiteWin != null)
                return Json("{status: 'ok'}");

            session.IsWhiteWin = data.IsWhiteWin;

			var white = _unitOfWork.User.Get(x => x.Id == session.WhiteId);
			var black = _unitOfWork.User.Get(x => x.Id == session.BlackId);

			if (data.IsWhiteWin == true)
			{
				white.Reputation += 1;
				black.Reputation -= 1;
			}
			else
			{
				white.Reputation -= 1;
				black.Reputation += 1;
			}

			_unitOfWork.Session.Update(session);
			_unitOfWork.User.Update(white);
			_unitOfWork.User.Update(black);

			_unitOfWork.Save();

			return Json("{status: 'ok'}"); 
        }
	}
}
