using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkovNextGen;
using System.IO;
using Newtonsoft.Json;

namespace MarkovConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var markov = new Markov();
            //var markov = new MarkovNextGen();
            Console.Write("markov> ");
            var cmd = Console.ReadLine();
            while (cmd != "exit")
            {
                if (cmd.StartsWith("train"))
                {
                    var rest = cmd.Substring(6);
                    if (File.Exists(rest))
                    {
                        var chain = new Dictionary<string, MarkovNextGen.Link>();
                        // var chain = new Dictionary<string, LinkNextGen>();
                        foreach (var line in File.ReadAllLines(rest))
                        {
                            // This way it only gets serialized once
                            chain = MarkovUtilities.Train(line);
                            markov.AddToChain(chain);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Target file does not exist");
                        Console.WriteLine("Resol: Ignoring invalid train request");
                        Console.WriteLine(Environment.NewLine);
                    }
                }
                else if (cmd.StartsWith("set"))
                {
                    var rest = cmd.Substring(4);
                    if (File.Exists(rest))
                    {
                        markov = new Markov(rest);
                        //markov = new MarkovNextGen(rest);
                    }
                    else
                    {
                        File.WriteAllText(rest, "{}");
                        markov = new Markov(rest);
                        // markov = new MarkovNextGen(rest);
                        Console.WriteLine("_Warn: Target file does not exist");
                        Console.WriteLine("Resol: Creating target PDO");
                        Console.WriteLine(Environment.NewLine);
                    }
                }
                else if (cmd.StartsWith("gen"))
                {
                    if (cmd == "gen")                               // Automatic length and random word
                    {
                        Console.WriteLine(markov.Generate());
                    }
                    else
                    {
                        var rest = cmd.Substring(4);
                        if (Int32.TryParse(rest, out int length))   // Length and random word
                        {
                            Console.WriteLine(markov.Generate(length));
                        }
                        else
                        {
                            if (!rest.Contains(" "))                // Automatic length and specific word
                            {
                                Console.WriteLine(markov.Generate(rest));
                            }
                            else                                    // Automatic length and random word
                            {
                                var arguments = rest.Split(' ');
                                if (Int32.TryParse(arguments[0], out int length2))
                                {
                                    Console.WriteLine(markov.Generate(length2, arguments[1]));
                                }
                                else
                                {
                                    Console.WriteLine("Error: Specified length was not an integer");
                                    Console.WriteLine("Resol: Ignoring invalid gen request");
                                    Console.WriteLine(Environment.NewLine);
                                }
                            }
                        }
                    }
                }
                else if (cmd.StartsWith("merge"))
                {
                    var rest = cmd.Substring(6);
                    var targets = rest.Split(' ');
                    if (targets.Length == 2)        // Merge two files
                    {
                        // File existence check is implemented in Merge methods
                        MarkovUtilities.MergeTo(targets[0], targets[1]);
                        //MarkovUtility.MergeTo(targets[0], targets[1]);
                    }
                    else if (targets.Length == 3)   // Merge two files into third
                    {
                        // File existence check implemented in Merge methods
                        // Additional target check implemented here
                        if (!File.Exists(targets[2]))
                        {
                            var merged = MarkovUtilities.MergeFrom(targets[0], targets[1]);
                            //var merged = MarkovUtility.MergeFrom(targets[0], targets[1]);
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
                }
                else if (cmd.StartsWith("load"))
                {
                    // Merges and serialzies PDO file into current chain
                    var rest = cmd.Substring(5);
                    if (File.Exists(rest))
                    {
                        var jsonfrom = File.ReadAllText(rest);
                        var from = JsonConvert.DeserializeObject<Dictionary<string, Link>>(jsonfrom);
                        //var from = JsonConvert.DeserializeObject<Dictionary<string, LinkNextGen>>(jsonfrom);
                        markov.AddToChain(from);
                    }
                    else
                    {
                        Console.WriteLine("Error: Target file does not exist");
                        Console.WriteLine("Resol: Ignoring invalid load request");
                        Console.WriteLine(Environment.NewLine);
                    }
                }
                Console.Write("markov> ");
                cmd = Console.ReadLine();
            }
        }
    }
}
