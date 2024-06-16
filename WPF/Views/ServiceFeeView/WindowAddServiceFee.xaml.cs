using BusinessObject.Object;
using DataAccess.Model.ServiceFeeModel;
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

namespace WPF.Views.ServiceFeeView
{
    /// <summary>
    /// Interaction logic for WindowAddServiceFee.xaml
    /// </summary>
    public partial class WindowAddServiceFee : Window
    {
        public event EventHandler ServiceAdded;
        private readonly ICombineRepository _repository;
        private readonly House _house;
        public WindowAddServiceFee(ICombineRepository repository, House house)
        {
            _repository = repository;
            InitializeComponent();
            _house = house;
        }
       

        private async void btnAddNewService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var service = new ServiceCreateReqModel
                {
                    Price = decimal.Parse(txtServicePrice.Text),
                    Name = txtServiceName.Text,
                    HouseId = _house.Id,
                };
                var result = await _repository.AddNewService(App.LoggedInUserId, service);
                if (result.IsSuccess)
                {
                    MessageBox.Show("Service added sucessfully");
                }
                else
                {
                    MessageBox.Show(result.Message, "Erorr");
                }
                ServiceAdded?.Invoke(this, EventArgs.Empty);
                Close();
            }
            catch
            {
                MessageBox.Show("Pls fill all the fields");
            }
        }
    }
}
