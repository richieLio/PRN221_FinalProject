using DataAccess.Helper;
using DataAccess.Model.HouseModel;
using DataAccess.Model.UserModel;
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
    /// Interaction logic for WindowAddHouse.xaml
    /// </summary>
    public partial class WindowAddHouse : Window
    {
        public event EventHandler HouseAdded;

        private readonly ICombineRepository _repository;
        public WindowAddHouse(ICombineRepository repository)
        {
            _repository = repository;
            InitializeComponent();
        }

        private void btnAddNewHouse_Click(object sender, RoutedEventArgs e)
        {
            var house = new HouseCreateReqModel
            {
                Name = HouseNameTextBox.Text,
                Address = AddressTextBox.Text,
            };
            var validationResults = ValidationHelper.ValidateModel(house);
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

            _repository.AddHouse(App.LoggedInUserId, house);
            MessageBox.Show("House created successfully");
            HouseAdded?.Invoke(this, EventArgs.Empty); 
            Close();
        }

    }
}
