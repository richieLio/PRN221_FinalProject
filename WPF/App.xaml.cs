using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

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



            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IHouseRepository, HouseRepository>();
            services.AddTransient<IRoomRepository, RoomRepository>();



            services.AddSingleton<MainWindow>();
            services.AddSingleton<WindowLogin>();
            services.AddSingleton<WindowRegister>();
            services.AddSingleton<WindowHouse>();
            services.AddSingleton<WindowContract>();
            services.AddSingleton<WindowBill>();
            services.AddSingleton<WindowStaff>();
            services.AddSingleton<WindowService>();
            services.AddSingleton<WindowHouseDetails>();




            services.AddTransient<WindowLogin>();
            services.AddTransient<WindowRegister>();
            services.AddTransient<ResetPasswordWindow>();
            services.AddTransient<MainWindow>(); 
            services.AddTransient<WindowHouse>();
            services.AddTransient<WindowContract>();
            services.AddTransient<WindowBill>();
            services.AddTransient<WindowService>();
            services.AddTransient<WindowHouseDetails>();


        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var loginWindow = serviceProvider.GetService<WindowLogin>();
            loginWindow.Show();
        }
    }
}


