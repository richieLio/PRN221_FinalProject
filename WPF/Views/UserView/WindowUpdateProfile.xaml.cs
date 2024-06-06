using BusinessObject.Object;
using DataAccess.Model.CustomerModel;
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
            var profile = new CustomerUpdateModel
            {
                Id = App.LoggedInUserId,
                Email = txtEmail.Text,
                PhoneNumber = txtPhoneNumber.Text,
                Address = txtAddress.Text,
                Gender = cmbGender.Text,
                Dob = DobDatePicker.SelectedDate ?? DateTime.MinValue,
                FullName = txtFullName.Text,
            };
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
