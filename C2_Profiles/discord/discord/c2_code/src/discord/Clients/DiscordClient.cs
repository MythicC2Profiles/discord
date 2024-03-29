using discord.Models.Server;
using Discord;
using Discord.Models.Mythic;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Threading.Channels;
using IDiscordClient = discord.Models.Server.IDiscordClient;

namespace discord.Clients
{
    public class DiscordClient : IDiscordClient
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly HttpClient _httpClient;
        private readonly IMythicClient _mythicClient;
        private AutoResetEvent _ready = new AutoResetEvent(false);
        private ITextChannel _channel;
        private readonly string _uuid;
        private readonly IServerConfig _config;
        public DiscordClient(IMythicClient mythicClient, IServerConfig config)
        {
            var discordConfig = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            _uuid = Guid.NewGuid().ToString();
            _config = config;
            _discordClient = new DiscordSocketClient(discordConfig);
            _discordClient.MessageReceived += MessageReceivedAsync;
            _discordClient.Ready += _client_Ready;
            _discordClient.StartAsync();
            _httpClient = new HttpClient();
            _mythicClient = mythicClient;
            _mythicClient.ReceiveFromMythicAsync();
            _mythicClient.OnMessageReceived += _mythicClient_OnMessageReceived;

        }
        private async Task _client_Ready()
        {
            _channel = (ITextChannel)_discordClient.GetChannel(ulong.Parse(_config.ChannelID));

            if (_channel is null)
            {
                Console.WriteLine("[WriteToChannel] Unable to get channel: Channel is null");
                Environment.Exit(0);
            }
            await this.CatchUp();
            _ready.Set();
        }

        private void _mythicClient_OnMessageReceived(object? sender, PushC2Services.PushC2MessageFromMythic e)
        {
            this.WriteToChannel(e.Message.ToStringUtf8(), e.TrackingID);
        }

        private async Task CatchUp()
        {
            try
            {
                var messages = await _channel.GetMessagesAsync().FlattenAsync();
                foreach (var message in messages)
                {
                    await this.MessageReceivedAsync(message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[CatchUp] {e.ToString()}");
            }
        }

        public async Task Start()
        {
            await _discordClient.LoginAsync(TokenType.Bot, _config.BotToken);

            if (_discordClient.LoginState != LoginState.LoggedIn)
            {
                Console.WriteLine("[Start] Failed to login to discord");
            }

            _ready.WaitOne();
            await Task.Delay(Timeout.Infinite);
        }

        private async Task MessageReceivedAsync(IMessage message)
        {
            MythicMessageWrapper discordMessage = null; 
            if (message.Attachments.Count > 0 && message.Attachments.FirstOrDefault().Filename.EndsWith("server"))
            {
                try
                {
                    discordMessage = JsonConvert.DeserializeObject<MythicMessageWrapper>(await GetFileContentsAsync(message.Attachments.FirstOrDefault().Url));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[MessageReceivedAsync] {e.Message}");
                }
            }
            else
            {
                try 
                { 
                    discordMessage = JsonConvert.DeserializeObject<MythicMessageWrapper>(message.Content);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[MessageReceivedAsync] {e.Message}");
                }
            }

            if (discordMessage is not null && discordMessage.to_server) //It belongs to us
            {
                try
                {
                    _ = message.DeleteAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[MessageReceivedAsync] {e.Message}");
                }

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

            if(_channel is null)
            {
                Console.WriteLine("[WriteToChannel] Unable to get channel: Channel is null");
                return;
            }

            if (message.Length > 1950)
            {
                using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(discordMessage))))
                {
                    try { 
                        await _channel.SendFileAsync(stream, discordMessage.client_id);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"[WriteToChannel] {e.ToString()}");
                    }
                }
            }
            else
            {
                try
                {
                    await _channel.SendMessageAsync(JsonConvert.SerializeObject(discordMessage));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[WriteToChannel] {e.ToString()}");
                }
            }

        }
        private async Task<string> GetFileContentsAsync(string url)
        {
            string message = String.Empty;

            try
            {
                using (HttpResponseMessage response = await _httpClient.GetAsync(url))
                {
                    using (HttpContent content = response.Content)
                    {
                        message = await content.ReadAsStringAsync();
                    }
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"[GetFileContentsAsync] {response.StatusCode.ToString()}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[GetFileContentsAsync] {e.ToString()}");
            }
            return Unescape(message) ?? "";
        }
        private string Unescape(string message)
        {
            return message.TrimStart('"').TrimEnd('"').Replace("\\\"", "\"");

        }
    }
}
