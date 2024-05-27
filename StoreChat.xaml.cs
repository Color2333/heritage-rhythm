using System;
using System.Windows;
using System.Windows.Controls;
using heritage_rhythm.UserControls;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Input;
using static heritage_rhythm.Chat;

namespace heritage_rhythm
{
    /// <summary>
    /// Chat.xaml 的交互逻辑
    /// </summary>
    public partial class StoreChat : Page
    {
        private DNO dno;
        private SqlConnection connection;

        private string
            userId = Properties.Settings.Default.MerchantId; // Assuming this is set correctly somewhere globally.

        public StoreChat()
        {
            InitializeComponent();
            this.KeepAlive = true; // Good for keeping the state alive if navigating away temporarily.
            dno = new DNO();
            connection = dno.Connection();
            LoadChatItems();
            chatFrame.Navigate(new GPTchat()); // Default navigation to a general chat page.
        }

        private void LoadChatItems()
        {
            List<ChatListItem> chatListItems = new List<ChatListItem>();
            try
            {
                connection.Open();
                string query = @"
SELECT 
    SenderId AS PartnerId,
    PartnerName = (SELECT usernick FROM Users WHERE userid = SenderId),  -- 获取用户昵称
    LastMessage = (SELECT TOP 1 MessageText FROM Messages WHERE ReceiverId = @CurrentUserId AND SenderId = SenderId ORDER BY SendTime DESC),
    MessageCount = (SELECT COUNT(*) FROM Messages WHERE ReceiverId = @CurrentUserId AND SenderId = SenderId)
FROM 
    Messages
WHERE 
    ReceiverId = @CurrentUserId  -- 确保是发给当前商家的消息
GROUP BY 
    SenderId;  -- 按发送者分组
";


                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CurrentUserId", userId);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    chatListItems.Add(new ChatListItem
                    {
                        PartnerId = reader.GetInt32(0),
                        PartnerName = reader.GetString(1),
                        LastMessage = reader.GetString(2),
                        MessageCount = reader.GetInt32(3)
                    });
                }

                reader.Close();
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

            ChatListItemsControl.ItemsSource = chatListItems; // Bind the list to the UI control.
        }

        private void Item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is ChatListItem chatItem)
            {
                // Navigate to the specific chat partner's chat interface.
                Partnerstorechat partnerChat = new Partnerstorechat(chatItem.PartnerId, chatItem.PartnerName);
                chatFrame.Navigate(partnerChat); // Ensure this Frame is defined in XAML for this purpose.
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
