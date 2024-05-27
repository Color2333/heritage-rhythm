using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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

namespace heritage_rhythm.UserControls
{
    /// <summary>
    /// PersonalInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class PersonalInfoControl : UserControl
    {
        public  SqlConnection connection;
        public PersonalInfoControl()
        {
            InitializeComponent();
            string userId = Properties.Settings.Default.UserId;
            DNO dno = new DNO();
            connection = dno.Connection();
            if (connection == null)
            {
                Debug.WriteLine("Connection failed to initialize.");
            }
            else
            {
                User user = GetUserDetails(userId);
                this.DataContext = user;
            }
        }
        public class User
        {
            public string UserName { get; set; }
            public string UserNick { get; set; }
            public string UserSex { get; set; }
            public string UserEmail { get; set; }
            public string PhoneNumber { get; set; }
        }
        public User GetUserDetails(string userId)
        {
            User user = null;
            
            string sql = "SELECT username, usernick, usersex, usermail, phonenumber FROM users WHERE userid = @userid";
            SqlCommand command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@userid", userId);

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                user = new User
                {
                    UserName = reader["username"].ToString(),
                    UserNick = reader["usernick"].ToString(),
                    UserSex = reader["usersex"].ToString(),
                    UserEmail = reader["usermail"].ToString(),
                    PhoneNumber = reader["phonenumber"].ToString()
                };
            }
            
            return user;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String userId = Properties.Settings.Default.UserId;
            string query = "UPDATE users SET usernick = @usernick, username = @username,phonenumber = @phonenumber, usersex = @usersex, userrole = @userrole WHERE userid = @userid";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@usernick", usernicktextBox.Text.Trim());
                command.Parameters.AddWithValue("@username", usernametextBox.Text.Trim());
                command.Parameters.AddWithValue("@phonenumber", userphonenumbertextBox.Hint.Trim());
                command.Parameters.AddWithValue("@usersex", usersexcomboBox.Text.Trim());
                command.Parameters.AddWithValue("@userrole", "0");
                command.Parameters.AddWithValue("@userid", userId); // 假设 Users 类中有一个名为 userid 的属性表示要更新的用户的标识符

                connection.Open();
                try
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("信息更新成功");
                    }
                    else
                    {
                        MessageBox.Show("未找到要更新的用户记录");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("更新用户信息时出现错误：" + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
