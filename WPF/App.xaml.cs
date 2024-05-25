using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using WPF.BillView;
using WPF.StaffView;
using WPF.Views.CustomerView;
using WPF.Views.HouseView;
using WPF.Views.RoomView;
using WPF.Views.ServiceFeeView;

namespace WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider serviceProvider;
        public static Guid LoggedInUserId { get; set; }
        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
            LoggedInUserId = Guid.Empty;
        }
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IHouseRepository, HouseRepository>();
            services.AddSingleton<IRoomRepository, RoomRepository>();
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            services.AddSingleton<IStaffRepository, StaffRepository>();
            services.AddSingleton<IServiceFeeRepository, ServiceFeeRepository>();
            services.AddSingleton<ICombineRepository, CombineRepository>();



            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IHouseRepository, HouseRepository>();
            services.AddTransient<IRoomRepository, RoomRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IStaffRepository, StaffRepository>();
            services.AddTransient<IServiceFeeRepository, ServiceFeeRepository>();
            services.AddTransient<ICombineRepository, CombineRepository>();



            services.AddSingleton<MainWindow>();
            services.AddSingleton<WindowLogin>();
            services.AddSingleton<WindowRegister>();
            services.AddSingleton<WindowHouse>();
            services.AddSingleton<WindowContract>();
            services.AddSingleton<WindowBill>();
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




            services.AddTransient<WindowLogin>();
            services.AddTransient<WindowRegister>();
            services.AddTransient<ResetPasswordWindow>();
            services.AddTransient<MainWindow>(); 
            services.AddTransient<WindowHouse>();
            services.AddTransient<WindowContract>();
            services.AddTransient<WindowBill>();
            services.AddTransient<WindowHouseDetails>();
            services.AddTransient<WindowUpdateHouse>();
            services.AddTransient<ConfirmDeleteHouse>();
            services.AddTransient<WindowAddRoom>();
            services.AddTransient<WindowCustomersInRoom>();
            services.AddTransient<WindowUpdateCustomer>();
            services.AddTransient<WindowServiceFee>();
            services.AddTransient<WindowAddServiceFee>();
            services.AddTransient<WindowUpdateServiceFee>();

        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var loginWindow = serviceProvider.GetService<WindowLogin>();
            loginWindow.Show();
        }
    }
}


