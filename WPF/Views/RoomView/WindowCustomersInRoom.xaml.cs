using BusinessObject.Object;
using DataAccess.Model.CustomerModel;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Views.RoomView
{
    /// <summary>
    /// Interaction logic for WindowCustomersInRoom.xaml
    /// </summary>
    public partial class WindowCustomersInRoom : UserControl
    {
        public WindowCustomersInRoom(Guid roomId)
        {
            InitializeComponent();
            LoadCustomers(roomId);
        }

        public async void LoadCustomers(Guid roomId)
        {
            try
            {
                ICustomerRepository customerRepository = new CustomerRepository();
                var result = await customerRepository.GetCustomerByRoomId(roomId);

                if (result.IsSuccess)
                {
                    if (result.Data is IEnumerable<User> users)
                    {
                        lvCustomers.ItemsSource = users;
                    }
                    else
                    {
                        MessageBox.Show("Invalid data type returned from repository.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackToRoomManagement_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {

        }
        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {

            ICustomerRepository customerRepository = new CustomerRepository();
            if (lvCustomers.SelectedItem is User selectedCustomer)
            {
                var userToUpdate = new CustomerUpdateModel
                {
                    Email = selectedCustomer.Email,
                    PhoneNumber = selectedCustomer.PhoneNumber,
                    Address = selectedCustomer.Address,
                    Gender = selectedCustomer.Gender,
                    Dob = selectedCustomer.Dob,
                    FullName = selectedCustomer.FullName,
                    LicensePlates = selectedCustomer.LicensePlates,
                    Status = selectedCustomer.Status,
                    CitizenIdNumber = selectedCustomer.CitizenIdNumber,
                };

                customerRepository.UpdateUserProfile(userToUpdate);
                MessageBox.Show($"Edit customer: {selectedCustomer.FullName}");
            }
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (lvCustomers.SelectedItem is User selectedCustomer)
            {
                // Your logic to delete the selected customer
                MessageBox.Show($"Delete customer: {selectedCustomer.FullName}");
            }
        }
    }
}
