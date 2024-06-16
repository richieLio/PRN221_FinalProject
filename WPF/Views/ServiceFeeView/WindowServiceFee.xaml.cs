using BusinessObject.Object;
using DataAccess.Model.ServiceFeeModel;
using DataAccess.Repository.CombineRepository;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WPF.Views.ServiceFeeView
{
    public partial class WindowServiceFee : UserControl
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICombineRepository _repository;

        public WindowServiceFee(ICombineRepository repository, IServiceProvider serviceProvider)
        {
            _repository = repository;
            InitializeComponent();
            _serviceProvider = serviceProvider;
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

        public async void LoadService(Guid houseId)
        {
            lvServices.ItemsSource = await _repository.GetServicesList(houseId);
        }


        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            if (cbHouses.SelectedItem is House selectedHouse)
            {
                var windowAddService = new WindowAddServiceFee(_serviceProvider.GetService<ICombineRepository>(), selectedHouse);
                windowAddService.ServiceAdded += (s, args) =>
                {
                    LoadService(selectedHouse.Id);
                };
                windowAddService.Show();
            }
            else
            {
                MessageBox.Show("Please select a house first.");
            }
        }

        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lvServices.SelectedItem is Service selectedService && cbHouses.SelectedItem is House selectedHouse)
                {
                    var updateServiceWindow = new WindowUpdateServiceFee(_serviceProvider.GetService<ICombineRepository>(), selectedService, selectedHouse);
                    updateServiceWindow.ServiceUpdated += (s, args) =>
                    {
                        LoadService(selectedHouse.Id);
                    };
                    updateServiceWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No service selected or no house selected.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private async void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (lvServices.SelectedItem is Service selectedService && cbHouses.SelectedItem is House selectedHouse)
            {
                MessageBoxResult confirm = MessageBox.Show($"Are you sure to delete {selectedService.Name}?", "Warning", MessageBoxButton.OKCancel);

                if (confirm == MessageBoxResult.OK)
                {
                    var result = await _repository.RemoveService(App.LoggedInUserId, selectedService.Id, selectedHouse.Id);
                    if (result.IsSuccess)
                    {
                        MessageBox.Show("Service deleted successfully");
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Error");
                    }
                    LoadService(selectedHouse.Id);
                }
            }
        }

        private void cbHouses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbHouses.SelectedItem is House selectedHouse)
            {
                LoadService(selectedHouse.Id);
            }
        }
    }
}
