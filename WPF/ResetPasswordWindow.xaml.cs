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
        public ResetPasswordWindow()
        {
            InitializeComponent();
            NewPasswordBox.IsEnabled = false;
            ConfirmPasswordBox.IsEnabled = false;
            ResetPasswordButton.IsEnabled = false;

        }
        private void OTPTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Check if OTP is valid, for example, if it has reached a certain length
            if (OTPTextBox.Text.Length >= 6) // Change '6' to your desired OTP length
            {
                // Enable the New Password TextBox
                NewPasswordBox.IsEnabled = true;
                ConfirmPasswordBox.IsEnabled = true;
            }
            else
            {
                // Disable the New Password TextBox
                NewPasswordBox.IsEnabled = false;
                ConfirmPasswordBox.IsEnabled = false;
            }
        }


       
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Check if passwords match
            if (NewPasswordBox.Password == ConfirmPasswordBox.Password)
            {
                // Enable the Reset Password Button
                ResetPasswordButton.IsEnabled = true;
            }
            else
            {
                // Disable the Reset Password Button
                ResetPasswordButton.IsEnabled = false;
            }
        }
        private void ResetPasswordButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ConfirmOTP_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SendOTPButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
