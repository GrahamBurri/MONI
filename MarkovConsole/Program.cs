using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarkovGenerator;
using System.IO;
using Newtonsoft.Json;

namespace MarkovConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var markov = new MarkovNextGen();
            Console.Write("markov> ");
            var cmd = Console.ReadLine();
            while (cmd != "exit")
            {
                if (cmd.StartsWith("train"))
                {
                    var rest = cmd.Substring(6);
                    if (File.Exists(rest))
                    {
                        foreach (var line in File.ReadAllLines(rest))
                        {
                            markov.AddToChain(line);
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
                        markov = new MarkovNextGen(rest);
                    }
                    else
                    {
                        markov = new MarkovNextGen(rest);
                        Console.WriteLine("_Warn: Target file does not exist");
                        Console.WriteLine("Resol: Continuing with set request");
                        Console.WriteLine(Environment.NewLine);
                    }
                }
                else if (cmd.StartsWith("gen"))
                {
                    var rest = cmd.Substring(4);
                    if (Int32.TryParse(rest, out int length))
                    {
                        Console.WriteLine(markov.Generate(length));
                    }
                    else
                    {
                        Console.WriteLine("Error: Provided length was not an integer");
                        Console.WriteLine("Resol: Ignoring invalid gen request");
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
