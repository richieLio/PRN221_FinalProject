using BusinessObject.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class HouseDAO
    {
        private static HouseDAO instance = null;
        private static readonly object instanceLock = new object();
        private HouseDAO() { }
        public static HouseDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new HouseDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<IEnumerable<House>> GetHouses(Guid userId)
        {
            using var context = new RmsContext();
            List<House> userHouses = context.Houses.Where(h => h.OwnerId == userId).ToList();
            return userHouses;
        }
    }
}
