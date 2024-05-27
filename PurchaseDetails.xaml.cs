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
using System.Windows.Shapes;

namespace heritage_rhythm
{
    /// <summary>
    /// PurchaseDetails.xaml 的交互逻辑
    /// </summary>
    public partial class PurchaseDetails : Page
    {
        private string productId;
        private SqlConnection connection;
        private int currentImageIndex = 0;
        private BitmapImage[] productImages;

        public PurchaseDetails(string productId)
        {
            InitializeComponent();
            this.productId = productId;
            DNO dno=new DNO();
            connection = dno.Connection();
            LoadProductDetails();
            LoadProductImages();
        }
        public class SelectedProduct
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Price { get; set; }
            public string Category { get; set; }
            public string StockQuantity { get; set; }
            public string Details { get; set; }
            public int Quantity { get; set; }
            public string StoreName { get; set; }
            public BitmapImage[] Images { get; set; }
        }

        private void LoadProductDetails()
        {
            try
            {
                connection.Open();
                string query = @"SELECT p.name AS productName, p.category, p.price, p.stock_quantity, p.details, m.storename 
                                 FROM products p
                                 JOIN merchants m ON p.store_id = m.merchantid
                                 WHERE p.product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        textBlockProductName.Text = reader["productName"].ToString();
                        textBlockCategory.Text = reader["category"].ToString();
                        textBlockPrice.Text = reader["price"].ToString()+"元";
                        textBlockStockQuantity.Text = reader["stock_quantity"].ToString();
                        textBlockDetails.Text = reader["details"].ToString();
                        textBlockStoreName.Text = reader["storename"].ToString();
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载商品详情时出错：" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void LoadProductImages()
        {
            try
            {
                string query = "SELECT image_data FROM product_images WHERE product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    var imageList = new System.Collections.Generic.List<BitmapImage>();
                    while (reader.Read())
                    {
                        byte[] imageData = (byte[])reader["image_data"];
                        if (imageData != null)
                        {
                            using (var stream = new MemoryStream(imageData))
                            {
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.StreamSource = stream;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();
                                imageList.Add(bitmap);
                            }
                        }
                    }
                    productImages = imageList.ToArray();
                    if (productImages.Length > 0)
                    {
                        ProductImage.Source = productImages[0];
                    }
                    ThumbnailsControl.ItemsSource = productImages;

                    // 如果有图像，设置第一张为默认图像
                    if (productImages.Length > 0)
                    {
                        ProductImage.Source = productImages[0];
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载商品图片时出错：" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void Thumbnail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var imageControl = sender as Image;
            if (imageControl != null)
            {
                // 更新主图
                ProductImage.Source = imageControl.Source;
            }
        }
        private void NextImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (productImages != null && productImages.Length > 0)
            {
                currentImageIndex = (currentImageIndex + 1) % productImages.Length;
                ProductImage.Source = productImages[currentImageIndex];
            }
        }
        private void ButtonDecrease_Click(object sender, RoutedEventArgs e)
        {
            int quantity = int.Parse(textBlockQuantity.Text);
            if (quantity > 1) // Assuming the quantity cannot go below 1
            {
                quantity--;
                textBlockQuantity.Text = quantity.ToString();
            }
        }

        private void ButtonIncrease_Click(object sender, RoutedEventArgs e)
        {
            int quantity = int.Parse(textBlockQuantity.Text);
            quantity++;
            textBlockQuantity.Text = quantity.ToString();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void BuyNowButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = new SelectedProduct
            {
                Id = productId,
                Name = textBlockProductName.Text,
                Price = textBlockPrice.Text,
                Category = textBlockCategory.Text,
                Details = textBlockDetails.Text,
                Quantity = int.Parse(textBlockQuantity.Text),
                StoreName = textBlockStoreName.Text,
                Images = productImages
            };

            var mainWindow = Application.Current.MainWindow as MainWindow;
            var confirmationPage = new ConfirmationPaymentPage
            {
                DataContext = selectedProduct
            };

            if (mainWindow != null)
            {
                mainWindow.mainFrame.Navigate(confirmationPage);
            }

        }
        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                // 查询当前用户是否已收藏该商品
                string checkFavoriteQuery = "SELECT COUNT(*) FROM product_favorites WHERE userid = @UserId AND productid = @ProductId";

                using (SqlCommand checkCommand = new SqlCommand(checkFavoriteQuery, connection))
                {
                    // 添加参数，分别为当前用户的用户ID和当前商品的商品ID
                    checkCommand.Parameters.AddWithValue("@UserId", "10000001");
                    checkCommand.Parameters.AddWithValue("@ProductId", productId);

                    connection.Open();
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        // 商品已收藏，执行取消收藏操作
                        string deleteFavoriteQuery = "DELETE FROM product_favorites WHERE userid = @UserId AND productid = @ProductId";
                        using (SqlCommand deleteCommand = new SqlCommand(deleteFavoriteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@UserId", "10000001");
                            deleteCommand.Parameters.AddWithValue("@ProductId", productId);

                            int rowsAffected = deleteCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("商品已取消收藏");
                            }
                            else
                            {
                                MessageBox.Show("取消收藏失败，请重试");
                            }
                        }
                    }
                    else
                    {
                        // 商品未收藏，执行添加收藏操作
                        string insertFavoriteQuery = "INSERT INTO product_favorites (userid, productid) VALUES (@UserId, @ProductId)";

                        using (SqlCommand insertCommand = new SqlCommand(insertFavoriteQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@UserId", "10000001");
                            insertCommand.Parameters.AddWithValue("@ProductId", productId);

                            int rowsAffected = insertCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("商品已成功收藏");
                            }
                            else
                            {
                                MessageBox.Show("商品收藏失败，请重试");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("收藏商品时出现错误：" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                // 查询当前商品是否已添加到购物车
                string checkCartItemQuery = "SELECT COUNT(*) FROM shopping_cart_items WHERE cartid IN (SELECT cartid FROM shopping_cart WHERE userid = @UserId) AND productid = @ProductId";

                using (SqlCommand checkCommand = new SqlCommand(checkCartItemQuery, connection))
                {
                    // 添加参数，分别为当前用户的用户ID和当前商品的商品ID
                    checkCommand.Parameters.AddWithValue("@UserId", "10000001");
                    checkCommand.Parameters.AddWithValue("@ProductId", productId);

                    connection.Open();
                    int count = Convert.ToInt32(checkCommand.ExecuteScalar());

                    if (count > 0)
                    {
                        // 商品已添加到购物车，执行从购物车中移除操作
                        string deleteCartItemQuery = "DELETE FROM shopping_cart_items WHERE cartid IN (SELECT cartid FROM shopping_cart WHERE userid = @UserId) AND productid = @ProductId";
                        using (SqlCommand deleteCommand = new SqlCommand(deleteCartItemQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@UserId", "10000001");
                            deleteCommand.Parameters.AddWithValue("@ProductId", productId);

                            int rowsAffected = deleteCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("商品已从购物车中移除");
                            }
                            else
                            {
                                MessageBox.Show("从购物车中移除商品失败，请重试");
                            }
                        }
                    }
                    else
                    {
                        // 商品未添加到购物车，执行添加到购物车操作
                        string insertCartItemQuery = "INSERT INTO shopping_cart_items (cartid, productid) VALUES ((SELECT cartid FROM shopping_cart WHERE userid = @UserId), @ProductId)";

                        using (SqlCommand insertCommand = new SqlCommand(insertCartItemQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@UserId", "10000001");
                            insertCommand.Parameters.AddWithValue("@ProductId", productId);

                            int rowsAffected = insertCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("商品已成功添加到购物车");
                            }
                            else
                            {
                                MessageBox.Show("添加商品到购物车失败，请重试");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理购物车商品时出现错误：" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                string query = @"SELECT m.merchantid AS merchantid
                                 FROM products p
                                 JOIN merchants m ON p.store_id = m.merchantid
                                 WHERE p.product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        var merchantId = reader["merchantid"].ToString();
                        var storePage = new storepage(merchantId);
                        NavigationService.Navigate(storePage);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载商品详情时出错：" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
             // Get this ID based on your application logic
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                connection.Open();
                string query = @"SELECT m.merchantid AS merchantid
                         FROM products p
                         JOIN merchants m ON p.store_id = m.merchantid
                         WHERE p.product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                        var merchantId = reader["merchantid"].ToString();
                        var chat = new Chat(int.Parse(merchantId));
                        NavigationService.Navigate(chat);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载商品详情时出错：" + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
