using discord.Models.Server;
using System.Text.Json;

namespace discord.Clients
{
    public class ServerConfig : IServerConfig
    {
        public ServerConfig()
        {
#if DEBUG
            string configText = File.ReadAllText(@"../../../../discord/dev_config.json");
            var configValues = JsonSerializer.Deserialize<Dictionary<string, string>>(configText);
            this.BotToken = configValues["botToken"];
            this.ChannelID = configValues["channelID"];
#else
            string configText = File.ReadAllText(@"config.json");
            var configValues = JsonSerializer.Deserialize<Dictionary<string, string>>(configText);
            this.BotToken = configValues["botToken"];
            this.ChannelID = configValues["channelID"];
#endif
        }
        public string BotToken { get; set; }
        public string ChannelID { get; set; }
        public bool IsValid()
        {
            return BotToken != null && ChannelID != null && ulong.TryParse(ChannelID, out _);
        }
    }
}
