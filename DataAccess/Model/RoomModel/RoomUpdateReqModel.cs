﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.RoomModel
{
    public class RoomUpdateReqModel
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }
        public decimal? Price { get; set; }
    }

    public class RoomUpdateStatusReqModel
    {
        public Guid Id { get; set; }
        public string? Status { get; set; }
    }
}
