﻿using BusinessObject.Object;
using DataAccess.Helper;
using DataAccess.Model.EmailModel;
using DataAccess.Model.OperationResultModel;
using DataAccess.Model.UserModel;
using DataAccess.Repository.CombineRepository;
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
    /// Interaction logic for WindowRegister.xaml
    /// </summary>
    public partial class WindowRegister : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICombineRepository _repository;
        public WindowRegister(IServiceProvider serviceProvider, ICombineRepository repository)
        {
            _repository = repository;
            InitializeComponent();
            _serviceProvider = serviceProvider;
            // Ẩn các thành phần OTP khi cửa sổ được tạo ra
            OTPTextBox.Visibility = Visibility.Collapsed;
            ConfirmOTP.Visibility = Visibility.Collapsed;
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var email = EmailTextBox.Text;
                var password = PasswordBox.Password;
                var phoneNumber = PhoneNumberTextBox.Text;
                var address = AddressTextBox.Text;
                var gender = GenderTextBox.Text;
                var dob = DobDatePicker.SelectedDate ?? DateTime.MinValue;
                var fullName = FullNameTextBox.Text;
                var createdAt = DateTime.Now;

                var account = new UserReqModel
                {
                    Email = email,
                    Password = password,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    Gender = gender,
                    Dob = dob,
                    FullName = fullName,
                    CreatedAt = createdAt
                };
                var existingUser = await _repository.GetUserByEmail(email);

                if (existingUser != null)
                {
                    MessageBox.Show("This email already exists!");
                    return;
                }
                var validationResults = ValidationHelper.ValidateModel(account);
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

                await _repository.CreateAccount(account);

                MessageBox.Show("An otp has been seen to your email!");
                OTPTextBox.Visibility = Visibility.Visible;
                ConfirmOTP.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during registration: " + ex.Message);
            }
        }

        private async void ConfirmOTP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var OTPtext = OTPTextBox.Text;
                var email = EmailTextBox.Text;
                var user = new EmailVerificationReqModel
                {
                    OTP = OTPtext,
                    Email = email,
                };

                // Verify the user with the OTP
                var result = await _repository.VerifyEmail(user);

                // Check the result and show a message
                MessageBox.Show(result.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during verification: " + ex.Message);
            }
        }


        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = _serviceProvider.GetService<WindowLogin>();
            if (loginWindow != null)
            {
                loginWindow.Show();
                Close();
            }
        }

    }

}

