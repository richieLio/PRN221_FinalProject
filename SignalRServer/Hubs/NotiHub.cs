using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Hubs
{
    public class NotiHub : Hub
    {
        public Task NotifyBillCreated(string ownerId, string message)
        {
            return Clients.User(ownerId).SendAsync("ReceiveNotification", message);
        }
        
    }
}
