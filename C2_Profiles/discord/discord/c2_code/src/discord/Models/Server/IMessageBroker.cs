using Discord.Models.Mythic;
using PushC2Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discord.Models.Server
{
    public interface IMessageBroker
    {
        public Task QueueMessage(MythicMessageWrapper message);
    }
}
