using BusinessObject.Object;
using DataAccess.Model.NotificationModel;
using DataAccess.Repository.CombineRepository;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF.Views.NotificationView
{
    /// <summary>
    /// Interaction logic for WindowAddNewNotification.xaml
    /// </summary>
    public partial class WindowAddNewNotification : Window
    {
        private readonly ICombineRepository _repository;
        private readonly House _house;
        public WindowAddNewNotification(ICombineRepository repository,  House house)
        {
            InitializeComponent();
            _repository = repository;
            _house = house;
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var house = await _repository.GetHouse(_house.Id);
            var notifcation = new SendNotificationModel
            {
                Subject = SubjectTextBox.Text,  
                Content = ContentTextBox.Text,
                HouseName = house.Name
            };
            var result =  await _repository.SendNotificationByEmail(_house.Id, notifcation);
            if ( result.IsSuccess)
            {
                MessageBox.Show(result.Message);
            }
            else
            {
                MessageBox.Show(result.Message);
            }
        }
    }
}
