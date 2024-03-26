using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discord.Models.Server
{
    public interface IDiscordClient
    {
        public Task WriteToChannel(string data, string id);
    }
}
