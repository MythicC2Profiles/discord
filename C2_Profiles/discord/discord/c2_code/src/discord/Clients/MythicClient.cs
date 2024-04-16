using System.Text;
using discord.Models.Server;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using PushC2Services;

namespace discord.Clients
{
    public class MythicClient : IMythicClient
    {
        private readonly GrpcChannel _mythicChannel;
        private readonly PushC2.PushC2Client _mythicConnection;
        private readonly AsyncDuplexStreamingCall<PushC2MessageFromAgent, PushC2MessageFromMythic> _mythicConnector;
        public event EventHandler<PushC2MessageFromMythic> OnMessageReceived;
        public MythicClient()
        {
#if DEBUG
            _mythicChannel = GrpcChannel.ForAddress("http://10.30.26.108:17444");
#else
            _mythicChannel = GrpcChannel.ForAddress("http://127.0.0.1:17444");
#endif
            _mythicConnection = new PushC2.PushC2Client(_mythicChannel);
            try
            {
                _mythicConnector = _mythicConnection.StartPushC2StreamingOneToMany();
                _mythicConnector.RequestStream.WriteAsync(new PushC2MessageFromAgent()
                {
                    C2ProfileName = "discord"
                }).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[MythicClient] {e.ToString()}");
            }
        }
        public async Task SendToMythic(string id, string data)
        {
            try
            {
                await _mythicConnector.RequestStream.WriteAsync(new PushC2MessageFromAgent
                {
                    C2ProfileName = "discord",
                    Base64Message = ByteString.CopyFrom(data, Encoding.UTF8),
                    TrackingID = id,
                    RemoteIP = "",
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SendToMythic] {e.ToString()}");
            }
        }
        public async Task ReceiveFromMythicAsync()
        {
            _ = Task.Run(async () =>
            {
                await foreach (var message in _mythicConnector.ResponseStream.ReadAllAsync())
                {
                    if (OnMessageReceived != null)
                    {
                        OnMessageReceived(this, _mythicConnector.ResponseStream.Current);
                    }
                }
            });
        }
    }
}