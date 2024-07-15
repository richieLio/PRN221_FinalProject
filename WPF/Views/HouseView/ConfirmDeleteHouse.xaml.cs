using BusinessObject.Object;
using System;
using System.Windows;

namespace WPF.Views.HouseView
{
    public partial class ConfirmDeleteHouse : Window
    {
        public string HouseName { get; private set; }
        public bool IsConfirmed { get; private set; } = false;
        public event EventHandler HouseDeleted;

        public ConfirmDeleteHouse(string houseName)
        {
            InitializeComponent();
            HouseName = houseName;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
          
            if (HouseNameTextBox.Text == "")
            {
                MessageBox.Show("House name cannot be null", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (HouseNameTextBox.Text == HouseName)
            {
                IsConfirmed = true;
                HouseDeleted?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            else
            {
                MessageBox.Show("The entered name does not match the house name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
