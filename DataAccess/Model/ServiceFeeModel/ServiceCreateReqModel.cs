﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.ServiceFeeModel
{
    public class ServiceCreateReqModel
    {
        public string? Name { get; set; }

        public decimal? Price { get; set; }

        public Guid HouseId { get; set; }
    }
}
