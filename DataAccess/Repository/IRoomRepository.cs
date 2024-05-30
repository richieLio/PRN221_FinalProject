using BusinessObject.Object;
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
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetRooms(Guid houseId);
        Task<Room> GetRoom(Guid roomId);
        Task<Room> GetRoomByName(Guid houseId, string name);
        Task<ResultModel> AddRoom(Guid userId, RoomCreateReqModel roomCreateReqModel);
        Task<IEnumerable<User>> GetListCustomerByRoomId(Guid roomId);
        Task<ResultModel> UpdateRoom(RoomUpdateReqModel roomUpdateReqModel);
        Task<ResultModel> DeleteRoom(Guid houseId, Guid roomId);
        Task<bool> AddUserToRoom(Guid userId, Guid roomId);
    }
}
