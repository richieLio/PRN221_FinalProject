﻿using BusinessObject.Object;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.ServiceFeeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.ServiceRepository
{
    public interface IServiceFeeRepository
    {
        Task<ResultModel> AddNewService(Guid userId, ServiceCreateReqModel service);
        Task<IEnumerable<Service>> GetServicesList(Guid houseId); // for creating bill
        Task<ResultModel> UpdateService(Guid userId, ServiceUpdateReqModel serviceUpdateModel);
        Task<ResultModel> RemoveService(Guid userId, Guid serviceId, Guid houseId);
        Task<Service> GetServiceById(Guid serviceId);
    }
}
