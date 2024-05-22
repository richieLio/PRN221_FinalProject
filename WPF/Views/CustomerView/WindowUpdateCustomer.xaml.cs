﻿using BusinessObject.Object;
using DataAccess.Model.CustomerModel;
using DataAccess.Model.HouseModel;
using DataAccess.Repository;
using Microsoft.VisualBasic.Devices;
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

namespace WPF.Views.CustomerView
{
    /// <summary>
    /// Interaction logic for WindowUpdateCustomer.xaml
    /// </summary>
    public partial class WindowUpdateCustomer : Window
    {
        public event EventHandler CustomerUpdated;
        private readonly User _customer;

        private readonly ICustomerRepository _customerRepository;

        public WindowUpdateCustomer(ICustomerRepository customerRepository, User customer)
        {
            InitializeComponent();
            _customerRepository = customerRepository;
            _customer = customer;
            DataContext = customer;
        }

        private void btnUpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customerUpdate = new CustomerUpdateModel
            {
                Id = _customer.Id,
                Email = txtEmail.Text,
                PhoneNumber = txtPhoneNumber.Text,
                Address = txtAddress.Text,
                Gender = txtGender.Text,
                Dob = DobDatePicker.SelectedDate ?? DateTime.MinValue,
                FullName = txtFullName.Text,
                LicensePlates = txtLicensePlates.Text,
                Status = "Acitve",
                CitizenIdNumber = txtCitizenID.Text,

        };
            _customerRepository.UpdateUserProfile(customerUpdate);
            MessageBox.Show($"Customer {customerUpdate.FullName} updated successfully");
            CustomerUpdated?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}
