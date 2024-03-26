using discord.Models.Server;

namespace discord.Clients
{
    public class ServerConfig : IServerConfig
    {
        public string BotToken { get; set; }
        public string ChannelID { get; set; }
        public bool IsValid()
        {
            return BotToken != null && ChannelID != null && ulong.TryParse(ChannelID, out _);
        }
    }
}
