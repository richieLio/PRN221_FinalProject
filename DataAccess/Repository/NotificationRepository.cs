using BusinessObject.Object;
using DataAccess.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        public Task<IEnumerable<Notification>> GetNotifications(Guid userId) => NotificationDAO.Instance.GetNotifications(userId);
    }
}
