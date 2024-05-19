using BusinessObject.Object;
using DataAccess.Repository;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                WindowHouseDetails detailsWindow = new WindowHouseDetails(_houseRepository, _roomRepository, _serviceProvider, selectedHouse.Name);
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



    }
}
