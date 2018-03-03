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

namespace Monika.AdminController
{
    public class AdminConsole
    {
        public DiscordSocketClient Client { get; set; }
        public MarkovNextGen Generator { get; set; }
        public EmotionManager Manager { get; set; }

        public String ParseCommand(string cmd)
        {
            if (cmd.StartsWith("markov"))
            {
                var rest = cmd.Substring(7);
                if (Int32.TryParse(rest, out int length))
                {
                    return Generator.Generate(length);
                }
                else
                {
                    return "Usage is as follows:  `markov $length` where $length is an integer";
                }
            }
            else if (cmd.StartsWith("avatar"))
            {
                var rest = cmd.Substring(7);
                if (File.Exists(rest))
                {
                    Manager.Emotion = rest;
                    Manager.UpdateAvatar().GetAwaiter().GetResult();
                    return "Avatar Updated:  " + rest;
                }
                else
                {
                    return "File does not exist:  " + rest;
                }
                
            }
            else if (cmd.StartsWith("cleanse"))
            {
                return "Implement this later";
            }
            else if (cmd.StartsWith("load"))
            {
                return "Implement this later";
            }
            else
            {
                return "Invalid Command " + cmd;
            }
        }
    }
}
