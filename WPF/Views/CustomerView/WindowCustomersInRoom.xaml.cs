using BusinessObject.Object;
using DataAccess.Model.CustomerModel;
using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPF.Views.HouseView;

namespace WPF.Views.CustomerView
{
    /// <summary>
    /// Interaction logic for WindowCustomersInRoom.xaml
    /// </summary>
    public partial class WindowCustomersInRoom : UserControl
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Guid _roomId;
        private readonly Guid _houseId;
        public WindowCustomersInRoom(Guid roomId, IServiceProvider serviceProvider, Guid houseId)
        {
            InitializeComponent();
            LoadCustomers(roomId);
            _roomId = roomId;
            _serviceProvider = serviceProvider;
            _houseId = houseId;
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

        private async void BackToRoomManagement_Click(object sender, RoutedEventArgs e)
        {
            IHouseRepository _houseRepository = new HouseRepository();
            IRoomRepository _roomRepository = new RoomRepository();
            var house = await _houseRepository.GetHouse(_houseId);
            // Get the main window
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                // Create a new instance of WindowHouse
                WindowHouseDetails houseWindowDetails = new WindowHouseDetails(_houseRepository, _roomRepository, _serviceProvider,
                    house.Name, house.Address, house.RoomQuantity, house.AvailableRoom, _houseId);
                houseWindowDetails.LoadRooms(_houseId);
                // Set the MainContentControl content to the new WindowHouse instance
                mainWindow.MainContentControl.Content = houseWindowDetails;
            }

        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerWindow = new WindowAddCustomer(_serviceProvider.GetService<ICustomerRepository>(), _serviceProvider.GetService<IHouseRepository>(), _houseId, _roomId);
            addCustomerWindow.CustomerAdded += (s, args) =>
            {
                // HouseAdded event handler, you might want to refresh the list of houses or take other actions
                LoadCustomers(_roomId);
            };
            addCustomerWindow.Show();

        }
        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
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
