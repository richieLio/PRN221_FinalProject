using BusinessObject.Object;
using DataAccess.Model.HouseModel;
using DataAccess.Model.OperationResultModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.HouseRepository
{
    public interface IHouseRepository
    {
        Task<IEnumerable<House>> GetHouses(Guid userId);
        Task<House> GetHouse(Guid houseId);
        public Task<ResultModel> AddHouse(Guid ownerId, HouseCreateReqModel formData);
        Task<ResultModel> UpdateHouse(Guid OwnerId, HouseUpdateReqModel houseUpdateReqModel);
        Task<ResultModel> DeleteHouse(Guid OwnerId, Guid houseId);

        Task<int?> GetRoomQuantityByHouseId(Guid houseId);
        Task<int?> GetAvailableRoomByHouseId(Guid houseId);
        Task<Dictionary<House, List<(DateTime? PaymentDate, decimal Revenue, bool IsPaid)>>> GetMonthlyRevenueByHouseWithPaidStatus(Guid userId, DateTime startDate, DateTime endDate);
       

    }
}
