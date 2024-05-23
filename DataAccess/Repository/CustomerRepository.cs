using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.CustomerModel;
using DataAccess.Model.OperationResultModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        public Task<ResultModel> AddCustomerToRoom(Guid userId, AddCustomerToRoomReqModel addCustomerToRoomReqModel) => CustomerDAO.Instance.AddCustomerToRoom(userId, addCustomerToRoomReqModel);

        public Task<ResultModel> GetCustomerByRoomId(Guid roomId) => CustomerDAO.Instance.GetCustomerByRoomId(roomId);


        public Task<ResultModel> GetCustomerProfile(Guid customerId) => CustomerDAO.Instance.GetCustomerProfile(customerId);

        public Task<ResultModel> UpdateUserProfile(CustomerUpdateModel customerUpdateModel) => CustomerDAO.Instance.UpdateUserProfile(customerUpdateModel);

        public Task<bool> IsUserInRoom(Guid roomId, string email, string phoneNumber, string licensePlates, string citizenIdNumber)
            => CustomerDAO.Instance.IsUserInRoom(roomId, email, phoneNumber, licensePlates, citizenIdNumber);

        public Task<ResultModel> DeleteCustomer(Guid customerId) => CustomerDAO.Instance.DeleteCustomer(customerId);
    }
}
