using System.Text;
using discord.Models.Server;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using PushC2Services;
using IDiscordClient = discord.Models.Server.IDiscordClient;

namespace discord.Clients
{
    public class MythicClient : IMythicClient
    {
        private readonly GrpcChannel _mythicChannel;
        private readonly PushC2.PushC2Client _mythicConnection;
        private readonly IDiscordClient _discordClient;
        private readonly AsyncDuplexStreamingCall<PushC2MessageFromAgent, PushC2MessageFromMythic> _mythicClient;
        public MythicClient(IDiscordClient discordClient)
        {
            _discordClient = discordClient;
#if DEBUG
            _mythicChannel = GrpcChannel.ForAddress("https://10.30.26.108:17444");
#else
            _mythicChannel = GrpcChannel.ForAddress("https://127.0.0.1:17444");
#endif
            _mythicConnection = new PushC2.PushC2Client(_mythicChannel);
            _mythicClient = _mythicConnection.StartPushC2StreamingOneToMany();
            _mythicClient.RequestStream.WriteAsync(new PushC2MessageFromAgent()
            {
                C2ProfileName = "discord"
            }).Wait();
        }

        public async Task<string> SendToMythic(string id, string data)
        {
            await _mythicClient.RequestStream.WriteAsync(new PushC2MessageFromAgent { 
                C2ProfileName = "discord",
                Base64Message = ByteString.CopyFrom(data, Encoding.UTF8),
                TrackingID = id,
                RemoteIP = "",
            });
            return "";
        }
        public async Task<string> ReceiveFromMythicAsync()
        {
            while (true)
            {
                await foreach(var message in _mythicClient.ResponseStream.ReadAllAsync())
                {

                }
            }
        }
    }
}