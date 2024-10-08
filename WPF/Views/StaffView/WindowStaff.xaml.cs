﻿using BusinessObject.Object;
using DataAccess.Repository.CombineRepository;
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
using WPF.Views.HouseView;
using WPF.Views.StaffView;

namespace WPF.StaffView
{
    /// <summary>
    /// Interaction logic for WindowStaff.xaml
    /// </summary>
    public partial class WindowStaff : UserControl
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICombineRepository _repository;

        public WindowStaff(IServiceProvider serviceProvider, ICombineRepository repository)
        {
            _repository = repository;
            InitializeComponent();
            _serviceProvider = serviceProvider;
            LoadStaffs();
        }
       

        public async void LoadStaffs()
        {
            try
            {
                lvStaffs.ItemsSource = await _repository.GetAllStaffByOwnerId(App.LoggedInUserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddStaff_Click(object sender, RoutedEventArgs e)
        {
            var addStaffWindow = new WindowAddNewStaff(_repository);
            addStaffWindow.StaffAdded += (s, args) =>
            {
                LoadStaffs();
            };
            addStaffWindow.Show();
        }

        private void EditStaff_Click(object sender, RoutedEventArgs e)
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
                            if (listView.SelectedItem is User selectedUser)
                            {
                                var updateStaffWindow = new WindowUpdateStaff(_serviceProvider.GetService<ICombineRepository>(), selectedUser);
                                updateStaffWindow.staffUpdated += (s, args) =>
                                {
                                    LoadStaffs();
                                };
                                updateStaffWindow.ShowDialog();
                            }
                            else
                            {
                                MessageBox.Show("No user selected.");
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

        private async void DeleteStaff_Click(object sender, RoutedEventArgs e)
        {

            if (lvStaffs.SelectedItem is User selectedStaff)
            {
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {selectedStaff.FullName}?", "Warning", MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    try
                    {
                        var deleteResult = await _repository.DeleteCustomer(selectedStaff.Id);
                        if (deleteResult.IsSuccess)
                        {
                            MessageBox.Show("Staff deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadStaffs();
                        }
                        else
                        {
                            MessageBox.Show(deleteResult.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while deleting staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
               

    }
}
