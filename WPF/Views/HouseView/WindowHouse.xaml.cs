using BusinessObject.Object;
using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPF.Views.HouseView;

namespace WPF
{
    public partial class WindowHouse : UserControl
    {
        private readonly IHouseRepository _houseRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceProvider _serviceProvider; 

        public WindowHouse(IHouseRepository houseRepository, IRoomRepository roomRepository, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _houseRepository = houseRepository;
            _roomRepository = roomRepository;
            _serviceProvider = serviceProvider;
            LoadHouses();
        }

        private async void LoadHouses()
        {
            lvHouses.ItemsSource = await _houseRepository.GetHouses(App.LoggedInUserId);
        }

        private void Border_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedHouse = (sender as Border)?.DataContext as House;
            if (selectedHouse != null)
            {
                WindowHouseDetails detailsWindow = new WindowHouseDetails(_houseRepository, _roomRepository, _serviceProvider, selectedHouse.Name, selectedHouse.Address
                    , selectedHouse.RoomQuantity, selectedHouse.AvailableRoom,  selectedHouse.Id);
                detailsWindow.LoadRooms(selectedHouse.Id);

                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainContentControl.Content = detailsWindow;
                }
            }
        }

        private void HouseBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border.DataContext is House house)
            {
                PopupHouseName.Text = house.Name;
                PopupHouseAddress.Text = $"Address: {house.Address}";
                PopupRoomQuantity.Text = $"Rooms: {house.RoomQuantity}";
                PopupAvailableRoom.Text = $"Available: {house.AvailableRoom}";
                PopupStatus.Text = $"Status: {house.Status}";
                HouseDetailsPopup.PlacementTarget = border;
                HouseDetailsPopup.IsOpen = true;
            }
            else
            {
                HouseDetailsPopup.IsOpen = false;
            }
        }

        private void HouseBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            HouseDetailsPopup.IsOpen = false;
        }

        private void AddNewHouse_Click(object sender, RoutedEventArgs e)
        {
            var addNewHouseWindow = new WindowAddHouse(_serviceProvider.GetService<IHouseRepository>());
            addNewHouseWindow.HouseAdded += (s, args) =>
            {
                // HouseAdded event handler, you might want to refresh the list of houses or take other actions
                LoadHouses();
            };
            addNewHouseWindow.Show();
        }

        private void UpdateHouse_Click(object sender, RoutedEventArgs e)
        {
            // Code to update the selected house
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                Border border = ((ContextMenu)menuItem.Parent).PlacementTarget as Border;
                if (border != null)
                {
                    // Assuming DataContext of the Border is the house item
                    var house = border.DataContext as House; // Replace House with your actual data type
                    if (house != null)
                    {
                        var updateHouseWindow = new WindowUpdateHouse(_serviceProvider.GetService<IHouseRepository>(), house);
                        updateHouseWindow.HouseUpdated += (s, args) =>
                        {
                            LoadHouses();
                        };
                        updateHouseWindow.Show();
                    }
                }
            }
        }
        private async void DeleteHouse_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                Border border = ((ContextMenu)menuItem.Parent).PlacementTarget as Border;
                if (border != null)
                {
                    var house = border.DataContext as House;
                    if (house != null)
                    {
                        ConfirmDeleteHouse confirmDialog = new ConfirmDeleteHouse(house.Name);
                        confirmDialog.HouseDeleted += async (s, args) =>
                        {
                            await _houseRepository.DeleteHouse(App.LoggedInUserId, house.Id);
                            MessageBox.Show("House deleted successfully.");
                            LoadHouses();
                        };
                        confirmDialog.ShowDialog();
                    }
                }
            }
        }
    }
}
