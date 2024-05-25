using BusinessObject.Object;
using DataAccess.Model.ServiceFeeModel;
using DataAccess.Repository;
using Microsoft.Extensions.DependencyInjection;
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
using WPF.Views.CustomerView;
using WPF.Views.RoomView;

namespace WPF.Views.ServiceFeeView
{
    /// <summary>
    /// Interaction logic for WindowServiceFee.xaml
    /// </summary>
    public partial class WindowServiceFee : UserControl
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICombineRepository _repository;
        private readonly House _house;
        public WindowServiceFee(ICombineRepository repository, IServiceProvider serviceProvider, House house)
        {
            _repository = repository;
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _house = house;
        }

        public async void LoadService()
        {
            lvServices.ItemsSource = await _repository.GetServicesList(_house.Id);
        }
        private void BackToHouseManagement_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                WindowHouse houseWindow = new WindowHouse(_serviceProvider, _repository);
                mainWindow.MainContentControl.Content = houseWindow;
            }
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            var windowAddService = new WindowAddServiceFee(_serviceProvider.GetService<ICombineRepository>(), _house);
            windowAddService.ServiceAdded += (s, args) =>
            {
                LoadService();
            };
            windowAddService.Show();
        }
        private void EditService_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = sender as MenuItem;
                if (menuItem != null)
                {
                    ContextMenu contextMenu = menuItem.Parent as ContextMenu;
                    if (contextMenu != null)
                    {
                        ListView listView = contextMenu.PlacementTarget as ListView;
                        if (listView != null)
                        {
                            if (listView.SelectedItem is Service selectedService)
                            {
                                var updateServiceWindow = new WindowUpdateServiceFee(_serviceProvider.GetService<ICombineRepository>(), selectedService, _house);
                                updateServiceWindow.ServiceUpdated += (s, args) =>
                                {
                                    LoadService();
                                };
                                updateServiceWindow.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("No service selected.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("ListView is null.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("ContextMenu is null.");
                    }
                }
                else
                {
                    MessageBox.Show("MenuItem is null.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
        private async void DeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (lvServices.SelectedItem is Service selectedService)
            {
                MessageBoxResult confirm = MessageBox.Show($"Are you sure to delete {selectedService.Name}?", "Warning", MessageBoxButton.OKCancel);

                if (confirm == MessageBoxResult.OK)
                {
                  var result = await _repository.RemoveService(App.LoggedInUserId ,selectedService.Id, _house.Id);
                    if (result.IsSuccess)
                    {
                        MessageBox.Show("Service deleted sucessfully");
                    }
                    else
                    {
                        MessageBox.Show(result.Message, "Erorr");
                    }
                    LoadService();
                }
            }
        }
    }
}
