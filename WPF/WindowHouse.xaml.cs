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
            // Tạo một instance mới của WindowHouseDetails
            WindowHouseDetails detailsWindow = new WindowHouseDetails(_houseRepository, _roomRepository, _serviceProvider);

            // Lấy houseId từ item được chọn trong lvHouses
            var selectedHouse = lvHouses.SelectedItem as House; // Giả sử House là một class trong BusinessObject.Object
            if (selectedHouse != null)
            {
                // Gọi phương thức LoadRooms với houseId tương ứng
                detailsWindow.LoadRooms(selectedHouse.Id);
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    // Thay đổi nội dung của MainContentControl sang WindowHouseDetails
                    mainWindow.MainContentControl.Content = detailsWindow;
                }
            }
        }




    }
}
