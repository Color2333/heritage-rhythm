using heritage_rhythm.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static heritage_rhythm.PurchaseDetails;
using static heritage_rhythm.UserControls.OrderData;
using static heritage_rhythm.UserControls.PersonalInfoControl;
using static heritage_rhythm.UserControls.ProductData;

namespace heritage_rhythm
{
    /// <summary>
    /// ConfirmationPaymentPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmationPaymentPage : Page
    {
        private SqlConnection connection;
        
        public ConfirmationPaymentPage()
        {
            InitializeComponent();
            DNO dno = new DNO();
            connection = dno.Connection();
            LoadAddresses();
        }
        private void LoadAddresses()
        {
            string userId = Properties.Settings.Default.UserId; // 获取用户ID
            var addresses = GetAddressesForUser(userId);       // 调用数据库查询方法

            addressComboBox.ItemsSource = addresses;           // 假设你有一个名为 addressComboBox 的 ComboBox
            addressComboBox.DisplayMemberPath = "FullAddress"; // 设置显示的属性
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Foreground == Brushes.Gray)
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black; // or your desired color for input text
            }
        }
        private void ConfirmationPaymentPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Handle a single SelectedProduct
            if (DataContext is SelectedProduct selectedProduct)
            {
                AddProductToUI(selectedProduct);
            }
            // Handle a list of SelectedProduct instances
            else if (DataContext is List<SelectedProduct> selectedProducts)
            {
                foreach (var product in selectedProducts)
                {
                    AddProductToUI(product);
                }
            }
        }

        private void AddProductToUI(SelectedProduct product)
        {
            var itemControl = new Item
            {
                ProductName = product.Name,
                Price = product.Price,
                StoreName = product.StoreName,
                Count = product.Quantity,
                Category = product.Category,
                ProductId = product.Id,
                Source = product.Images?.FirstOrDefault(), // Assuming Images is not null and has at least one entry
                IsSelected = true
            };

            ItemsStackPanel.Children.Add(itemControl);

        }



        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }


        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Foreground = Brushes.Gray;
                // 下面的文本需要替换成对应的默认占位符文本
                textBox.Text = "Cardholder's Name"; // or "Card Number", depending on the TextBox
            }
        }
        public class OrderDetails
        {
            public string UserId { get; set; }
            public string MerchantId { get; set; }
            public string ProductId { get; set; }
            public int AddressId { get; set; }
            public int PurchaseQuantity { get; set; }
        }
        private string GetMerchantIdByProductId(string productId)
        {
            string merchantId="0"; // 默认为0，表示未找到
            try
            {
                connection.Open();
                string query = "SELECT store_id FROM products WHERE product_id = @ProductId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    object result = command.ExecuteScalar(); // 使用ExecuteScalar，因为我们只期待一个返回值
                    if (result != null)
                    {
                        merchantId = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("在获取商家ID时发生错误：" + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return merchantId;
        }

        private async void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            string userId = Properties.Settings.Default.UserId;

            // 从ComboBox获取地址ID
            string addressId = ((ShippingAddress)addressComboBox.SelectedItem).AddressId;

            // 准备存储订单信息
            List<OrderDetails> orders = new List<OrderDetails>();

            // 遍历ItemsStackPanel中的每个Item控件
            foreach (Item itemControl in ItemsStackPanel.Children)
            {
                if (itemControl.IsSelected)
                {
                    string productId = itemControl.ProductId;
                    string merchantId = GetMerchantIdByProductId(productId);
                    orders.Add(new OrderDetails
                    {
                        UserId = userId,
                        MerchantId = merchantId,  // 确保Item控件中有MerchantId属性
                        ProductId = itemControl.ProductId,    // 确保Item控件中有ProductId属性
                        AddressId = int.Parse(addressId),
                        PurchaseQuantity = itemControl.Count
                    });
                }
            }

            // 处理每个订单详情
            foreach (var order in orders)
            {
                TrySubmitOrder(order);
            }

            // 清空Card Section的内容
            CardSectionGrid.Children.Clear();

            // 添加支付成功的消息
            TextBlock successMessage = new TextBlock
            {
                Text = "支付成功！",
                Foreground = Brushes.White,
                FontSize = 24,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            CardSectionGrid.Children.Add(successMessage);

            // 等待一段时间以显示支付成功的消息
            await Task.Delay(1000); // 延迟2秒

            // 开始右滑动画
            Storyboard storyboard = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation
            {
                From = 0,
                To = CardSectionGrid.ActualWidth,
                Duration = TimeSpan.FromSeconds(0.7) // 增加动画持续时间
            };
            Storyboard.SetTarget(animation, CardSectionGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));
            storyboard.Children.Add(animation);
            CardSectionGrid.RenderTransform = new TranslateTransform();
            storyboard.Begin();

            // 动画完成后导航回MainPage
            storyboard.Completed += (s, args) =>
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.mainFrame.Source = new Uri("MainPage.xaml", UriKind.Relative);
                }
            };
        }
        public class ShippingAddress
        {
            public string AddressId { get; set; }
            public int UserId { get; set; }
            public string RecipientName { get; set; }
            public string PhoneNumber { get; set; }
            public string Province { get; set; }
            public string City { get; set; }
            public string District { get; set; }
            public string Street { get; set; }
            public string AddressDetail { get; set; }

            // 将地址信息组合成一个字符串以便在 ComboBox 中显示
            public string FullAddress
            {
                get { return $"{Province} {City} {District} {Street}, {AddressDetail}"; }
            }
        }
        private void TrySubmitOrder(OrderDetails order)
        {
            try
            {
                connection.Open();

                // 开始事务
                SqlTransaction transaction = connection.BeginTransaction();

                // 插入订单
                string insertQuery = "INSERT INTO Orders (userid, merchantid, productid, addressid, PurchaseQuantity) VALUES (@userid, @merchantid, @productid, @addressid, @PurchaseQuantity)";
                SqlCommand command = new SqlCommand(insertQuery, connection, transaction);
                command.Parameters.AddWithValue("@userid", order.UserId);
                command.Parameters.AddWithValue("@merchantid", order.MerchantId);
                command.Parameters.AddWithValue("@productid", order.ProductId);
                command.Parameters.AddWithValue("@addressid", order.AddressId);
                command.Parameters.AddWithValue("@PurchaseQuantity", order.PurchaseQuantity);

                int orderRowsAffected = command.ExecuteNonQuery();

                // 更新库存
                string updateStockQuery = @"UPDATE products SET stock_quantity = stock_quantity - @Quantity WHERE product_id = @ProductId";
                SqlCommand stockCommand = new SqlCommand(updateStockQuery, connection, transaction);
                stockCommand.Parameters.AddWithValue("@Quantity", order.PurchaseQuantity);
                stockCommand.Parameters.AddWithValue("@ProductId", order.ProductId);

                int stockRowsAffected = stockCommand.ExecuteNonQuery();

                // 检查订单和库存更新是否都成功执行
                if (orderRowsAffected > 0 && stockRowsAffected > 0)
                {
                    transaction.Commit(); // 提交事务
                    MessageBox.Show("订单创建成功，库存更新成功！");
                }
                else
                {
                    transaction.Rollback(); // 回滚事务
                    MessageBox.Show("订单创建失败或库存更新失败！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理订单时出现错误：" + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        public List<ShippingAddress> GetAddressesForUser(string userId)
        {
            var addresses = new List<ShippingAddress>();

            string query = "SELECT addressid, userid, recipientname, phonenumber, province, city, district, street, address_detail FROM shipping_addresses WHERE userid = @userId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var address = new ShippingAddress
                            {
                                AddressId = reader["addressid"].ToString(),
                                UserId = Convert.ToInt32(reader["userid"]),
                                RecipientName = reader["recipientname"].ToString(),
                                PhoneNumber = reader["phonenumber"].ToString(),
                                Province = reader["province"].ToString(),
                                City = reader["city"].ToString(),
                                District = reader["district"].ToString(),
                                Street = reader["street"].ToString(),
                                AddressDetail = reader["address_detail"].ToString(),
                            };
                            addresses.Add(address);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 使用更适合桌面应用的日志记录或错误处理方式
                    Console.WriteLine("查询地址时出现错误：" + ex.Message); // 或使用其他日志记录方法
                }
                finally
                {
                    connection.Close();

                }
            }

            return addresses;
        }



    }
}
