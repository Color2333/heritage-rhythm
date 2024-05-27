using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static heritage_rhythm.UserControls.Address;
using static heritage_rhythm.UserControls.PersonalInfoControl;

namespace heritage_rhythm.UserControls
{
    /// <summary>
    /// PersonalInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class Address : UserControl
    {
        private SqlConnection connection;
        public string userId;

        public Address()
        {

            InitializeComponent();
            DNO dno = new DNO();
            connection = dno.Connection();
            userId = Properties.Settings.Default.UserId;
            LoadAddresses();
            LoadProvinces();
        }

        private void LoadAddresses()
        {
            string userId = Properties.Settings.Default.UserId; // 获取用户ID
            var addresses = GetAddressesForUser(userId); // 调用数据库查询方法

            addressComboBox.ItemsSource = addresses; // 假设你有一个名为 addressComboBox 的 ComboBox
            addressComboBox.DisplayMemberPath = "FullAddress"; // 设置显示的属性
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

        public List<ShippingAddress> GetAddressesForUser(string userId)
        {
            var addresses = new List<ShippingAddress>();

            string query =
                "SELECT addressid, userid, recipientname, phonenumber, province, city, district, street, address_detail FROM shipping_addresses WHERE userid = @userId";

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

        private void LoadProvinces()
        {
            comboBox1.Items.Clear();

            string query = "SELECT name FROM Provinces";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader["name"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading provinces: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedProvince = comboBox1.SelectedItem.ToString();
            comboBox2.Text = string.Empty;
            comboBox3.Text = string.Empty;
            comboBox4.Text = string.Empty;
            LoadCities(selectedProvince);
        }

        private void LoadCities(string provinceName)
        {
            comboBox2.Items.Clear();

            string query =
                "SELECT name FROM Cities WHERE provinceCode = (SELECT code FROM Provinces WHERE name = @provinceName)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@provinceName", provinceName);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comboBox2.Items.Add(reader["name"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading cities: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }



        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCity = comboBox2.SelectedItem.ToString();
            comboBox3.Text = string.Empty;
            comboBox4.Text = string.Empty;
            LoadDistricts(selectedCity);
        }

        private void LoadDistricts(string cityName)
        {
            comboBox3.Items.Clear();

            string query =
                "SELECT name FROM District WHERE provinceCode = (SELECT code FROM Provinces WHERE name = @provinceName) AND cityCode in (SELECT code FROM Cities WHERE name = @cityName)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@cityName", comboBox2.Text);
            command.Parameters.AddWithValue("@provinceName", comboBox1.Text);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comboBox3.Items.Add(reader["name"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading districts: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }


        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDistrict = comboBox3.SelectedItem.ToString();
            comboBox4.Text = string.Empty;
            LoadStreets(selectedDistrict);
        }

        private void LoadStreets(string districtName)
        {
            comboBox4.Items.Clear();

            string query =
                "SELECT name FROM Streets WHERE cityCode IN (SELECT code FROM Cities WHERE name = @cityName) AND provinceCode = (SELECT code FROM Provinces WHERE name = @provinceName) AND areaCode = (SELECT code FROM District WHERE name = @districtName)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@cityName", comboBox2.Text);
            command.Parameters.AddWithValue("@districtName", districtName);
            command.Parameters.AddWithValue("@provinceName", comboBox1.Text);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    comboBox4.Items.Add(reader["name"].ToString());
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading streets: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void addressComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (addressComboBox.SelectedItem is ShippingAddress selectedAddress)
            {
                // Assuming comboBox1 is meant to display the province of the selected address/ Clear existing entries if necessary
                comboBox1.Text = selectedAddress.Province;

                // Similarly, update other ComboBoxes or controls as needed
                comboBox2.Text = selectedAddress.City;

                comboBox3.Text = selectedAddress.District;

                comboBox4.Text = selectedAddress.Street;

                TextBox1.Hint = selectedAddress.RecipientName;

                TextBox2.Hint = selectedAddress.PhoneNumber;

                TextBox3.Text = selectedAddress.AddressDetail;
            }
        }

        private void saveBtn_Click(object sender, RoutedEventArgs e)
        {
            string province = comboBox1.Text;
            string city = comboBox2.Text;
            string district = comboBox3.Text;
            string street = comboBox4.Text;
            string addressDetail = TextBox3.Text.Trim();
            string recipientName = TextBox1.Hint.Trim();
            string phoneNumber = TextBox2.Hint.Trim();

            // 构建插入地址的 SQL 查询语句
            string query =
                "INSERT INTO shipping_addresses (userid, recipientname, phonenumber, province, city, district, street, address_detail) VALUES (@userid, @recipientname, @phonenumber, @province, @city, @district, @street, @address_detail)";

            // 使用 SQL 查询语句创建 SqlCommand 对象，并添加参数
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userid", userId); // 假设 Users 类包含了当前用户的 userid
                command.Parameters.AddWithValue("@recipientname", recipientName);
                command.Parameters.AddWithValue("@phonenumber", phoneNumber);
                command.Parameters.AddWithValue("@province", province);
                command.Parameters.AddWithValue("@city", city);
                command.Parameters.AddWithValue("@district", district);
                command.Parameters.AddWithValue("@street", street);
                command.Parameters.AddWithValue("@address_detail", addressDetail);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("收货地址添加成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("添加收货地址时出现错误：" + ex.Message, "错误");
                }
                finally
                {
                    connection.Close();
                }
            }

            RequestReload?.Invoke(this, EventArgs.Empty);

        }

        private void Addbtn_Click(object sender, RoutedEventArgs e)
        {
            comboBox1.Items.Clear();

            // Similarly, update other ComboBoxes or controls as needed
            comboBox2.Items.Clear();

            comboBox3.Items.Clear();

            comboBox4.Items.Clear();
            TextBox3.Text = String.Empty;
            TextBox1.Hint = String.Empty;
            TextBox2.Hint = String.Empty;
            LoadProvinces();
            editbtn.Visibility = Visibility.Collapsed;
            addressComboBox.Visibility = Visibility.Collapsed;
            saveBtn.Visibility = Visibility.Visible;
        }

        public event EventHandler RequestReload;
    }
}
