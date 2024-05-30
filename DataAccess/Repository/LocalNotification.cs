using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class LocalNotification : ILocalNotification
    {
        public Task<ResultModel> DeleteLocalNotifications()
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> GetLocalNotifications(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> InsertLocalNotifications()
        {
            throw new NotImplementedException();
        }
    }
}
