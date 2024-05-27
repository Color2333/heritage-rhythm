using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace heritage_rhythm
{
    /// <summary>
    /// AdminDashBoard.xaml 的交互逻辑
    /// </summary>
    public partial class AdminDashBoard : Page, INotifyPropertyChanged
    {
        private SqlConnection connection;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private SeriesCollection _totalSalesSeries;

        public SeriesCollection TotalSalesSeries
        {
            get => _totalSalesSeries;
            set
            {
                _totalSalesSeries = value;
                OnPropertyChanged(nameof(TotalSalesSeries));
            }
        }

        private SeriesCollection _orderCountSeries;

        public SeriesCollection OrderCountSeries
        {
            get => _orderCountSeries;
            set
            {
                _orderCountSeries = value;
                OnPropertyChanged(nameof(OrderCountSeries));
            }
        }

        private SeriesCollection _favoritesCountSeries;

        public SeriesCollection FavoritesCountSeries
        {
            get => _favoritesCountSeries;
            set
            {
                _favoritesCountSeries = value;
                OnPropertyChanged(nameof(FavoritesCountSeries));
            }
        }

        private SeriesCollection _cartQuantitySeries;

        public SeriesCollection CartQuantitySeries
        {
            get => _cartQuantitySeries;
            set
            {
                _cartQuantitySeries = value;
                OnPropertyChanged(nameof(CartQuantitySeries));
            }
        }

        private SeriesCollection _popularMerchantsSeries;

        public SeriesCollection PopularMerchantsSeries
        {
            get => _popularMerchantsSeries;
            set
            {
                _popularMerchantsSeries = value;
                OnPropertyChanged(nameof(PopularMerchantsSeries));
            }
        }

        private SeriesCollection _popularCategoriesSeries;

        public SeriesCollection PopularCategoriesSeries
        {
            get => _popularCategoriesSeries;
            set
            {
                _popularCategoriesSeries = value;
                OnPropertyChanged(nameof(PopularCategoriesSeries));
            }
        }

        public ChartValues<string> TopMerchantsLabels { get; set; }
        public ChartValues<string> PopularCategoriesLabels { get; set; }
        public SeriesCollection UsersGrowthSeries { get; set; }

        public AdminDashBoard()
        {
            InitializeComponent();
            DNO dno = new DNO();
            connection = dno.Connection();
            DataContext = this;
            TopMerchantsLabels = new ChartValues<string>();  // 初始化标签集合
            PopularCategoriesLabels = new ChartValues<string>();
            UsersGrowthSeries = new SeriesCollection();// 初始化标签集合// 将页面的数据上下文设置为此页面的实例
            LoadChartsData();
        }

        private void LoadChartsData()
        {
            try
            {
                connection.Open();
                LoadUsersGrowthData1();
                LoadTotalSalesData1();
                LoadOrderCountData1();
                LoadFavoritesCountData1();
                LoadCartQuantityData1();
                LoadPopularMerchantsData1();
                LoadPopularCategoriesData1();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        private void LoadUsersGrowthData()
        {
            string query = "SELECT COUNT(*) AS UserCount, CAST(creation_time AS DATE) AS DateGrouped FROM users GROUP BY CAST(creation_time AS DATE) ORDER BY DateGrouped";
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataReader reader = command.ExecuteReader();

            ChartValues<int> userCounts = new ChartValues<int>();
            List<string> dates = new List<string>();

            while (reader.Read())
            {
                userCounts.Add((int)reader["UserCount"]);
                dates.Add(((DateTime)reader["DateGrouped"]).ToShortDateString());
            }

            UsersGrowthSeries.Add(new LineSeries
            {
                Values = userCounts,
                Title = "User Growth"
            });

            // 假设你有一个用于显示日期的AxisX
            //AxisXLabels = dates;

            OnPropertyChanged(nameof(UsersGrowthSeries)); // 通知视图更新
            reader.Close();
        }

        private void LoadTotalSalesData()
        {
            SqlCommand cmd = new SqlCommand("SELECT SUM(PurchaseQuantity) AS TotalSales FROM Orders", connection);
            var totalSales = (int)cmd.ExecuteScalar();
            TotalSalesSeries = new SeriesCollection
            {
                new PieSeries
                {
                    Values = new ChartValues<int> { totalSales },
                    Title = "Total Sales"
                }
            };
        }

        private void LoadOrderCountData()
        {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) AS OrderCount FROM Orders", connection);
            var orderCount = (int)cmd.ExecuteScalar();
            OrderCountSeries = new SeriesCollection
    {
        new PieSeries
        {
            Values = new ChartValues<int> { orderCount },
            Title = "Order Count"
        }
    };
        }

        private void LoadFavoritesCountData()
        {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) AS FavoritesCount FROM product_favorites", connection);
            var favoritesCount = (int)cmd.ExecuteScalar();
            FavoritesCountSeries = new SeriesCollection
    {
        new PieSeries
        {
            Values = new ChartValues<int> { favoritesCount },
            Title = "Favorites Count"
        }
    };
        }

        private void LoadCartQuantityData()
        {
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) AS CartQuantity FROM shopping_cart_items", connection);
            var cartQuantity = (int)cmd.ExecuteScalar();
            CartQuantitySeries = new SeriesCollection
    {
        new PieSeries
        {
            Values = new ChartValues<int> { cartQuantity },
            Title = "Cart Quantity"
        }
    };
        }

        private void LoadPopularMerchantsData()
        {
            SqlCommand cmd = new SqlCommand("SELECT TOP 3 storename, SUM(PurchaseQuantity) AS Sales FROM Orders JOIN merchants ON Orders.merchantid = merchants.merchantid GROUP BY storename ORDER BY Sales DESC", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            ChartValues<string> labels = new ChartValues<string>();
            ChartValues<int> values = new ChartValues<int>();
            while (reader.Read())
            {
                labels.Add(reader["storename"].ToString());
                values.Add((int)reader["Sales"]);
            }
            PopularMerchantsSeries = new SeriesCollection
    {
        new ColumnSeries
        {
            Values = values,
            DataLabels = true,
            LabelPoint = point => point.Y.ToString()
        }
    };
            TopMerchantsLabels = labels;
            reader.Close();
        }

        private void LoadPopularCategoriesData()
        {
            SqlCommand cmd = new SqlCommand("SELECT TOP 3 category, SUM(PurchaseQuantity) AS Sales FROM Orders JOIN products ON Orders.productid = products.product_id GROUP BY category ORDER BY Sales DESC", connection);
            SqlDataReader reader = cmd.ExecuteReader();
            ChartValues<string> labels = new ChartValues<string>();
            ChartValues<int> values = new ChartValues<int>();
            while (reader.Read())
            {
                labels.Add(reader["category"].ToString());
                values.Add((int)reader["Sales"]);
            }
            PopularCategoriesSeries = new SeriesCollection
    {
        new ColumnSeries
        {
            Values = values,
            DataLabels = true,
            LabelPoint = point => point.Y.ToString()
        }
    };
            PopularCategoriesLabels = labels;
            reader.Close();
        }
        private Random rnd = new Random();
        private int daysCount = 30; // 例如过去30天

        private List<string> GenerateDates()
        {
            return Enumerable.Range(0, daysCount).Select(i => DateTime.Today.AddDays(-i).ToShortDateString()).ToList();
        }

        private void LoadUsersGrowthData1()
        {
            ChartValues<int> userCounts = new ChartValues<int>(Enumerable.Range(0, daysCount).Select(x => rnd.Next(10, 50)));
            UsersGrowthSeries = new SeriesCollection
    {
        new LineSeries
        {
            Values = userCounts,
            Title = "User Growth"
        }
    };

            OnPropertyChanged(nameof(UsersGrowthSeries));
        }

        private void LoadTotalSalesData1()
        {
            List<string> dates = GenerateDates();
            ChartValues<int> salesData = new ChartValues<int>(dates.Select(x => rnd.Next(5000, 10000)));

            TotalSalesSeries = new SeriesCollection
    {
        new LineSeries
        {
            Values = salesData,
            Title = "Total Sales"
        }
    };

            OnPropertyChanged(nameof(TotalSalesSeries));
        }

        private void LoadOrderCountData1()
        {
            List<string> dates = GenerateDates();
            ChartValues<int> orderData = new ChartValues<int>(dates.Select(x => rnd.Next(100, 500)));

            OrderCountSeries = new SeriesCollection
    {
        new LineSeries
        {
            Values = orderData,
            Title = "Order Count"
        }
    };

            OnPropertyChanged(nameof(OrderCountSeries));
        }

        private void LoadFavoritesCountData1()
        {
            List<string> dates = GenerateDates();
            ChartValues<int> favoritesData = new ChartValues<int>(dates.Select(x => rnd.Next(50, 150)));

            FavoritesCountSeries = new SeriesCollection
    {
        new LineSeries
        {
            Values = favoritesData,
            Title = "Favorites Count"
        }
    };

            OnPropertyChanged(nameof(FavoritesCountSeries));
        }

        private void LoadCartQuantityData1()
        {
            List<string> dates = GenerateDates();
            ChartValues<int> cartData = new ChartValues<int>(dates.Select(x => rnd.Next(20, 100)));

            CartQuantitySeries = new SeriesCollection
    {
        new LineSeries
        {
            Values = cartData,
            Title = "Cart Quantity"
        }
    };

            OnPropertyChanged(nameof(CartQuantitySeries));
        }

        private void LoadPopularMerchantsData1()
        {
            List<string> labels = new List<string> { "济忆山海", "美丽固始", "赛博山海" };
            ChartValues<int> sales = new ChartValues<int>(labels.Select(x => rnd.Next(1000, 5000)));

            PopularMerchantsSeries = new SeriesCollection
    {
        new ColumnSeries
        {
            Values = sales,
            Title = "Top Merchants by Sales",
            DataLabels = true,
            LabelPoint = point => point.Y.ToString()
        }
    };

            TopMerchantsLabels = new ChartValues<string>(labels);
            OnPropertyChanged(nameof(PopularMerchantsSeries));
        }

        private void LoadPopularCategoriesData1()
        {
            List<string> labels = new List<string> { "农产品", "陶瓷工艺", "金属工艺" };
            ChartValues<int> sales = new ChartValues<int>(labels.Select(x => rnd.Next(500, 2000)));

            PopularCategoriesSeries = new SeriesCollection
    {
        new ColumnSeries
        {
            Values = sales,
            Title = "Popular Categories",
            DataLabels = true,
            LabelPoint = point => point.Y.ToString()
        }
    };

            PopularCategoriesLabels = new ChartValues<string>(labels);
            OnPropertyChanged(nameof(PopularCategoriesSeries));
        }

    }


}
