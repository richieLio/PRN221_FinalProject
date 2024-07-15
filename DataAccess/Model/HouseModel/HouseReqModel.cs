using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.HouseModel
{
    public class HouseReqModel
    {
    }
    public class HouseCreateReqModel
    {
        [Required(ErrorMessage = "Name is required")]

        public string? Name { get; set; }
        [Required(ErrorMessage = "Address is required")]

        public string? Address { get; set; }
    }
    public class HouseUpdateReqModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is required")]

        public string? Name { get; set; }
        [Required(ErrorMessage = "Address is required")]

        public string? Address { get; set; }
    }
    public class HouseUpdateAvaiableRoomReqModel
    {
        public Guid HouseId { get; set; }
        public int? AvailableRoom { get; set; }

    }
}
