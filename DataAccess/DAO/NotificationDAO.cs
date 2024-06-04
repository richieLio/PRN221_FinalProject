using BusinessObject.Object;
using DataAccess.Enums;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class NotificationDAO
    {
        private static NotificationDAO instance = null;
        private static readonly object instanceLock = new object();
        private NotificationDAO() { }
        public static NotificationDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new NotificationDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<IEnumerable<Notification>> GetNotifications(Guid userId)
        {
            using var context = new RmsContext();
            IUserRepository userRepository = new UserRepository();
            IEnumerable<Notification> notifications = new List<Notification>();

            var user = await userRepository.GetUserById(userId);
            if (user.Role == UserEnum.OWNER)
            {
                notifications = await context.Notifications.Include(n => n.House)
                                 .Where(n => n.House.OwnerId == userId)
                                 .ToListAsync(); 
            }
            else if (user.Role == UserEnum.STAFF)
            {
                notifications = await 
                        context.Notifications
                    .Include(n => n.House)
                    .ThenInclude(hs => hs.Staff)
                    .Where(n => n.House.Staff
                    .Any(hs => hs.Id == userId))
                    .ToListAsync();
            }
            return notifications;

        }
    }
}

