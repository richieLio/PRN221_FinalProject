using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.BillModel;
using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.BillRepository
{
    public class BillRepository : IBillRepository
    {
        public Task<ResultModel> CreateBill(Guid userId, BillCreateReqModel billCreateReqModel)
            => BillDAO.Instance.CreateBill(userId, billCreateReqModel);

        public Task<ResultModel> GetAllBills(Guid userId)
            => BillDAO.Instance.GetAllBills(userId);

        public Task<Bill> getBillById(Guid billId)
            => BillDAO.Instance.GetBillById(billId);
        public Task<ResultModel> GetBillByRoomID(Guid roomId)
            => BillDAO.Instance.GetBillByRoomID(roomId);


        public Task<ResultModel> getBillDetails(Guid userId, Guid billId)
            => BillDAO.Instance.getBillDetails(userId, billId);

        public Task<ResultModel> RemoveBill(Guid userId, Guid billId)
            => BillDAO.Instance.RemoveBill(userId, billId);

        public Task<ResultModel> UpdateBill(Guid userId, BillUpdateReqModel billUpdateReqModel)
            => BillDAO.Instance.UpdateBill(userId, billUpdateReqModel);

        public Task<ResultModel> UpdateBillStatus(Guid userId, BillUpdateStatusReqModel billUpdateStatusReqModel)
            => BillDAO.Instance.UpdateBillStatus(userId, billUpdateStatusReqModel);
    }
}
