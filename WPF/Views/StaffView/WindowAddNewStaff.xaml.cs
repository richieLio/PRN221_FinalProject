using DataAccess.Helper;
using DataAccess.Model.UserModel;
using DataAccess.Repository.CombineRepository;
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

namespace WPF.Views.StaffView
{
    /// <summary>
    /// Interaction logic for WindowAddNewStaff.xaml
    /// </summary>
    public partial class WindowAddNewStaff : Window
    {
        private readonly ICombineRepository _repository;
        public event EventHandler StaffAdded;

        public WindowAddNewStaff(ICombineRepository repository)
        {
            _repository = repository;
            InitializeComponent();
        }

        private async void CreateButton_Click(object sender, RoutedEventArgs e)
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

                await _repository.AddStaff(App.LoggedInUserId, account);
                MessageBox.Show("Staff created successfully");

                StaffAdded?.Invoke(this, EventArgs.Empty);
                Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during registration: " + ex.Message);
            }
        }
    }
}
