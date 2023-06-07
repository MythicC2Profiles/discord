using C2Send.Models.Server;
using C2Send.Clients;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Discord.Models.Discord;
using Newtonsoft.Json;

namespace C2Send
{
    class Program
    {
        public static ServerConfig ServerConf;
        /// <summary>
        /// Main loop
        /// </summary>
        public static void Main(string[] args)
        {
            try
            {

#if DEBUG
                ServerConf = JsonConvert.DeserializeObject<ServerConfig>(System.IO.File.ReadAllText(@"C:\Users\scott\source\repos\discord\C2_Profiles\discord\c2_code\config.json")) ?? new ServerConfig();
#else
                ServerConf = JsonConvert.DeserializeObject<ServerConfig>(System.IO.File.ReadAllText(@"config.json")) ?? new ServerConfig();
#endif

                if (ServerConf.IsAnyNullOrEmpty())
                {
                    Console.WriteLine("[Main] Config is null or empty.");
                    return;
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Main] Error parsing config: {e.Message}");
                Environment.Exit(e.HResult);
            }

          
            //Start the handler
            AsyncMain(args).GetAwaiter().GetResult();
        }
        public static async Task AsyncMain(string[] args)
        {


            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                // Tokens should be considered secret data and never hard-coded.
                // We can read from the environment variable to avoid hard coding.
                await client.LoginAsync(TokenType.Bot, ServerConf.BotToken);
                await client.StartAsync();

                // Here we initialize the logic required to register our commands.
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }
        private static ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
