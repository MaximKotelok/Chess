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
        #region Constructor and params
        private readonly IUnitOfWork _unitOfWork;

        private static ConcurrentDictionary<string, List<UserConnection>> lobby { get; set; }

        public SignalR(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            if (lobby == null)
            {
                lobby = new ConcurrentDictionary<string, List<UserConnection>>();
            }
        }
        #endregion
        #region Sends
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

                await Clients.Group(sessionId).SendAsync("ReceiveMessage", user, move);

            }

        }
        #endregion
        #region Lobby
        public async Task JoinLobby(string sessionId, string userid)
        {
            if (lobby.TryGetValue(sessionId, out List<UserConnection> groups))
            {
                var groupsCopy = new List<UserConnection>(groups);

                int index = groupsCopy.FindIndex(a => a.UserId == userid);

                if (index == -1)
                {
                    groupsCopy.Add(new UserConnection { Connections = new List<string>() { this.Context.ConnectionId }, UserId = userid });
                }
                else
                {
                    if (!groupsCopy[index].Connections.Contains(this.Context.ConnectionId))
                    {
                        groupsCopy[index].Connections.Add(this.Context.ConnectionId);
                    }
                }
                lobby.TryUpdate(sessionId, groupsCopy, groupsCopy);


					if (groupsCopy.Count == 2)
					{
						var item = _unitOfWork.Session.Get(a => a.Id == sessionId);
						
						item.BeginOfGame = DateTime.Now;
						item.IsWaiting = false;

                    _unitOfWork.Session.Update(item);
						_unitOfWork.Save();

                    foreach (var entity in groupsCopy)
                    {
                        foreach (var connection in entity.Connections)
                        {
                            await Clients.Clients(connection).SendAsync("Started");
                            await LeaveLobbyWithUserId(sessionId, userid, connection);
                        }

                    }
                    lobby.TryRemove(sessionId, out _);
                }
                


            }
            else
            {
                groups = new List<UserConnection> {
                    new UserConnection {
                        UserId=userid,
                        Connections = new List<string>{this.Context.ConnectionId }
                    }
                };
                lobby.TryAdd(sessionId, groups);
            }


            await Groups.AddToGroupAsync(this.Context.ConnectionId, sessionId);
        }

        public async Task LeaveLobby(string sessionId, string userid)
        {
            await LeaveLobbyWithUserId(sessionId, userid, this.Context.ConnectionId);

        }

        private async Task LeaveLobbyWithUserId(string sessionId, string userid, string userId)
        {
            if (lobby.TryGetValue(sessionId, out List<UserConnection> groups))
            {
                var user = groups.Find(a => a.UserId == userid);
                if (user != null)
                {


                    user.Connections.Remove(userId);

                    if (user.Connections.Count > 0)
                    {
                        lobby.TryUpdate(sessionId, groups, groups);
                    }
                    else
                    {
                        groups.Remove(user);
                        lobby.TryUpdate(sessionId, groups, groups);
                        if (groups.Count == 0)
                            lobby.TryRemove(sessionId, out _);
                    }
                }
            }

            await Groups.RemoveFromGroupAsync(userId, sessionId);
        }
        #endregion
        #region GameGroup
        public async Task JoinGameGroup(string sessionId, string connectionId)
        {


            await Groups.AddToGroupAsync(this.Context.ConnectionId, sessionId);
        }


        public async Task LeaveGameGroup(string sessionId)
        {

			//await Clients.Group(sessionId).SendAsync("Win");
			await Groups.RemoveFromGroupAsync(this.Context.ConnectionId, sessionId);

        }
        public async Task GiveUp(string sessionId, bool isWhite)
        {

			await Clients.Group(sessionId).SendAsync("Win", isWhite);
			await Groups.RemoveFromGroupAsync(this.Context.ConnectionId, sessionId);

        }
        #endregion
    }

}