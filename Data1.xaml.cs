using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;


namespace heritage_rhythm
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class Data1 : Page
    {
        private SqlConnection connection;
        public Data1()
        {
            InitializeComponent();
            DNO dno = new DNO();
            connection = dno.Connection();
            DataContext = this;
            LoadUsers();
        }
        public class User
        {
            public int UserId { get; set; }
            public string UserNick { get; set; }
            public string UserMail { get; set; }
            public string UserName { get; set; }
            public string UserSex { get; set; }
            public int UserRole { get; set; }
            public DateTime CreationTime { get; set; }
            public DateTime UpdateTime { get; set; }
            public string Password { get; set; }  // 注意安全处理
            public string PhoneNumber { get; set; }
        }

        private void LoadUsers()
        {
            var users = new ObservableCollection<User>();
            try
            {
                connection.Open();
                string query = "SELECT userid, usernick, usermail, username, usersex, userrole, creation_time, update_time, phonenumber FROM users";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        UserId = reader.GetInt32(reader.GetOrdinal("userid")),
                        UserNick = reader.GetString(reader.GetOrdinal("usernick")),
                        UserMail = reader.IsDBNull(reader.GetOrdinal("usermail")) ? null : reader.GetString(reader.GetOrdinal("usermail")),
                        UserName = reader.IsDBNull(reader.GetOrdinal("username")) ? null : reader.GetString(reader.GetOrdinal("username")),
                        UserSex = reader.IsDBNull(reader.GetOrdinal("usersex")) ? null : reader.GetString(reader.GetOrdinal("usersex")),
                        UserRole = reader.GetInt32(reader.GetOrdinal("userrole")),
                        CreationTime = reader.GetDateTime(reader.GetOrdinal("creation_time")),
                        UpdateTime = reader.GetDateTime(reader.GetOrdinal("update_time")),
                        PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phonenumber")) ? null : reader.GetString(reader.GetOrdinal("phonenumber"))
                    });
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载用户信息时出现错误：" + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            userDataGrid.ItemsSource = users; // 确保数据源绑定无误
        }


        public void DeleteUser(User user)
        {
            string connectionString = "Your Connection String Here";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string commandText = "DELETE FROM users WHERE userid = @UserId";
                SqlCommand command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@UserId", user.UserId);
                connection.Open();
                int result = command.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("用户删除成功！");
                    //Users.Remove(user);
                }
                else
                {
                    MessageBox.Show("删除失败，请重试！");
                }
            }
        }







    }
}
