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
        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }
        private void ConfigureServices(ServiceCollection services)
        {
            services.AddSingleton<IUserRepository, UserRepository>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<WindowLogin>();
            services.AddSingleton<WindowRegister>();
            services.AddSingleton<ResetPasswordWindow>();
            services.AddTransient<WindowLogin>();
            services.AddTransient<WindowRegister>();
            services.AddTransient<ResetPasswordWindow>();
            services.AddTransient<MainWindow>();

        }
        private void OnStartup(object sender, StartupEventArgs e)
        {
            var loginWindow = serviceProvider.GetService<WindowLogin>();
            loginWindow.Show();
        }
    }
}


