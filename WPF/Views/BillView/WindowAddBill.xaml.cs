﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BusinessObject.Object;
using DataAccess.DAO;
using DataAccess.Enums;
using DataAccess.Model.BillModel;
using DataAccess.Model.ServiceFeeModel;
using DataAccess.Repository.CombineRepository;
using Microsoft.AspNetCore.SignalR.Client;
using WPF.Views.ReportView;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WPF.Views.BillView
{
    public partial class WindowAddBill : Window
    {
        private readonly ICombineRepository _repository;
        public event EventHandler BillAdded;
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
                    ServiceQuantities = serviceQuantities
                };
                var user = await _repository.GetUserById(App.LoggedInUserId);

                var result = await _repository.CreateBill(App.LoggedInUserId, newBill);
                if (result.IsSuccess)
                {
                    MessageBox.Show("Bill created successfully!");
                    try
                    {

                        var billCreatedBy = await _repository.GetUserById(App.LoggedInUserId);


                        var message = $"A new bill in {_room.Name} of {_house.Name} has been created by staff {user.FullName}";
                        // add localNoti
                        var localNotification = new LocalNotification
                        {
                            Id = Guid.NewGuid(),
                            Subject = "New bill created",
                            Content = message,
                            CreatedAt = DateTime.Now,
                            UserId = billCreatedBy.Role == UserEnum.OWNER ? App.LoggedInUserId : billCreatedBy.OwnerId,
                            IsRead = false,
                        };
                        await _repository.InsertLocalNotifications(App.LoggedInUserId, localNotification);
                        BillAdded?.Invoke(this, EventArgs.Empty);
                       
                        Close();

                        var unReadNoti = _repository.GetNotificationQuantity(user.OwnerId.Value);

                        //send signal R
                        var bill = result.Data as Bill;
                        if (bill != null)
                        {
                            await App.SignalRConnection.InvokeAsync("NotifyBillCreated", _house.OwnerId, bill.Id,
                                message, unReadNoti);
                        }
                       
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("signalR: " + ex.Message);
                    }
                    this.Close();
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
