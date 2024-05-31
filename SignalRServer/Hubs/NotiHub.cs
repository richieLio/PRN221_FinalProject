using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Hubs
{
    public class NotiHub : Hub
    {

        public Task NotifyBillCreated(Guid ownerId, Guid billId, string message, int unReadNoti)
        {

            return Clients.All.SendAsync("ReceiveNotification", ownerId, billId, message, unReadNoti);
        }
       
    }
}
