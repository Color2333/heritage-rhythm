using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.Json;
using heritage_rhythm.UserControls;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace heritage_rhythm
{
    /// <summary>
    /// Chat.xaml 的交互逻辑
    /// </summary>
    public partial class Chat : Page
    {
        private DNO dno;
        private SqlConnection connection;
        private string userId = Properties.Settings.Default.UserId;
        private int? newMerchantId;

        public Chat(int? newMerchantId = null)
        {
            InitializeComponent();
            this.KeepAlive = true;
            dno = new DNO();
            connection = dno.Connection();
            this.newMerchantId = newMerchantId;
            LoadChatItems();
            chatFrame.Navigate(new GPTchat());
        }
        public class ChatListItem
        {
            public int PartnerId { get; set; }
            public string PartnerName { get; set; }
            public string LastMessage { get; set; }
            public int MessageCount { get; set; }
        }


        private void LoadChatItems()
        {
            List<ChatListItem> chatListItems = new List<ChatListItem>();
            try
            {
                connection.Open();

                // 查询现有聊天列表项
                string query = @"
    SELECT 
        ReceiverId AS PartnerId,
        PartnerName = (SELECT storename FROM Merchants WHERE merchantid = ReceiverId),
        LastMessage = (SELECT TOP 1 MessageText FROM Messages WHERE SenderId = @CurrentUserId AND ReceiverId = ReceiverId ORDER BY SendTime DESC),
        MessageCount = (SELECT COUNT(*) FROM Messages WHERE SenderId = @CurrentUserId AND ReceiverId = ReceiverId)
    FROM 
        Messages
    WHERE 
        SenderId = @CurrentUserId AND SenderRole = '0'  -- '0'假设是用户的角色标识
    GROUP BY 
        ReceiverId;
";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CurrentUserId", userId);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    chatListItems.Add(new ChatListItem
                    {
                        PartnerId = reader.GetInt32(reader.GetOrdinal("PartnerId")),
                        PartnerName = reader.GetString(reader.GetOrdinal("PartnerName")),
                        LastMessage = reader.GetString(reader.GetOrdinal("LastMessage")),
                        MessageCount = reader.GetInt32(reader.GetOrdinal("MessageCount"))
                    });
                }

                reader.Close(); // 关闭第一个查询的DataReader

                // 如果有新商户ID传递进来，则查询新商户信息并生成新的聊天列表项
                if (newMerchantId.HasValue)
                {
                    query = "SELECT storename FROM Merchants WHERE merchantid = @MerchantId";

                    command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MerchantId", newMerchantId.Value);
                    string newMerchantName = command.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(newMerchantName))
                    {
                        // 查询新商户的最后一条消息和消息计数
                        // 请根据实际情况完善查询逻辑

                        string newMessage = "New message for the new merchant"; // 示例消息
                        int newMessageCount = 1; // 示例消息计数

                        // 创建新的聊天列表项并添加到集合中
                        chatListItems.Add(new ChatListItem
                        {
                            PartnerId = newMerchantId.Value,
                            PartnerName = newMerchantName,
                            LastMessage = newMessage,
                            MessageCount = newMessageCount
                        });
                    }
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

            // 设置聊天列表项的数据源
            ChatListItemsControl.ItemsSource = chatListItems;
        }

        private void Item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ChatListItem chatItem)
            {
                // 创建新的PartnerChat控件实例，使用从列表项获取的ID和名称
                Partnerchat partnerChat = new Partnerchat(chatItem.PartnerId, chatItem.PartnerName);
                chatFrame.Navigate(partnerChat);  // 假设chatFrame是您用于导航的Frame
            }
        }
        private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
        {

            // Navigate to the specific chat partner's chat interface.
            GPTchat GPTChat = new GPTchat();
            chatFrame.Navigate(GPTChat); // Ensure this Frame is defined in XAML for this purpose.

        }


    }
}
