using DataAccess.Model.CustomerModel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        [CustomEndDateValidation(ErrorMessage = "End date must be in the future.")]
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "FileUpload is required.")]
        public string? FileUrl { get; set; }

    }

    public class ContractInfoResModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Customer Name is required.")]

        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Room Name is required.")]

        public string RoomName { get; set; }
        [Required(ErrorMessage = "House  Name is required.")]

        public string HouseName { get; set; }
        [Required(ErrorMessage = "Start date is required.")]

        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "End date is required.")]

        [CustomEndDateValidation(ErrorMessage = "End date must be in the future.")]

        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "FileUpload is required.")]

        public string? FileUrl { get; set; }

    }


}
