using BusinessObject.Object;
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
            List<Room> rooms = context.Rooms.Where(r => r.HouseId== houseId).ToList();
            return rooms;
        }
    }
}
