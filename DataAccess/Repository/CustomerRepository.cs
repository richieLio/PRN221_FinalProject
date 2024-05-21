using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.CustomerModel;
using DataAccess.Model.OperationResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        public Task<ResultModel> GetCustomerByRoomId(Guid roomId) => CustomerDAO.Instance.GetCustomerByRoomId(roomId);
       

        public Task<ResultModel> GetCustomerProfile(Guid customerId) => CustomerDAO.Instance.GetCustomerProfile(customerId);

        public Task<ResultModel> UpdateUserProfile(CustomerUpdateModel customerUpdateModel) => CustomerDAO.Instance.UpdateUserProfile(customerUpdateModel);
    }
}
