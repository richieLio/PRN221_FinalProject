using BusinessObject.Object;
using DataAccess.Model.ServiceFeeModel;
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

namespace WPF.Views.ServiceFeeView
{
    /// <summary>
    /// Interaction logic for WindowUpdateServiceFee.xaml
    /// </summary>
    public partial class WindowUpdateServiceFee : Window
    {
        public event EventHandler ServiceUpdated;
        private readonly Service _service;
        private readonly House _house;


        private readonly ICombineRepository _repository;
        public WindowUpdateServiceFee(ICombineRepository repository, Service service, House house)
        {
            _repository = repository;
            InitializeComponent();
            _service = service;
            DataContext = service;
            _house = house;
        }

        private void btnUpdateService_Click(object sender, RoutedEventArgs e)
        {
            var serviceUpdate = new ServiceUpdateReqModel
            {
                Id = _service.Id,
                Name = txtServiceName.Text,
                Price = decimal.Parse(txtServicePrice.Text),
                HouseId = _house.Id,
            };
            _repository.UpdateService(App.LoggedInUserId, serviceUpdate);
            MessageBox.Show("Service updated successfully");
            ServiceUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}
