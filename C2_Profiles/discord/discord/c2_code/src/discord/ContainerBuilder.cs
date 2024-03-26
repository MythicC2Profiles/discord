using Autofac;
using discord.Clients;
using discord.Models.Server;

namespace discord
{
    public static class ContainerBuilder
    {
        public static Autofac.ContainerBuilder Build()
        {
            Console.WriteLine("building.");
            var containerBuilder = new Autofac.ContainerBuilder();
            Console.WriteLine("building.");
            containerBuilder.RegisterType<ServerConfig>().As<IServerConfig>().SingleInstance();
            Console.WriteLine("building.");
            containerBuilder.RegisterType<MythicClient>().As<IMythicClient>().SingleInstance();
            Console.WriteLine("building.");
            containerBuilder.RegisterType<discord.Clients.DiscordClient>().As<IDiscordClient>().SingleInstance();
            Console.WriteLine("returning.");
            return containerBuilder;
        }
    }
}
