using BusinessObject.Object;
using DataAccess.Model.OperationResultModel;
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
        Task<ResultModel> DeleteLocalNotifications(Guid userId, LocalNotification localNotification);

        int GetNotificationQuantity(Guid userId);
        Task UpdateIsReadNoti(Guid userId);

    }
}
