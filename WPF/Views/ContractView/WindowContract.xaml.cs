using BusinessObject.Object;
using DataAccess.Model.BillModel;
using DataAccess.Model.ContractModel;
using DataAccess.Repository;
using DataAccess.Repository.CombineRepository;
using DataAccess.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using WPF.Views.BillView;
using WPF.Views.ContractView;

namespace WPF.ContractView
{
    /// <summary>
    /// Interaction logic for WindowContract.xaml
    /// </summary>
    public partial class WindowContract : UserControl
    {
        private readonly CloudStorage _cloudStorage;
        private readonly ICombineRepository _repository;

        public WindowContract(CloudStorage cloudStorage, ICombineRepository repository)
        {
            InitializeComponent();
            _cloudStorage = cloudStorage;
            _repository = repository;
            LoadHouses();
        }

        public async void LoadHouses()
        {
            var houses = await _repository.GetHouses(App.LoggedInUserId);
            cbHouses.ItemsSource = houses;
            if (houses.Any())
            {
                cbHouses.SelectedIndex = 0;
            }
        }
        private void cbHouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbHouses.SelectedItem is House selectedHouse)
            {
                LoadContracts(App.LoggedInUserId ,selectedHouse.Id);
            }
        }
        private async void LoadContracts(Guid userId, Guid houseId)
        {
            var result = await _repository.GetContractList(userId, houseId);
            if (result.IsSuccess)
            {
                lvContracts.ItemsSource = result.Data as List<ContractInfoResModel>;
            }
            else
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
      
       

       
        private async void ViewContractDetails_Click(object sender, RoutedEventArgs e)
        {
       
            if (lvContracts.SelectedItem is ContractInfoResModel selectedContract)
            {
                 var detailsWindow = new WindowContractDetails(_repository, selectedContract, _cloudStorage);
                detailsWindow.ShowDialog();
            }
        }   
     
       
    }
}
