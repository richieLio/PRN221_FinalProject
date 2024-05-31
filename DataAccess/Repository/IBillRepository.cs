using BusinessObject.Object;
using DataAccess.Model.BillModel;
using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IBillRepository
    {

        Task<ResultModel> CreateBill(Guid userId, BillCreateReqModel billCreateReqModel);
        Task<ResultModel> UpdateBill(Guid userId, BillUpdateReqModel billUpdateReqModel);
        Task<ResultModel> UpdateBillStatus(Guid userId, BillUpdateStatusReqModel billUpdateStatusReqModel);
        Task<ResultModel> GetAllBills(Guid userId);
        Task<ResultModel> GetBillByRoomID(Guid roomId);
        Task<ResultModel> getBillDetails(Guid userId, Guid billId);
        Task<Bill> getBillById(Guid billId);
    }
}
