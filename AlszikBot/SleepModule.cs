using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace AlszikBot
{
    public class SleepModule : ModuleBase<SocketCommandContext>
    {

        private Game game;
        public SleepModule(Game game)
        {
            this.game = game;
        }

        [Command("jatekosok")]
        [Summary("Hozzáadja az embereket a játékhoz.")]
        public async Task PlayersAsync([Remainder][Summary("Játékosok")] string playerArg)
        {
            try
            {
                string[] players = playerArg.Trim().Split(',');
                for (int i = 0; i < players.Length; i++)
                {
                    players[i] = players[i].Trim();
                }
                game.AddPlayers(players, Context);
                game.Players.ForEach(Console.WriteLine);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }
        [Command("szerep")]
        [Summary("Szerepet ad egy játékosnak.")]
        public async Task RoleAsync(string role, [Remainder][Summary("Játékosok")] string playerArg)
        {

            try
            {
                string[] players = playerArg.Trim().Split(',');
                for (int i = 0; i < players.Length; i++)
                {
                    players[i] = players[i].Trim();
                }
                game.ManualRole(players, role, Context);



            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("printjatekosok")]
        [Alias("printszerepek")]
        [Summary("Kiírja a játékosokat.")]
        public async Task PrintPlayersAsync()
        {

            try
            {
                game.PrintPlayers(Context);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("randomszerepek")]
        [Summary("Random szerepeket ad a játékosoknak.")]
        public async Task GiveRandomRolesAsync([Remainder][Summary("szerepek")] string roleArg)
        {

            try
            {
                string[] roles = roleArg.Trim().Split(',');
                for (int i = 0; i < roles.Length; i++)
                {
                    roles[i] = roles[i].Trim();
                }
                game.GiveRandomRoles(roles, Context);



            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("nemjatekos")]
        [Summary("Eltávolít játékosokat a játékból.")]
        public async Task RemovePlayersAsync([Remainder][Summary("szerepek")] string nameArg)
        {

            try
            {
                string[] names = nameArg.Trim().Split(',');
                for (int i = 0; i < names.Length; i++)
                {
                    names[i] = names[i].Trim();
                }
                game.RemovePlayers(names, Context);



            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("halott")]
        [Alias("meghalt")]
        [Summary("A játékost kirúgják a Collegiumból.")]
        public async Task KillPlayerAsync([Remainder] string name)
        {

            try
            {
                game.KillPlayer(name, Context);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }
        [Command("nema")]
        [Summary("A játékost némítja.")]
        public async Task MutePlayerAsync(string name)
        {
            try
            {
                game.MutePlayer(name, Context);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }
        [Command("nemszav")]
        [Summary("A játékos nem szavazhat.")]
        public async Task CannotVoteAsync(string name)
        {
            try
            {
                game.CannotVote(name, Context);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("reset")]
        [Summary("Visszaállítja a játékot.")]
        public async Task ResetAsync()
        {
            try
            {
                game.ResetGame(Context);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("restart")]
        [Summary("Újraindítja a játékot.")]
        public async Task RestartAsync()
        {
            try
            {
                game.RestartGame(Context);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [Command("help")]
        [Summary("Újraindítja a játékot.")]
        public async Task HelpAsync(string helpType="", string help="")
        {
            try
            {
                if (helpType == "alszik")
                {
                    game.Help(help, Context);
                }
                else
                {
                    await ReplyAsync("Több témában is kérhetsz segítséget:\nAlszik a Coli: !help alszik\n");
                }
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }
    }
}
