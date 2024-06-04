using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Model.NotificationModel
{
    public class NotificationModel
    {
        public string? Subject { get; set; }

        public string? Content { get; set; }

        public string? HouseName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
