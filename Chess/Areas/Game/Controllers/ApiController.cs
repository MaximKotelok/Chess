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
        
        [HttpGet("GetFriends")]
        public IActionResult GetFriends()
        {
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var user = _unitOfWork.User.Get(a => a.Id == userId, includeProperties: "SendedUserFriends,ReceivedUserFriends");



				var data = user.SendedUserFriends.Concat(user.ReceivedUserFriends)
					.Where(a => a.IsReceived == true)
					 .Select(a =>
					 {
						 if (a.SenderUserId == userId)
							 return a.ReceiverUserId;
						 else
							 return a.SenderUserId;
					 })
					 .Select(a => _unitOfWork.User.Get(b => b.Id == a))
                     .Select(a => new UserFriendViewModel { AvatarPath = a.AvatarPath, Id = a.Id, UserName = a.UserName })
                     .ToList();




			

            var result = JsonConvert.SerializeObject(data);

            return Json(result);


        }
            [HttpGet("GetSended")]
            public IActionResult GetSended()
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var user = _unitOfWork.User.Get(a => a.Id == userId, includeProperties: "SendedUserFriends,ReceivedUserFriends");


			var received = user.SendedUserFriends.Where(a => a.IsReceived == null || a.IsReceived == false)
						.Select(a => _unitOfWork.User.Get(b => a.ReceiverUserId == b.Id))
                        .Select(a => new UserFriendViewModel { AvatarPath = a.AvatarPath, Id = a.Id, UserName = a.UserName })
                        .ToList();


			var result = JsonConvert.SerializeObject(received);

			return Json(result);

		}


        [HttpGet("GetReceived")]
        public IActionResult GetReceived()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var user = _unitOfWork.User.Get(a => a.Id == userId, includeProperties: "SendedUserFriends,ReceivedUserFriends");



			var requests = user.ReceivedUserFriends.Where(a => a.IsReceived == null || a.IsReceived == false)
						.Select(a => _unitOfWork.User.Get(b => a.SenderUserId == b.Id))
                        .Select(a => new UserFriendViewModel { AvatarPath = a.AvatarPath, Id = a.Id, UserName = a.UserName })
                        .ToList();

			var result = JsonConvert.SerializeObject(requests);

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
        public async void SetWinner([FromBody] WinnerData data)
        {
			var session = _unitOfWork.Session.Get(a => a.Id == data.SessionId);
            session.IsWhiteWin = data.IsWhiteWin;
            _unitOfWork.Session.Update(session);
			_unitOfWork.Save();
		}


	}
}
