using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MarkovGenerator;
using Newtonsoft.Json;
using BestGirl.Responses;

namespace Monika
{
    class Program
    {
        static void Main(string[] args)
        {
            var mkbot = new MonikaBot();
            mkbot.MainAsync().GetAwaiter().GetResult();
        }
    }

    public class MonikaBot
    {
        const string TOKFILE = "tokens.pdo";
        Random randy = new Random();
        MarkovNextGen generator = new MarkovNextGen();
        DiscordSocketClient client = new DiscordSocketClient();

        TokenSet Tokens
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

        public static Boolean TaggedIn(SocketMessage msg)
        {
            var _tagged = false;
            foreach (var usr in msg.MentionedUsers)
            {
                if (usr.Username == "Monika" || usr.Username == "Monika Dev")
                {
                    _tagged = true;
                }
            }
            return _tagged;
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

            if (author != "Monika" && author != "Monika Dev") // Don't process our own messages
            {
                if (TaggedIn(msg))
                {
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
                            var response = generator.Generate(i);
                            await msg.Channel.SendMessageAsync(response);
                        }
                    }
                    else
                    {
                        var line = Lines.VoiceLines.RandomElement<string>();
                        var response = line.Replace("[player]", msg.Author.Mention);
                        await msg.Channel.SendMessageAsync(response);
                    }
                }
                else if (text.StartsWith("delete"))
                {
                    var character = text.Substring(7);
                    var response = character + ".chr deleted";
                    await msg.Channel.SendMessageAsync(response);
                }
                else if (msg.Channel.Name.StartsWith("markov-")) 
                {
                    generator.AddToChain(text); // Record the message
                }
            }
        }

        public async Task ChangeAvatar(string filename)
        {
            var me = client.CurrentUser;
            using (FileStream fs = File.OpenRead(filename))
            {
                var avatar = new Image(fs);
                await me.ModifyAsync(x =>
                {
                    x.Avatar = avatar;
                });
            }
        }

        public async Task Ready()
        {
            // await ChangeAvatar("someavatar.jpg");
        }

        public async Task MainAsync()
        {

            client.Log += Log;
            client.MessageReceived += MessageReceived;
            client.Ready += Ready;

            var TOKEN = Tokens.Release;

            await client.LoginAsync(TokenType.Bot, TOKEN);
            await client.StartAsync();

            // Keep it running
            await Task.Delay(-1);
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
