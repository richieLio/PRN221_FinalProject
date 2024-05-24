using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly ICombineRepository _repository;
        private string _houseName;
        private string _address;
        private int? _roomQuantity;
        private int? _availableRoom;
        private Guid _houseId;

        public WindowHouseDetails(IServiceProvider serviceProvider, ICombineRepository repository,
            string houseName, string address, int? roomQuantity, int? availableRoom, Guid houseId)
        {
            InitializeComponent();

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
            _repository = repository;
            Initialize();
        }

        public async void LoadRooms(Guid houseId)
        {
            lvRooms.ItemsSource = await _repository.GetRooms(houseId);
        }

        private void BackToHouseManagement_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                WindowHouse houseWindow = new WindowHouse(_serviceProvider, _repository);
                mainWindow.MainContentControl.Content = houseWindow;
            }
        }

        private void Border_Click(object sender, MouseButtonEventArgs e)
        {
            var selectedRoom = (sender as Border)?.DataContext as Room;
            if (selectedRoom != null)
            {
                WindowCustomersInRoom customersWindow = new WindowCustomersInRoom(_serviceProvider, _repository, _houseId, selectedRoom.Id);
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
            var windowAddRoom = new WindowAddRoom(_serviceProvider.GetService<ICombineRepository>(), _houseId);
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
                var customers = await _repository.GetListCustomerByRoomId(room.Id);
                PopupRoomName.Text = room.Name;
                PopupRoomPrice.Text = $"Price: {room.Price}";
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
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                Border border = ((ContextMenu)menuItem.Parent).PlacementTarget as Border;
                if (border != null)
                {
                    var room = border.DataContext as Room;
                    if (room != null)
                    {
                        var updateRoomWindow = new WindowUpdateRoom(_serviceProvider.GetService<ICombineRepository>(), room);
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
                            await _repository.DeleteRoom(_houseId, room.Id);
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
                var selectedStaff = cmbStaffList.SelectedItem as User;
                var result = await _repository.AddStaffToHouse(selectedStaff.Id, _houseId);
                if (result)
                {
                    MessageBox.Show("Staff assigned successfully", "Success");
                    Initialize();  // Refresh the UI to reflect changes
                }
                else
                {
                    MessageBox.Show("Failed to assign staff.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Please select a staff member to assign.", "Error");
            }
        }

        private async void Initialize()
        {
            var staffResult = await _repository.GetAllStaffByOwnerId(App.LoggedInUserId);

            if (staffResult != null)
            {
                cmbStaffList.ItemsSource = staffResult;
                cmbStaffList.DisplayMemberPath = "FullName";
            }
            else
            {
                MessageBox.Show("Invalid data type for staff list.", "Error");
            }

            var assignedStaffResult = await _repository.GetAssignedStaffByHouseId(_houseId);
            if (assignedStaffResult.IsSuccess && assignedStaffResult.Data != null)
            {
                var assignedStaff = assignedStaffResult.Data as User;
                if (assignedStaff != null)
                {
                    txtStaffName.Text = $"Managed by: {assignedStaff.FullName}";
                    txtStaffName.Visibility = Visibility.Visible;
                    cmbStaffList.Visibility = Visibility.Collapsed;
                    cmbStaffList.IsEnabled = false;
                    AssignStaffButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show("Invalid data type for assigned staff.", "Error");
                }
            }
            else
            {
                txtStaffName.Visibility = Visibility.Collapsed;
                cmbStaffList.Visibility = Visibility.Visible;
                cmbStaffList.IsEnabled = true;
                AssignStaffButton.Visibility = Visibility.Visible;

            }
        }

    }
}
