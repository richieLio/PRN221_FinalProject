using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPF.Views.CustomerView;
using WPF.Views.HouseView;
using WPF.Views.RoomView;

namespace WPF
{
    public partial class WindowHouseDetails : UserControl
    {
        private readonly IHouseRepository _houseRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStaffRepository _staffRepository;
        private string _houseName;
        private string _address;
        private int? _roomQuantity;
        private int? _availableRoom;
        private Guid _houseId;

       

        public WindowHouseDetails(IHouseRepository houseRepository, IRoomRepository roomRepository, IServiceProvider serviceProvider,
            string houseName, string address, int? roomQuantity, int? availableRoom, Guid houseId, IStaffRepository staffRepository)
        {
            InitializeComponent();
            _houseRepository = houseRepository;
            _roomRepository = roomRepository;
            _serviceProvider = serviceProvider;
            _houseName = houseName;
            _address = address;
            _roomQuantity = roomQuantity;
            _availableRoom = availableRoom;
            HouseNameTextBlock.Text = _houseName;
            AddressTextBlock.Text = _address;
            RoomQuantityTextBlock.Text = _roomQuantity.ToString();
            AvailableRoomTextBlock.Text = _availableRoom.ToString();
            _houseId = houseId;
            _staffRepository = staffRepository;
            Initialize();
        }

        public async void LoadRooms(Guid houseId)
        {
            lvRooms.ItemsSource = await _roomRepository.GetRooms(houseId);
        }

        private void BackToHouseManagement_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            // Get the main window
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                // Create a new instance of WindowHouse
                WindowHouse houseWindow = new WindowHouse(_houseRepository, _roomRepository, _serviceProvider);

                // Set the MainContentControl content to the new WindowHouse instance
                mainWindow.MainContentControl.Content = houseWindow;
            }

        }
        private void Border_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedRoom = (sender as Border)?.DataContext as Room;
            if (selectedRoom != null)
            {
                WindowCustomersInRoom customersWindow = new WindowCustomersInRoom( selectedRoom.Id, _serviceProvider, _houseId);
                customersWindow.LoadCustomers(selectedRoom.Id);

                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.MainContentControl.Content = customersWindow;
                }
            }
        }
        private void AddNewRoom_Click(object sender, RoutedEventArgs e)
        {

            var windowAddRoom =  new WindowAddRoom(_serviceProvider.GetService<IRoomRepository>(), _houseId );
            windowAddRoom.RoomAdded += (s, args) =>
             {
                 LoadRooms(_houseId);
                 
             };
            windowAddRoom.Show();
        }
        private async void RoomBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border border && border.DataContext is Room room)
            {
                var customers = await _roomRepository.GetListCustomerByRoomId(room.Id);
                PopupRoomName.Text = room.Name;
                PopupRoomPrice.Text = $"Price: {room.Price}";  // Assuming you meant room.Price
                PopupListCustomers.Text = "Customers: " + string.Join(", ", customers.Select(customer => customer.FullName));
                PopupStatus.Text = $"Status: {room.Status}";
                RoomDetailsPopup.PlacementTarget = border;
                RoomDetailsPopup.IsOpen = true;
            }
            else
            {
                RoomDetailsPopup.IsOpen = false;
            }
        }

        private void RoomBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            RoomDetailsPopup.IsOpen = false;
        }
        private void UpdateRoom_Click(object sender, RoutedEventArgs e)
        {
            // Code to update the selected house
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                Border border = ((ContextMenu)menuItem.Parent).PlacementTarget as Border;
                if (border != null)
                {
                    // Assuming DataContext of the Border is the house item
                    var room = border.DataContext as Room; // Replace House with your actual data type
                    if (room != null)
                    {
                        var updateRoomWindow = new WindowUpdateRoom(_serviceProvider.GetService<IRoomRepository>(), room);
                        updateRoomWindow.RoomUpdated += (s, args) =>
                        {
                            LoadRooms(_houseId);
                        };
                        updateRoomWindow.Show();
                    }
                }
            }
        }
        private async void DeleteRoom_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                Border border = ((ContextMenu)menuItem.Parent).PlacementTarget as Border;
                if (border != null)
                {
                    var room = border.DataContext as Room;
                    if (room != null)
                    {
                        WindowDeleteRoom confirmDialog = new WindowDeleteRoom(room.Name);
                        confirmDialog.RoomDeleted += async (s, args) =>
                        {
                            await _roomRepository.DeleteRoom(_houseId, room.Id);
                            MessageBox.Show("Room deleted successfully.");
                            LoadRooms(_houseId);
                        };
                        confirmDialog.ShowDialog();
                    }
                }
            }
        }
        private async void AssignStaff_Click(object sender, RoutedEventArgs e)
        {
            if (cmbStaffList.SelectedItem != null)
            {
                var selectedStaff = cmbStaffList.SelectedItem as User; // Assuming User is your staff model
                var result = await _staffRepository.AddStaffToHouse(selectedStaff.Id, _houseId);
                if (result)
                {
                    MessageBox.Show("Staff assign successfully", "Success");
                   
                }
                else
                {
                    MessageBox.Show("bu cu", "vc");
                }
            }
            else
            {
                MessageBox.Show("Please select a staff member to assign.", "Error");
            }
        }
        private async void Initialize()
        {
            var staffResult = await _staffRepository.GetAllStaffByOwnerId(App.LoggedInUserId);
            if (staffResult.IsSuccess)
            {
                var staffList = staffResult.Data as List<User>; // Assuming staffResult.Data is a List<User>
                if (staffList != null)
                {
                    cmbStaffList.ItemsSource = staffList;
                    cmbStaffList.DisplayMemberPath = "FullName"; // Assuming there's a property named FullName in your User model
                }
                else
                {
                    MessageBox.Show("Invalid data type for staff list.", "Error");
                }
            }
            else
            {
                MessageBox.Show(staffResult.Message, "Error");
            }
        }



    }
}



