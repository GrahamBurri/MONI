using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkovNextGen;
using Discord;
using Discord.WebSocket;
using Monika.Emotions;
using System.IO;
using Newtonsoft.Json;

namespace Monika.AdminController
{
    public class AdminConsole
    {
        public DiscordSocketClient Client { get; set; }
        public Markov Generator { get; set; }
        public EmotionManager Manager { get; set; }

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
                    Console.WriteLine("Resol: Ignoring invalid markov request");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            else if (cmd.StartsWith("avatar"))
            {
                var rest = cmd.Substring(7);
                if (File.Exists(rest))
                {
                    Manager.Emotion = rest;
                    Manager.UpdateAvatar().GetAwaiter().GetResult();
                    Console.WriteLine("Avatar Updated: " + rest);
                }
                else
                {
                    Console.WriteLine("Error: Avatar file does not exist");
                    Console.WriteLine("Resol: Ignoring invalid avatar update request");
                    Console.WriteLine(Environment.NewLine);
                }
                
            }
            else if (cmd.StartsWith("cleanse"))
            {
                // Honestly I'm not sure what you want to do with cleanse
                Console.WriteLine("_Warn: CLEANSE Request not yet implemented");
                Console.WriteLine("Resol: Ignoring invalid request");
                Console.WriteLine(Environment.NewLine);
            }
            else if (cmd.StartsWith("load"))
            {
                var rest = cmd.Substring(5);
                if (File.Exists(rest))
                {
                    var jsonfrom = File.ReadAllText(rest);
                    var from = JsonConvert.DeserializeObject<Dictionary<string, Link>>(jsonfrom);
                    Generator.AddToChain(from);
                }
                else
                {
                    Console.WriteLine("Error: Target file does not exist");
                    Console.WriteLine("Resol: Ignoring invalid load request");
                    Console.WriteLine(Environment.NewLine);
                }
            }
            else
            {
                Console.WriteLine("Error: Unknown request");
                Console.WriteLine("Resol: Ignoring unknown request");
                Console.WriteLine(Environment.NewLine);
            }
        }
    }
}
