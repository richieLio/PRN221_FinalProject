using BusinessObject.Object;
using DataAccess.Helper;
using DataAccess.Model.CustomerModel;
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

namespace WPF.Views.UserView
{
    /// <summary>
    /// Interaction logic for WindowUpdateProfile.xaml
    /// </summary>
    public partial class WindowUpdateProfile : Window
    {
        private readonly ICombineRepository _repository;
        public WindowUpdateProfile(ICombineRepository repository, User user)
        {
            _repository = repository;
            InitializeComponent();
            DataContext = user;
        }
       
        private void btnUpdateProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = new UserUpdateModel
            {
                Id = App.LoggedInUserId,
                Email = txtEmail.Text,
                PhoneNumber = txtPhoneNumber.Text,
                Address = txtAddress.Text,
                Gender = cmbGender.Text,
                Dob = DobDatePicker.SelectedDate ?? DateTime.MinValue,
                FullName = txtFullName.Text,
            };
            var validationResults = ValidationHelper.ValidateModel(profile);
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
            _repository.UpdateUserProfile(profile);
            MessageBox.Show($"Profile updated successfully");
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
