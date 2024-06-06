using DataAccess.DAO;
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
        public Task<ResultModel> GetContractList(Guid userId) => ContractDAO.Instance.GetContractList(userId);
    }
}
