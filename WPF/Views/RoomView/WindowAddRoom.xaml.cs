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
        private readonly ICombineRepository _repository;
        private readonly Guid _houseId;

        public WindowAddRoom(ICombineRepository repository, Guid houseId)
        {
            _repository = repository;
            InitializeComponent();
            _houseId = houseId;
        }

        private void btnAddNewRoom_Click(object sender, RoutedEventArgs e)
        { try
            {
                var room = new RoomCreateReqModel
                {
                    RoomId = Guid.NewGuid(),
                    HouseId = _houseId,
                    Name = RoomNameTextBox.Text,
                    Price = decimal.Parse(RoomPriceTextBox.Text),
                };
                _repository.AddRoom(App.LoggedInUserId, room);
                MessageBox.Show("Room created successfully");
                RoomAdded?.Invoke(this, EventArgs.Empty);
                Close();
            } catch {
                MessageBox.Show("Pls fill all fields");
            }
            
        }
    }
}
