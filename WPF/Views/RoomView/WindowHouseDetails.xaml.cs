using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Enums;
using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using WPF.BillView;
using WPF.Views.BillView;
using WPF.Views.CustomerView;
using WPF.Views.HouseView;
using WPF.Views.RoomView;

namespace WPF
{
    public partial class WindowHouseDetails : UserControl
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICombineRepository _repository;
        private readonly House _house;

        public WindowHouseDetails(IServiceProvider serviceProvider, ICombineRepository repository,
           House house)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;
            _house = house;
            HouseNameTextBlock.Text = _house.Name;
            AddressTextBlock.Text = _house.Address;
            RoomQuantityTextBlock.Text = _house.RoomQuantity.ToString();
            AvailableRoomTextBlock.Text = _house.AvailableRoom.ToString();
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
                WindowCustomersInRoom customersWindow = new WindowCustomersInRoom(_serviceProvider, _repository, _house.Id, selectedRoom.Id);
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
            var windowAddRoom = new WindowAddRoom(_serviceProvider.GetService<ICombineRepository>(), _house.Id);
            windowAddRoom.RoomAdded += (s, args) =>
            {
                LoadRooms(_house.Id);
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
                            LoadRooms(_house.Id);
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
                            await _repository.DeleteRoom(_house.Id, room.Id);
                            MessageBox.Show("Room deleted successfully.");
                            LoadRooms(_house.Id);
                        };
                        confirmDialog.ShowDialog();
                    }
                }
            }
        }
        private async void AddBill_Click(object sender, RoutedEventArgs e)
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
                        WindowAddBill windowAddBill = new WindowAddBill(_repository, room, _house);
                        windowAddBill.ShowDialog();
                    }
                }
            }
        }
        private async void ViewListBill_Click(object sender, RoutedEventArgs e)
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
                        WindowBill windowBill = new WindowBill(_serviceProvider, _repository);
                        windowBill.LoadBillByRoomId(room.Id);

                        MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                        if (mainWindow != null)
                        {
                            mainWindow.MainContentControl.Content = windowBill;
                        }
                    }
                }
            }
        }

        private async void AssignStaff_Click(object sender, RoutedEventArgs e)
        {
            if (cmbStaffList.SelectedItem != null)
            {
                var selectedStaff = cmbStaffList.SelectedItem as User;
                var result = await _repository.AddStaffToHouse(selectedStaff.Id, _house.Id);
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
        private async void UnAssignStaff_Click(object sender, RoutedEventArgs e)
        {
            var staffResult = await _repository.GetAssignedStaffByHouseId(_house.Id);

            if (staffResult.IsSuccess)
            {
                var staff = staffResult.Data;

                if (staff != null)
                {
                    if (staff is User user) 
                    {
                        var result = await _repository.RemoveStaffFromHouse(user.Id, _house.Id);

                        if (result)
                        {
                            MessageBox.Show("Staff unassigned successfully", "Success");
                            Initialize();
                        }
                        else
                        {
                            MessageBox.Show("Failed to unassign staff.", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid staff type.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("No staff assigned to this house.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Failed to retrieve assigned staff.", "Error");
            }
        }




        private async void Initialize()
        {
            var user = await _repository.GetUserById(App.LoggedInUserId);
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

            var assignedStaffResult = await _repository.GetAssignedStaffByHouseId(_house.Id);
            if (assignedStaffResult.IsSuccess && assignedStaffResult.Data != null)
            {
                var assignedStaff = assignedStaffResult.Data as User;
                if (assignedStaff != null && user.Role == UserEnum.OWNER)
                {
                    txtStaffName.Text = $"Managed by: {assignedStaff.FullName}";
                    txtStaffName.Visibility = Visibility.Visible;
                    cmbStaffList.Visibility = Visibility.Collapsed;
                    cmbStaffList.IsEnabled = false;
                    AssignStaffButton.Visibility = Visibility.Collapsed;
                    UnAssignStaffButton.Visibility = Visibility.Visible;
                }
                
            }
            else
            {
                txtStaffName.Visibility = Visibility.Collapsed;
                cmbStaffList.Visibility = Visibility.Visible;
                cmbStaffList.IsEnabled = true;
                AssignStaffButton.Visibility = Visibility.Visible;
                UnAssignStaffButton.Visibility = Visibility.Collapsed;

            }
        }

    }
}
