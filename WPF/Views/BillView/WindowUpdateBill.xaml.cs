using BusinessObject.Object;
using DataAccess.Model.BillModel;
using DataAccess.Model.ServiceFeeModel;
using DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WPF.Views.BillView
{
    public partial class WindowUpdateBill : Window
    {
        private readonly ICombineRepository _repository;
        private readonly Room _room;
        private readonly Bill _existingBill;
        private IEnumerable<ServiceViewModel> _services;

        public WindowUpdateBill(ICombineRepository repository, Room room, Bill existingBill)
        {
            InitializeComponent();
            _repository = repository;
            _room = room;
            _existingBill = existingBill;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {
                Guid houserId = _room.HouseId.Value;
                IEnumerable<Service> services = await _repository.GetServicesList(houserId);

                _services = services.Select(s => new ServiceViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsSelected = _existingBill.BillServices.Any(bs => bs.ServiceId == s.Id),
                    Quantity = _existingBill.BillServices.FirstOrDefault(bs => bs.ServiceId == s.Id)?.Quantity ?? 0
                }).ToList();

                servicesListBox.ItemsSource = _services;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading services: {ex.Message}");
            }
        }

        private async void UpdateBillButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Lấy danh sách các dịch vụ được chọn
                var selectedServices = _services.Where(s => s.IsSelected).ToList();

                // Tạo một dictionary để lưu các dịch vụ và số lượng tương ứng
                var serviceQuantities = new Dictionary<Guid, decimal>();

                // Lặp qua tất cả các dịch vụ trong danh sách
                foreach (var service in _services)
                {
                    // Nếu dịch vụ đó được chọn
                    if (service.IsSelected)
                    {
                        // Lấy số lượng từ dịch vụ đã chọn
                        serviceQuantities.Add(service.Id, service.Quantity);
                    }
                    else
                    {
                        // Nếu không được chọn, đặt số lượng thành 0
                        serviceQuantities.Add(service.Id, 0);
                    }
                }

                // Tạo đối tượng BillUpdateReqModel để cập nhật hóa đơn
                var updateBill = new BillUpdateReqModel
                {
                    BillId = _existingBill.Id,
                    RoomId = _room.Id,
                    ServiceQuantities = serviceQuantities
                };

                // Gọi phương thức cập nhật hóa đơn từ repository
                var result = await _repository.UpdateBill(App.LoggedInUserId, updateBill);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Bill updated successfully!");
                }
                else
                {
                    MessageBox.Show($"Failed to update bill: {result.Message}");
                }

                // Đóng cửa sổ
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating bill: {ex.Message}");
            }
        }

    }


}
