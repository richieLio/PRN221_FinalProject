﻿using BusinessObject.Object;
using DataAccess.Model.ContractModel;
using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.ContractRepository
{
    public interface IContractRepository
    {
        Task<ResultModel> GetContractList(Guid userId, Guid houseId);
        Task<Contract> GetContract(Guid contractId);
        Task UpdateContract(ContractUpdateModel contract);
    }
}
