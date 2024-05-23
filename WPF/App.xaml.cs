using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using WPF.Views.CustomerView;
using WPF.Views.HouseView;
using WPF.Views.RoomView;

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



            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IHouseRepository, HouseRepository>();
            services.AddTransient<IRoomRepository, RoomRepository>();
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IStaffRepository, StaffRepository>();



            services.AddSingleton<MainWindow>();
            services.AddSingleton<WindowLogin>();
            services.AddSingleton<WindowRegister>();
            services.AddSingleton<WindowHouse>();
            services.AddSingleton<WindowContract>();
            services.AddSingleton<WindowBill>();
            services.AddSingleton<WindowStaff>();
            services.AddSingleton<WindowService>();
            services.AddSingleton<WindowHouseDetails>();
            services.AddSingleton<WindowAddHouse>();
            services.AddSingleton<WindowUpdateHouse>();
            services.AddSingleton<ConfirmDeleteHouse>();
            services.AddSingleton<WindowAddRoom>();
            services.AddSingleton<WindowCustomersInRoom>();
            services.AddSingleton<WindowUpdateCustomer>();




            services.AddTransient<WindowLogin>();
            services.AddTransient<WindowRegister>();
            services.AddTransient<ResetPasswordWindow>();
            services.AddTransient<MainWindow>(); 
            services.AddTransient<WindowHouse>();
            services.AddTransient<WindowContract>();
            services.AddTransient<WindowBill>();
            services.AddTransient<WindowService>();
            services.AddTransient<WindowHouseDetails>();
            services.AddTransient<WindowUpdateHouse>();
            services.AddTransient<ConfirmDeleteHouse>();
            services.AddTransient<WindowAddRoom>();
            services.AddTransient<WindowCustomersInRoom>();
            services.AddTransient<WindowUpdateCustomer>();


        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var loginWindow = serviceProvider.GetService<WindowLogin>();
            loginWindow.Show();
        }
    }
}


