using BusinessObject.Object;
using DataAccess.DAO;
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
    }
}
