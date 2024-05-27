using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace heritage_rhythm.UserControls
{
    /// <summary>
    /// ProductData.xaml 的交互逻辑
    /// </summary>
    public partial class OrderData : UserControl
    {
        public ObservableCollection<Order> Orders { get; set; }
        private SqlConnection connection;
        public OrderData()
        {
            InitializeComponent();
            Orders = new ObservableCollection<Order>();
            ordersDataGrid.ItemsSource = Orders;
            DNO dno = new DNO();
            connection = dno.Connection();
            LoadOrdersFromDatabase();
        }
        public class Order
        {
            public string OrderId { get; set; }
            public DateTime OrderDate { get; set; }
            public int PurchaseQuantity { get; set; }
            public string CustomerNick { get; set; }
            public int Score { get; set; }
        }

        private void LoadOrdersFromDatabase()
        {
            try
            {
                string merchantId = Properties.Settings.Default.MerchantId;
                connection.Open();
                string query = @"
                SELECT o.OrderId, o.OrderTime AS OrderDate, o.PurchaseQuantity, 
                       u.usernick AS CustomerNick, o.Score
                FROM Orders o
                JOIN Users u ON o.userid = u.userid
                WHERE o.merchantid = @MerchantId";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MerchantId", merchantId);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Orders.Add(new Order
                    {
                        OrderId = reader["OrderId"].ToString(),
                        OrderDate = (DateTime)reader["OrderDate"],
                        PurchaseQuantity = (int)reader["PurchaseQuantity"],
                        CustomerNick = reader["CustomerNick"].ToString(),
                        Score = reader.IsDBNull(reader.GetOrdinal("Score")) ? 0 : reader.GetInt32(reader.GetOrdinal("Score"))
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading orders: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}
