﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkovNextGen;
using Discord;
using Discord.WebSocket;
using Monika.IdentityController;
using System.IO;
using Newtonsoft.Json;

namespace Monika.AdminController
{
    public class AdminConsole
    {
        public DiscordSocketClient Client { get; set; }
        public Markov Generator { get; set; }
        public Personality Personality { get; set; }
        public List<ISocketMessageChannel> Channels { get; set; }
        private string CurrentDirectory
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
            set
            {
                Directory.SetCurrentDirectory(value);
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
                    Console.WriteLine("Resol: Ignoring invalid markov request");
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
                    Console.WriteLine("Resol: Ignoring invalid avatar update request");
                }

            }
            else if (cmd.StartsWith("cd"))
            {
                var rest = cmd.Substring(3);
                if(Directory.Exists(CurrentDirectory + rest))
                {
                    // Directory.SetCurrentDirectory(CurrentDirectory + rest);
                    // ^ Don't need that anymore, can just do the following:
                    CurrentDirectory += rest;
                    Console.WriteLine("Succesfully changed current directory to " + rest + ".");
                }
                else if(Directory.Exists(rest))
                {
                    // Directory.SetCurrentDirectory(rest);
                    CurrentDirectory = rest;
                    Console.WriteLine("Succesfully changed current directory to " + rest + ".");
                }
                else
                {
                    Console.WriteLine("Error: Path does not exist");
                    Console.WriteLine("Resol: Ignoring invalid cd request");
                }
            }
            else if (cmd.Equals("pwd"))
            {
                Console.WriteLine(CurrentDirectory);
            }
            else if (cmd.Equals("ls"))
            {
                foreach(String file in Directory.GetFiles(CurrentDirectory))
                {
                    Console.WriteLine(file);
                }
            }
            else if (cmd.StartsWith("say"))
            {
                var rest = cmd.Substring(4);
                for (int i = 0; i < Channels.Count, i++)
                {
                    Console.WriteLine(String.Format("{} :  {} {}", i, Channel.Name, Channel.Id));
                }
                Console.WriteLine("Which channel should the message be sent on?");
                Console.Write("Which channel ?> ");
                if (Int32.TryParse(Console.ReadLine(), out int index))
                {
                    var msgtask = Channels[index].SendMessageAsync(rest);
                    msgtask.GetAwaiter().GetResult(); // Don't return until the message is sent
                    Console.WriteLine("Message Sent");
                }
                else
                {
                    Console.WriteLine("Index out of range (or just shitty)");
                }

            }
            else if (cmd.StartsWith("load"))
            {
                var rest = cmd.Substring(5);
                if (File.Exists(CurrentDirectory + rest))
                {
                    var jsonfrom = File.ReadAllText(rest);
                    var from = JsonConvert.DeserializeObject<Dictionary<string, Link>>(jsonfrom);
                    Generator.AddToChain(from);
                }
                else
                {
                    Console.WriteLine("Error: Target file does not exist");
                    Console.WriteLine("Resol: Ignoring invalid load request");
                }
            }
            else
            {
                Console.WriteLine("Error: Unknown request");
                Console.WriteLine("Resol: Ignoring unknown request");
            }
        }
    }

}
