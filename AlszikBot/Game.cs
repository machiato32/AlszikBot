using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using AlszikBot.Exceptions;
using System.Linq;
using System.Text.RegularExpressions;

namespace AlszikBot
{
    public class Game
    {
        public List<Player> Players { get; private set; }

        public static Dictionary<string, List<string>> Abbreviations { get; }
        public static Dictionary<string, string> GameRoles { get; }
        public static Dictionary<string, string> GuildRoles { get; }
        public static string NagyklubId { get; }
        public static string BaroId { get; }
        static Game()
        {
            Abbreviations = new Dictionary<string, List<string>>
            {
                { "Mr. és Miss Eötvös", new List<string>{"mrms" } },
                { "Átlag",new List<string>{"Átlag", "atl", "atlag" } },
                { "Rendszergazda",new List<string>{"Rendszergazda", "rg", "rend" } },
                { "Alfonsó",new List<string>{"Alfonsó","alf", "alfonso" } },
                { "Választmányi elnök",new List<string>{"el", "elnok", "eln" } },
                { "Választmányi alelnök",new List<string>{"alel", "alelnok" } },
                { "Gondnokasszony",new List<string>{"Gondnokasszony", "gond", "ga" } },
                { "Miss Minden lében kanál",new List<string>{"miss" } },
                { "Estike főcsapos",new List<string>{"ef", "focsap", "focsapos" } },
                { "Hannibál",new List<string>{"Hannibál", "han", "hannibal" } },
                { "Huba",new List<string>{"Huba", "huba" } },
                { "Portás",new List<string>{"Portás", "por", "portas" } },
                { "Csapos",new List<string>{"Csapos", "csap", "csapos" } }
            };
            GameRoles = new Dictionary<string, string>
            {
                { "Átlag","701024099509731442" },
                { "Rendszergazda","701024099509731443" },
                { "Alfonsó","701024099509731444" },
                { "Választmányi elnök","701024099509731446" },
                { "Választmányi alelnök","701024099509731447" },
                { "Gondnokasszony","701024099509731445" },
                { "Mr. és Miss Eötvös","" },
                { "Miss Minden lében kanál","701024099509731448" },
                { "Estike főcsapos","701024099782099004" },
                { "Hannibál","701024099782099005" },
                { "Huba","701024099782099006" },
                { "Portás","701024099782099007" },
                { "Csapos","701024099782099008" }
            };
            GuildRoles = new Dictionary<string, string>
            {
                { "Alszik", "701024098381332555" },
                { "Halott", "701024098381332554" },
                { "Nema", "690617293595607090" },
                { "NemSzav", "690617000405237792" }
            };
            NagyklubId = "701024099509731440";
            BaroId = "701024099509731441";
            //GameRoles = new Dictionary<string, string>
            //{
            //    { "Átlag","690611160944476210" },
            //    { "Rendszergazda","690616565766553652" },
            //    { "Alfonsó","690620095797526569" },
            //    { "Választmányi elnök","690618690495840267" },
            //    { "Választmányi alelnök","690618988585025616" },
            //    { "Gondnokasszony","690618632962572409" },
            //    { "Mr. és Miss Eötvös","" },
            //    { "Miss Minden lében kanál","690634323929333860" },
            //    { "Estike főcsapos","690634699470798857" },
            //    { "Hannibál","690634389230583968" },
            //    { "Huba","690634493878337596" },
            //    { "Portás","690634591907610684" },
            //    { "Csapos","690662236401827880" }
            //};
            //NagyklubId = "690619495135576144";
            //BaroId = "690660730361348216";
            //GuildRoles = new Dictionary<string, string>
            //{
            //    { "Alszik", "690610316035489863" },
            //    { "Halott", "690660872816689223" },
            //    { "Nema", "690617293595607090" },
            //    { "NemSzav", "690617000405237792" }
            //};
        }

        public Game()
        {
            Players = new List<Player>();
        }

        public void ManualRole(string[] names, string gameRole, SocketCommandContext context)
        {
            if (names.Length > 1)
            {
                AddRole(names[0], gameRole, context, names[1]);
            }
            else
            {
                AddRole(names[0], gameRole, context);
            }
        }

        private void AddRole(string name, string role, SocketCommandContext context, string name2 = "")
        {
            try
            {
                string parsedRole = Abbreviations.First(x => x.Value.Contains(role)).Key;
                var user = FindUser(name, context);
                if (parsedRole != "Mr. és Miss Eötvös")
                {
                    var channel = FindChannel(GameRoles[parsedRole], context);
                    channel.AddPermissionOverwriteAsync(user, new Discord.OverwritePermissions(1049600, 0));
                    context.Channel.SendMessageAsync(name + " jogot kapott a " + channel.Name + " szobához.");
                }
                else
                {
                    Player player2 = Players.Find(p => p.Username == name2 || p.Nickname == name2);
                    player2.AddRole(parsedRole);
                    context.Channel.SendMessageAsync(name + " és " + name2 + " mostantól Mr. és Miss Eötvös.");
                }
                Player player = Players.Find(p => p.Username == name || p.Nickname == name);
                if (player == null)
                {
                    player = new Player(user.Username, user.Nickname, parsedRole);
                    Players.Add(player);
                }
                else
                {
                    player.AddRole(parsedRole);
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(UserNotFoundException))
                    throw;
                else throw new AddRoleException("Hiba a szerep kiosztásakor. :frowning:");
            }
        }

        public void PrintPlayers(SocketCommandContext context)
        {
            try
            {
                context.Channel.SendMessageAsync("\n" + string.Join("\n", Players));
            }
            catch
            {

                throw new PrintPlayersException("Nem tudtam kiírni a játékosokat :frowning:");
            }        
        }

        public void GiveRandomRoles(string[] roles, SocketCommandContext context)
        {
            try
            {
                List<string> chosenRoles = new List<string>();
                Random random = new Random();
                foreach (Player player in Players)
                {
                    if (player.GameRoles.Count > 0)
                        continue;
                    int randomNumber;
                    do
                    {
                        randomNumber = (int)Math.Floor(random.NextDouble() * roles.Length);
                    } while (chosenRoles.Contains(roles[randomNumber]));
                    chosenRoles.Add(roles[randomNumber]);
                    AddRole(player.Username, Regex.Replace(roles[randomNumber], @"[\d]", ""), context);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public void RemovePlayers(string[] names, SocketCommandContext context)
        {
            foreach(string name in names)
            {
                try
                {
                    var user = FindUser(name, context);
                    user.RemoveRolesAsync(new List<SocketRole> {
                        FindGuildRole(GuildRoles["Alszik"],context),
                        FindGuildRole(GuildRoles["Halott"], context)
                    });
                    Player player = Players.Find(p => p.Username == user.Username);
                    foreach(string gameRole in player.GameRoles)
                    {
                        var channel = FindChannel(GameRoles[gameRole], context);
                        channel.RemovePermissionOverwriteAsync(user);
                    }
                    Players.Remove(player);
                    
                    context.Channel.SendMessageAsync("Eltávolítottam a(z) " + name + " nevű játékost.");
                }
                catch
                {

                    throw new Exception("Nem tudtam eltávolítani a(z) "+name+" nevű játékost.");
                }
            }
        }

        public void AddPlayers(string[] names, SocketCommandContext context)
        {
            try
            {
                SocketRole alszikRole = FindGuildRole(GuildRoles["Alszik"], context);
                foreach (string name in names)
                {
                    if (Players.Find(p => p.Nickname == name || p.Username == name) == null) 
                    {
                        var user = FindUser(name, context);
                        user.AddRoleAsync(alszikRole);
                        Players.Add(new Player(user.Username, user.Nickname));
                    }
                }
                context.Channel.SendMessageAsync("A játékosok felvéve a listára.");
            }
            catch(Exception ex)
            {
                if (ex.GetType() == typeof(UserNotFoundException))
                    throw;
                else throw new AddPlayerException("Hiba a játékosok hozzáadásakor. :frowning:");
            }
        }

        public void KillPlayer(string name, SocketCommandContext context)
        {
            try
            {

                var user = FindUser(name, context);
                Player player = Players.Find(p => p.Username == user.Username);
                var nagyklub = FindChannel(NagyklubId, context);
                var baro = FindChannel(BaroId, context);

                if (player.GameRoles.Contains("Mr. és Miss Eötvös"))
                {
                    Player player2 = Players.Find(p => p.Username != player.Username && p.GameRoles.Contains("Mr. és Miss Eötvös"));
                    var user2 = FindUser(player2.Username, context);
                    user2.AddRoleAsync(FindGuildRole(GuildRoles["Halott"], context));
                    user2.RemoveRoleAsync(FindGuildRole(GuildRoles["Alszik"], context));

                    user2.ModifyAsync(x => x.Channel = baro as SocketVoiceChannel);
                    System.Threading.Thread.Sleep(1000);
                    user2.ModifyAsync(x => x.Channel = nagyklub as SocketVoiceChannel);
                    context.Channel.SendMessageAsync(player2.Nickname + " is halott!");
                    if (player2.GameRoles.Contains("Estike főcsapos"))
                    {
                        context.Channel.SendMessageAsync("Vigyázz, " + player2.Nickname + " Estike főcsapos!");
                    }
                }
                if (player.GameRoles.Contains("Estike főcsapos"))
                {
                    context.Channel.SendMessageAsync("Vigyázz, " + player.Nickname + " Estike főcsapos!");
                }
                user.AddRoleAsync(FindGuildRole(GuildRoles["Halott"], context));
                user.RemoveRoleAsync(FindGuildRole(GuildRoles["Alszik"], context));
                user.ModifyAsync(x => x.Channel = baro as SocketVoiceChannel);
                System.Threading.Thread.Sleep(200);
                user.ModifyAsync(x => x.Channel = nagyklub as SocketVoiceChannel);
                context.Channel.SendMessageAsync(player.Nickname + " halott!");
            }
            catch
            {
                throw;
            }

        }

        public void CannotVote(string name, SocketCommandContext context)
        {
            try
            {
                var user = FindUser(name, context);
                user.AddRoleAsync(FindGuildRole(GuildRoles["NemSzav"], context));
                user.RemoveRoleAsync(FindGuildRole(GuildRoles["Alszik"], context));
                context.Channel.SendMessageAsync(name + " nem szavazhat.");
            }
            catch
            {
                throw new Exception(name + "sajnos meg mindig szavazhat.");
            }
            
        }

        public void MutePlayer(string name, SocketCommandContext context)
        {
            try
            {
                var user = FindUser(name, context);
                var nagyklub = FindChannel(NagyklubId, context);
                var baro = FindChannel(BaroId, context);
                user.AddRoleAsync(FindGuildRole(GuildRoles["Nema"], context));
                user.RemoveRoleAsync(FindGuildRole(GuildRoles["Alszik"], context));
                user.ModifyAsync(x => x.Channel = baro as SocketVoiceChannel);
                System.Threading.Thread.Sleep(1000);
                user.ModifyAsync(x => x.Channel = nagyklub as SocketVoiceChannel);
                context.Channel.SendMessageAsync(name + " nem beszélhet.");
            }
            catch
            {
                throw new Exception(name + "sajnos meg mindig beszélhet.");
            }
        }

        public void RestartGame(SocketCommandContext context)
        {
            PrintPlayers(context);
            var nagyklub = FindChannel(NagyklubId, context);
            var baro = FindChannel(BaroId, context);
            foreach (Player player in Players)
            {
                try
                {
                    var user = FindUser(player.Username, context);
                    user.AddRoleAsync(FindGuildRole(GuildRoles["Alszik"], context));
                    System.Threading.Thread.Sleep(200);
                    user.RemoveRoleAsync(FindGuildRole(GuildRoles["Halott"], context));
                    user.ModifyAsync(x => x.Channel = baro as SocketVoiceChannel);
                    System.Threading.Thread.Sleep(200);
                    user.ModifyAsync(x => x.Channel = nagyklub as SocketVoiceChannel);
                    player.GameRoles.Remove("Mr. és Miss Eötvös");
                    foreach (string gameRole in player.GameRoles)
                    {
                        var channel = FindChannel(GameRoles[gameRole], context);
                        channel.RemovePermissionOverwriteAsync(user);
                    }
                    player.GameRoles = new List<string>();
                    
                }
                catch
                {
                    throw new Exception(player.Username + "dolgait nem sikerült visszaállitani.");
                }

            }
            context.Channel.SendMessageAsync("A játék újraindult");
        }

        public void ResetGame(SocketCommandContext context)
        {
            PrintPlayers(context);
            foreach (Player player in Players)
            {
                try
                {
                    
                    var user = FindUser(player.Username, context);
                    user.RemoveRolesAsync(new List<SocketRole> {
                        FindGuildRole(GuildRoles["Alszik"], context),
                        FindGuildRole(GuildRoles["Halott"], context)
                    });
                    player.GameRoles.Remove("Mr. és Miss Eötvös");
                    foreach (string gameRole in player.GameRoles)
                    {
                        var channel = FindChannel(GameRoles[gameRole], context);
                        channel.RemovePermissionOverwriteAsync(user);
                    }
                    Players = new List<Player>();
                }
                catch
                {
                    throw new Exception(player.Username + "dolgait nem sikerült visszaállitani.");
                }

            }


            context.Channel.SendMessageAsync("A játék újraindult");
        }

        public void Help(string help, SocketCommandContext context)
        {
            switch (help)
            {
                case "szerep":
                    context.Channel.SendMessageAsync("Ezzel a paranccsal szereped tudsz adni egy játékosnak. Utána a többieket akár random is kisorsoltathatod.\nHasználat: !szerep <játékos szerepe, lásd rövidítések> <játékos>");
                    break;
                case "printszerepek":
                    context.Channel.SendMessageAsync("Ez a parancs kiírja az összes játékost és az ő játékban betöltött szerepüket. Ugyanaz, mint printjatekosok.\nHasználat: !printszerepek");
                    break;
                case "printjatekosok":
                    context.Channel.SendMessageAsync("Ez a parancs kiírja az összes játékost és az ő játékban betöltött szerepüket. Ugyanaz, mint printszerepek.\nHasználat: !printjatekosok");
                    break;
                case "randomszerepek":
                    context.Channel.SendMessageAsync("Ezzel a paranccsal ki tudod sorsolni a szereppel még nem rendelkező (kezdetkor mindenki) játékosok között a beírt szerepeket.\nHasználat: !randomszerpek <szerep1>, <szerep2> stb.\nFontos a vesszők használata!");
                    break;
                case "nemjatekos":
                    context.Channel.SendMessageAsync("Ezzel a paranccsal ki tudsz venni embereket a játékosok közül.\nHasználat: !nemjateksok <játékos1>, <játékos2> stb.");
                    break;
                case "jatekosok":
                    context.Channel.SendMessageAsync("Ezzel a paranccsal tudsz hozzáadni embereket a játékosokhoz.\nHasználat: !jatekosok <játékos1>, <játékos2> stb.");
                    break;
                case "meghalt":
                    context.Channel.SendMessageAsync("Ezzel a paranccsal rúgsz ki egy játékos a Collegiumból. Ugyanaz, mint halott.\nHasználat: !meghalt <játékos>");
                    break;
                case "halott":
                    context.Channel.SendMessageAsync("Ezzel a paranccsal rúgsz ki egy játékos a Collegiumból. Ugyanaz, mint meghalt.\nHasználat: !halott <játékos>");
                    break;
                case "nemszav":
                    context.Channel.SendMessageAsync("Ezzel a paranccsal el tudod venni a szavazati jogot egy játékostól.\nHasználat: !nemszav <játékos>");
                    break;
                case "nema":
                    context.Channel.SendMessageAsync("Ezzel a paranccsal el tudod venni a hangját egy játékosnak.\nHasználat: !nema <játékos>");
                    break;
                case "restart":
                    context.Channel.SendMessageAsync("Ez a parancs újraindítja a játékot, megtartva az előző kör játékosait. Új kör indításakor használd.\nHasználat: !restart\n*Sajnos nem mindig működik tökéletesen* :frowning:");
                    break;
                case "reset":
                    context.Channel.SendMessageAsync("Ez a parancs visszaállítja a játék előtti állapotot, ezt használd, ha vége a játéknak.\nHasználat: !reset");
                    break;
                case "rovid":
                    string messageString = "A rövidítések:\n";
                    foreach(KeyValuePair<string, List<string>> pair in Abbreviations)
                    {
                        messageString += pair.Key + ": " + string.Join(", ", pair.Value) + "\n";
                    } 
                    context.Channel.SendMessageAsync(messageString);
                    break;
                case "tippek":
                    context.Channel.SendMessageAsync("A játék kezdete előtt adj hozzá mindenkit a játékhoz a !jatekosok paranccsal. Ez ad nekik jogot megtekinteni a játékhoz kapcsolódó szobákat.\nAz éjszaka történt eseményeket jegyezd fel. Erre alkalmas lehet a játékvezetői csatorna.\nA szavazásokat a közgyűlés csatornában hozd létre: \\poll <játékos> ki,<játékos> marad paranccsal.\nÉs a legfontosabb: ha kérdeznek valamilyen szabályt, amit nem tudsz, legyél kreatív :smile:");
                    break;
                default:
                    context.Channel.SendMessageAsync("Alszik a Coliban a következő parancsok léteznek:\n-szerep\n-printszerepek\n-printjatekosok\n-randomszerepek\n-nemjatekos\n-jatekosok\n-meghalt\n-halott\n-nemszav\n-nema\n-restart\n-reset\nEzekhez segítséget a !help alszik <parancs neve> paranccsal kaphatsz!\nEzen kívül kikérheted a rövidítéseket (!help alszik rovid), vagy kérhetsz általános tippeket is (!help alszik tippek)");
                    break;
            }
        }

        public static SocketGuildChannel FindChannel(string id, SocketCommandContext context)
        {
            SocketGuildChannel channel = null;
            foreach (var guildChannel in context.Guild.Channels)
            {
                if (guildChannel.Id.ToString()==id)
                {
                    channel = guildChannel;
                    break;
                }
            }
            return channel;
        }

        public static SocketGuildUser FindUser(string name, SocketCommandContext context)
        {
            SocketGuildUser user = null;
            try
            {
                foreach (var guildUser in context.Guild.Users)
                {
                    if (guildUser.Username == name || guildUser.Nickname == name) 
                    {
                        user = guildUser;
                        break;
                    }
                }
                if (user == null)
                {
                    throw new UserNotFoundException("Nem talaltam a(z) " + name + " nevű játékost. :frowning:");
                }
                return user;
            }
            catch
            {
                throw new UserNotFoundException("Nem talaltam a(z) "+name+" nevű játékost. :frowning:");
            }
            
        }

        public static SocketRole FindGuildRole(string id, SocketCommandContext context)
        {
            try
            {
                var guildRole = context.Guild.Roles.First(x => x.Id.ToString() == id);

                if (guildRole == null) throw new GuildRoleNotFoundException("Nem talaltam a keresett szerepet");

                return guildRole;
            }
            catch
            {
                throw new GuildRoleNotFoundException("Nem talaltam a keresett szerepet");
            }
        }
    }
}
