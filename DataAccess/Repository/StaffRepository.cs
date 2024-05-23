using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class StaffRepository : IStaffRepository
    {
        public Task<ResultModel> AddStaff(Guid ownerId, UserReqModel user) => StaffDAO.Instance.AddStaff(ownerId, user);

        public Task<IEnumerable<User>> GetAllStaffByOwnerId(Guid ownerId)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetStaffById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
