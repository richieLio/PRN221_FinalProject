using DataAccess.Repository;
using Microsoft.AspNetCore.SignalR.Client;
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

namespace WPF
{
    /// <summary>
    /// Interaction logic for WindowContract.xaml
    /// </summary>
    public partial class WindowContract : UserControl
    {
        private readonly HubConnection _connection;

        public WindowContract()
        {
            InitializeComponent();
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7259/notihub")
                .WithAutomaticReconnect()
                .Build();

            openConnect();
        }
        private async void openConnect()
        {
            try
            {



                _connection.On<Guid, string>("ReceiveNotification", (ownerId, message) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        var newMessage = $"{ownerId}: {message}";
                        if (App.LoggedInUserId == ownerId)
                        {
                            messages.Items.Add(newMessage);
                        }
                    });
                });

                await _connection.StartAsync();
                messages.Items.Add("Connection started");
            }
            catch (Exception ex)
            {
                messages.Items.Add($"Error: {ex.Message}");
            }
        }

        private async void openConnection_Click(object sender, RoutedEventArgs e)
        {
            openConnect();
        }

    }
}
