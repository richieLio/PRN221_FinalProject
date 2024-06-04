using BusinessObject.Object;
using DataAccess.Model.OperationResultModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface ILocalNotificationRepository
    {
        Task<ResultModel> GetLocalNotifications(Guid userId);
        Task<ResultModel> InsertLocalNotifications(Guid userId, LocalNotification localNotification);
        Task DeleteLocalNotifications(Guid localNotificationId);

        int GetNotificationQuantity(Guid userId);
        Task UpdateIsReadNoti(Guid userId);
        Task<LocalNotification> GetLocalNotificationByMessage(string message);
       

    }
}
