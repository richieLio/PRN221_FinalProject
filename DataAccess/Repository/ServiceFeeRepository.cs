using BusinessObject.Object;
using DataAccess.DAO;
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
            => ServiceFeeDAO.Instance.AddNewService(userId, service);
        public Task<IEnumerable<Service>> GetServicesList(Guid userId, Guid houseId)
            => ServiceFeeDAO.Instance.GetServicesList(userId, houseId);

        public Task<ResultModel> RemoveService(Guid userId, Guid serviceId, Guid houseId)
            => ServiceFeeDAO.Instance.RemoveService(userId, serviceId, houseId);

        public Task<ResultModel> UpdateService(Guid userId, ServiceUpdateReqModel serviceUpdateModel)
            => ServiceFeeDAO.Instance.UpdateService(userId, serviceUpdateModel);
    }
}
