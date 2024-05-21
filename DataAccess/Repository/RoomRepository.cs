using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.RoomModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class RoomRepository : IRoomRepository
    {
        public Task<ResultModel> AddRoom(Guid userId, RoomCreateReqModel roomCreateReqModel) => RoomDAO.Instance.AddRoom(userId, roomCreateReqModel);

        public Task<Room> GetRoomByName(Guid houseId, string name) => RoomDAO.Instance.GetRoomByName(houseId, name);

        public Task<IEnumerable<Room>> GetRooms(Guid houseId) => RoomDAO.Instance.GetRooms(houseId);
        public Task<IEnumerable<User>> GetListCustomerByRoomId(Guid roomId) => RoomDAO.Instance.GetListCustomerByRoomId(roomId);

        public Task<ResultModel> UpdateRoom(RoomUpdateReqModel roomUpdateReqModel) => RoomDAO.Instance.UpdateRoom(roomUpdateReqModel);

        public Task<ResultModel> DeleteRoom(Guid roomId) => RoomDAO.Instance.DeleteRoom(roomId);
    }
}
