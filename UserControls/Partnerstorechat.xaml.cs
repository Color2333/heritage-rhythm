using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
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

namespace heritage_rhythm.UserControls
{
    /// <summary>
    /// Partnerchat.xaml 的交互逻辑
    /// </summary>
    public partial class Partnerstorechat : UserControl
    {
        private DNO dno;
        private SqlConnection connection;
        private int partnerId;
        private string partnerName;

        private string userId = Properties.Settings.Default.MerchantId;
        public Partnerstorechat(int partnerId,string partnerName)
        {
            InitializeComponent();
            this.partnerId = partnerId;
            this.partnerName = partnerName;
            Nameblock.Text= partnerName;
            dno = new DNO();
            connection = dno.Connection();
            LoadRecentMessages();
        }
        private  void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取消息文本框的内容
                // 获取消息文本框的内容
                string userMessage = textBoxMessage.Text;

                // 检查消息是否为空
                if (string.IsNullOrWhiteSpace(userMessage))
                {
                    MessageBox.Show("Please enter a message.");
                    return;
                }

                try
                {
                    connection.Open();

                    // 插入消息到数据库
                    string insertQuery = @"INSERT INTO Messages (SenderId, ReceiverId, MessageText, SendTime)
                               VALUES (@SenderId, @ReceiverId, @MessageText, @SendTime)";
                    SqlCommand insertCommand = new SqlCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@SenderId", userId);
                    insertCommand.Parameters.AddWithValue("@ReceiverId", partnerId);
                    insertCommand.Parameters.AddWithValue("@MessageText", userMessage);
                    insertCommand.Parameters.AddWithValue("@SendTime", DateTime.Now);

                    int rowsAffected = insertCommand.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // 添加用户消息到聊天界面中
                        AddUserMessageToChat(userMessage, DateTime.Now.ToString("h:mm tt"));
                        // 清空消息文本框
                        textBoxMessage.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Failed to send message.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error sending message: " + ex.Message);
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }


        }

        private void AddUserMessageToChat(string message, string time)
        {
            // 创建用户消息框
            var userMessageChat = new MyMessageChat();
            userMessageChat.Message = message;
            chatPanel.Children.Add(userMessageChat);

            // 添加时间信息
            var timeTextBlock = new TextBlock();
            timeTextBlock.Text = time;
            timeTextBlock.Style = (Style)Application.Current.Resources["timeTextRight"]; // 使用时间文本样式
            chatPanel.Children.Add(timeTextBlock);
        }

        private void AddResponseToChat(string message, string time)
        {
            // 创建 GPT 模型回复消息框
            var gptMessageChat = new MessageChat();
            gptMessageChat.Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF82A3")); // 设置颜色
            gptMessageChat.Message = message;
            chatPanel.Children.Add(gptMessageChat);

            // 添加时间信息
            var timeTextBlock = new TextBlock();
            timeTextBlock.Text = time;
            timeTextBlock.Style = (Style)Application.Current.Resources["timeText"]; // 使用时间文本样式
            chatPanel.Children.Add(timeTextBlock);
        }
        private void LoadRecentMessages()
        {

            try
            {
                connection.Open();
                string query = @"
SELECT TOP 7
    SenderId, ReceiverId, MessageText, SendTime
FROM
    Messages
WHERE
    (SenderId = @UserId AND ReceiverId = @PartnerId) OR
    (SenderId = @PartnerId AND ReceiverId = @UserId)
ORDER BY
    SendTime DESC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@PartnerId", partnerId);
                SqlDataReader reader = command.ExecuteReader();

                StackPanel tempPanel = new StackPanel(); // Temporary stack to hold messages in proper order

                while (reader.Read())
                {
                    string message = reader["MessageText"].ToString();
                    DateTime time = Convert.ToDateTime(reader["SendTime"]);
                    string formattedTime = time.ToString("h:mm tt");

                    if (reader["SenderId"].ToString() == userId)
                    {
                        AddUserMessageToChat( message, formattedTime);
                    }
                    else
                    {
                        AddResponseToChat( message, formattedTime);
                    }
                }

                // Add messages in reverse order to display recent messages at the bottom
                for (int i = tempPanel.Children.Count - 1; i >= 0; i--)
                {
                    chatPanel.Children.Add(tempPanel.Children[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading chat items: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }


    }
}
