using BusinessObject.Object;
using DataAccess.Repository.CombineRepository;
using LiveCharts.Wpf.Charts.Base;
using LiveCharts.Wpf;
using LiveCharts;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualBasic.ApplicationServices;
using DataAccess.Model.HouseModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

namespace WPF.Views.ReportView
{
    public partial class WindowReport : UserControl
    {
        private readonly SolidColorBrush[] seriesColors = { Brushes.Blue, Brushes.OrangeRed, Brushes.Pink };
        private readonly ICombineRepository _repository;
        private IEnumerable<House> _houses;

        public Func<double, string> Formatter { get; set; }
        public List<string> HouseNames { get; set; }

        public WindowReport(ICombineRepository repository)
        {
            InitializeComponent();
            _repository = repository;
            Formatter = value => value.ToString("N0", CultureInfo.InvariantCulture);
            DataContext = this;
            LoadHouses(App.LoggedInUserId);
        }

        private async void LoadHouses(Guid userId)
        {
            try
            {
                _houses = await _repository.GetHouses(userId);
                await UpdateChartData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving houses: {ex.Message}");
            }
        }

        public async Task UpdateChartData()
        {
            if (_houses == null || !_houses.Any()) return;

            DateTime startDate = DateTime.Now.AddYears(-1);
            DateTime endDate = DateTime.Now.AddMinutes(30);
            var monthlyData = await _repository.GetMonthlyRevenueByHouseWithPaidStatus(App.LoggedInUserId, startDate, endDate);

            var totalRevenues = monthlyData.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Sum(data => data.Revenue)
            );

            var topHouses = totalRevenues.OrderByDescending(kvp => kvp.Value)
                                         .Take(5)
                                         .Select(kvp => kvp.Key)
                                         .ToList();

            TopHousesGrid.Children.Clear();

            for (int i = 0; i < topHouses.Count; i++)
            {
                var house = topHouses[i];
                var label = new Label
                {
                    Content = $"Top {i + 1}: {house.Name}",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 10 + (i * 25), 0, 0)
                };
                TopHousesGrid.Children.Add(label);
            }

            var topMonthlyData = monthlyData.Where(kvp => topHouses.Contains(kvp.Key))
                                            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            await UpdateColumnChart(topMonthlyData);
            await UpdateChart(topMonthlyData);
        }

        private async Task UpdateColumnChart(Dictionary<House, List<(DateTime? PaymentDate, decimal Revenue, bool IsPaid)>> monthlyData)
        {
            ColumnChart.Series.Clear();

            var paidCounts = new ChartValues<int>();
            var unpaidCounts = new ChartValues<int>();
            var houseNames = new List<string>();

            foreach (var houseData in monthlyData)
            {
                var house = houseData.Key;
                var data = houseData.Value;

                int countPaid = data.Count(d => d.IsPaid);
                int countUnpaid = data.Count(d => !d.IsPaid);

                paidCounts.Add(countPaid);
                unpaidCounts.Add(countUnpaid);
                houseNames.Add(house.Name);
            }

            ColumnChart.Series.Add(CreateColumnSeries("Paid", paidCounts, Brushes.Blue));
            ColumnChart.Series.Add(CreateColumnSeries("Unpaid", unpaidCounts, Brushes.OrangeRed));

            HouseNames = houseNames;
            ColumnChart.DataContext = this;
        }
        private ColumnSeries CreateColumnSeries(string title, ChartValues<int> values, SolidColorBrush color)
        {
            return new ColumnSeries
            {
                Title = title,
                Values = values,
                DataLabels = true,
                Fill = color
            };
        }

        private async Task UpdateChart(Dictionary<House, List<(DateTime? PaymentDate, decimal Revenue, bool IsPaid)>> monthlyData)
        {
            ReportChart.Series.Clear();
            TitleColorHousePanel.Children.Clear();

            var titleTextBlock = new TextBlock
            {
                Text = "Revenue report",
                VerticalAlignment = VerticalAlignment.Center,
                Style = (Style)FindResource("titleText"),
                Margin = new Thickness(0, 0, 10, 0)
            };
            TitleColorHousePanel.Children.Add(titleTextBlock);

            for (int i = 0; i < monthlyData.Count; i++)
            {
                var houseData = monthlyData.ElementAt(i);
                var house = houseData.Key;
                var monthlyRevenue = houseData.Value;

                // Sắp xếp dữ liệu theo tháng
                var sortedMonthlyRevenue = monthlyRevenue
                    .OrderBy(data => data.PaymentDate?.Year)
                    .ThenBy(data => data.PaymentDate?.Month)
                    .ToList();

                var series = new LineSeries
                {
                    Title = house.Name,
                    Values = new ChartValues<decimal>(),
                    PointGeometrySize = 5,
                    StrokeThickness = 2,
                    Stroke = seriesColors[i % seriesColors.Length],
                    LabelPoint = (point) =>
                    {
                        var index = (int)point.X;
                        if (index >= 0 && index < sortedMonthlyRevenue.Count)
                        {
                            var paymentDate = sortedMonthlyRevenue[index].PaymentDate;
                            var monthName = paymentDate.HasValue ? paymentDate.Value.ToString("MMMM") : "No Date";
                            return $"{monthName}: {point.Y}";
                        }
                        return "";
                    }
                };

                ReportChart.Series.Add(series);

                foreach (var dataPoint in sortedMonthlyRevenue)
                {
                    series.Values.Add(dataPoint.Revenue);
                }

                TitleColorHousePanel.Children.Add(CreateColorBorder(i));
                TitleColorHousePanel.Children.Add(new TextBlock { Text = house.Name, Margin = new Thickness(10, 12, 0, 0) });
            }
        }

        private Border CreateColorBorder(int index)
        {
            return new Border
            {
                Width = 12,
                Height = 12,
                Background = seriesColors[index % seriesColors.Length],
                CornerRadius = new CornerRadius(3),
                Margin = new Thickness(5, 4, 0, 0)
            };
        }
    }
}