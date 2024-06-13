using AutoMapper;
using BusinessObject.Object;
using Data.Enums;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.RoomModel;
using DataAccess.Repository;
using DataAccess.Repository.HouseRepository;
using DataAccess.Repository.UserRepostory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class RoomDAO
    {
        private static RoomDAO instance = null;
        private static readonly object instanceLock = new object();
        private RoomDAO() { }
        public static RoomDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new RoomDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<IEnumerable<Room>> GetRooms(Guid houseId)
        {
            using var context = new RmsContext();
            List<Room> rooms = await context.Rooms
                                    .Where(r => r.HouseId == houseId)
                                    .OrderBy(r => r.CreatedAt)
                                    .ToListAsync();

            return rooms;
        }
        public async Task<ResultModel> AddRoom(Guid userId, RoomCreateReqModel roomCreateReqModel)
        {
            IHouseRepository _houseRepository = new HouseRepository();
            IUserRepository _userRepository = new UserRepository();
            using var context = new RmsContext();

            ResultModel Result = new();
            try
            {
                int? UpdateRoomQuantity = await _houseRepository.GetRoomQuantityByHouseId(roomCreateReqModel.HouseId);
                var house = await _houseRepository.GetHouse(roomCreateReqModel.HouseId);
                var user = await _userRepository.GetUserById(userId);
                var existingRoom = await GetRoomByName(roomCreateReqModel.HouseId, roomCreateReqModel.Name);
                if (existingRoom != null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Room with this name already exists.";
                    return Result;
                }
                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 404;
                    Result.Message = "User not found.";
                    return Result;
                }
                if (house == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "House not found";
                }

                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<RoomCreateReqModel, Room>();
                });
                IMapper mapper = config.CreateMapper();
                Room newRoom = mapper.Map<RoomCreateReqModel, Room>(roomCreateReqModel);

                newRoom.Id = Guid.NewGuid();
                newRoom.HouseId = roomCreateReqModel.HouseId;
                newRoom.Name = roomCreateReqModel.Name;
                newRoom.Status = RoomStatus.EMPTY;
                newRoom.Price = roomCreateReqModel.Price;
                newRoom.CreatedAt = DateTime.Now;
                context.Add(newRoom);
                context.SaveChanges();
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = newRoom;
                Result.Message = "Create rooms successfully!";

                if (UpdateRoomQuantity.HasValue)
                {
                    house.RoomQuantity += 1;
                    house.AvailableRoom += 1;
                    context.Update(house);
                    context.SaveChanges();
                }

            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> UpdateRoom(RoomUpdateReqModel roomUpdateReqModel)
        {
            using var context = new RmsContext();
            ResultModel result = new();
            try
            {
                var Room = await GetRoomById(roomUpdateReqModel.Id);
                if (Room == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Not found";
                    return result;
                }
                Room.Name = roomUpdateReqModel.Name;
                Room.Price = roomUpdateReqModel.Price;
                context.Update(Room);
                context.SaveChanges();
                result.IsSuccess = true;
                result.Code = 200;
                result.Message = "Room updated successfully";
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<Room> GetRoomById(Guid roomId)
        {
            using var context = new RmsContext();
            return await context.Rooms.FindAsync(roomId);
        }

        public async Task<Room> GetRoomByName(Guid houseId, string name)
        {
            using var context = new RmsContext();
            return await context.Rooms.FirstOrDefaultAsync(r => r.Name == name && r.HouseId == houseId);
        }

        public async Task<IEnumerable<User>> GetListCustomerByRoomId(Guid roomId)
        {
            var context = new RmsContext();
            return await context.Users
           .Where(u => u.Rooms.Any(r => r.Id == roomId))
           .ToListAsync();
        }

        public async Task<ResultModel> DeleteRoom(Guid houseId, Guid roomId)
        {
            using var context = new RmsContext();

            // Retrieve the room to delete
            var room = await context.Rooms.FindAsync(roomId);
            if (room == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = "Room not found."
                };
            }

            // Update house room quantity
            var house = await context.Houses.FindAsync(houseId);
            if (house == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Message = "House not found."
                };
            }
            house.RoomQuantity--;
            house.AvailableRoom--;
            context.Houses.Update(house);

            // Remove the room
            context.Rooms.Remove(room);

            // Save changes
            await context.SaveChangesAsync();

            return new ResultModel
            {
                IsSuccess = true,
                Message = "Room deleted successfully."
            };
        }

        public async Task<bool> AddUserToRoom(Guid userId, Guid roomId)
        {
            using var context = new RmsContext();

            var room = await GetRoomById(roomId);
            if (room == null)
                return false;

            var user = context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return false;


            if (room.Users.Any(u => u.Id == userId))
                return false;
            room.Users.Add(user);
            context.Update(room);
            context.SaveChanges();

            return true;
        }
    }
    }



