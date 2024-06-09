using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.ContractModel;
using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.ContractRepository
{
    public class ContractRepository : IContractRepository
    {
        public Task<Contract> GetContract(Guid contractId) => ContractDAO.Instance.GetContract(contractId);

        public Task<ResultModel> GetContractList(Guid userId) => ContractDAO.Instance.GetContractList(userId);

        public Task UpdateContract(ContractUpdateModel contract) => ContractDAO.Instance.UpdateContract(contract);
    }
}
