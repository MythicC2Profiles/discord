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
            containerBuilder.RegisterType<DiscordClient>().As<IDiscordClient>().SingleInstance();
            containerBuilder.RegisterType<MythicClient>().As<IMythicClient>().SingleInstance();
            containerBuilder.RegisterType<ServerConfig>().As<IServerConfig>().SingleInstance();
            return containerBuilder;
        }
    }
}
