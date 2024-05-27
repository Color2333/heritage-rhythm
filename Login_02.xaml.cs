using heritage_rhythm.UserControls;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace heritage_rhythm
{
    /// <summary>
    /// Login_02.xaml 的交互逻辑
    /// </summary>
    public partial class Login_02 : Page
    {
        private DNO dno;
        private SqlConnection connection;
        public string selectedGender;
        public Login_02()
        {
            InitializeComponent();
            dno = new DNO();
            connection = dno.Connection();
        }
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
        public event EventHandler SwitchToLogin01;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SwitchToLogin01?.Invoke(this, EventArgs.Empty);
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidEmail(emailTextBox.Text))
            {
                MessageBox.Show("邮箱格式不正确，请输入有效的邮箱地址！");
                return;
            }

            if (!IsValidPassword(passwordTextBox.Text))
            {
                MessageBox.Show("密码格式不正确，请输入包含大小写字母和数字的不少于8位的密码！");
                return;
            }
            string query = "INSERT INTO users (usernick, username, usermail, phonenumber, password, usersex, userrole) VALUES (@usernick, @username, @usermail, @phonenumber, @password, @usersex, @userrole)";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@usernick", usernickTextBox.Text.Trim());
                command.Parameters.AddWithValue("@username", nameTextBox.Text.Trim());
                command.Parameters.AddWithValue("@usermail", emailTextBox.Text.Trim());
                command.Parameters.AddWithValue("@phonenumber", mobileTextBox.Text.Trim());
                command.Parameters.AddWithValue("@password", passwordTextBox.Text.Trim());
                command.Parameters.AddWithValue("@usersex", selectedGender);
                command.Parameters.AddWithValue("@userrole", "0");

                connection.Open();
                try
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("信息添加成功");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= 8 && password.Any(char.IsUpper) && password.Any(char.IsLower) && password.Any(char.IsDigit);
        }
        // 外部代码
        private void ParentRadioButton1_Checked(object sender, RoutedEventArgs e)
        {
            selectedGender = "男"; 
            var myOption = (sender as RadioButton)?.Content as MyOption;
            if (myOption != null)
            {
                myOption.SetInternalRadioButtonChecked(true);
            }
        }
        private void ParentRadioButton2_Checked(object sender, RoutedEventArgs e)
        {
            selectedGender = "女";
            var myOption = (sender as RadioButton)?.Content as MyOption;
            if (myOption != null)
            {
                myOption.SetInternalRadioButtonChecked(true);
            }
        }

    }
}
