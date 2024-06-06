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

namespace WPF.Views.StaffView
{
    /// <summary>
    /// Interaction logic for WindowUpdateStaff.xaml
    /// </summary>
    public partial class WindowUpdateStaff : Window
    {
        public event EventHandler staffUpdated;
        private readonly User _staff;

        private readonly ICombineRepository _repository;

        public WindowUpdateStaff(ICombineRepository repository, User staff)
        {
            _repository = repository;
            InitializeComponent();
            _staff = staff;
            DataContext = staff;
        }

        private void btnUpdateStaff_Click(object sender, RoutedEventArgs e)
        {
            var staffUpdate = new CustomerUpdateModel
            {
                Id = _staff.Id,
                Email = txtEmail.Text,
                PhoneNumber = txtPhoneNumber.Text,
                Address = txtAddress.Text,
                Gender = cmbGender.Text,
                Dob = DobDatePicker.SelectedDate ?? DateTime.MinValue,
                FullName = txtFullName.Text,
                LicensePlates = txtLicensePlates.Text,
                Status = "Acitve",
                CitizenIdNumber = txtCitizenID.Text,

            };
            _repository.UpdateUserProfile(staffUpdate);
            MessageBox.Show($"Staff {staffUpdate.FullName} updated successfully");
            staffUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
