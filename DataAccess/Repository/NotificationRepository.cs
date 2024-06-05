using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.NotificationModel;
using DataAccess.Model.OperationResultModel;
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

        public  Task<ResultModel> SendNotificationByEmail(Guid houseId, SendNotificationModel sendNotificationModel) => NotificationDAO.Instance.SendNotificationByEmail(houseId, sendNotificationModel);
        public Task<IEnumerable<string>> GetAllCustomerEmailByHouseId(Guid houseId) => NotificationDAO.Instance.GetAllCustomerEmailByHouseId(houseId);

    }
}
