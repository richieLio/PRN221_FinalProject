using BusinessObject.Object;
using DataAccess.Helper;
using DataAccess.Model.HouseModel;
using DataAccess.Repository.CombineRepository;
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
    /// Interaction logic for WindowUpdateHouse.xaml
    /// </summary>
    public partial class WindowUpdateHouse : Window
    {
        public event EventHandler HouseUpdated;
        private readonly House _house;


        private readonly ICombineRepository _repository;
        public WindowUpdateHouse(ICombineRepository repository, House house)
        {
            _repository = repository;
            InitializeComponent();
            _house = house;
            DataContext = house;

        }

        private void btnUpdateHouse_Click(object sender, RoutedEventArgs e)
        {
            var houseUpdate = new HouseUpdateReqModel
            {
                Id = _house.Id,
                Name = HouseNameTextBox.Text,
                Address = AddressTextBox.Text,
            };
            var validationResults = ValidationHelper.ValidateModel(houseUpdate);
            if (validationResults.Count > 0)
            {
                // Handle validation errors
                foreach (var validationResult in validationResults)
                {
                    MessageBox.Show(validationResult.ErrorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                }
                return;
            }
            _repository.UpdateHouse(App.LoggedInUserId, houseUpdate);
            MessageBox.Show("House updated successfully");
            HouseUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}
