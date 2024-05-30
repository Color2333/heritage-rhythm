using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace heritage_rhythm.UserControls
{
    /// <summary>
    /// GPTchat.xaml 的交互逻辑
    /// </summary>
    public partial class GPTchat : UserControl
    {
        public GPTchat()
        {
            InitializeComponent();
        }
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 获取消息文本框的内容
                string userMessage = textBoxMessage.Text;

                // 添加用户消息到聊天界面中
                AddUserMessageToChat(userMessage, DateTime.Now.ToString("h:mm tt"));
                // 清空消息文本框
                textBoxMessage.Text = string.Empty;
                // 发送消息给 GPT 模型，并获取回复
                string gptResponse = await GetGPTResponse(userMessage);

                // 添加 GPT 模型的回复消息到聊天界面中，并使用动画效果
                await AddGPTResponseWithAnimationToChat(gptResponse, DateTime.Now.ToString("h:mm tt"));

            }
            catch (Exception ex)
            {
                MessageBox.Show($"发送消息时出错：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task AddGPTResponseWithAnimationToChat(string message, string time)
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

            // 添加透明度动画
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = 1;
            animation.Duration = TimeSpan.FromSeconds(1); // 设置动画持续时间为1秒
            gptMessageChat.BeginAnimation(UIElement.OpacityProperty, animation);

            // 等待动画完成
            await Task.Delay(1000); // 等待1秒，确保动画完成
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

        private void AddGPTResponseToChat(string message, string time)
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
        private async Task<string> GetGPTResponse(string userMessage)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://api.freegpt.art/v1/chat/completions";
                //string apiUrl = "https://api.oaifree.com/v1/chat/completions";
                //string apiKey = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6Ik1UaEVOVUpHTkVNMVFURTRNMEZCTWpkQ05UZzVNRFUxUlRVd1FVSkRNRU13UmtGRVFrRXpSZyJ9.eyJodHRwczovL2FwaS5vcGVuYWkuY29tL3Byb2ZpbGUiOnsiZW1haWwiOiJqamhoQGpqaGgueHl6IiwiZW1haWxfdmVyaWZpZWQiOnRydWV9LCJodHRwczovL2FwaS5vcGVuYWkuY29tL2F1dGgiOnsicG9pZCI6Im9yZy1UVU1oTHlCUUtxRU8zSkdIbXpob1FCeTUiLCJ1c2VyX2lkIjoidXNlci1RTUN4MmFHNEVJYllGU0U0UDY1cEx5ZDEifSwiaXNzIjoiaHR0cHM6Ly9hdXRoMC5vcGVuYWkuY29tLyIsInN1YiI6ImF1dGgwfDY1ODY4YzAzNzZjZTI1Yjk2MmYyYWFiOCIsImF1ZCI6WyJodHRwczovL2FwaS5vcGVuYWkuY29tL3YxIiwiaHR0cHM6Ly9vcGVuYWkub3BlbmFpLmF1dGgwYXBwLmNvbS91c2VyaW5mbyJdLCJpYXQiOjE3MTUyNjM0NTMsImV4cCI6MTcxNjEyNzQ1Mywic2NvcGUiOiJvcGVuaWQgcHJvZmlsZSBlbWFpbCBtb2RlbC5yZWFkIG1vZGVsLnJlcXVlc3Qgb3JnYW5pemF0aW9uLnJlYWQgb2ZmbGluZV9hY2Nlc3MiLCJhenAiOiJwZGxMSVgyWTcyTUlsMnJoTGhURTlWVjliTjkwNWtCaCJ9.BcEc5Hh3J6rbDZ7dKXRGDFR6Z3exsjJHkX_cptqgmssi_ScQCLakiXmfOTVzYbj6A-DjQap2okZw8A4mc2C1c7_tQcunQGL0-nwE6DYhYHd6hieH0LNAO_x2QOW2T1cVdu1cf1EYgZOkPpbXZwamogN2RUrn4NJPEkNoJ6IzxSr2oQcgQZdZUqTB1_tYPIgVfVZlj1thUNXIqHzj6Ep8i5_NYHQfdeUK6z1ASFgbKkrPNkuWnkjvHXshfudAr0hPYmBBuSLqHiOZDHa31mqCtfWOP3EyfDoVld09Ys1VhjRJOG_iQ81Il_u5aerJqR3Fv9HT33nIl8uexXRhzJ25ZA";
                string apiKey = "sk-rC0qckh6tvfjdyU0573fC3F1D4Bf42A789C85fF324548309";

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var payload = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                new { role = "system", content = "你的名字是遗韵GPT，是一个为了购买平台解答的机器人，请回答我下面的问题" },
                new { role = "user", content = userMessage}
                    }
                };

                var jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var httpResponse = await client.PostAsync(apiUrl, content);

                if (httpResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Response status code: {httpResponse.StatusCode}");
                    var responseJson = await httpResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response content: {responseJson}");
                    var responseObject = JsonSerializer.Deserialize<dynamic>(responseJson);
                    string responseText = responseObject.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                    return responseText;
                }
                else
                {
                    throw new Exception($"请求失败：{httpResponse.ReasonPhrase}");
                }
            }
        }

    }
}
