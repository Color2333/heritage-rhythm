using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static heritage_rhythm.Login_01;

namespace heritage_rhythm
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private Login_01 _loginPage01;
        private Login_02 _loginPage02;
        public event EventHandler<LoginSuccessEventArgs> LoginSuccessful;
        public Login()
        {
            InitializeComponent();
            NavigateToLogin01();
        }
        private void NavigateToLogin01()
        {
            if (_loginPage01 == null)
            {
                _loginPage01 = new Login_01();
                _loginPage01.LoginSuccessful += LoginPage_LoginSuccessful;
                _loginPage01.SwitchToLogin02 += (sender, e) => NavigateToLogin02();
            }
            loginFrame1.Navigate(_loginPage01);
        }

        private void NavigateToLogin02()
        {
            if (_loginPage02 == null)
            {
                _loginPage02 = new Login_02();
                _loginPage02.SwitchToLogin01 += (sender, e) => NavigateToLogin01();
            }
            loginFrame1.Navigate(_loginPage02);
        }

        private void LoginPage_LoginSuccessful(object sender, Login_01.LoginSuccessEventArgs e)
        {
            // 将登录成功事件传递给 App.xaml.cs
            LoginSuccessful?.Invoke(this, e);
        }

        public event EventHandler SwitchToLogin02;

        public void OnSwitchToLogin02()
        {
            SwitchToLogin02?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler SwitchToLogin01;

        public void OnSwitchToLogin01()
        {
            SwitchToLogin01?.Invoke(this, EventArgs.Empty);
        }
        private void MinimizeButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

    }
}
