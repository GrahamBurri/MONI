using System;
using System.Collections.Generic;
using MarkovNextGen;
using Discord.WebSocket;
using System.IO;
using Newtonsoft.Json;

namespace Monika.AdminController
{
    public class AdminConsole
    {
        public DiscordSocketClient Client { get; set; }
        public Markov Generator { get; set; }
        public Personality Personality { get; set; }
        public List<ChannelInfo> Channels
        {
            get
            {
                var contents = File.ReadAllText("channels.pdo");
                return JsonConvert.DeserializeObject<List<ChannelInfo>>(contents);
            }
        }

        public void ParseCommand(string cmd)
        {
            if (cmd.StartsWith("markov"))
            {
                var rest = cmd.Substring(7);
                if (Int32.TryParse(rest, out int length))
                {
                    Console.WriteLine(Generator.Generate(length));
                }
                else
                {
                    Console.WriteLine("Error: Invalid length");
                }
            }
            else if (cmd.StartsWith("avatar")) // TODO protect against bad input
            {
                var rest = cmd.Substring(7);
                if (File.Exists(rest))
                {
                    Personality.SetAvatar(rest).GetAwaiter().GetResult();
                    Console.WriteLine("Avatar Updated: " + rest);
                }
                else
                {
                    Console.WriteLine("Error: Avatar file does not exist");
                }

            }
            else if (cmd.Equals("ls"))
            {
                // Should list characters
            }
            else if (cmd.StartsWith("bundle"))
            {
                var rest = cmd.Substring(7);
                var contents = File.ReadAllText(rest);
                var manifest = JsonConvert.DeserializeObject<ManifestNextGen>(contents);
                Directory.CreateDirectory(manifest.Name + ".chr");
                foreach (KeyValuePair<string, string> kvp in manifest.Files)
                {
                    File.Copy(kvp.Value, manifest.Name + ".chr\\" + kvp.Value);
                }
                File.Copy(rest, manifest.Name + ".chr\\" + rest);
            }
            else if (cmd.StartsWith("say"))
            {
                var rest = cmd.Substring(4);
                Console.WriteLine("Channels :");
                for (int i = 0; i < Channels.Count; i++)
                {
                    Console.WriteLine(String.Format("\t{0} : '{1}' In '{2}' : {3}", i, Channels[i].ChannelName, Channels[i].ServerName, Channels[i].Id));
                }
                Console.Write(Environment.NewLine + "Which channel ?> ");
                if (Int32.TryParse(Console.ReadLine(), out int index))
                {
                    var _channel = Client.GetChannel(Channels[index].Id) as ISocketMessageChannel;
                    var msgtask = _channel.SendMessageAsync(rest);
                    msgtask.GetAwaiter().GetResult(); // Don't return until the message is sent
                    Console.WriteLine("Message Sent");
                }
                else
                {
                    Console.WriteLine("Error: Index out of range (or just shitty)");
                }
            }
            else if (cmd.StartsWith("load"))
            {
                var rest = cmd.Substring(5);
                if (File.Exists(rest))
                {
                    var jsonfrom = File.ReadAllText(rest);
                    Generator.Chain = JsonConvert.DeserializeObject<Dictionary<string, Link>>(jsonfrom);
                }
                else
                {
                    Console.WriteLine("Error: Target file does not exist");
                }
            }
            else
            {
                Console.WriteLine("Error: Unknown request");
            }
        }
    }

}
