using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.HouseModel;
using DataAccess.Model.OperationResultModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class HouseRepository : IHouseRepository
    {
        public Task<IEnumerable<House>> GetHouses(Guid userId) => HouseDAO.Instance.GetHouses(userId);

        public Task<House> GetHouse(Guid houseId) => HouseDAO.Instance.GetHouse(houseId);

        public Task<ResultModel> AddHouse(Guid ownerId, HouseCreateReqModel formData) => HouseDAO.Instance.AddHouse(ownerId, formData);

        public Task<ResultModel> UpdateHouse(Guid OwnerId, HouseUpdateReqModel houseUpdateReqModel) => HouseDAO.Instance.UpdateHouse(OwnerId, houseUpdateReqModel);

        public Task<ResultModel> DeleteHouse(Guid OwnerId, Guid houseId) => HouseDAO.Instance.DeleteHouse(OwnerId, houseId);
        public Task<int?> GetRoomQuantityByHouseId(Guid houseId) => HouseDAO.Instance.GetRoomQuantityByHouseId(houseId);

        public Task<int?> GetAvailableRoomByHouseId(Guid houseId) => HouseDAO.Instance.GetAvailableRoomByHouseId(houseId);

       
    }
}
