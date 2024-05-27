using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace heritage_rhythm
{
    /// <summary>
    /// Login_01.xaml 的交互逻辑
    /// </summary>
    public partial class Login_01 : Page
    {
        private DNO dno;
        private SqlConnection connection;
        public event EventHandler<LoginSuccessEventArgs> LoginSuccessful;
        public LoginMode loginMode;
        public Login_01()
        {
            InitializeComponent();
            dno = new DNO();
            connection = dno.Connection();
            loginMode=LoginMode.User;
        }
        public enum LoginMode
        {
            User, // 用户版
            Seller // 商家版
        }
        public class LoginSuccessEventArgs : EventArgs
        {
            public string UserId { get; set; }
            public LoginMode Mode { get; set; }

            public LoginSuccessEventArgs(string userId, LoginMode mode)
            {
                UserId = userId;
                Mode = mode;
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(passwordBox.Password) && passwordBox.Password.Length > 0)
                textPassword.Visibility = Visibility.Collapsed;
            else
                textPassword.Visibility = Visibility.Visible;
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            passwordBox.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string usermail = txtEmail.Text.Trim();
            string password = passwordBox.Password;
            ValidateLogin(usermail, password);
            
        }

        private void txtEmail_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text) && txtEmail.Text.Length > 0)
                textEmail.Visibility = Visibility.Collapsed;
            else
                textEmail.Visibility = Visibility.Visible;
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtEmail.Focus();
        }
            public event EventHandler SwitchToLogin02;
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SwitchToLogin02?.Invoke(this, EventArgs.Empty);
        }
        private void ValidateLogin(string usermail, string password)
        {
            string query = loginMode == LoginMode.User ?
               "SELECT userid, usernick, usermail, username, usersex, userrole, password, phonenumber FROM users WHERE usermail = @usermail" :
               "SELECT merchantid, storename, phonenumber, ownername, email, password, businesslicense, isofficial, isverified FROM merchants WHERE email = @usermail";

            connection.Open();
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@usermail", usermail);
                using (SqlDataReader mydatareader = command.ExecuteReader())
                {
                    if (mydatareader.Read())
                    {

                        string realpassword = mydatareader["password"].ToString();
                        if (loginMode == LoginMode.User)
                        {
                            if (password == realpassword)
                            {
                                string loggedInUserId = mydatareader["userid"].ToString();
                                // 这里可以进行其他操作，比如跳转到主界面
                                //Users.userid=mydatareader["userid"].ToString();
                                Properties.Settings.Default.UserId = loggedInUserId;
                                Properties.Settings.Default.Save();
                                LoginSuccessful?.Invoke(this, new LoginSuccessEventArgs(loggedInUserId, LoginMode.User));
                            }
                            else
                            {
                                MessageBox.Show("账号或密码错误，请重新输入！");
                                txtEmail.Clear();
                                passwordBox.Clear();
                            }
                        }
                        else if (loginMode == LoginMode.Seller)
                            {
                            if (password == realpassword)
                            {
                                string loggedInUserId = mydatareader["merchantid"].ToString();
                                // 这里可以进行其他操作，比如跳转到主界面
                                //Users.userid=mydatareader["userid"].ToString();
                                Properties.Settings.Default.MerchantId = loggedInUserId;
                                Properties.Settings.Default.Save();
                                LoginSuccessful?.Invoke(this, new LoginSuccessEventArgs(loggedInUserId, LoginMode.Seller));
                            }

                            else
                            {
                                MessageBox.Show("账号或密码错误，请重新输入！");
                                txtEmail.Clear();
                                passwordBox.Clear();
                            }
                        }


                    }
                    else if ((usermail == "Admin@tongji.edu.cn" && password == "Admin123456"))
                    {
                        var mainWindow = new AdminWindow();
                        Application.Current.MainWindow = mainWindow; // 设置为应用的主窗口
                        mainWindow.Show(); // 显示新的主窗口
                        mainWindow.Activate(); // 激活窗口
                        var parentWindow = Window.GetWindow(this); // 获取当前 Page 的父窗口
                        if (parentWindow != null)
                        {
                            parentWindow.Close(); // 关闭父窗口
                        }
                    }
                    else
                    {
                        MessageBox.Show("账号不存在！");
                    }
                }
            }
            connection.Close();
        }

        private void change_Click(object sender, RoutedEventArgs e)
        {
            loginMode = loginMode == LoginMode.User ? LoginMode.Seller : LoginMode.User;
            change.Content = loginMode == LoginMode.User ? "切换商家版" : "切换个人版";
        }
    }
}
