using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.ContractModel
{
    public class ContractReqModel
    {
        public Guid Id { get; set; }

        public IFormFile ImagesUrl { get; set; }

    }
    public class ContractUpdateModel
    {
        public Guid Id { get; set; }

        public DateTime? EndDate { get; set; }

        public string? FileUrl { get; set; }

    }

    public class ContractInfoResModel
    {
        public Guid Id { get; set; }

        public string CustomerName { get; set; }

        public string RoomName { get; set; }
        public string HouseName { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? FileUrl { get; set; }

    }

   
}
