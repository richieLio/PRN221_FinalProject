using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface ILocalNotification
    {
        Task<ResultModel> GetLocalNotifications(Guid userId);
        Task<ResultModel> InsertLocalNotifications();
        Task<ResultModel> DeleteLocalNotifications();
    }
}
