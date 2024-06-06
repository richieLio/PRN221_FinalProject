using ControlzEx.Standard;
using DataAccess.Model.NotificationModel;
using DataAccess.Repository.CombineRepository;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF.NotificationView
{
    /// <summary>
    /// Interaction logic for WindowNotification.xaml
    /// </summary>
    public partial class WindowNotification : UserControl
    {
        private readonly ICombineRepository _repository;

        public WindowNotification(ICombineRepository repository)
        {
            InitializeComponent();
            _repository = repository;
             LoadNotifications();


        }

        public async Task LoadNotifications()
        {
            var notifications = await _repository.GetNotifications(App.LoggedInUserId);
            var notificationModels = new List<NotificationModel>();

            foreach (var notification in notifications)
            {
                var house = await _repository.GetHouse(notification.HouseId.Value);
                var notificationModel = new NotificationModel
                {
                    Subject = notification.Subject,
                    Content = notification.Content,
                    HouseName = house.Name,
                    CreatedAt = notification.CreatedAt
                };
                notificationModels.Add(notificationModel);
            }

            lvNotifications.ItemsSource = notificationModels;
        }


        public void DeleteNotification_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
