using C2Send.Clients;
using Discord.Commands;
using Discord.Models.Mythic;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Reflection;

namespace Discord.Models.Discord
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        static MythicClient mythicClient = new MythicClient();
        HttpClient httpClient = new HttpClient();

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            // Register modules that are public and inherit ModuleBase<T>.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {            
            if (!(rawMessage is SocketUserMessage message))
                return;


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
                rawMessage.DeleteAsync();

                //Forward to Mythic
                discordMessage.message = await mythicClient.SendToMythic(discordMessage.message);
                discordMessage.to_server = false;


                //Return response to Agent
                var context = new CommandContext(_discord, message);

                if (discordMessage.message.Length > 1950)
                {
                    //Upload Attachment
                    byte[] msgBytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(discordMessage));
                    
                    using (MemoryStream stream = new MemoryStream(msgBytes))
                    {
                        FileAttachment athenaMsg = new FileAttachment(stream, discordMessage.sender_id + ".txt");
                        await context.Channel.SendFileAsync(athenaMsg);
                    }
                }
                else
                {
                    await context.Channel.SendMessageAsync(JsonConvert.SerializeObject(discordMessage));
                }
            }
        }

        private async Task<string> GetFileContentsAsync(string url)
        {
            string message;
            using (HttpResponseMessage response = httpClient.GetAsync(url).Result)
            {
                using (HttpContent content = response.Content)
                {
                    message = content.ReadAsStringAsync().Result;
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