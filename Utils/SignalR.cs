using Microsoft.AspNetCore.SignalR;

namespace Utils
{
    public class SignalR : Hub
    {
        public async Task SendMessageToGroup(string groupName, string user, string message)
        {         
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendMessage(string user, string message)
        { 
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGroup(string groupName)
        {         
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {         
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }

}