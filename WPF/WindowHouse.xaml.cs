using DataAccess.Repository;
using System.Windows.Controls;

namespace WPF
{
    public partial class WindowHouse : UserControl
    {
        private readonly IHouseRepository _houseRepository;

        public WindowHouse(IHouseRepository houseRepository)
        {
            InitializeComponent();
            _houseRepository = houseRepository;
            LoadHouses();
        }

        private async void LoadHouses()
        {
            lvHouses.ItemsSource = await _houseRepository.GetHouses(App.LoggedInUserId);
        }
    }
}
