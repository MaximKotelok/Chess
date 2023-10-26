using Azure.Core;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
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
            if(userFriend != null)
            {
                userFriend.IsReceived = true;
                _unitOfWork.UserFriend.Update(userFriend);
                _unitOfWork.Save();
                TempData["Succcess"] = "New Friend Added";
            }
            else
            {
                TempData["Succcess"] = "Something wrong";
            }

            return RedirectToAction(nameof(Index));
            

        }


        public IActionResult Profile(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var profileUser = _unitOfWork.User.Get(a => a.Id == id);

            var userFriend = _unitOfWork.UserFriend.Get(a => (
                    (a.SenderUserId == userId && a.ReceiverUserId == id) ||
                    (a.SenderUserId == id && a.ReceiverUserId == userId)
                ));

            RequestState state;
            if (userFriend == null)
                state = RequestState.NOTHING;
            else if (userFriend.IsReceived == null || userFriend.IsReceived == false)
                state = RequestState.SENDED;
            else
                state = RequestState.FRIENDS;

            ProfileViewModel model = new ProfileViewModel {
                User = profileUser,
                State = state
                };


            return View(model);
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var friendsIds = _unitOfWork.UserFriend.GetAll(
                a=> (a.SenderUserId == userId || a.ReceiverUserId == userId)
                ).Select(a=> {
                    RequestState state = RequestState.FRIENDS;
                    if (a.SenderUserId != userId)
                    {

                        if (a.IsReceived == null || a.IsReceived == false)
                            state = RequestState.SENDED;

                        return new {id= a.SenderUserId, state};
                    }
                    else
                    {
                        if (a.IsReceived == null || a.IsReceived == false)
                            state = RequestState.NOTHING;
                        return new {id= a.ReceiverUserId, state };
                    }
                }).ToList();

            var friends = friendsIds.Where(a=>a.state == RequestState.FRIENDS)
                            .Select(a => _unitOfWork.User.Get(b => b.Id == a.id)).ToList();
            var requests = friendsIds.Where(a => a.state == RequestState.SENDED)
                            .Select(a => _unitOfWork.User.Get(b => b.Id == a.id)).ToList();


            return View(new UserFriendViewModel { UserId=userId, Friends = friends, Requests=requests });
        }

        [HttpPost]
        public IActionResult AddFriend(string id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var friend = _unitOfWork.UserFriend.Get(
                a => (
                    (a.SenderUserId == userId && a.ReceiverUserId == id) ||
                    (a.SenderUserId == id && a.ReceiverUserId == userId)
                ));

            if(friend == null)
            {
                friend = new UserFriend { SenderUserId = userId, ReceiverUserId = id };
                _unitOfWork.UserFriend.Add(friend);
                _unitOfWork.Save();
                TempData["Success"] = "Request sended";
            }
            else if(friend.IsReceived == null || friend.IsReceived == false)
            {
                TempData["Warning"] = "You have already sent a request";
            }
            else if (friend.IsReceived == true)
            {
                TempData["Warning"] = "You are already friends";
            }


            return RedirectToAction(nameof(Profile), new {id=id});
        }
    }
}
