using BusinessObject.Object;
using DataAccess.Model.NotificationModel;
using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.NotificationRepository
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetNotifications(Guid userId);
        Task<ResultModel> SendNotificationByEmail(Guid houseId, SendNotificationModel sendNotificationModel);
        Task<IEnumerable<string>> GetAllCustomerEmailByHouseId(Guid houseId);


    }
}
