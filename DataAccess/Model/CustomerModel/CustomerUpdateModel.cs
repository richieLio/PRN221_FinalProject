﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.CustomerModel
{
    public class CustomerCreateReqModel
    {


        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }

        public string? FullName { get; set; }
        /// Biển số xe
        public string? LicensePlates { get; set; }
        public string? Role { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? CitizenIdNumber { get; set; }
        // Ngày kết thúc hợp đồng
        public DateTime EndDate { get; set; }
        public Guid RoomId { get; set; }
    }
    public class RoomUserCreateReqModel
    {
        public Guid RoomId { set; get; }

        public Guid CustomerId { set; get; }
    }


    public class CustomerUpdateModel
    {
        public Guid Id { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public DateTime? Dob { get; set; }

        public string? FullName { get; set; }

        public string? LicensePlates { get; set; }

        public string? Status { get; set; }

        public string? CitizenIdNumber { get; set; }


    }
}
