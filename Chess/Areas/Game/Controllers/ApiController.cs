using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Newtonsoft.Json;
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

            List<User> data = new List<User>();
            if (!System.String.IsNullOrEmpty(username))
                data = _unitOfWork.User.GetAll(a => a.UserName.StartsWith(username)).ToList();

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
        public async void SetWinner([FromBody] WinnerData data)
        {
			var session = _unitOfWork.Session.Get(a => a.Id == data.SessionId);
            session.IsWhiteWin = data.IsWhiteWin;
            _unitOfWork.Session.Update(session);
			_unitOfWork.Save();
		}


	}
}
