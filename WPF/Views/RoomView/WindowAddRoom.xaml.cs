using BusinessObject.Object;
using DataAccess.Model.HouseModel;
using DataAccess.Model.RoomModel;
using DataAccess.Repository;
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

namespace WPF.Views.RoomView
{
    /// <summary>
    /// Interaction logic for WindowAddRoom.xaml
    /// </summary>
    public partial class WindowAddRoom : Window
    {
        public event EventHandler RoomAdded;
        private readonly IRoomRepository _roomRepository;
        private readonly Guid _houseId;

        public WindowAddRoom(IRoomRepository roomRepository, Guid houseId)
        {
            InitializeComponent();
            _roomRepository = roomRepository;
            _houseId = houseId;
        }

        private void btnAddNewRoom_Click(object sender, RoutedEventArgs e)
        {
            var room = new RoomCreateReqModel
            {
                RoomId = Guid.NewGuid(),
                HouseId = _houseId,
                Name = RoomNameTextBox.Text,
                Price = decimal.Parse(RoomPriceTextBox.Text),
            };

            _roomRepository.AddRoom(App.LoggedInUserId, room);
            MessageBox.Show("Room created successfully");
            RoomAdded?.Invoke(this, EventArgs.Empty); 
            Close();
        }
    }
}
