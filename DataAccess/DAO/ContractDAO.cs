﻿using BusinessObject.Object;
using Data.Enums;
using DataAccess.Enums;
using DataAccess.Model.ContractModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Repository.HouseRepository;
using DataAccess.Repository.RoomRepository;
using DataAccess.Repository.UserRepostory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class ContractDAO
    {
        private static ContractDAO instance = null;
        private static readonly object instanceLock = new object();
        private ContractDAO() { }
        public static ContractDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ContractDAO();
                    }
                    return instance;
                }
            }
        }
        public async Task<IEnumerable<Contract>> GetContracts(Guid userId, Guid houseId)
        {
            using var context = new RmsContext();
            IUserRepository userRepository = new UserRepository();
            var user = await userRepository.GetUserById(userId);

            List<Contract> contracts = new List<Contract>();

            if (user.Role == UserEnum.OWNER)
            {
                contracts = await context.Contracts
                    .Where(c => c.OwnerId == userId && c.Room.HouseId == houseId)
                    .OrderByDescending(h => h.CreatedAt)
                    .ToListAsync();
            }
            else if (user.Role == UserEnum.STAFF)
            {
                contracts = await context.Contracts
                    .Include(c => c.Room)
                    .ThenInclude(r => r.House)
                    .Where(c => c.Room.HouseId == houseId && c.Room.House.Staff.Any(s => s.Id == userId))
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            return contracts;
        }




        public async Task<ResultModel> GetContractList(Guid userId, Guid houseId)
        {
            IUserRepository _userRepository = new UserRepository();
            IRoomRepository _roomRepository = new RoomRepository();
            IHouseRepository _houseRepository = new HouseRepository();
            ResultModel result = new ResultModel();
            try
            {
                var user = await _userRepository.GetUserById(userId); 
                if (user == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "User not found";
                    return result;
                }
               

                var contracts = await GetContracts(userId, houseId);
                List<ContractInfoResModel> contractList = new List<ContractInfoResModel>();

                foreach (var contract in contracts)
                {
                    var owner = await _userRepository.GetUserById(contract.OwnerId.Value); 
                    var customer = await GetCustomerByContractId(contract.Id);
                    var room = await _roomRepository.GetRoom(contract.RoomId.Value); 
                    var house = await _houseRepository.GetHouse(room.HouseId.Value);

                    ContractInfoResModel contractInfo = new ContractInfoResModel
                    {
                        Id = contract.Id,
                        CustomerName = customer.FullName,
                        RoomName = room.Name,
                        HouseName = house.Name,
                        StartDate = contract.StartDate,
                        EndDate = contract.EndDate,
                        FileUrl = contract.FileUrl,
                    };

                    contractList.Add(contractInfo);
                }


                result.IsSuccess = true;
                result.Code = 200;
                result.Data = contractList;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<User> GetCustomerByContractId(Guid? contractId)
        {

            using var context = new RmsContext();
            var contract = await context.Contracts
                                        .Where(c => c.Id == contractId)
                                        .FirstOrDefaultAsync();

            if (contract == null)
                return null;

            var user = await context.Users
                                    .Where(u => u.Id == contract.CustomerId)
                                    .FirstOrDefaultAsync();

            return user;
        }

        public async Task<Contract> GetContract(Guid contractId)
        {
            using var context = new RmsContext();
            return await context.Contracts.FindAsync(contractId);
        }

        public async Task UpdateContract(ContractUpdateModel contractModel)
        {
            using var context = new RmsContext();

            var contract = await context.Contracts.FindAsync(contractModel.Id);
            if (contract == null)
            {
                throw new Exception("Contract not found");
            }

            contract.EndDate = contractModel.EndDate;
            contract.FileUrl = contractModel.FileUrl;

            // Lưu thay đổi
            await context.SaveChangesAsync();
        }

    }
}
