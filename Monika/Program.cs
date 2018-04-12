using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MarkovNextGen;
using Newtonsoft.Json;
using Monika.AdminController;

namespace Monika
{
    class Program
    {
        public static void Main(string[] args)
        {

            // Generate reversion pdo
            /*
            var chr = new Character();
            chr.Name = "Sayori";
            chr.Avatar = "Data\\Sayori\\25.png";
            chr.Personality = "Data\\Sayori\\edgy.pdo";
            chr.Responses = new List<string>();
            chr.PdoFiles = new Dictionary<string, string>();
            chr.PdoFiles.Add("default", "Data\\Sayori\\edgy.pdo");
            chr.Emotions = new Dictionary<string, string>();
            chr.Emotions.Add("default", "Data\\Sayori\\25.png");
            var chr_json = JsonConvert.SerializeObject(chr, Formatting.Indented);
            File.WriteAllText("sayoritest.pdo", chr_json);
            */

            var mkbot = new MonikaBot();
            mkbot.MainAsync().GetAwaiter().GetResult(); // Just let it run in background

            // I really hate doing this because it'll block main() entirely while the bot boots up in the background
            // There's alternative ways but I don't want to have to go back and forth between event handlers, yk?
            // We can maybe implement that in a future update
            while (!mkbot.IsReady)
            {
                // Wait for bot to be ready
            }

            AdminConsole admin = new AdminConsole();

            admin.Client = mkbot.Client;
            admin.Generator = mkbot.Generator;
            admin.Personality = mkbot.Personality;

            Console.Write("> ");
            var cmd = Console.ReadLine();
            while (cmd != "exit")
            {
                admin.ParseCommand(cmd);
                Console.Write("> ");
                cmd = Console.ReadLine();
            }
        }
    }

    public class MonikaBot
    {
        const string TOKFILE = "default.tokens.pdo";
        Random randy = new Random();

        private TokenSet Tokens
        {
            get
            {
                if (!File.Exists(TOKFILE))
                {
                    const string DEVELOPMENT = "Dev Token Here";
                    const string RELEASE = "Release Token Here";
                    return new TokenSet(DEVELOPMENT, RELEASE);
                }
                else
                {
                    var json = File.ReadAllText(TOKFILE);
                    return JsonConvert.DeserializeObject<TokenSet>(json);
                }
            }
        }

        public Markov Generator { get; private set; } = new Markov();
        public DiscordSocketClient Client { get; private set; } = new DiscordSocketClient();
        public List<String> ResponsesList { get; set; }
        public Boolean IsReady { get; private set; } = false;
        public String CurrentCharacter { get; set; } = "Sayori";
        public Personality Personality { get; set; }

        public MonikaBot()
        {
            // Maybe initialize ResponsesList
            ResponsesList = new List<string>();
            Personality = new Personality(Client);
        }

        public MonikaBot(string path)
        {
            Generator = new Markov(path);
            ResponsesList = new List<string>();
            Personality = new Personality(Client);
        }

        public static Boolean TaggedIn(SocketMessage msg, String username)
        {
            var _tagged = false;
            foreach (var usr in msg.MentionedUsers)
            {
                if (usr.Username == username)
                {
                    _tagged = true;
                }
            }
            return _tagged;
        }

        public ManifestNextGen GetManifestByName(string name)
        {
            var filename = String.Format("{0}.chr\\{0}.pdo", name);
            if (File.Exists(filename))
            {
                var contents = File.ReadAllText(filename);
                return JsonConvert.DeserializeObject<ManifestNextGen>(contents);
            }
            else
            {
                var contents = File.ReadAllText("Sayori.chr\\Sayori.pdo");
                return JsonConvert.DeserializeObject<ManifestNextGen>(contents);
            }
        }

        public List<ChannelInfo> GetChannels()
        {
            if (File.Exists("channels.pdo"))
            {
                var contents = File.ReadAllText("channels.pdo");
                return JsonConvert.DeserializeObject<List<ChannelInfo>>(contents);
            }
            else
            {
                return new List<ChannelInfo>();
            }
        }

        public void AddChannel(string server, string name, UInt64 id)
        {
            var channels = GetChannels();
            var info = new ChannelInfo(server, name, id);
            channels.Add(info);
            var json = JsonConvert.SerializeObject(channels, Formatting.Indented);
            File.WriteAllText("channels.pdo", json);
        }

        public static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        public async Task MessageReceived(SocketMessage msg)
        {
            var author = msg.Author.Username;
            var text = msg.Content;

            if (author != Client.CurrentUser.Username) // Don't process our own messages
            {
                if (TaggedIn(msg, Client.CurrentUser.Username))
                {
                    // Check if channel exists in our file
                    var matches = GetChannels().Where(c => c.Id == msg.Channel.Id).ToList();
                    if (matches.Count == 0) // If the channel isn't already in the list
                    {
                        if (msg.Channel is SocketGuildChannel) // Only returns true we're in a server as opposed to DM or gorup chat
                        {
                            var guildchannel = msg.Channel as SocketGuildChannel;
                            AddChannel(guildchannel.Guild.Name, guildchannel.Name, msg.Channel.Id);
                        }
                    }

                    if (text.Contains(" say "))
                    {
                        var response = text.Substring(text.IndexOf("say") + 4);
                        await msg.Channel.SendMessageAsync(response);
                    }
                    else if (text.Contains(" markov "))
                    {
                        var ss = text.Substring(text.IndexOf("markov") + 7);
                        if (Int32.TryParse(ss, out int i))
                        {
                            var response = Generator.Generate(i);
                            await msg.Channel.SendMessageAsync(response);
                        }
                    }
                    else if (text.Contains(" be "))
                    {
                        var rest = text.Substring(text.IndexOf("be") + 3);
                        var man = GetManifestByName(CurrentCharacter);
                        var avatar = (man.Files.ContainsKey(rest)) ? (man.Name + ".chr\\" + man.Files[rest]) : (man.Name + ".chr\\" + man.Files["_avatar"]);
                        await Personality.SetAvatar(avatar);
                    }
                    else if (text.Contains(" act "))
                    {
                        var rest = text.Substring(text.IndexOf("act") + 4);
                        var man = GetManifestByName(CurrentCharacter);
                        var pdo = (man.Files.ContainsKey(rest)) ? (man.Name + ".chr\\" + man.Files[rest]) : (man.Name + ".chr\\" + man.Files["_markov"]);
                        if (File.Exists(pdo))
                        {
                            var contents = File.ReadAllText(pdo);
                            Generator.Chain = JsonConvert.DeserializeObject<Dictionary<string, Link>>(contents);
                        }
                    }
                    else if (text.Contains(" nick "))
                    {
                        var ss = text.Substring(text.IndexOf("nick") + 5);
                        // We'll need a check to make sure no one else was tagged
                        // Done
                        foreach (var usr in msg.MentionedUsers)
                        {
                            if (usr.Username == Client.CurrentUser.Username)
                            {
                                await (usr as IGuildUser).ModifyAsync(x => x.Nickname = ss);
                            }
                        }
                    }
                    else if (text.Contains(" load "))
                    {
                        var chr = text.Substring(text.IndexOf("load") + 5);
                        var name = Path.GetFileNameWithoutExtension(chr);
                        CurrentCharacter = name;
                        var man = (Directory.Exists(name + ".chr")) ? GetManifestByName(name) : GetManifestByName("Sayori");
                        await Personality.SetAvatar(man.Name + ".chr\\" + man.Files["_avatar"]);
                        var pdofile = man.Name + ".chr\\" + man.Files["_markov"];
                        if (File.Exists(pdofile))
                        {
                            var contents = File.ReadAllText(pdofile);
                            Generator.Chain = JsonConvert.DeserializeObject<Dictionary<string, Link>>(contents);
                        }
                        foreach (var usr in msg.MentionedUsers)
                        {
                            if (usr.Username == Client.CurrentUser.Username)
                            {
                                await (usr as IGuildUser).ModifyAsync(x => x.Nickname = man.Name);
                            }
                        }
                    }
                    else
                    {
                        if (ResponsesList.Count > 0) // If we have responses
                        {
                            var line = ResponsesList.RandomElement();
                            var response = line.Replace("[player]", msg.Author.Mention);
                            await msg.Channel.SendMessageAsync(response);
                        }
                        // var line = Lines.VoiceLines.RandomElement<string>(); TODO FIX THIS SHIT
                        // var response = line.Replace("[player]", msg.Author.Mention);
                        // await msg.Channel.SendMessageAsync(response);
                    }
                }
                else if (text.StartsWith("delete"))
                {
                    var character = text.Substring(7);
                    var response = character + ".chr deleted";
                    await msg.Channel.SendMessageAsync("os.remove(" + character + ".chr)\n");
                    await msg.Channel.SendMessageAsync(response);
                }
                else if (msg.Channel.Name.StartsWith("markov-"))
                {
                    Generator.AddToChain(text); // Record the message
                }
            }
        }
        public async Task Ready()
        {
            CurrentCharacter = Client.CurrentUser.Username;
            IsReady = true; // Tell Main() to continue
        }
        public async Task MainAsync()
        {

            Client.Log += Log;
            Client.MessageReceived += MessageReceived;
            Client.Ready += Ready;

            var TOKEN = Tokens.Development;

            await Client.LoginAsync(TokenType.Bot, TOKEN);
            await Client.StartAsync();

            // Removed delay
        }
    }

    public class TokenSet
    {
        public String Development { get; set; }
        public String Release { get; set; }

        public TokenSet(String dev, String release)
        {
            Development = dev;
            Release = release;
        }

        public TokenSet()
        {
            Development = String.Empty;
            Release = String.Empty;
        }
    }

    // Don't call any of these rapidly
    public static class Extensions
    {
        public static T RandomElement<T>(this List<T> lst)
        {
            Random randy = new Random();
            return lst[randy.Next(0, lst.Count)];
        }
        public static T RandomElement<T>(this T[] lst)
        {
            Random randy = new Random();
            return lst[randy.Next(0, lst.Length)];
        }
        public static Boolean OneIn(int i)
        {
            Random randy = new Random();
            return (randy.Next(0, i) == 1);
        }
    }

}
