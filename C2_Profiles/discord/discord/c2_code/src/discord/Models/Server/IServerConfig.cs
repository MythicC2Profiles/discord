using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discord.Models.Server
{
    public interface IServerConfig
    {
        public string BotToken { get; set; }
        public string ChannelID { get; set; }
        public bool IsValid();
    }
}
