using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace AlszikBot
{
    class CommandHandler
    {
        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;


        public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            this.client = client;
            this.commands = commands;
            this.services = services;
        }

        public async Task InstallCommandsAsync()
        {
            client.MessageReceived += HandleCommandAsync;

            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: services);
        }


        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            //if ((message.Author as SocketGuildUser).Roles.First(x => x.Id.ToString() == "701024098381332558") == null) return;
            int argPos = 0;
            

            if (!(message.HasCharPrefix('!', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos) || message.Author.IsBot))
                return;

            var context = new SocketCommandContext(client, message);

            var result = await commands.ExecuteAsync(context: context, argPos: argPos, services: services);
        }
    }
}
