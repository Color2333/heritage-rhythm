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
using System.IO;
using System.Web.UI;

namespace heritage_rhythm
{
    /// <summary>
    /// Myorder.xaml 的交互逻辑
    /// </summary>
    public partial class Myorder : System.Windows.Controls.Page
    {
        private SqlConnection connection;
        public string userId= Properties.Settings.Default.UserId;
        public Myorder()
        {
            DNO dno = new DNO();
            connection = dno.Connection();
            InitializeComponent();
            LoadOrders();
        }
        public class OrderData
        {
            public string ProductId { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public string StoreName { get; set; }
            public int Count { get; set; }
            public string Category { get; set; }
            public ImageSource Source { get; set; } // 用于存储图像的ImageSource属性

        }

        private void LoadOrders()
        {
            List<OrderData> orders = new List<OrderData>();
            try
            {
                connection.Open();
                string query = @"
SELECT 
    p.product_id, 
    p.name AS ProductName, 
    m.storename AS StoreName, 
    p.category AS Category, 
    p.price AS Price, 
    o.PurchaseQuantity AS PurchaseQuantity,  -- 假设订单表中包含了购买数量字段
    (SELECT TOP 1 image_data FROM product_images pi WHERE pi.product_id = p.product_id ORDER BY pi.image_id) AS ImageData
FROM products p
JOIN merchants m ON p.store_id = m.merchantid
JOIN orders o ON p.product_id = o.productid  -- 连接订单表
WHERE o.userid = @UserId  -- 假设订单表中有指向用户的字段
";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId); // 确保有Users.userid

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new OrderData
                            {
                                ProductId = reader["product_id"].ToString(),
                                ProductName = reader["ProductName"].ToString(),
                                StoreName = reader["StoreName"].ToString(),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Count = reader.GetInt32(reader.GetOrdinal("PurchaseQuantity")), // 假设你保存了购买数量
                            };
                            byte[] imageData = (byte[])reader["ImageData"];
                            if (imageData != null)
                            {
                                using (var ms = new MemoryStream(imageData))
                                {
                                    BitmapImage bitmap = new BitmapImage();
                                    bitmap.BeginInit();
                                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // This helps to load the image synchronously
                                    bitmap.StreamSource = ms;
                                    bitmap.EndInit();
                                    bitmap.Freeze(); // This makes the image usable across threads
                                    order.Source = bitmap;                 // Now you can assign the bitmap to an Image control or use it wherever needed
                                }
                            }
                            orders.Add(order);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading orders: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            OrdersControl.ItemsSource = orders;
        }

    }
}
