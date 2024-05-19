using DataAccess.Model.HouseModel;
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

namespace WPF.Views.HouseView
{
    /// <summary>
    /// Interaction logic for WindowAddHouse.xaml
    /// </summary>
    public partial class WindowAddHouse : Window
    {
        public event EventHandler HouseAdded;

        private readonly IHouseRepository _houseRepository;
        public WindowAddHouse(IHouseRepository houseRepository)
        {
            InitializeComponent();
            _houseRepository = houseRepository;
        }

        private void btnAddNewHouse_Click(object sender, RoutedEventArgs e)
        {
            var house = new HouseCreateReqModel
            {
                Name = HouseNameTextBox.Text,
                Address = AddressTextBox.Text,
            };

            _houseRepository.AddHouse(App.LoggedInUserId, house);
            MessageBox.Show("House created successfully");
            HouseAdded?.Invoke(this, EventArgs.Empty); // Raise the event
            Close();
        }

    }
}
