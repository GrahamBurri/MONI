using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkovGenerator;
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
        public MarkovNextGen Generator { get; set; }
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
            else if (cmd.StartsWith("merge"))
            {
                var rest = cmd.Substring(6);
                var targets = rest.Split(' ');
                if (targets.Length == 2)        // Merge two files
                {
                    // File existence check is implemented in Merge methods
                    MarkovUtility.MergeTo(targets[0], targets[1]);
                }
                else if (targets.Length == 3)   // Merge two files into third
                {
                    // File existence check implemented in Merge methods
                    // Additional target check implemented here
                    if (!File.Exists(targets[2]))
                    {
                        var merged = MarkovUtility.MergeFrom(targets[0], targets[1]);
                        var jsonmerged = JsonConvert.SerializeObject(merged, Formatting.Indented);
                        File.WriteAllText(targets[2], jsonmerged);
                    }
                    else
                    {
                        Console.WriteLine("Error: Target file already exists");
                        Console.WriteLine("Resol: Ignoring merge request");
                        Console.WriteLine("_Note: If this is intentional, merge twice");
                        Console.WriteLine(Environment.NewLine);
                    }
                }
                else // You entered a weird number of filenames
                {
                    Console.WriteLine("Error: Invalid merge request");
                    Console.WriteLine("Resol: Ignoring invalid merge request");
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
                // This is temporary unless you call a method that writes to chain
                // For persistence just merge your file with markov.pdo
                var rest = cmd.Substring(5);
                if (File.Exists(rest))
                {
                    var jsonfrom = File.ReadAllText(rest);
                    var from = JsonConvert.DeserializeObject<Dictionary<string, LinkNextGen>>(jsonfrom);
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
