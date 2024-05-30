using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.BillModel
{
       public class BillCreateReqModel
    {
        public Guid RoomId { get; set; }
        public Dictionary<Guid, decimal> ServiceQuantities { get; set; }
    }
}
