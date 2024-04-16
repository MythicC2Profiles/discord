using Autofac;
using discord.Clients;
using discord.Models.Server;

namespace discord
{
    public static class ContainerBuilder
    {
        public static Autofac.ContainerBuilder Build()
        {
            var containerBuilder = new Autofac.ContainerBuilder();
            containerBuilder.RegisterType<ServerConfig>().As<IServerConfig>().SingleInstance();
            containerBuilder.RegisterType<MythicClient>().As<IMythicClient>().SingleInstance();
            containerBuilder.RegisterType<discord.Clients.DiscordClient>().As<IDiscordClient>().SingleInstance();
            return containerBuilder;
        }
    }
}
