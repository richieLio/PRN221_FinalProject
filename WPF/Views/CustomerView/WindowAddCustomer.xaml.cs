using BusinessObject.Object;
using DataAccess.Model.CustomerModel;
using DataAccess.Model.HouseModel;
using DataAccess.Repository;
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
                    },
                    houseUpdateAvaiableRoom = new HouseUpdateAvaiableRoomReqModel
                    {
                        HouseId = _houseId,
                        AvailableRoom = availableRoom,
                    }
                };

                var result = await _repository.AddCustomerToRoom(App.LoggedInUserId, customer);

                if (result.IsSuccess)
                {
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
