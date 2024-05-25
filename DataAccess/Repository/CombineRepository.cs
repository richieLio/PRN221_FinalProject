using BusinessObject.Object;
using DataAccess.Model.CustomerModel;
using DataAccess.Model.EmailModel;
using DataAccess.Model.HouseModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.RoomModel;
using DataAccess.Model.ServiceFeeModel;
using DataAccess.Model.UserModel;
using DataAccess.Model.VerifyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CombineRepository : ICombineRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly IHouseRepository _houseRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IServiceFeeRepository _serviceFeeRepository;


        public CombineRepository(IUserRepository userRepository, IHouseRepository houseRepository,
            IRoomRepository roomRepository, IStaffRepository staffRepository
            , ICustomerRepository customerRepository, IServiceFeeRepository serviceFeeRepository
            )
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _houseRepository = houseRepository;
            _roomRepository = roomRepository;
            _staffRepository = staffRepository;
            _serviceFeeRepository = serviceFeeRepository;
        }
        public Task<ResultModel> AddCustomerToRoom(Guid userId, AddCustomerToRoomReqModel addCustomerToRoomReqModel)
            => _customerRepository.AddCustomerToRoom(userId, addCustomerToRoomReqModel);

        public Task<ResultModel> AddHouse(Guid ownerId, HouseCreateReqModel formData)
            => _houseRepository.AddHouse(ownerId, formData);

        public Task<ResultModel> AddNewService(Guid userId, ServiceCreateReqModel service)
            => _serviceFeeRepository.AddNewService(userId, service);

        public Task<ResultModel> AddRoom(Guid userId, RoomCreateReqModel roomCreateReqModel)
            => _roomRepository.AddRoom(userId, roomCreateReqModel);

        public Task<ResultModel> AddStaff(Guid ownerId, UserReqModel user)
            => _staffRepository.AddStaff(ownerId, user);

        public Task<bool> AddStaffToHouse(Guid staffId, Guid houseId)
             => _staffRepository.AddStaffToHouse(staffId, houseId);

        public Task<bool> AddUserToRoom(Guid userId, Guid roomId)
            => _roomRepository.AddUserToRoom(userId, roomId);

        public Task CreateAccount(UserReqModel RegisterForm)
            => _userRepository.CreateAccount(RegisterForm);

        public Task<ResultModel> DeleteCustomer(Guid customerId)
            => _customerRepository.DeleteCustomer(customerId);

        public Task<ResultModel> DeleteHouse(Guid OwnerId, Guid houseId)
             => _houseRepository.DeleteHouse(OwnerId, houseId);

        public Task<ResultModel> DeleteRoom(Guid houseId, Guid roomId)
            => (_roomRepository.DeleteRoom(houseId, roomId));

        public Task<IEnumerable<House>> GetAllHouseByStaffId(Guid staffId)
            => _staffRepository.GetAllHouseByStaffId(staffId);

        public Task<IEnumerable<User>> GetAllStaffByOwnerId(Guid ownerId)
            => _staffRepository.GetAllStaffByOwnerId(ownerId);

        public Task<ResultModel> GetAssignedStaffByHouseId(Guid houseId)
            => _staffRepository.GetAssignedStaffByHouseId(houseId);

        public Task<int?> GetAvailableRoomByHouseId(Guid houseId)
            => _houseRepository.GetAvailableRoomByHouseId(houseId);

        public Task<ResultModel> GetCustomerByRoomId(Guid roomId) 
            => _customerRepository.GetCustomerByRoomId(roomId);

        public Task<ResultModel> GetCustomerProfile(Guid customerId)
            => _customerRepository.GetCustomerProfile(customerId);

        public Task<House> GetHouse(Guid houseId)
            => _houseRepository.GetHouse(houseId);

        public Task<IEnumerable<House>> GetHouses(Guid userId)
 => _houseRepository.GetHouses(userId);


        public Task<IEnumerable<User>> GetListCustomerByRoomId(Guid roomId)
  => _roomRepository.GetListCustomerByRoomId(roomId);

        public Task<Room> GetRoom(Guid roomId)
 => _roomRepository.GetRoom(roomId);

        public Task<Room> GetRoomByName(Guid houseId, string name)
 => _roomRepository.GetRoomByName(houseId, name);

        public Task<int?> GetRoomQuantityByHouseId(Guid houseId)
 => _houseRepository.GetRoomQuantityByHouseId(houseId);

        public Task<IEnumerable<Room>> GetRooms(Guid houseId)
 => _roomRepository.GetRooms(houseId);

        public Task<IEnumerable<Service>> GetServicesList(Guid houseId)
=> _serviceFeeRepository.GetServicesList(houseId);

        public Task<ResultModel> GetStaffById(Guid id)
=> _staffRepository.GetStaffById(id);

        public Task<User> GetUserByEmail(string Email)
=> _userRepository.GetUserByEmail(Email);

        public Task<User> GetUserById(Guid id)
 => _userRepository.GetUserById(id);

        public Task<User> GetUserByVerificationToken(string otp)
 => _userRepository.GetUserByVerificationToken(otp);

        public Task<bool> IsUserInRoom(Guid roomId, string email, string phoneNumber, string licensePlates, string citizenIdNumber)
 => _customerRepository.IsUserInRoom(roomId, email, phoneNumber, licensePlates, citizenIdNumber);

        public Task<ResultModel> Login(UserLoginReqModel userLoginReqModel)
 => _userRepository.Login(userLoginReqModel);

        public Task<ResultModel> RemoveService(Guid userId, Guid serviceId, Guid houseId)
=> _serviceFeeRepository.RemoveService(userId, serviceId, houseId);

        public Task ResetPassword(UserResetPasswordReqModel ResetPasswordReqModel)
=> _userRepository.ResetPassword(ResetPasswordReqModel);

        public Task<ResultModel> SendOTPEmailRequest(SendOTPReqModel sendOTPReqModel)
=> _userRepository.SendOTPEmailRequest(sendOTPReqModel);

        public Task<ResultModel> UpdateHouse(Guid OwnerId, HouseUpdateReqModel houseUpdateReqModel)
 => _houseRepository.UpdateHouse(OwnerId, houseUpdateReqModel);

        public Task<ResultModel> UpdateRoom(RoomUpdateReqModel roomUpdateReqModel)
=> _roomRepository.UpdateRoom(roomUpdateReqModel);

        public Task<ResultModel> UpdateService(Guid userId, ServiceUpdateReqModel serviceUpdateModel)
=> _serviceFeeRepository.UpdateService(userId, serviceUpdateModel);

        public Task<ResultModel> UpdateUserProfile(CustomerUpdateModel customerUpdateModel)
=> _customerRepository.UpdateUserProfile(customerUpdateModel);

        public Task VerifyEmail(EmailVerificationReqModel verificationModel)
=> _userRepository.VerifyEmail(verificationModel);

        public Task<ResultModel> VerifyOTPCode(string email, string otpCode)
=> _userRepository.VerifyOTPCode(email, otpCode);
    }
}
