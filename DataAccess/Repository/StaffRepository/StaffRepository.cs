using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.StaffRepository
{
    public class StaffRepository : IStaffRepository
    {
        public Task<ResultModel> AddStaff(Guid ownerId, UserReqModel user) => StaffDAO.Instance.AddStaff(ownerId, user);

        public Task<bool> AddStaffToHouse(Guid staffId, Guid houseId) => StaffDAO.Instance.AddStaffToHouse(staffId, houseId);
        public Task<bool> RemoveStaffFromHouse(Guid staffId, Guid houseId) => StaffDAO.Instance.RemoveStaffFromHouse(staffId, houseId);

        public Task<IEnumerable<User>> GetAllStaffByOwnerId(Guid ownerId) => StaffDAO.Instance.GetAllStaffByOwnerId(ownerId);

        public Task<ResultModel> GetAssignedStaffByHouseId(Guid houseId) => StaffDAO.Instance.GetAssignedStaffByHouseId(houseId);

        public Task<ResultModel> GetStaffById(Guid id) => StaffDAO.Instance.GetStaffById(id);

    }
}
