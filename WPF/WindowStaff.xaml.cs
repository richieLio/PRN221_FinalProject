using BusinessObject.Object;
using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
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
using WPF.Views.CustomerView;

namespace WPF
{
    /// <summary>
    /// Interaction logic for WindowStaff.xaml
    /// </summary>
    public partial class WindowStaff : UserControl
    {
        public WindowStaff()
        {
            InitializeComponent();
            LoadStaffs();
        }
        public async void LoadStaffs()
        {
            try
            {
                IStaffRepository staffRepository = new StaffRepository();
                var result = await staffRepository.GetAllStaffByOwnerId(App.LoggedInUserId);

                if (result.IsSuccess)
                {
                    if (result.Data is IEnumerable<User> users)
                    {
                        lvStaffs.ItemsSource = users;
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

        private void AddStaff_Click(object sender, RoutedEventArgs e)
        {

        }
        private void EditStaff_Click(object sender, RoutedEventArgs e)
        {
           /* try
            {
                MenuItem menuItem = sender as MenuItem;
                if (menuItem != null)
                {
                    ContextMenu contextMenu = menuItem.Parent as ContextMenu;
                    if (contextMenu != null)
                    {
                        ListView listView = contextMenu.PlacementTarget as ListView;
                        if (listView != null)
                        {
                            if (listView.SelectedItem is User selectedUser)
                            {
                                var updateCustomerWindow = new WindowUpdateCustomer(_serviceProvider.GetService<ICustomerRepository>(), selectedUser);
                                updateCustomerWindow.CustomerUpdated += (s, args) =>
                                {
                                    LoadCustomers(_roomId);
                                };
                                updateCustomerWindow.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("No user selected.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("ListView is null.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("ContextMenu is null.");
                    }
                }
                else
                {
                    MessageBox.Show("MenuItem is null.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }*/
        }


        private void DeleteStaff_Click(object sender, RoutedEventArgs e)
        {
/*            ICustomerRepository customerRepository = new CustomerRepository();
            if (lvCustomers.SelectedItem is User selectedCustomer)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure to delete {selectedCustomer.FullName}?", "Warning", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    customerRepository.DeleteCustomer(selectedCustomer.Id);
                    LoadCustomers(_roomId);
                }
            }*/
        }
    }
}
