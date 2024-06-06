using AutoMapper;
using BusinessObject.Object;
using DataAccess.Enums;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.ServiceFeeModel;
using DataAccess.Repository;
using DataAccess.Repository.UserRepostory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class ServiceFeeDAO
    {
        private static ServiceFeeDAO instance = null;
        private static readonly object instanceLock = new object();
        private ServiceFeeDAO() { }
        public static ServiceFeeDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ServiceFeeDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<ResultModel> AddNewService(Guid userId, ServiceCreateReqModel serviceReqModel)
        {
            using var context = new RmsContext();
            IUserRepository _userRepository = new UserRepository();
            ResultModel Result = new();
            try
            {


                var user = await _userRepository.GetUserById(userId);

                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "User not found.";
                    return Result;
                }
                if (user.Role == UserEnum.STAFF)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Staff cannot create services";
                    return Result;
                }


                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ServiceCreateReqModel, Service>();
                });
                IMapper mapper = config.CreateMapper();
                Service newService = mapper.Map<ServiceCreateReqModel, Service>(serviceReqModel);
                newService.Id = Guid.NewGuid();
                newService.Name = serviceReqModel.Name;
                newService.Price = serviceReqModel.Price;
                newService.CreatedBy = user.Id;


                context.Add(newService);
                context.SaveChanges();
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = newService;
                Result.Message = "Create service successfully!";



            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<IEnumerable<Service>> GetServicesList(Guid houseId)
        {
            using var context = new RmsContext();

            var services = await context.Services
                .Where(s => s.HouseId == houseId)
                .ToListAsync();
            return services;

        }


        public async Task<ResultModel> RemoveService(Guid userId, Guid serviceId, Guid houseId)
        {
            using var context = new RmsContext();
            IUserRepository _userRepository = new UserRepository();

            var user = await _userRepository.GetUserById(userId);
            if (user.Role == UserEnum.STAFF)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = "Staff cannot remove services"
                };
            }

            var service = await context.Services
                .FirstOrDefaultAsync(s => s.Id == serviceId && s.HouseId == houseId && s.CreatedBy == userId);
            context.Services.Remove(service);
            await context.SaveChangesAsync();
            return new ResultModel
            {
                IsSuccess = true,
                Message = "Service deleted successfully"
            };
        }

        public async Task<ResultModel> UpdateService(Guid userId, ServiceUpdateReqModel serviceUpdateModel)
        {
            using var context = new RmsContext();
            IUserRepository _userRepository = new UserRepository();

            var user = await _userRepository.GetUserById(userId);
            if (user.Role == UserEnum.STAFF)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = "Staff cannot update services"
                };
            }


            var service = await context.Services
                .FirstOrDefaultAsync(s => s.Id == serviceUpdateModel.Id
                && s.HouseId == serviceUpdateModel.HouseId
                && s.CreatedBy == userId);
            if (service == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = "Service not found"
                };
            }
            service.Price = serviceUpdateModel.Price;
            service.Name = serviceUpdateModel.Name;
            context.Services.Update(service);
            await context.SaveChangesAsync();
            return new ResultModel
            {
                IsSuccess = true,
                Message = "Service deleted successfully"
            };
        }

        public async Task<Service> GetServiceById(Guid serviceId)
        {
            using var context = new RmsContext();
            return await context.Services.FindAsync(serviceId);

        }
    }
}
