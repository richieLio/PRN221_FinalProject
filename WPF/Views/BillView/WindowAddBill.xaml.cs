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
using Microsoft.AspNetCore.SignalR.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WPF.Views.BillView
{
    public partial class WindowAddBill : Window
    {
        private readonly ICombineRepository _repository;
        private readonly Room _room;
        private readonly House _house;
        private IEnumerable<ServiceViewModel> _services;
        HubConnection _connection;


        public WindowAddBill(ICombineRepository repository, Room room, House house)
        {
            InitializeComponent();
            _repository = repository;
            _room = room;
            _house = house;
            LoadData();
            SetupSignalR();
        }
        private async void SetupSignalR()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7259/notihub")
            .WithAutomaticReconnect()
                .Build();

            await _connection.StartAsync();

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
                var user =  await _repository.GetUserById(App.LoggedInUserId);

                var result = await _repository.CreateBill(App.LoggedInUserId, newBill);

                if (result.IsSuccess)
                {
                    MessageBox.Show("Bill created successfully!");


                    try { 
                    await _connection.InvokeAsync("NotifyBillCreated", _house.OwnerId,
                        $"A new bill in {_room.Name} of {_house.Name} has been created by {user.FullName}");
                    } catch (Exception ex)
                    {
                        MessageBox.Show("signalR: " + ex.Message);
                    }
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
