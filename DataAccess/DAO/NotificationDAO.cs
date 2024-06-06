using BusinessObject.Object;
using DataAccess.Enums;
using DataAccess.Model.CustomerModel;
using DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using EmailUltilities = DataAccess.Utilities.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.NotificationModel;
using DataAccess.Repository.UserRepostory;

namespace DataAccess.DAO
{
    public class NotificationDAO
    {
        private static NotificationDAO instance = null;
        private static readonly object instanceLock = new object();
        private NotificationDAO() { }
        public static NotificationDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new NotificationDAO();
                    }
                    return instance;
                }
            }
        }

        public async Task<IEnumerable<Notification>> GetNotifications(Guid userId)
        {
            using var context = new RmsContext();
            IUserRepository userRepository = new UserRepository();
            IEnumerable<Notification> notifications = new List<Notification>();

            var user = await userRepository.GetUserById(userId);
            if (user.Role == UserEnum.OWNER)
            {
                notifications = await context.Notifications.Include(n => n.House)
                                 .Where(n => n.House.OwnerId == userId)
                                 .ToListAsync(); 
            }
            else if (user.Role == UserEnum.STAFF)
            {
                notifications = await 
                        context.Notifications
                    .Include(n => n.House)
                    .ThenInclude(hs => hs.Staff)
                    .Where(n => n.House.Staff
                    .Any(hs => hs.Id == userId))
                    .ToListAsync();
            }
            return notifications;

        }

        public async Task<ResultModel> SendNotificationByEmail(Guid houseId, SendNotificationModel sendNotificationModel)
        {
            using var context = new RmsContext();
            ResultModel result = new();

            var customerEmails = await GetAllCustomerEmailByHouseId(houseId);
            foreach (var customerEmail in customerEmails)
            {
                string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataAccess", "TemplateEmail", "Notification.html");
                string newPath = FilePath.Replace("WPF\\bin\\Debug\\net8.0-windows\\", "");
                string Html = await File.ReadAllTextAsync(newPath);

                Html = Html.Replace("{{Subject}}", $"{sendNotificationModel.Subject}"); 
                Html = Html.Replace("{{Content}}", $"{sendNotificationModel.Content}");
                Html = Html.Replace("{{HouseName}}", $"{sendNotificationModel.HouseName}");
                bool emailSent = await EmailUltilities.SendEmail(customerEmail, "Email Notification", Html);

                if (!emailSent)
                {
                    // Xử lý trường hợp gửi email không thành công
                    result.IsSuccess = false;
                    result.Message = "Email cannot be send.";
                    return result;
                }
            }
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                Subject = sendNotificationModel.Subject,
                Content = sendNotificationModel.Content,
                CreatedAt = DateTime.Now,
                HouseId = houseId,
            };
            context.Notifications.Add(notification);
            await context.SaveChangesAsync();

            result.IsSuccess = true;
            result.Message = "Email send successuflly.";
            return result;
        }

        public async Task<IEnumerable<string>> GetAllCustomerEmailByHouseId(Guid houseId)
        {
            using var context = new RmsContext();

            var emails = await context.Houses
                .Where(h => h.Id == houseId)
                .SelectMany(h => h.Rooms)
                .SelectMany(r => r.Users)
                .Where(u => !string.IsNullOrEmpty(u.Email))
                .Select(u => u.Email)
                .ToListAsync();

            return emails;
        }
    }
}

