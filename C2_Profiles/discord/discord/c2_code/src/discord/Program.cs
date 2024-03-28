using Discord;
using Discord.Commands;
using discord.Clients;
using Autofac;
using discord.Models.Server;

namespace C2Send
{
    class Program
    {
        /// <summary>
        /// Main loop
        /// </summary>
        public static void Main(string[] args)
        { 
            //Start the handler
            AsyncMain(args).GetAwaiter().GetResult();
        }
        public static async Task AsyncMain(string[] args)
        {
            var containerBuilder = discord.ContainerBuilder.Build();
            var container = containerBuilder.Build();
            using (var scope = container.BeginLifetimeScope())
            {
                var discordClient = scope.Resolve<discord.Models.Server.IDiscordClient>();
                await discordClient.Start();
            }
        }
    }
}
