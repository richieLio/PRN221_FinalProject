using BusinessObject.Object;
using ControlzEx.Standard;
using DataAccess.Enums;
using DataAccess.Model.BillModel;
using DataAccess.Model.UserModel;
using DataAccess.Repository.CombineRepository;
using Google.Apis.Storage.v1.Data;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using WPF.BillView;
using WPF.NotificationView;
using WPF.StaffView;
using WPF.Views.PaymentView;
using WPF.Views.UserView;

namespace WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICombineRepository _repository;
        private readonly HubConnection _connection;
        public MainWindow(IServiceProvider serviceProvider, ICombineRepository repository)
        {
            _repository = repository;
            InitializeComponent();
            _serviceProvider = serviceProvider;


            UpdateStaffButtonVisibility();

            var staffWindow = _serviceProvider.GetService<WindowStaff>();
            staffWindow.LoadStaffs();


            MainContentControl.Content = _serviceProvider.GetService<WindowHouse>();

            _connection = new HubConnectionBuilder()
               .WithUrl("https://localhost:7259/notihub")
               .WithAutomaticReconnect()
               .Build();

            openConnect();

           LoadUserFullName();

        }

        private void LoadUserFullName()
        {
            var name = _repository.GetUserFullName(App.LoggedInUserId);
            UserReqModel userReqModel = new UserReqModel
            {
                FullName = name,
            };
            DataContext = userReqModel;
        }

        private async void openConnect()
        {
            try
            {
                _connection.On<Guid, Guid, string, int>("ReceiveNotification", async (ownerId, billId, message, unReadNoti) =>
                {
                    await Dispatcher.InvokeAsync(async () =>
                    {
                        var bill = await _repository.getBillById(billId);
                        if (bill != null && App.LoggedInUserId == ownerId && bill.CreateBy != App.LoggedInUserId)
                        {
                            var name = _repository.GetUserFullName(App.LoggedInUserId);

                            // Update UnreadNotificationCount
                            var userReqModel = new UserReqModel
                            {
                                FullName = name,
                                UnreadNotificationCount = unReadNoti,
                            };
                            DataContext = userReqModel;
                        }
                    });
                });

                _connection.On<string, string, string, int>("ReceiveCustomerNotification", async (roomName, customerName, message, unReadNoti) =>
                {

                    await Dispatcher.InvokeAsync(() =>
                    {
                        var name = _repository.GetUserFullName(App.LoggedInUserId);

                        // Update UnreadNotificationCount
                        var userReqModel = new UserReqModel
                        {
                            FullName = name,
                            UnreadNotificationCount = unReadNoti,
                        };
                        DataContext = userReqModel;
                    });
                });


                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to SignalR: {ex.Message}");
            }
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
                    case "contractWindow":
                        MainContentControl.Content = _serviceProvider.GetService<WindowContract>();
                        break;
                    case "notificationWindow":
                        MainContentControl.Content = _serviceProvider.GetService<WindowNotification>();
                        break;
                    case "billWindow":
                        var windowBill = _serviceProvider.GetService<WindowBill>();
                        MainContentControl.Content = windowBill;
                        windowBill.LoadAllBill();
                        break;
                    case "paymentWindow":
                        MainContentControl.Content = _serviceProvider.GetService<WindowPayment>();
                        break;
                }
            }
        }
        private async void UpdateStaffButtonVisibility()
        {

            var user = await _repository.GetUserById(App.LoggedInUserId);

            var isUserLicene = await _repository.IsUserLicence(App.LoggedInUserId);
            if(!isUserLicene)
            {
                houseRadioButton.Visibility = Visibility.Collapsed;
                staffRadioButton.Visibility = Visibility.Collapsed;
                contractRadioButton.Visibility = Visibility.Collapsed;
                notificationRadioButton.Visibility = Visibility.Collapsed;
                billRadioButton.Visibility = Visibility.Collapsed;
            }
            if (user != null && isUserLicene)
            {
                staffRadioButton.Visibility = user.Role == UserEnum.OWNER ? Visibility.Visible : Visibility.Collapsed;
                paymentRadioButton.Visibility = user.Role == UserEnum.OWNER ? Visibility.Visible : Visibility.Collapsed;
                NotificationButton.Visibility = user.Role == UserEnum.OWNER ? Visibility.Visible : Visibility.Collapsed;
                BadgedNotiCount.Visibility = user.Role == UserEnum.OWNER ? Visibility.Visible : Visibility.Collapsed;
            }
            
        }
        private async void NotificationButton_Click(object sender, RoutedEventArgs e)
        {

            if (NotificationPopup.IsOpen)
            {
                NotificationPopup.IsOpen = false;
                messages.Items.Clear();
                await _repository.UpdateIsReadNoti(App.LoggedInUserId);
                var name = _repository.GetUserFullName(App.LoggedInUserId);
                var userReqModel = new UserReqModel
                {
                    FullName = name,
                    UnreadNotificationCount = 0,
                };
                DataContext = userReqModel;
            }
            else
            {
                FetchNotifications();
                NotificationPopup.IsOpen = true;
            }
        }

       

        private async void FetchNotifications()
        {
            try
            {
                // Call the GetLocalNotifications function to retrieve notifications
                var notifications = await _repository.GetLocalNotifications(App.LoggedInUserId);
                if (notifications.Data is IEnumerable<LocalNotification> localNotifications)
                {
                    var messagessss = "";
                    foreach (var localNotification in localNotifications)
                    {

                        messagessss = "[" + localNotification.Subject + "]" + " [" + localNotification.Content + "]" + " at " + localNotification.CreatedAt;

                        messages.Items.Add(messagessss);
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void DeleteNotification_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                StackPanel stackPanel = button.Parent as StackPanel;
                if (stackPanel != null)
                {
                    TextBlock textBlock = stackPanel.Children[0] as TextBlock;
                    if (textBlock != null)
                    {
                        string message = textBlock.Text;
                        messages.Items.Remove(message);

                        var notification = await _repository.GetLocalNotificationByMessage(message);
                        if (notification != null)
                        {
                           await _repository.DeleteLocalNotifications(notification.Id);
                        }
                    }
                }
            }
        }




        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var windowLogin = _serviceProvider.GetService<WindowLogin>();
            windowLogin.Show();
            Close();
        }
        private void btnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var windowChangePassword = _serviceProvider.GetService<WindowChangePassword>();
            windowChangePassword.Show();
        }
        private async void btnUpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            var user = await _repository.GetUserById(App.LoggedInUserId);
            var windowUpdateProfile = new WindowUpdateProfile(_repository, user);
            windowUpdateProfile.Show();
        }
    }
}