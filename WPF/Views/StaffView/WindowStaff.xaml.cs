using BusinessObject.Object;
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
        private readonly IUserRepository _userRepository;
        private readonly IStaffRepository _staffRepository;

        public WindowStaff(IServiceProvider serviceProvider, IUserRepository userRepository, IStaffRepository staffRepository)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
            _staffRepository = staffRepository;
            LoadStaffs();
        }

        public async void LoadStaffs()
        {
            try
            {
                var result = await _staffRepository.GetAllStaffByOwnerId(App.LoggedInUserId);

                if (result.IsSuccess)
                {
                    if (result.Data is IEnumerable<User> users)
                    {
                        lvStaffs.ItemsSource = users;
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
                MessageBox.Show($"An error occurred while loading staff: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddStaff_Click(object sender, RoutedEventArgs e)
        {
            var addStaffWindow = new WindowAddNewStaff(_userRepository, _staffRepository);
            addStaffWindow.StaffAdded += (s, args) =>
            {
                LoadStaffs();
            };
            addStaffWindow.Show();
        }

        private void EditStaff_Click(object sender, RoutedEventArgs e)
        {
            // Implement edit functionality
        }

        private void DeleteStaff_Click(object sender, RoutedEventArgs e)
        {
            // Implement delete functionality
        }
    }
}
