using DataAccess.Enums;
using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.BillView;
using WPF.StaffView;
using WPF.Views.ServiceFeeView;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICombineRepository _repository;

        public MainWindow(IServiceProvider serviceProvider, ICombineRepository repository)
        {
            _repository = repository;
            InitializeComponent();
            _serviceProvider = serviceProvider;
            UpdateStaffButtonVisibility();
            var staffWindow = _serviceProvider.GetService<WindowStaff>();
            staffWindow.LoadStaffs();
            MainContentControl.Content = _serviceProvider.GetService<WindowHouse>();
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton != null)
            {
                switch (radioButton.Tag.ToString())
                {
                    case "houseWindow":
                        MainContentControl.Content = _serviceProvider.GetService<WindowHouse>();
                        break;
                    case "staffWindow":
                        MainContentControl.Content = _serviceProvider.GetService<WindowStaff>();
                        break;
                    case "serviceWindow":
                        MainContentControl.Content = _serviceProvider.GetService<WindowServiceFee>();
                        break;
                    case "contractWindow":
                        MainContentControl.Content = _serviceProvider.GetService<WindowContract>();
                        break;
                    case "notificationWindow":
                        MainContentControl.Content = _serviceProvider.GetService<WindowNotification>();
                        break;
                    case "billWindow":
                        MainContentControl.Content = _serviceProvider.GetService<WindowBill>();
                        break;
                }
            }

        }
        private async void UpdateStaffButtonVisibility()
        {
           
            var user = await _repository.GetUserById(App.LoggedInUserId);
            if (user != null)
            {
                staffRadioButton.Visibility = user.Role == UserEnum.OWNER ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var windowLogin = _serviceProvider.GetService<WindowLogin>();
            windowLogin.Show();
            Close();
        }
    }
}