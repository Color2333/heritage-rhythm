using heritage_rhythm.UserControls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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

namespace heritage_rhythm
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class Collection : Page
    {
        private DNO dno;
        private SqlConnection connection;
        public Collection()
        {
            dno = new DNO();
            connection = dno.Connection();
            InitializeComponent();
            LoadFavoriteProducts();
        }
        private void LoadFavoriteProducts()
        {
            try
            {
                connection.Open();
                string query = @"SELECT p.product_id, p.name, m.storename, p.category, p.price, p.stock_quantity, p.details,
       (SELECT TOP 1 image_data FROM product_images pi WHERE pi.product_id = p.product_id) AS image_data
FROM products p
INNER JOIN merchants m ON p.store_id = m.merchantid
INNER JOIN product_favorites pf ON p.product_id = pf.productid
WHERE pf.userid = @UserId
";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", "10000001");
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var productControl = new Product
                            {
                                ProductId = reader["product_id"].ToString(),
                                ProductTitle = reader["name"].ToString(),
                                SellerName = reader["storename"].ToString(),
                                ProductDescription = reader["details"].ToString(),
                                Price = Convert.ToDecimal(reader["price"]),
                                // ProductImageSource and SellerAvatarSource need to be set appropriately
                            };
                            byte[] imageData = reader["image_data"] as byte[];
                            if (imageData != null)
                            {
                                using (var ms = new MemoryStream(imageData))
                                {
                                    var bitmapImage = new BitmapImage();
                                    bitmapImage.BeginInit();
                                    bitmapImage.StreamSource = ms;
                                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                    bitmapImage.EndInit();
                                    bitmapImage.Freeze(); // 确保图片可以跨线程使用
                                    productControl.ProductImageSource = bitmapImage;
                                }
                            }

                            // TODO: Load and set the ProductImageSource and SellerAvatarSource

                            // Add the product control to the WrapPanel
                            FavoriteProductsWrapPanel.Children.Add(productControl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading favorite products: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
