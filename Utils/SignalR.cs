using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Utils
{
    public class SignalR : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        private static ConcurrentDictionary<string, List<string>> lobby { get; set; }

        public SignalR(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
			if (lobby == null)
			{
				lobby = new ConcurrentDictionary<string, List<string>>();
			}
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

			public async Task JoinLobby(string sessionId)
			{
				if (lobby.TryGetValue(sessionId, out List<string> groups))
				{
					List<string> groupsCopy = new List<string>(groups); // Создаем копию коллекции

					groupsCopy.Add(this.Context.ConnectionId);

					if (groupsCopy.Count == 2)
					{
						var item = _unitOfWork.Session.Get(a => a.Id == sessionId);

						item.IsWaiting = false;
						_unitOfWork.Save();

						foreach (var connection in groupsCopy)
						{
							await Clients.Clients(connection).SendAsync("Started");
							await LeaveLobby(sessionId, connection);
						}
					}

					foreach (var connection in groupsCopy)
					{
						await Clients.Clients(connection).SendAsync("Started");
						await LeaveLobby(sessionId, connection);
					}

					lobby.TryRemove(sessionId, out _);
				}
				else
				{
					groups = new List<string> { this.Context.ConnectionId };
					lobby.TryAdd(sessionId, groups);
				}

				foreach (var connection in groups)
				{
					await Clients.Clients(connection).SendAsync("Count", sessionId);
					
				}

				await Groups.AddToGroupAsync(this.Context.ConnectionId, sessionId);
			}



		public async Task LeaveLobby(string sessionId, string userId)
		{
			if (lobby.TryGetValue(sessionId, out List<string> groups))
			{
				groups.Remove(userId);

				if (groups.Count > 0)
				{
					lobby.TryUpdate(sessionId, groups, groups);
				}
				else
				{
					lobby.TryRemove(sessionId, out _);
				}
			}

			await Groups.RemoveFromGroupAsync(userId, sessionId);
		}

		public async Task JoinGameGroup(string sessionId, string connectionId)
        {
  

             await Groups.AddToGroupAsync(this.Context.ConnectionId, sessionId);
        }


        public async Task LeaveGameGroup(string sessionId, string connectionId)
        {

            await Groups.RemoveFromGroupAsync(this.Context.ConnectionId, sessionId);
        }
    }

}