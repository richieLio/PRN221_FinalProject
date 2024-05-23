using DataAccess.Helper;
using DataAccess.Model.UserModel;
using DataAccess.Model.VerifyModel;
using DataAccess.Repository;
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
    /// Interaction logic for ResetPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserRepository _userRepository;
        public ResetPasswordWindow(IServiceProvider serviceProvider, IUserRepository userRepository)
        {
            InitializeComponent();
            NewPasswordBox.IsEnabled = false;
            ConfirmPasswordBox.IsEnabled = false;
            ResetPasswordButton.IsEnabled = false;
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
        }
        private void OTPTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (OTPTextBox.Text.Length >= 6) // need to change
            {
                NewPasswordBox.IsEnabled = true;
                ConfirmPasswordBox.IsEnabled = true;
            }
            else
            {
                NewPasswordBox.IsEnabled = false;
                ConfirmPasswordBox.IsEnabled = false;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (NewPasswordBox.Password == ConfirmPasswordBox.Password)
            {
                ResetPasswordButton.IsEnabled = true;
            }
            else
            {
                ResetPasswordButton.IsEnabled = false;
            }
        }
        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var email = EmailTextBox.Text;
                var pass = NewPasswordBox.Password;
                var form = new UserResetPasswordReqModel
                {
                    Email = email,
                    Password = pass
                };

                var validationResults = ValidationHelper.ValidateModel(form);
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
                _userRepository.ResetPassword(form);
                MessageBox.Show("Password reseted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);


            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during confirming: " + ex.Message);

            }

        }

        private async void ConfirmOTP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var OTP = OTPTextBox.Text;
                var email = EmailTextBox.Text;

                if (string.IsNullOrEmpty(OTP))
                {
                    MessageBox.Show("OTP cannot be null", "Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else if (OTP.Length != 6 || !int.TryParse(OTP, out _))
                {
                    MessageBox.Show("OTP must be a 6-digit number!", "Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var confirmResult = await _userRepository.VerifyOTPCode(email, OTP);

                if (confirmResult.IsSuccess)
                {
                    MessageBox.Show(confirmResult.Message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Additional logic for success
                }
                else
                {
                    MessageBox.Show(confirmResult.Message, "Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Additional logic for failure
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during confirming: " + ex.Message);
            }
        }


        private void SendOTPButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var email = EmailTextBox.Text;
                var form = new SendOTPReqModel
                {
                    Email = email
                };

                _userRepository.SendOTPEmailRequest(form);
                MessageBox.Show("an OTP has been send to your email", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                var validationResults = ValidationHelper.ValidateModel(form);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during sending: " + ex.Message);
            }
        }
        private void ResendOTP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var email = EmailTextBox.Text;
                var form = new SendOTPReqModel
                {
                    Email = email
                };

                _userRepository.SendOTPEmailRequest(form);
                MessageBox.Show("an OTP has been send to your email", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                var validationResults = ValidationHelper.ValidateModel(form);
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during sending: " + ex.Message);
            }
        }
    }
}
