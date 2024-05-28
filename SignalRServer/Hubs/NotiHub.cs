using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Hubs
{
    public class NotiHub : Hub
    {

        public Task NotifyBillCreated(Guid ownerId, string message)
        {

            return Clients.All.SendAsync("ReceiveNotification", ownerId, message);
        }
    

    }
}
