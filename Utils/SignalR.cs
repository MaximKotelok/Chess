using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace Utils
{
    public class SignalR : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public SignalR(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SendMove(string sessionId, string user, string move)
        {

            var session = _unitOfWork?.Session?.Get(a => a.Id == sessionId);
            if (session != null)
            {
				if (String.IsNullOrEmpty(session.Steps))
                    session.Steps = move;
                else
					session.Steps = $"{session.Steps} {move}";


				_unitOfWork?.Session?.Update(session);
                _unitOfWork?.Save();
                
                await SendMessageToGroup(sessionId, user, move);
                
            }

        }

        private async Task SendMessageToGroup(string sessionId, string user, string message)
        {         
            await Clients.Group(sessionId).SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGameGroup(string sessionId, string connectionId)
        {
           
            await Groups.AddToGroupAsync(this.Context.ConnectionId, sessionId);
        }

        public async Task LeaveGroupGroup(string sessionId, string connectionId)
        {
            await Groups.RemoveFromGroupAsync(this.Context.ConnectionId, sessionId);
        }
    }

}