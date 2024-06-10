using BusinessObject.Object;
using DataAccess.Model.CustomerModel;
using DataAccess.Repository.CombineRepository;
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
        private readonly ICombineRepository _repository;
        private  Guid _roomId;
        private  Guid _houseId;
        public WindowCustomersInRoom( IServiceProvider serviceProvider,ICombineRepository repository)
        {
            _repository = repository;
            InitializeComponent();
            _serviceProvider = serviceProvider;
            LoadHouses();
        }
        public async void LoadHouses()
        {
            var houses = await _repository.GetHouses(App.LoggedInUserId);
            cbHouses.ItemsSource = houses;
            if (houses.Any())
            {
                cbHouses.SelectedIndex = 0;
            }
        }
        private void cbHouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbHouses.SelectedItem is House selectedHouse)
            {
                LoadRooms(selectedHouse.Id);
                _houseId = selectedHouse.Id;
            }
        }
        private async void cbRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbRooms.SelectedItem is Room selectedRoom)
            {
                _roomId = selectedRoom.Id;
                LoadCustomers(_roomId);
            }
        }
        private async Task LoadRooms(Guid houseId)
        {
            try
            {
                var rooms = await _repository.GetRooms(houseId);
                if (rooms != null)
                {
                    cbRooms.ItemsSource = rooms;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading rooms: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async void LoadCustomers(Guid roomId)
        {
            try
            {
                var result = await _repository.GetCustomerByRoomId(roomId);

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

       
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerWindow = new WindowAddCustomer(_serviceProvider.GetService<ICombineRepository>(), _houseId, _roomId);
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
                                var updateCustomerWindow = new WindowUpdateCustomer(_serviceProvider.GetService<ICombineRepository>(), selectedUser);
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
                MessageBoxResult result = MessageBox.Show($"Are you sure to delete {selectedCustomer.FullName}?", "Warning", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    _repository.DeleteCustomer(selectedCustomer.Id);
                    LoadCustomers(_roomId);
                }
            }
        }

    }
}
