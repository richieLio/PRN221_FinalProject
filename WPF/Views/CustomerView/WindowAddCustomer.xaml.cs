using BusinessObject.Object;
using DataAccess.Enums;
using DataAccess.Helper;
using DataAccess.Model.CustomerModel;
using DataAccess.Model.HouseModel;
using DataAccess.Repository.CombineRepository;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace WPF.Views.CustomerView
{
    public partial class WindowAddCustomer : Window
    {
        private readonly ICombineRepository _repository;
        public event EventHandler CustomerAdded;
        private readonly Guid _houseId;
        private readonly Guid _roomId;

        public WindowAddCustomer(ICombineRepository repository, Guid houseId, Guid roomId)
        {
            _repository = repository;
            InitializeComponent();
            _houseId = houseId;
            _roomId = roomId;
        }

        private async void btnCreateCustomer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? availableRoom = await _repository.GetRoomQuantityByHouseId(_houseId);
                if (availableRoom == null)
                {
                    MessageBox.Show("House not found.");
                    return;
                }

                var customer = new AddCustomerToRoomReqModel
                {
                    customerCreateReqModel = new CustomerCreateReqModel
                    {
                        FullName = txtFullName.Text.Trim(),
                        Address = txtAddress.Text.Trim(),
                        PhoneNumber = txtPhoneNumber.Text.Trim(),
                        Gender = txtGender.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        EndDate = EndDateDatePicker.SelectedDate ?? DateTime.MinValue,
                        CreatedAt = DateTime.Now,
                        Dob = DobDatePicker.SelectedDate ?? DateTime.MinValue,
                        LicensePlates = txtLicensePlates.Text.Trim(),
                        RoomId = _roomId,
                        CitizenIdNumber = txtCitizenID.Text.Trim(),
                    },
                    houseUpdateAvaiableRoom = new HouseUpdateAvaiableRoomReqModel
                    {
                        HouseId = _houseId,
                        AvailableRoom = availableRoom,
                    }

                };
                var validationResults = ValidationHelper.ValidateModel(customer.customerCreateReqModel);
                if (validationResults.Count > 0)
                {
                    // Handle validation errors
                    foreach (var validationResult in validationResults)
                    {
                        MessageBox.Show(validationResult.ErrorMessage, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
                    return;
                }


                var result = await _repository.AddCustomerToRoom(App.LoggedInUserId, customer);


                if (result.IsSuccess)
                {
                    var room = await _repository.GetRoom(_roomId);
                    var house = await _repository.GetHouse(_houseId);
                    var creator = await _repository.GetUserById(App.LoggedInUserId);

                    var message = $"A new customer has been added to {room.Name} of {house.Name} by staff {creator.FullName}";
                    var user = await _repository.GetUserById(App.LoggedInUserId);
                    if (user.Role != UserEnum.OWNER)
                    {
                        // add localNoti
                        var localNotification = new LocalNotification
                        {
                            Id = Guid.NewGuid(),
                            Subject = "New customer added ",
                            Content = message,
                            CreatedAt = DateTime.Now,
                            UserId = creator.Role == UserEnum.OWNER ? App.LoggedInUserId : creator.OwnerId,
                            IsRead = false,
                        };
                        await _repository.InsertLocalNotifications(App.LoggedInUserId, localNotification);
                        var unReadNoti = _repository.GetNotificationQuantity(creator.OwnerId.Value);

                        // Send SignalR notification
                        await App.SignalRConnection.InvokeAsync("NotifyCustomerAdded", room.Name, txtFullName.Text.Trim(), message, unReadNoti);
                    }

                    MessageBox.Show("Customer added to room successfully.");
                    CustomerAdded?.Invoke(this, EventArgs.Empty);

                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Failed to add customer to room: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }
    }
}
