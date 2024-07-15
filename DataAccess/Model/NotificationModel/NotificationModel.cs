using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.NotificationModel
{
    public class NotificationModel
    {
        [Required(ErrorMessage = "Subject is required.")]

        public string? Subject { get; set; }
        [Required(ErrorMessage = "Content is required.")]

        public string? Content { get; set; }

        public string? HouseName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
    public class SendNotificationModel
    {
        [Required(ErrorMessage = "Subject is required.")]

        public string? Subject { get; set; }
        [Required(ErrorMessage = "Content is required.")]

        public string? Content { get; set; }

        public string? HouseName { get; set; }
    }
}
