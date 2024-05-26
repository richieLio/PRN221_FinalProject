using DataAccess.DAO;
using DataAccess.Model.BillModel;
using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class BillRepository : IBillRepository
    {
        public Task<ResultModel> CreateBill(Guid userId, BillCreateReqModel billCreateReqModel)
            => BillDAO.Instance.CreateBill(userId, billCreateReqModel);

        public Task<ResultModel> GetAllBills(Guid userId)
            => BillDAO.Instance.GetAllBills(userId);

        public Task<ResultModel> getBillDetails(Guid userId, Guid billId)
            => BillDAO.Instance.getBillDetails(userId, billId);
    }
}
