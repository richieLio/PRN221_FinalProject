using AutoMapper;
using BusinessObject.Object;
using Data.Enums;
using DataAccess.Enums;
using DataAccess.Model.BillModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Repository;
using DataAccess.Repository.BillRepository;
using DataAccess.Repository.HouseRepository;
using DataAccess.Repository.RoomRepository;
using DataAccess.Repository.ServiceRepository;
using DataAccess.Repository.UserRepostory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class BillDAO
    {
        private static BillDAO instance = null;
        private static readonly object instanceLock = new object();
        private BillDAO() { }
        public static BillDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new BillDAO();
                    }
                    return instance;
                }
            }
        }
        public async Task<ResultModel> getBillDetails(Guid userId, Guid billId)
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

                var bills = await GetBillDetails(userId, billId);
                if (bills == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Bill not found";
                    return result;
                }



                var billServices = await GetBillServicesForBill(billId);
                if (billServices == null)
                {
                    result.IsSuccess = false;
                    result.Code = 404;
                    result.Message = "Bill services not found";
                    return result;
                }
                // take room and house name
                var bill = await GetBillById(billId);

                var room = await _roomRepository.GetRoom(bill.RoomId.Value);
                string roomName = room.Name;
                var house = await _houseRepository.GetHouse(room.HouseId.Value);
                string houseName = house.Name;

                BillDetailsResModel billDetails = new BillDetailsResModel();
                billDetails.Id = billId;
                billDetails.TotalPrice = bills.TotalPrice;
                billDetails.Month = bills.Month;
                billDetails.IsPaid = bills.IsPaid;
                billDetails.PaymentDate = bills.PaymentDate;
                billDetails.RoomName = roomName;
                billDetails.HouseName = houseName;

                if (billServices != null)
                {
                    billDetails.Services = billServices.Select(bs => new BillServiceDetails
                    {
                        ServiceName = bs.Service?.Name,
                        Quantity = bs.Quantity
                    }).ToList();
                }

                result.IsSuccess = true;
                result.Code = 200;
                result.Data = billDetails;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 500; // Internal Server Error
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<Bill> GetBillDetails(Guid userId, Guid billId)
        {
            using var _context = new RmsContext();
            var billDetails = await (from bill in _context.Bills
                                     where bill.Id == billId && bill.CreateBy == userId
                                     join room in _context.Rooms on bill.RoomId equals room.Id
                                     join user in _context.Users on bill.CreateBy equals user.Id
                                     join billService in _context.BillServices on bill.Id equals billService.BillId into services
                                     select new Bill
                                     {
                                         Id = bill.Id,
                                         Room = room,
                                         CreateBy = user.Id,
                                         Month = bill.Month,
                                         PaymentDate = bill.PaymentDate,
                                         TotalPrice = bill.TotalPrice,
                                         BillServices = services.ToList()
                                     }).FirstOrDefaultAsync();

            return billDetails;
        }

        public async Task<ResultModel> GetAllBills(Guid userId, Guid houseId, Guid roomId)
        {
            using var context = new RmsContext();
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
                IEnumerable<Bill> bills;

                if (user.Role == UserEnum.STAFF)
                {
                    bills = await GetBillsByStaffUserId(userId, roomId);
                }
                else
                {
                    bills = await GetAllBillsByOwnerUserId(userId, roomId);
                }
                List<BillResModel> billList = new List<BillResModel>();

                foreach (var bill in bills)
                {
                    var room = await _roomRepository.GetRoom(roomId);
                    var house = await _houseRepository.GetHouse(houseId);
                    string houseName = house.Name;
                    string roomName = room.Name;
                    BillResModel bl = new BillResModel()
                    {
                        Id = bill.Id,
                        TotalPrice = bill.TotalPrice,
                        Month = bill.Month,
                        IsPaid = bill.IsPaid,
                        PaymentDate = bill.PaymentDate,
                        CreateBy = bill.CreateBy,
                        RoomId = bill.RoomId,
                        RoomName = roomName,
                        HouseName = houseName
                    };
                    billList.Add(bl);
                }



                result.IsSuccess = true;
                result.Code = 200;
                result.Data = billList;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 500; // Internal Server Error
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ResultModel> CreateBill(Guid userId, BillCreateReqModel billCreateReqModel)
        {
            using var context = new RmsContext();
            IUserRepository _userRepository = new UserRepository();
            IRoomRepository _roomRepository = new RoomRepository();
            IServiceFeeRepository _serviceFeeRepository = new ServiceFeeRepository();
            ResultModel result = new ResultModel();
            try
            {
                var user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "User not found."
                    };
                }

                var room = await _roomRepository.GetRoom(billCreateReqModel.RoomId);
                if (room.Status == RoomStatus.EMPTY)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Empty room cannot create bill"
                    };
                }

                var config = new MapperConfiguration(cfg => cfg.CreateMap<BillCreateReqModel, Bill>());
                IMapper mapper = config.CreateMapper();
                Bill newBill = mapper.Map<BillCreateReqModel, Bill>(billCreateReqModel);

                // Thêm bill
                newBill.Id = Guid.NewGuid();
                newBill.Month = DateTime.Now;
                newBill.IsPaid = false;
                newBill.CreateBy = userId;
                newBill.RoomId = billCreateReqModel.RoomId;
                context.Add(newBill);
                await context.SaveChangesAsync();

                var serviceQuantities = billCreateReqModel.ServiceQuantities;


                if (!await AddServicesToBill(newBill.Id, serviceQuantities, room.Price))
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "Failed to add service to bill."
                    };
                }
                await context.SaveChangesAsync();

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Data = newBill,
                    Message = "Bill created successfully!"
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 400,
                    ResponseFailed = e.InnerException?.Message + "\n" + e.StackTrace ?? e.Message + "\n" + e.StackTrace
                };
            }
        }

        private async Task<bool> AddServicesToBill(Guid billId, Dictionary<Guid, decimal> serviceQuantities, decimal? roomPrice)
        {
            using var context = new RmsContext();
            var bill = await context.Bills.FindAsync(billId);
            if (bill == null)
                return false;
            decimal? totalPrice = 0;

            foreach (var (serviceId, quantity) in serviceQuantities)
            {
                var service = context.Services.FirstOrDefault(s => s.Id == serviceId);
                if (service == null)
                    return false;

                if (bill.BillServices.Any(s => s.ServiceId == serviceId))
                    return false;

                bill.BillServices.Add(new BillService
                {
                    BillId = billId,
                    ServiceId = serviceId,
                    Quantity = quantity
                });
                totalPrice = quantity * service.Price + roomPrice;
            }
            bill.TotalPrice = totalPrice;
            context.Update(bill);
            await context.SaveChangesAsync();

            return true;
        }
        public async Task<ResultModel> RemoveBill(Guid userId, Guid billId)
        {
            using var context = new RmsContext();
            IUserRepository _userRepository = new UserRepository();
            ResultModel result = new ResultModel();
            try
            {
                var user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "User not found."
                    };
                }

                var bill = await context.Bills
                                        .Include(b => b.BillServices)
                                        .FirstOrDefaultAsync(b => b.Id == billId);

                if (bill == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "Bill not found."
                    };
                }

                // Remove associated services
                context.BillServices.RemoveRange(bill.BillServices);

                // Remove the bill
                context.Bills.Remove(bill);

                await context.SaveChangesAsync();

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Bill removed successfully!"
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 400,
                    ResponseFailed = e.InnerException?.Message + "\n" + e.StackTrace ?? e.Message + "\n" + e.StackTrace
                };
            }
        }



        public async Task<Bill> GetBillById(Guid billId)
        {
            using (var context = new RmsContext())
            {
                return await context.Bills
                    .Include(b => b.BillServices)
                    .FirstOrDefaultAsync(b => b.Id == billId);
            }
        }

        public async Task<List<BillService>> GetBillServicesForBill(Guid billId)
        {
            using var context = new RmsContext();
            return await context.BillServices
                .Where(bs => bs.BillId == billId)
                .Include(bs => bs.Service)
                .ToListAsync();
        }
        public async Task<IEnumerable<Bill>> GetBillsByStaffUserId(Guid staffUserId, Guid roomId)
        {
            using var context = new RmsContext();

            var staffHouses = await context.Houses
                .Where(h => h.Staff.Any(s => s.Id == staffUserId))
                .Select(h => h.Id)
                .ToListAsync();

            return await context.Bills
                .Include(b => b.Room)
                .ThenInclude(r => r.House)
                .Where(b => b.Room.HouseId != null && staffHouses.Contains(b.Room.HouseId.Value))
                .Where(b => b.RoomId == roomId)
                .OrderByDescending(b => b.Month)
                .ToListAsync();
        }
        public async Task<IEnumerable<Bill>> GetAllBillsByOwnerUserId(Guid ownerId,Guid roomId)
        {
            using var context = new RmsContext();
            var staffIds = await context.Users
                                        .Where(u => u.OwnerId == ownerId)
                                        .Select(u => u.Id)
                                        .ToListAsync();

            return await context.Bills
                                .Where(b => b.CreateBy == ownerId || staffIds.Contains(b.CreateBy.Value))
                                .Where(b => b.RoomId == roomId)
                                .OrderByDescending(b => b.Month)
                                .ToListAsync();
        }



        public async Task<ResultModel> GetBillByRoomID(Guid roomId)
        {
            using var context = new RmsContext();
            IRoomRepository _roomRepository = new RoomRepository();
            IHouseRepository _houseRepository = new HouseRepository();

            ResultModel result = new ResultModel();
            try
            {


                IEnumerable<Bill> bills = await GetAllBillsRoomId(roomId);

                List<BillResModel> billList = new List<BillResModel>();

                foreach (var bill in bills)
                {
                    var room = await _roomRepository.GetRoom(roomId);
                    var house = await _houseRepository.GetHouse(room.HouseId.Value);
                    string houseName = house.Name;
                    string roomName = room.Name;
                    BillResModel bl = new BillResModel()
                    {
                        Id = bill.Id,
                        TotalPrice = bill.TotalPrice,
                        Month = bill.Month,
                        IsPaid = bill.IsPaid,
                        PaymentDate = bill.PaymentDate,
                        CreateBy = bill.CreateBy,
                        RoomId = bill.RoomId,
                        RoomName = roomName,
                        HouseName = houseName
                    };
                    billList.Add(bl);
                }



                result.IsSuccess = true;
                result.Code = 200;
                result.Data = billList;
                return result;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Code = 500; // Internal Server Error
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<IEnumerable<Bill>> GetAllBillsRoomId(Guid roomId)
        {
            using var context = new RmsContext();
            return await context.Bills.Where(b => b.RoomId == roomId).ToListAsync();

        }

        public async Task<ResultModel> UpdateBill(Guid userId, BillUpdateReqModel billUpdateReqModel)
        {
            using var context = new RmsContext();
            ResultModel result = new ResultModel();
            IRoomRepository _roomRepository = new RoomRepository();
            IUserRepository _userRepository = new UserRepository();

            try
            {
                var bill = await context.Bills.Include(b => b.BillServices).FirstOrDefaultAsync(b => b.Id == billUpdateReqModel.BillId);
                if (bill == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "Bill not found."
                    };
                }

                var user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 404,
                        Message = "User not found."
                    };
                }

                var room = await _roomRepository.GetRoom(bill.RoomId.Value);
                if (room.Status == RoomStatus.EMPTY)
                {
                    return new ResultModel
                    {
                        IsSuccess = false,
                        Code = 400,
                        Message = "Empty room cannot update bill"
                    };
                }

                var serviceQuantities = billUpdateReqModel.ServiceQuantities;

                decimal? totalPrice = 0;

                foreach (var (serviceId, quantity) in serviceQuantities)
                {
                    var service = await context.Services.FindAsync(serviceId);
                    if (service == null)
                    {
                        return new ResultModel
                        {
                            IsSuccess = false,
                            Code = 404,
                            Message = "Service not found."
                        };
                    }

                    var existingBillService = bill.BillServices.FirstOrDefault(bs => bs.ServiceId == serviceId);
                    if (existingBillService != null)
                    {
                        if (quantity == 0)
                        {
                            context.BillServices.Remove(existingBillService); // Remove service if quantity is 0
                        }
                        else
                        {
                            existingBillService.Quantity = quantity; // Update quantity if service exists
                        }
                    }
                    else if (quantity > 0)
                    {
                        bill.BillServices.Add(new BillService // Add new service if quantity is greater than 0 and not already exists
                        {
                            BillId = billUpdateReqModel.BillId,
                            ServiceId = serviceId,
                            Quantity = quantity
                        });
                    }

                    totalPrice = quantity * service.Price + room.Price;
                }

                bill.TotalPrice = totalPrice;
                await context.SaveChangesAsync();

                return new ResultModel
                {
                    IsSuccess = true,
                    Code = 200,
                    Message = "Bill updated successfully!"
                };
            }
            catch (Exception e)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 400,
                    ResponseFailed = e.InnerException?.Message + "\n" + e.StackTrace ?? e.Message + "\n" + e.StackTrace
                };
            }
        }

        public async Task<ResultModel> UpdateBillStatus(Guid userId, BillUpdateStatusReqModel billUpdateStatusReqModel)
        {
            using var context = new RmsContext();
            IUserRepository _userRepository = new UserRepository();
            IBillRepository _billRepository = new BillRepository();

            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    Code = 404,
                    Message = "User not found."
                };
            }
            var bill = await _billRepository.getBillById(billUpdateStatusReqModel.BillId);

            bill.IsPaid = billUpdateStatusReqModel.Status;
            bill.PaymentDate = billUpdateStatusReqModel.PaymentDay;
            context.Update(bill);
            await context.SaveChangesAsync();

            return new ResultModel
            {
                IsSuccess = true,
                Code = 200,
                Message = "Bill Updated successfully"
            };

        }
    }
}
