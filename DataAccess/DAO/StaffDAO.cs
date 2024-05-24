using AutoMapper;
using BusinessObject.Object;
using DataAccess.Enums;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.UserModel;
using DataAccess.Repository;
using Encoder = DataAccess.Utilities.Encoder;
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

        public async Task<ResultModel> AddStaff(Guid ownerId, UserReqModel RegisterForm)
        {
            IUserRepository userRepository = new UserRepository();
            using var context = new RmsContext();
            try
            {
                var user = await userRepository.GetUserByEmail(RegisterForm.Email);

                if (user != null)
                {
                    return new ResultModel { IsSuccess = false, Message = "This email already exists!" };

                }
                else
                {

                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<UserReqModel, User>().ForMember(dest => dest.Password, opt => opt.Ignore());
                    });
                    IMapper mapper = config.CreateMapper();
                    User NewUser = mapper.Map<UserReqModel, User>(RegisterForm);
                    if (RegisterForm.Password == null)
                    {
                        RegisterForm.Password = Encoder.GenerateRandomPassword();
                    }
                    NewUser.Id = Guid.NewGuid();
                    NewUser.Status = UserStatus.ACTIVE;
                    NewUser.CreatedAt = DateTime.Now;
                    NewUser.Role = UserEnum.STAFF;
                    var HashedPasswordModel = Encoder.CreateHashPassword(RegisterForm.Password);
                    NewUser.Password = HashedPasswordModel.HashedPassword;
                    NewUser.Salt = HashedPasswordModel.Salt;
                    NewUser.Email = RegisterForm.Email;
                    NewUser.PhoneNumber = RegisterForm.PhoneNumber;
                    NewUser.Address = RegisterForm.Address;
                    NewUser.Gender = RegisterForm.Gender;
                    NewUser.Dob = RegisterForm.Dob;
                    NewUser.OwnerId = ownerId;
                    context.Add(NewUser);
                    context.SaveChanges();
                    return new ResultModel { IsSuccess = true, Message = "Create staff sucessfully" };
                }
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace
                };
            }
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

        public async Task<bool> AddStaffToHouse(Guid staffId, Guid houseId)
        {
            IHouseRepository houseRepository = new HouseRepository();
            using var context = new RmsContext();

            var house = await houseRepository.GetHouse(houseId);
            if (house == null)
            {
                return false;
            }
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == staffId);
            if (user == null)
            {
                return false;
            }

            if (house.Staff.Any(u => u.Id == staffId))
                return false;
            house.Staff.Add(user);
            context.Update(house);
            context.SaveChanges();

            return true;
        }
        public async Task<ResultModel> GetAssignedStaffByHouseId(Guid houseId)
        {
            using var context = new RmsContext();
            try
            {
                var house = await context.Houses
                                         .Include(h => h.Staff)
                                         .FirstOrDefaultAsync(h => h.Id == houseId);

                if (house == null || !house.Staff.Any())
                {
                    return new ResultModel { IsSuccess = false, Message = "No staff assigned to this house." };
                }

                var staff = house.Staff.First();

                return new ResultModel
                {
                    IsSuccess = true,
                    Data = staff
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace
                };
            }
        }
    }
}
