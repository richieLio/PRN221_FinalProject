﻿using BusinessObject.Object;
using Data.Enums;
using DataAccess.Enums;
using DataAccess.Model.CustomerModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using EmailUltilities = DataAccess.Utilities.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class CustomerDAO
    {
        private static CustomerDAO instance = null;
        private static readonly object instanceLock = new object();
        private CustomerDAO() { }
        public static CustomerDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CustomerDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<ResultModel> GetCustomerProfile(Guid customerId)
        {
            using var context = new RmsContext();
            IUserRepository _userRepository = new UserRepository();
            ResultModel Result = new();
            try
            {
                var user = await _userRepository.GetUserById(customerId);

                if (user == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Not found";
                    return Result;
                }


                var userProfile = new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.PhoneNumber,
                    user.Address,
                    user.Gender,
                    user.Dob,
                    user.LicensePlates,
                    user.CitizenIdNumber,
                };

                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = userProfile;
            }
            catch (Exception e)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return Result;
        }

        public async Task<ResultModel> UpdateUserProfile(CustomerUpdateModel customerUpdateModel)
        {
            using var context = new RmsContext();
            IUserRepository _userRepository = new UserRepository();
            ResultModel Result = new();
            try
            {

                var userToUpdate = await _userRepository.GetUserById(customerUpdateModel.Id);

                if (userToUpdate == null)
                {
                    Result.IsSuccess = false;
                    Result.Code = 400;
                    Result.Message = "Customer not found";
                    return Result;
                }



                userToUpdate.Email = customerUpdateModel.Email;
                userToUpdate.PhoneNumber = customerUpdateModel.PhoneNumber;
                userToUpdate.Address = customerUpdateModel.Address;
                userToUpdate.Gender = customerUpdateModel.Gender;
                userToUpdate.Dob = customerUpdateModel.Dob;
                userToUpdate.FullName = customerUpdateModel.FullName;
                userToUpdate.LicensePlates = customerUpdateModel.LicensePlates;
                userToUpdate.Status = customerUpdateModel.Status;

                context.Update(userToUpdate);
                context.SaveChanges();
                Result.IsSuccess = true;
                Result.Code = 200;
                Result.Data = userToUpdate;
                Result.Message = "Profile updated successfully";
            }
            catch (Exception ex)
            {
                Result.IsSuccess = false;
                Result.Code = 400;
                Result.Message = "User doesn't have the required roles";

            }
            return Result;
        }
        public async Task<ResultModel> GetCustomerByRoomId(Guid roomId)
        {
            ResultModel result = new ResultModel();

            try
            {
                
                var customers = await GetCustomers(roomId);

                var customerModels = customers.Select(c => new CustomerResModel
                {

                    Id = c.Id,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Address = c.Address,
                    Gender = c.Gender,
                    Dob = c.Dob.HasValue == true ? c.Dob.Value.ToString("dd/MM/yyyy") : null,
                    FullName = c.FullName,
                    LicensePlates = c.LicensePlates,
                    CreatedAt = c.CreatedAt.HasValue == true ? c.CreatedAt.Value.ToString("dd/MM/yyy") : null,
                    CitizenIdNumber = c.CitizenIdNumber,
                    // Map other properties accordingly
                }).ToList();
                if (customers == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "There is no customer in this room";
                    return result;
                }
                else
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Data = customers;
                }

            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<List<User>> GetCustomers(Guid roomId)
        {
            using var context = new RmsContext();
            return await context.Users
               .Where(u => u.Rooms.Any(r => r.Id == roomId))
               .ToListAsync();
        }

        public async Task<ResultModel> AddCustomerToRoom(Guid userId, AddCustomerToRoomReqModel addCustomerToRoomReqModel)
        {
            using var context = new RmsContext();
            IUserRepository _userRepository = new UserRepository();
            IRoomRepository _roomRepository = new RoomRepository();
            IHouseRepository _houseRepository = new HouseRepository();
            ResultModel result = new();
            try
            {
                var customerCreateReqModel = addCustomerToRoomReqModel.customerCreateReqModel;
                var houseUpdateAvaiableRoom = addCustomerToRoomReqModel.houseUpdateAvaiableRoom;
                var room = await _roomRepository.GetRoom(customerCreateReqModel.RoomId);

                var user = await _userRepository.GetUserById(userId);
                var house = await _houseRepository.GetHouse(houseUpdateAvaiableRoom.HouseId);
                int? availableRoom = await _houseRepository.GetAvailableRoomByHouseId(houseUpdateAvaiableRoom.HouseId);

                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found.";
                    return result;
                }
                if (house == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "House not found.";
                    return result;
                }
               /* var check = await _userRepository.CheckIfCustomerIsExisted(customerCreateReqModel.RoomId, customerCreateReqModel.Email, customerCreateReqModel.PhoneNumber,
                    customerCreateReqModel.CitizenIdNumber, customerCreateReqModel.LicensePlates);
                if (check != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "User is existed";
                    return result;
                }*/
                
                    // thêm thông tin khách

                    var newUser = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = customerCreateReqModel.Email,
                        PhoneNumber = customerCreateReqModel.PhoneNumber,
                        Address = customerCreateReqModel.Address,
                        Gender = customerCreateReqModel.Gender,
                        Dob = customerCreateReqModel.Dob,
                        FullName = customerCreateReqModel.FullName,
                        LicensePlates = customerCreateReqModel.LicensePlates,
                        Status = GeneralStatus.ACTIVE,
                        CreatedAt = DateTime.Now,
                        Role = UserEnum.CUSTOMER,
                        CitizenIdNumber = customerCreateReqModel.CitizenIdNumber
                    };


                    context.Add(newUser);
                    context.SaveChanges();
                    
                    // thêm khách vào phòng
                    var isAddedToRoom = await _roomRepository.AddUserToRoom(newUser.Id, customerCreateReqModel.RoomId);
                    if (!isAddedToRoom)
                    {
                        result.IsSuccess = false;
                        result.Code = 404;
                        result.Message = "Failed to add customer to room. Room not found or user already exists in the room.";
                        return result;
                    }

                    // thêm hợp đồng
                    var contract = new Contract
                    {
                        Id = Guid.NewGuid(),
                        OwnerId = userId,
                        CustomerId = newUser.Id,
                        RoomId = customerCreateReqModel.RoomId,
                        StartDate = DateTime.Now,
                        EndDate = customerCreateReqModel.EndDate,
                        Status = GeneralStatus.ACTIVE,
                    };
                     context.Add(contract);
                    context.SaveChanges();

                    // sửa số phòng còn trống
                    if (room.Status == RoomStatus.EMPTY)
                    {
                        house.AvailableRoom = availableRoom - 1;
                        context.Update(house);
                        context.SaveChanges();
                    }

                    //gửi mail mật khẩu cấp 2 cho khách


                    string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataAccess", "TemplateEmail", "Welcome.html");
                    string newPath = FilePath.Replace("WPF\\bin\\Debug\\net8.0-windows\\", "");
                    string Html = await File.ReadAllTextAsync(newPath);

                    Html = Html.Replace("{{RoomName}}", $"{room.Name}");
                    bool emailSent = await EmailUltilities.SendEmail(customerCreateReqModel.Email, "Email Notification", Html);

                    if (!emailSent)
                    {
                        // Xử lý trường hợp gửi email không thành công
                        result.IsSuccess = false;
                        result.Code = 500;
                        result.Message = "Email cannot be send.";
                        return result;
                    }
                    //update status
                    room.Status = RoomStatus.ENTIRE;
                    context.Update(room);
                    context.SaveChanges();

                    result.IsSuccess = true;
                    result.Code = 200;
                    result.Message = "Customer added to room successfully.";
                    
                
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 500;
                result.Message = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
