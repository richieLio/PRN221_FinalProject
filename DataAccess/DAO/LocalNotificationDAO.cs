using BusinessObject.Object;
using DataAccess.Model.OperationResultModel;
using DataAccess.Repository;
using Google.Apis.Storage.v1.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class LocalNotificationDAO
    {
        private static LocalNotificationDAO instance = null;
        private static readonly object instanceLock = new object();
        private LocalNotificationDAO() { }
        public static LocalNotificationDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new LocalNotificationDAO();
                    }
                    return instance;
                }
            }
        }

        internal async Task<ResultModel> DeleteLocalNotifications(Guid userId, LocalNotification localNotification)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultModel> GetLocalNotifications(Guid userId)
        {
            using var context = new RmsContext();
            var notifications = await context.LocalNotifications.Where(l => l.UserId == userId).ToListAsync();
            return new ResultModel
            {
                IsSuccess = true,
                Data = notifications,
                Message = "Get notiSucess",
            };
        }

        public async Task<ResultModel> InsertLocalNotifications(Guid userId, LocalNotification localNotification)
        {
            IUserRepository userRepository = new UserRepository();
            using var context = new RmsContext();
            var user = userRepository.GetUserById(userId);
            if (user == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }
            var result = context.Add(localNotification);
            await context.SaveChangesAsync();
            return new ResultModel
            {
                IsSuccess = true,
                Data = localNotification,
                Message = "Local notification added successfully"
            };
        }
        public async Task UpdateIsReadNoti(Guid userId)
        {
            using var context = new RmsContext();

            var result = await GetLocalNotifications(userId);

            if (result != null && result.Data is IEnumerable<LocalNotification> localNotifications)
            {
                foreach (var notification in localNotifications)
                {
                    if (!notification.IsRead)
                    {
                        notification.IsRead = true;
                    }
                }

                context.UpdateRange(localNotifications);
                await context.SaveChangesAsync();

            }
        }



        public int GetNotificationQuantity(Guid userId)
        {
            using (var context = new RmsContext())
            {
                return context.LocalNotifications
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .Count();
            }
        }


    }
}
