using BusinessObject.Object;
using DataAccess.Model.CustomerModel;
using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task<ResultModel> UpdateUserProfile(CustomerUpdateModel customerUpdateModel);
        Task<ResultModel> GetCustomerProfile(Guid customerId);
        Task<ResultModel> DeleteCustomer(Guid customerId);
        Task<ResultModel> GetCustomerByRoomId(Guid roomId);
        Task<ResultModel> AddCustomerToRoom(Guid userId, AddCustomerToRoomReqModel addCustomerToRoomReqModel);
        Task<bool> IsUserInRoom(Guid roomId, string email, string phoneNumber, string licensePlates, string citizenIdNumber); // New method

    }
}
