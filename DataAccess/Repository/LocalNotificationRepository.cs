using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class LocalNotificationRepository : ILocalNotificationRepository
    {
        public Task<ResultModel> DeleteLocalNotifications(Guid userId, LocalNotification localNotification) 
            => LocalNotificationDAO.Instance.DeleteLocalNotifications(userId, localNotification);

        public Task<ResultModel> GetLocalNotifications(Guid userId)
            => LocalNotificationDAO.Instance.GetLocalNotifications(userId);

        public int GetNotificationQuantity(Guid userId)
            => LocalNotificationDAO.Instance.GetNotificationQuantity(userId);

        public Task<ResultModel> InsertLocalNotifications(Guid userId, LocalNotification localNotification)
           => LocalNotificationDAO.Instance.InsertLocalNotifications(userId, localNotification);

        public Task UpdateIsReadNoti(Guid userId) 
            => LocalNotificationDAO.Instance.UpdateIsReadNoti(userId);
    }
}
