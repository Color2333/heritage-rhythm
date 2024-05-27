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
using static heritage_rhythm.UserControls.PersonalInfoControl;

namespace heritage_rhythm
{
    /// <summary>
    /// storepage.xaml 的交互逻辑
    /// </summary>
    public partial class storepage : Page
    {
        private string merchantId;
        private SqlConnection connection; 
        public storepage(string merchantId)
        {
            InitializeComponent();
            DNO dno=new DNO();
            connection = dno.Connection();
            this.merchantId = merchantId;
            LoadStoreProducts();
        }
        private void LoadStoreProducts()
        {
            string userId = Properties.Settings.Default.UserId;
            try
            {
                connection.Open();
                string query = @"SELECT p.product_id, p.name, m.storename, p.category, p.price, p.stock_quantity, p.details,
                            (SELECT TOP 1 image_data FROM product_images pi WHERE pi.product_id = p.product_id) AS image_data
                         FROM products p
                         INNER JOIN merchants m ON p.store_id = m.merchantid
                         WHERE p.store_id = @MerchantId
";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@MerchantId", merchantId);
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
                            textBlockStoreName.Text = reader["storename"].ToString();
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
                            textBlockStoreName.Text= productControl.SellerName;
                            // TODO: Load and set the ProductImageSource and SellerAvatarSource

                            // Add the product control to the WrapPanel
                            storeProductsWrapPanel.Children.Add(productControl);
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
    }

}
