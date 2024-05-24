using DataAccess.Model.OperationResultModel;
using DataAccess.Model.ServiceFeeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ServiceFeeRepository : IServiceFeeRepository
    {
        public Task<ResultModel> AddNewService(Guid userId, ServiceCreateReqModel service)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> GetServicesList(Guid userId, int page)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> GetServicesList(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> RemoveService(Guid userId, Guid serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<ResultModel> UpdateService(Guid userId, ServiceUpdateReqModel serviceUpdateModel)
        {
            throw new NotImplementedException();
        }
    }
}
