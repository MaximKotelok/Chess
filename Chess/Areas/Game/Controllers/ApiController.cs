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

    [Area("Game")]
    [ApiController]
    [Route("/Game/api")]
    public class ApiController : Controller
    {
            
            private readonly IUnitOfWork _unitOfWork;
            public ApiController(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
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


    }
}
