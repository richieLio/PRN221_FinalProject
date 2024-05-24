using AutoMapper;
using BusinessObject.Object;
using Data.Enums;
using DataAccess.Model.HouseModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{

    public class HouseDAO
    {

        private static HouseDAO instance = null;
        private static readonly object instanceLock = new object();
        private HouseDAO() { }
        public static HouseDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new HouseDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<IEnumerable<House>> GetHouses(Guid userId)
        {
            using var context = new RmsContext();
            List<House> userHouses = context.Houses.Where(h => h.OwnerId == userId).OrderBy(h => h.CreatedAt).ToList();
            return userHouses;
        }

        public async Task<House> GetHouse(Guid houseId)
        {
            using var context = new RmsContext();
            return context.Houses.FirstOrDefault(h => h.Id == houseId);

        }

        public async Task<ResultModel> AddHouse(Guid ownerId, HouseCreateReqModel houseCreateReqModel)
        {
            using var context = new RmsContext();
            ResultModel Result = new();
            try
            {
                var existingHouse = await GetHouseByName(houseCreateReqModel.Name);
                if (existingHouse != null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "House with this name already exists.";
                    return Result;
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<HouseCreateReqModel, House>(); ;
                });
                IMapper mapper = config.CreateMapper();
                House NewHouse = mapper.Map<HouseCreateReqModel, House>(houseCreateReqModel);




                NewHouse.OwnerId = ownerId;
                NewHouse.RoomQuantity = 0;
                NewHouse.AvailableRoom = 0;
                NewHouse.Status = GeneralStatus.ACTIVE;
                NewHouse.CreatedAt = DateTime.Now;

                context.Add(NewHouse);
                context.SaveChanges();

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = NewHouse;
                Result.Message = "House created successfully";
            }


            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }
        public async Task<ResultModel> UpdateHouse(Guid OwnerId, HouseUpdateReqModel houseUpdateReqModel)
        {
            IUserRepository _userRepository = new UserRepository();
            using var context = new RmsContext();
            ResultModel result = new();
            try
            {
                var owner = await _userRepository.GetUserById(OwnerId);
                if (owner == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Not found";
                    return result;
                }
                var house = await GetHouseById(houseUpdateReqModel.Id);
                if (house == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Not found";
                    return result;
                }
                house.OwnerId = OwnerId;
                house.Name = houseUpdateReqModel.Name;
                house.Address = houseUpdateReqModel.Address;
                context.Update(house);
                context.SaveChanges();
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = house;
                result.Message = "House updated successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<House> GetHouseByName(string name)
        {
            using var context = new RmsContext();
            return await context.Houses.FirstOrDefaultAsync(h => h.Name == name);
        }
        public async Task<House> GetHouseById(Guid id)
        {
            using var context = new RmsContext();
            return await context.Houses.FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<ResultModel> DeleteHouse(Guid ownerId, Guid houseId)
        {
            using var context = new RmsContext();
            var house = await context.Houses
                .FirstOrDefaultAsync(h => h.OwnerId == ownerId && h.Id == houseId);

            if (house == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = "House not found."
                };
            }

            context.Houses.Remove(house);
            await context.SaveChangesAsync();

            return new ResultModel
            {
                IsSuccess = true,
                Message = "House deleted successfully."
            };
        }

        public async Task<int?> GetRoomQuantityByHouseId(Guid houseId)
        {
            using var context = new RmsContext();
            var house = await context.Houses.FindAsync(houseId);
            return house?.RoomQuantity;
        }

        public async Task<int?> GetAvailableRoomByHouseId(Guid houseId)
        {
            using var context = new RmsContext();
            var house = await context.Houses.FindAsync(houseId);
            return house?.AvailableRoom;
        }
    }
}
