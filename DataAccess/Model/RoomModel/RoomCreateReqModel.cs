using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.RoomModel
{
    public class RoomCreateReqModel
    {

        public Guid RoomId { get; set; }
        public Guid HouseId { get; set; }
      
        public decimal? Price { get; set; }
        public string? Name { get; set; }
    }

}
