using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Chess.Controllers
{
    
    [ApiController]
    public class ApiController : Controller
    {
        private IHubContext<SignalR> _hubContext;
        private readonly IUnitOfWork _unitOfWork;
        public ApiController(IHubContext<SignalR> hubContext, IUnitOfWork unitOfWork)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
        }


        [HttpPost("sessions")]
        public async Task<IActionResult> GetSessionData(string sessionId)
        {
            var session = _unitOfWork.Session.Get(a=>a.Id == sessionId);

            if (session != null)
            {
                string jsonData = JsonConvert.SerializeObject(session.Steps);
                return new JsonResult(jsonData);
            }

            return Problem();
            
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame(string host, string guest)
        {
            string id = StaticFunctions.CreateUniqueId();
            Session session = new Session { Id = id, WhiteId = host, BlackId = guest, Steps=""};
            _unitOfWork.Session.Add(session);            
            await _hubContext.Groups.AddToGroupAsync(id, host);
            return Ok();
        }

        [HttpPost("join")]
        public async Task<IActionResult> JoinGameGroup(string sessionId, string connectionId)
        {            
            await _hubContext.Groups.AddToGroupAsync(connectionId, sessionId);
            return Ok();
        }

        [HttpPost("move")]
        public async Task<IActionResult> SendMove(string sessionId, string user, string move)
        {
            if (StaticFunctions.IsMoveValidRegex(move))
            {            
                var session = _unitOfWork?.Session?.Get(a => a.Id == sessionId);
                if (session != null)
                {
                    session.Steps = $"{session.Steps} {move}";
                    _unitOfWork?.Session?.Update(session);
                    _unitOfWork?.Save();

                    await _hubContext.Clients.Group(sessionId).SendAsync("ReceiveMove", user, move);
                    return Ok();
                }
            }

            return Problem();
        }

    }
}
