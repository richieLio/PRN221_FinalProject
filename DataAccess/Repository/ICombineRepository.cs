using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface ICombineRepository 
        : IUserRepository, 
        IStaffRepository, 
        ICustomerRepository, 
        IHouseRepository, 
        IRoomRepository, 
        IServiceFeeRepository,
        IBillRepository
    {
    }
}
