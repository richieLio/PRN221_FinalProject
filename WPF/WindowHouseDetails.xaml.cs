using BusinessObject.Object;
using DataAccess.Repository;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF
{
    public partial class WindowHouseDetails : UserControl
    {
        private readonly IHouseRepository _houseRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IServiceProvider _serviceProvider;
        private string _houseName;

        public WindowHouseDetails(IHouseRepository houseRepository, IRoomRepository roomRepository, IServiceProvider serviceProvider, string houseName)
        {
            InitializeComponent();
            _houseRepository = houseRepository;
            _roomRepository = roomRepository;
            _serviceProvider = serviceProvider;
            _houseName = houseName;
            HouseNameTextBlock.Text = _houseName;
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
           
        }
    }
}



