using BusinessObject.Object;
using DataAccess.DAO;
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
        public Task<IEnumerable<Room>> GetRooms(Guid houseId) => RoomDAO.Instance.GetRooms(houseId);
    }
}
