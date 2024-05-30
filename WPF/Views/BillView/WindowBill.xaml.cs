using BusinessObject.Object;
using DataAccess.Model.BillModel;
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
using WPF.Views.BillView;

namespace WPF.BillView
{
    /// <summary>
    /// Interaction logic for WindowBill.xaml
    /// </summary>
    public partial class WindowBill : UserControl
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICombineRepository _repository;
        public WindowBill(IServiceProvider serviceProvider, ICombineRepository repository)
        {
            _repository = repository;
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        public async void LoadAllBill()
        {
            try
            {
                var result = await _repository.GetAllBills(App.LoggedInUserId);

                if (result.IsSuccess)
                {
                    if (result.Data is IEnumerable<BillResModel> bills)
                    {
                        lvBills.ItemsSource = bills;
                    }
                    else
                    {
                        MessageBox.Show("Invalid data type returned from repository.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public async void LoadBillByRoomId(Guid roomId)
        {
            try
            {
                var result = await _repository.GetBillByRoomID(roomId);

                if (result.IsSuccess)
                {
                    if (result.Data is IEnumerable<BillResModel> bills)
                    {
                        lvBills.ItemsSource = bills;
                    }
                    else
                    {
                        MessageBox.Show("Invalid data type returned from repository.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show(result.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading customers: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async void EditBill_Click(object sender, RoutedEventArgs e)
        {
            if (lvBills.SelectedItem is BillResModel selectedBill)
            {
                var billResult = await _repository.getBillById(selectedBill.Id);
                if (billResult == null)
                {
                    MessageBox.Show("Failed to fetch bill details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                

                var roomResult = await _repository.GetRoom(billResult.RoomId.Value);
                if (roomResult == null)
                {
                    MessageBox.Show("Failed to fetch room details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var updateWindow = new WindowUpdateBill(_repository, roomResult, billResult);
                updateWindow.ShowDialog();

                // Reload the bills after update
                LoadAllBill();
            }
        }

        private void DeleteBill_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
