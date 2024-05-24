using BusinessObject.Object;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IStaffRepository
    {
        Task<ResultModel> GetAllStaffByOwnerId(Guid ownerId);

        Task<ResultModel> GetStaffById(Guid id);
        Task <ResultModel> AddStaff(Guid ownerId, UserReqModel user);

        Task<bool> AddStaffToHouse(Guid staffId, Guid houseId);
        Task<ResultModel> GetAssignedStaffByHouseId(Guid houseId);

        Task<IEnumerable<House>> GetAllHouseByStaffId(Guid staffId);
    }
}
