using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Chess.Areas.Partial.Controllers
{
	[Authorize]
	[Area("Partial")]
	public class FriendsController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public FriendsController(
			IUnitOfWork unitOfWork
			)
		{
			_unitOfWork = unitOfWork;

		}

		


		[HttpPost]		
		public IActionResult GetSended()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var user = _unitOfWork.User.Get(a => a.Id == userId, includeProperties: "SendedUserFriends,ReceivedUserFriends");


			var received = user.SendedUserFriends.Where(a => a.IsReceived == null || a.IsReceived == false)
						.Select(a => _unitOfWork.User.Get(b => a.ReceiverUserId == b.Id))
						.Select(a => new UserFriendViewModel { AvatarPath = a.AvatarPath, Id = a.Id, UserName = a.UserName })
						.ToList();


			

			return PartialView(received);

		}
		[HttpPost]
		public IActionResult GetReceived()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var user = _unitOfWork.User.Get(a => a.Id == userId, includeProperties: "SendedUserFriends,ReceivedUserFriends");



			var requests = user.ReceivedUserFriends.Where(a => a.IsReceived == null || a.IsReceived == false)
						.Select(a => _unitOfWork.User.Get(b => a.SenderUserId == b.Id))
						.Select(a => new UserFriendViewModel { AvatarPath = a.AvatarPath, Id = a.Id, UserName = a.UserName })
						.ToList();
			

			return PartialView(requests);

		}
		[HttpPost]
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

			return PartialView(data);
		}
	}
}
