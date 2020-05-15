using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AlszikBot
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(new Game())
            .BuildServiceProvider();


        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            client.Log += Log;
            
            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
            await client.StartAsync();

            IServiceProvider services = BuildServiceProvider();

            await new CommandHandler(client, new Discord.Commands.CommandService(), services).InstallCommandsAsync();

            await Task.Delay(-1);
        }

        

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
