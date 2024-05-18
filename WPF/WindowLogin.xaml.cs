using AutoMapper.Execution;
using BusinessObject.Object;
using DataAccess.Helper;
using DataAccess.Model.UserModel;
using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client.NativeInterop;
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

namespace WPF
{
    /// <summary>
    /// Interaction logic for WindowLogin.xaml
    /// </summary>
    public partial class WindowLogin : Window
    {
        private readonly IUserRepository _userRepository;
        private readonly IServiceProvider _serviceProvider;
        public WindowLogin(IServiceProvider serviceProvider,IUserRepository userRepository)
        {
            InitializeComponent();
            _userRepository = userRepository;
            _serviceProvider = serviceProvider;
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Password))
                {
                    MessageBox.Show("Please enter username and password.");
                    return;
                }

                var userLoginReqModel = new UserLoginReqModel
                {
                    Email = txtUsername.Text,
                    Password = txtPassword.Password
                };
                var validationResults = ValidationHelper.ValidateModel(userLoginReqModel);
                if (validationResults.Count > 0)
                {
                    // Handle validation errors
                    foreach (var validationResult in validationResults)
                    {
                        MessageBox.Show(validationResult.ErrorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
                    return;
                }
                var loginResult = await _userRepository.Login(userLoginReqModel);

                if (loginResult.IsSuccess)
                {
                    var homeWindow = _serviceProvider.GetService<MainWindow>();
                    homeWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show($"Login failed: {loginResult.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            var resetPasswordWindow = _serviceProvider.GetService<ResetPasswordWindow>();
            if (resetPasswordWindow != null)
            {
                resetPasswordWindow.Show();
                Close();
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = _serviceProvider.GetService<WindowRegister>();
            if (registerWindow != null)
            {
                registerWindow.Show();
                Close();
            }
        }
    }
}
