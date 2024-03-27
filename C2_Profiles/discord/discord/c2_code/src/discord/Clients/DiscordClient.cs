using discord.Models.Server;
using Discord;
using Discord.Models.Mythic;
using Discord.WebSocket;
using Newtonsoft.Json;
using IDiscordClient = discord.Models.Server.IDiscordClient;

namespace discord.Clients
{
    public class DiscordClient : IDiscordClient
    {
        private readonly DiscordSocketClient _client;
        private readonly HttpClient _httpClient;
        private readonly IMythicClient _mythicClient;
        private readonly string _uuid;
        private readonly IServerConfig _config;
        public async Task Start()
        {
            Console.WriteLine("Logging in with token: " + _config.BotToken);
            await _client.LoginAsync(TokenType.Bot, _config.BotToken);
            Console.WriteLine(_client.LoginState);
            Console.WriteLine("Starting Loop.");
            await Task.Delay(Timeout.Infinite);
        }
        public DiscordClient(IMythicClient mythicClient, IServerConfig config)
        {
            var discordConfig = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            _uuid = Guid.NewGuid().ToString();
            _config = config;
            _client = new DiscordSocketClient(discordConfig);
            _client.MessageReceived += MessageReceivedAsync;
            _client.StartAsync();
            _httpClient = new HttpClient();
            _mythicClient = mythicClient;
            _mythicClient.ReceiveFromMythicAsync();
            _mythicClient.OnMessageReceived += _mythicClient_OnMessageReceived;

        }

        private void _mythicClient_OnMessageReceived(object? sender, PushC2Services.PushC2MessageFromMythic e)
        {
            this.WriteToChannel(e.Message.ToStringUtf8(), e.TrackingID);
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            Console.WriteLine("Message Received: " + message.Content);
            MythicMessageWrapper discordMessage;
            if (message.Attachments.Count > 0 && message.Attachments.FirstOrDefault().Filename.EndsWith("server"))
            {
                discordMessage = JsonConvert.DeserializeObject<MythicMessageWrapper>(await GetFileContentsAsync(message.Attachments.FirstOrDefault().Url));
            }
            else
            {
                discordMessage = JsonConvert.DeserializeObject<MythicMessageWrapper>(message.Content);
            }

            if (discordMessage is not null && discordMessage.to_server) //It belongs to us
            {
                Console.WriteLine("Got Message: " + discordMessage.message);
                _ = message.DeleteAsync();
                await _mythicClient.SendToMythic(discordMessage.sender_id, discordMessage.message);
            }
        }
        public async Task WriteToChannel(string message, string id)
        {
            MythicMessageWrapper discordMessage = new MythicMessageWrapper()
            {
                to_server = false,
                sender_id = _uuid,
                message = message,
                client_id = id,
            };

            ITextChannel channel = (ITextChannel)_client.GetChannel(ulong.Parse(_config.ChannelID));

            if (message.Length > 1950)
            {
                await channel.SendFileAsync(JsonConvert.SerializeObject(discordMessage), discordMessage.sender_id + ".txt");
            }
            else
            {
                await channel.SendMessageAsync(JsonConvert.SerializeObject(discordMessage));
            }

        }
        private async Task<string> GetFileContentsAsync(string url)
        {
            string message;
            using (HttpResponseMessage response = await _httpClient.GetAsync(url))
            {
                using (HttpContent content = response.Content)
                {
                    message = await content.ReadAsStringAsync();
                }
            }
            return await Unescape(message) ?? "";
        }
        private async Task<string> Unescape(string message)
        {
            return message.TrimStart('"').TrimEnd('"').Replace("\\\"", "\"");

        }
    }
}
