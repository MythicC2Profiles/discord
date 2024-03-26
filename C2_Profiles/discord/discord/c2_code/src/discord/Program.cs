using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using discord.Clients;
using discord;

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
                ServerConf = JsonConvert.DeserializeObject<ServerConfig>(System.IO.File.ReadAllText(@"C:\Users\dev\source\repos\discord\C2_Profiles\discord\c2_code\config.json")) ?? new ServerConfig();
#else
                ServerConf = JsonConvert.DeserializeObject<ServerConfig>(System.IO.File.ReadAllText(@"config.json")) ?? new ServerConfig();
#endif

                if (!ServerConf.IsValid())
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
            var containerBuilder = discord.ContainerBuilder.Build();
            var container = containerBuilder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
            }
        }
    }
}
