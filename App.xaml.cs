using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace heritage_rhythm
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private MainAdminWindow _sellerMainWindow;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 创建并显示登录窗口
            Login loginWindow = new Login();
            loginWindow.LoginSuccessful += LoginWindow_LoginSuccessful;
            loginWindow.Show();
        }


        // 当登录成功时调用
        private void LoginWindow_LoginSuccessful(object sender, Login_01.LoginSuccessEventArgs e)
        {
            if (e.Mode == Login_01.LoginMode.User)
            {
                var mainWindow = new MainWindow();
                Application.Current.MainWindow = mainWindow; // 设置为应用的主窗口
                mainWindow.Show(); // 显示新的主窗口
                mainWindow.Activate(); // 激活窗口
                ((Window)sender).Close(); // 关闭登录窗口
            }
            else if (e.Mode == Login_01.LoginMode.Seller)
            {
                if (_sellerMainWindow == null)
                {
                    _sellerMainWindow = new MainAdminWindow();
                }
                Application.Current.MainWindow = _sellerMainWindow; // 设置为应用的主窗口
                _sellerMainWindow.Show();
                _sellerMainWindow.Activate();
                ((Window)sender).Close();
            }
        }




    }
}
