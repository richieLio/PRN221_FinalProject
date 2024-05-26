using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Model.BillModel;
using DataAccess.Model.ServiceFeeModel;
using DataAccess.Repository;

namespace WPF.Views.BillView
{
    public partial class WindowAddBill : Window
    {
        private readonly ICombineRepository _repository;
        private readonly Room _room;
        private readonly House _house;
        private IEnumerable<ServiceViewModel> _services;


        public WindowAddBill(ICombineRepository repository, Room room, House house)
        {
            InitializeComponent();
            _repository = repository;
            _room = room;
            _house = house;
            LoadData();
        }

        private async void LoadData()
        {
            try
            {

                IEnumerable<Service> services = await _repository.GetServicesList(_house.Id);

                _services = services.Select(s => new ServiceViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    IsSelected = false, 
                    Quantity = 0 
                }).ToList();

                servicesListBox.ItemsSource = _services;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading services: {ex.Message}");
            }
        }

        private async void CreateBill_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedServices = _services.Where(s => s.IsSelected).ToList();
                var serviceQuantities = selectedServices.ToDictionary(s => s.Id, s => s.Quantity);

                var newBill = new BillCreateReqModel
                {
                    RoomId = _room.Id,
                    ServiceQuantities = serviceQuantities // Assign the mapped dictionary
                };

                var result = await _repository.CreateBill(App.LoggedInUserId, newBill);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Bill created successfully!");
                }
                else
                {
                    MessageBox.Show($"Failed to create bill: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating bill: {ex.Message}");
            }
        }

       
    }
}
