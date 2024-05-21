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

namespace WPF.Views.RoomView
{
    /// <summary>
    /// Interaction logic for WindowDeleteRoom.xaml
    /// </summary>
    public partial class WindowDeleteRoom : Window
    {
        public string RoomName { get; private set; }
        public bool IsConfirmed { get; private set; } = false;
        public event EventHandler RoomDeleted;
        public WindowDeleteRoom(string roomName)
        {
            InitializeComponent();
            RoomName = roomName;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (RoomNameTextBox.Text == RoomName)
            {
                IsConfirmed = true;
                RoomDeleted?.Invoke(this, EventArgs.Empty);
                this.Close();
            }
            else
            {
                MessageBox.Show("The entered name does not match the room name.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
