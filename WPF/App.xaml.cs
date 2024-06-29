
using DataAccess.Repository.BillRepository;
using DataAccess.Repository.CombineRepository;
using DataAccess.Repository.ContractRepository;
using DataAccess.Repository.CustomerRepository;
using DataAccess.Repository.HouseRepository;
using DataAccess.Repository.LicenceRepository;
using DataAccess.Repository.LocalNotificationRepository;
using DataAccess.Repository.NotificationRepository;
using DataAccess.Repository.RoomRepository;
using DataAccess.Repository.ServiceRepository;
using DataAccess.Repository.StaffRepository;
using DataAccess.Repository.TransactionRepository;
using DataAccess.Repository.UserRepostory;
using DataAccess.Utilities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using WPF.BillView;
using WPF.ContractView;
using WPF.NotificationView;
using WPF.StaffView;
using WPF.Views.BillView;
using WPF.Views.ContractView;
using WPF.Views.CustomerView;
using WPF.Views.HouseView;
using WPF.Views.NotificationView;
using WPF.Views.PaymentView;
using WPF.Views.ReportView;
using WPF.Views.RoomView;
using WPF.Views.ServiceFeeView;
using WPF.Views.UserView;

namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;
        public static Guid LoggedInUserId { get; set; }
        public static HubConnection SignalRConnection { get; set; }

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
            LoggedInUserId = Guid.Empty;

            SetupSignalR();

        }
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IHouseRepository, HouseRepository>();
            services.AddSingleton<IRoomRepository, RoomRepository>();
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            services.AddSingleton<IStaffRepository, StaffRepository>();
            services.AddSingleton<IServiceFeeRepository, ServiceFeeRepository>();
            services.AddSingleton<IBillRepository, BillRepository>();
            services.AddSingleton<ILocalNotificationRepository, LocalNotificationRepository>();
            services.AddSingleton<ITransactionRepository, TransactionRepository>();
            services.AddSingleton<ILicenceRepository, LicenceRepository>();
            services.AddSingleton<ICombineRepository, CombineRepository>();
            services.AddSingleton<INotificationRepository, NotificationRepository>();
            services.AddSingleton<IContractRepository, ContractRepository>();



            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IHouseRepository, HouseRepository>();
            services.AddTransient<IRoomRepository, RoomRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IStaffRepository, StaffRepository>();
            services.AddTransient<IServiceFeeRepository, ServiceFeeRepository>();
            services.AddTransient<IBillRepository, BillRepository>();
            services.AddTransient<ILocalNotificationRepository, LocalNotificationRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<ILicenceRepository, LicenceRepository>();
            services.AddTransient<ICombineRepository, CombineRepository>();
            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<IContractRepository, ContractRepository>();



            services.AddSingleton<MainWindow>();
            services.AddSingleton<WindowLogin>();
            services.AddSingleton<WindowRegister>();
            services.AddSingleton<WindowHouse>();
            services.AddSingleton<WindowContract>();
            services.AddSingleton<WindowStaff>();
            services.AddSingleton<WindowHouseDetails>();
            services.AddSingleton<WindowAddHouse>();
            services.AddSingleton<WindowUpdateHouse>();
            services.AddSingleton<ConfirmDeleteHouse>();
            services.AddSingleton<WindowAddRoom>();
            services.AddSingleton<WindowCustomersInRoom>();
            services.AddSingleton<WindowUpdateCustomer>();
            services.AddSingleton<WindowServiceFee>();
            services.AddSingleton<WindowAddServiceFee>();
            services.AddSingleton<WindowUpdateServiceFee>();
            services.AddSingleton<WindowBill>();
            services.AddSingleton<WindowAddBill>();
            services.AddSingleton<WindowUpdateBill>();
            services.AddSingleton<WindowUpdateBillStatus>();
            services.AddSingleton<WindowChangePassword>();
            services.AddSingleton<WindowUpdateProfile>();
            services.AddSingleton<WindowPayment>();
            services.AddSingleton<WindowAddNewNotification>();
            services.AddSingleton<WindowNotification>();
            services.AddSingleton<WindowContractDetails>();
            services.AddSingleton<WindowReport>();
            services.AddSingleton<CloudStorage>(_ =>
       new CloudStorage("firebaseKey.Json"));





            services.AddTransient<WindowLogin>();
            services.AddTransient<WindowRegister>();
            services.AddTransient<ResetPasswordWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<WindowHouse>();
            services.AddTransient<WindowContract>();
            services.AddTransient<WindowNotification>();
            services.AddTransient<WindowHouseDetails>();
            services.AddTransient<WindowUpdateHouse>();
            services.AddTransient<ConfirmDeleteHouse>();
            services.AddTransient<WindowAddRoom>();
            services.AddTransient<WindowCustomersInRoom>();
            services.AddTransient<WindowUpdateCustomer>();
            services.AddTransient<WindowServiceFee>();
            services.AddTransient<WindowAddServiceFee>();
            services.AddTransient<WindowUpdateServiceFee>();
            services.AddTransient<WindowBill>();
            services.AddTransient<WindowAddBill>();
            services.AddTransient<WindowUpdateBill>();
            services.AddTransient<WindowUpdateBillStatus>();
            services.AddTransient<WindowChangePassword>();
            services.AddTransient<WindowUpdateProfile>();
            services.AddTransient<WindowPayment>();
            services.AddTransient<WindowAddNewNotification>();
            services.AddTransient<WindowContractDetails>();
            services.AddTransient<WindowReport>();

        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var loginWindow = serviceProvider.GetService<WindowLogin>();
            loginWindow.Show();
        }
        private async void SetupSignalR()
        {
            SignalRConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7259/notihub")
                .WithAutomaticReconnect()
                .Build();
            try
            {
                await SignalRConnection.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"SignalR connection error: {ex.Message}");
            }
        }
    }
}


