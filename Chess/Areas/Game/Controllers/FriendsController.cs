using Azure.Core;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Chess.Areas.Game.Controllers
{
    [Authorize]
    [Area("Game")]
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
        public IActionResult Accept(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userFriend = _unitOfWork.UserFriend.Get(a => (
                    (a.SenderUserId == userId && a.ReceiverUserId == id) ||
                    (a.SenderUserId == id && a.ReceiverUserId == userId)
                ));
            if (userFriend != null)
            {
                userFriend.IsReceived = true;
                _unitOfWork.UserFriend.Update(userFriend);
                _unitOfWork.Save();
                return Json("{status: 'ok'}");
            }

            return Json("{status: 'problem'}");


        }
        [HttpPost]
        public IActionResult RemoveFriend(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userFriend = _unitOfWork.UserFriend.Get(a => (
                    (a.SenderUserId == userId && a.ReceiverUserId == id) ||
                    (a.SenderUserId == id && a.ReceiverUserId == userId)
                ));
            if (userFriend != null)
            {

                _unitOfWork.UserFriend.Remove(userFriend);
                _unitOfWork.Save();
                return Json("{status: 'ok'}");
            }

            return Json("{status: 'problem'}");


        }

        [HttpPost]
        public IActionResult Reject(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userFriend = _unitOfWork.UserFriend.Get(a => (
                    (a.SenderUserId == userId && a.ReceiverUserId == id) ||
                    (a.SenderUserId == id && a.ReceiverUserId == userId)
                ));
            if (userFriend != null)
            {
                if (userFriend.IsReceived == null || userFriend.IsReceived != true)
                {
                    _unitOfWork.UserFriend.Remove(userFriend);
                    _unitOfWork.Save();
                    return Json("{status: 'ok'}");
                }

            }

            return Json("{status: 'problem'}");

        }

        [HttpPost]
        public IActionResult Recall(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userFriend = _unitOfWork.UserFriend.Get(a => (
                    (a.SenderUserId == userId && a.ReceiverUserId == id) ||
                    (a.SenderUserId == id && a.ReceiverUserId == userId)
                ));
            if (userFriend != null)
            {
                if (userFriend.IsReceived == null || userFriend.IsReceived != true)
                {
                    _unitOfWork.UserFriend.Remove(userFriend);
                    _unitOfWork.Save();
                    return Json("{status: 'ok'}");
                }


            }

            return Json("{status: 'problem'}");

        }

        public IActionResult Profile(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            User profileUser;
            RequestState state;
            if (userId != id)
            {
                profileUser = _unitOfWork.User.Get(a => a.Id == id);


                var userFriend = _unitOfWork.UserFriend.Get(a => (
                        (a.SenderUserId == userId && a.ReceiverUserId == id) ||
                        (a.SenderUserId == id && a.ReceiverUserId == userId)
                    ));


                if (userFriend == null)
                {
                    state = RequestState.NOTHING;
                }
                else if (userFriend.IsReceived == null || userFriend.IsReceived == false)
                {
                    if (userFriend.SenderUserId == userId)
                        state = RequestState.SENDED_FROM_YOU;
                    else
                        state = RequestState.SENDED_FROM_THEM;
                }
                else
                {
                    state = RequestState.FRIENDS;
                }
            }
            else
            {
                profileUser = _unitOfWork.User.Get(a => a.Id == userId);
                state = RequestState.ITS_YOU;

            }
            ProfileViewModel model = new ProfileViewModel
            {
                User = profileUser,
                State = state
            };


            return View(model);
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddFriend(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != id)
            {
                var friend = _unitOfWork.UserFriend.Get(
                    a => (
                        (a.SenderUserId == userId && a.ReceiverUserId == id) ||
                        (a.SenderUserId == id && a.ReceiverUserId == userId)
                    ));

                if (friend == null)
                {
                    friend = new UserFriend { SenderUserId = userId, ReceiverUserId = id };
                    _unitOfWork.UserFriend.Add(friend);
                    _unitOfWork.Save();
                    TempData["Success"] = "Request sended";
                }
                else if (friend.IsReceived == null || friend.IsReceived == false)
                {
                    TempData["Warning"] = "You have already sent a request";
                }
                else if (friend.IsReceived == true)
                {
                    TempData["Warning"] = "You are already friends";
                }

            }
            return RedirectToAction(nameof(Profile), new { id = id });
        }

        public IActionResult Top()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var user = _unitOfWork.User.Get(a => a.Id == userId, includeProperties: "SendedUserFriends,ReceivedUserFriends");

			var top = user.SendedUserFriends.Concat(user.ReceivedUserFriends)
				.Where(a => a.IsReceived == true)
				 .Select(a =>
				 {
					 if (a.SenderUserId == userId)
						 return a.ReceiverUserId;
					 else
						 return a.SenderUserId;
				 })
				 .Select(a => _unitOfWork.User.Get(b => b.Id == a))
				 .ToList();

            top.Add(user);

			top = top.OrderByDescending(user => user.Reputation).ToList();


            TopViewModel viewModel = new TopViewModel();
            viewModel.CurrentPlayer = user;
            viewModel.Top = top;

			return View(viewModel);
        }


    }
}
