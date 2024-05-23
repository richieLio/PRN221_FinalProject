﻿using BusinessObject.Object;
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
        Task<IEnumerable<User>> GetAllStaffByOwnerId(Guid ownerId);

        Task<User> GetStaffById(Guid id);
        Task <ResultModel> AddStaff(Guid ownerId, UserReqModel user);
    }
}
