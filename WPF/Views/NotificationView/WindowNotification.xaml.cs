using BusinessObject.Object;
using ControlzEx.Standard;
using DataAccess.Model.NotificationModel;
using DataAccess.Repository.CombineRepository;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
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
using WPF.Views.BillView;
using WPF.Views.NotificationView;

namespace WPF.NotificationView
{
    /// <summary>
    /// Interaction logic for WindowNotification.xaml
    /// </summary>
    public partial class WindowNotification : UserControl
    {
        private readonly ICombineRepository _repository;
        private readonly IServiceProvider _serviceProvider;
        private Guid _houseId;

        public WindowNotification(ICombineRepository repository, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _repository = repository;
            LoadHouses();
            _serviceProvider = serviceProvider;
        }

        public async void LoadHouses()
        {
            try
            {
                var houses = await _repository.GetHouses(App.LoggedInUserId);
                cbHouses.ItemsSource = houses;
                if (houses.Any())
                {
                    cbHouses.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, show a message to the user)
                MessageBox.Show("Failed to load houses. Please try again later.");
            }
        }

        private void cbHouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbHouses.SelectedItem is House selectedHouse)
            {
                _houseId = selectedHouse.Id;
                LoadNotifications(_houseId);
            }
        }

        public async void LoadNotifications(Guid houseId)
        {
            try
            {
                var notifications = await _repository.GetNotifications(App.LoggedInUserId);
                var notificationModels = new List<NotificationModel>();

                foreach (var notification in notifications.Where(n => n.HouseId == houseId))
                {
                    var house = await _repository.GetHouse(houseId);
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
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, show a message to the user)
                MessageBox.Show("Failed to load notifications. Please try again later.");
            }
        }

        public void DeleteNotification_Click(object sender, RoutedEventArgs e)
        {
            // Handle deletion of notifications
        }

        private void sendNoti_Click(object sender, RoutedEventArgs e)
        {
            if (cbHouses.SelectedItem is House selectedHouse)
            {
                var windowsendNoti = new WindowAddNewNotification(_serviceProvider.GetService<ICombineRepository>(), selectedHouse);
                windowsendNoti.NotiAdded += (s, args) =>
                {
                    LoadNotifications(_houseId);
                };
                windowsendNoti.Show();
            }
            else
            {
                MessageBox.Show("Please select a house first.");
            }
        }
    }
}
