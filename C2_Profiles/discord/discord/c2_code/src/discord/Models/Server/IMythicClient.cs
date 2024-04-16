using PushC2Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discord.Models.Server
{
    public interface IMythicClient
    {
        Task SendToMythic(string id, string data);
        Task ReceiveFromMythicAsync();
        public event EventHandler<PushC2MessageFromMythic> OnMessageReceived;
    }
}
