using BusinessObject.Object;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.UserModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class StaffDAO
    {
        private static StaffDAO instance = null;
        private static readonly object instanceLock = new object();
        private StaffDAO() { }
        public static StaffDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new StaffDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<ResultModel> AddStaff(Guid ownerId, UserReqModel user)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultModel> GetAllStaffByOwnerId(Guid ownerUserId)
        {
            using var context = new RmsContext();
           
            var staffUsers = await context.Users
                                           .Where(user => user.Role == "Staff" && user.OwnerId == ownerUserId)
                                           .ToListAsync();
            return new ResultModel
            {
                IsSuccess = true,
                Data = staffUsers
            };
        }

        public async Task<ResultModel> GetStaffById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
