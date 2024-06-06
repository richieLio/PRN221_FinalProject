using DataAccess.Repository.BillRepository;
using DataAccess.Repository.CombineRepository;
using DataAccess.Repository.ContractRepository;
using DataAccess.Repository.CustomerRepository;
using DataAccess.Repository.HouseRepository;
using DataAccess.Repository.LicenceRepository;
using DataAccess.Repository.LocalNotificationRepository;
using DataAccess.Repository.NotificationRepository;
using DataAccess.Repository.RoomRepository;
using DataAccess.Repository.ServiceRepository;
using DataAccess.Repository.StaffRepository;
using DataAccess.Repository.TransactionRepository;
using DataAccess.Repository.UserRepostory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccess.Repository.CombineRepository
{
    public interface ICombineRepository
        : IUserRepository,
        IStaffRepository,
        ICustomerRepository,
        IHouseRepository,
        IRoomRepository,
        IServiceFeeRepository,
        IBillRepository,
        ILocalNotificationRepository,
        ITransactionRepository,
        ILicenceRepository,
        INotificationRepository,
        IContractRepository
    {

    }
}
