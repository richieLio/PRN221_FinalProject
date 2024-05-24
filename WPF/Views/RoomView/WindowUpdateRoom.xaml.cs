using BusinessObject.Object;
using DataAccess.Model.HouseModel;
using DataAccess.Model.RoomModel;
using DataAccess.Repository;
using Microsoft.VisualBasic.Devices;
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
    /// Interaction logic for WindowUpdateRoom.xaml
    /// </summary>
    public partial class WindowUpdateRoom : Window
    {
        public event EventHandler RoomUpdated;
        private readonly Room _room;

        private readonly ICombineRepository _repository;

        public WindowUpdateRoom(ICombineRepository repository, Room room)
        {
            _repository = repository;
            InitializeComponent();
            _room = room;
            DataContext = room;
        }
        private void btnUpdateRoom_Click(object sender, RoutedEventArgs e)
        {
            var roomUpdate = new RoomUpdateReqModel
            {
                Id = _room.Id,
                Name = RoomNameTextBox.Text,
                Price = decimal.Parse(PriceTextBox.Text),
            };
            _repository.UpdateRoom(roomUpdate);
            MessageBox.Show("Room updated successfully");
            RoomUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }

    }
}
