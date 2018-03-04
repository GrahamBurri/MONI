using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace MarkovGenerator
{
    public class MarkovNextGen
    {
        private Random randy = new Random();
        private string filename;

        public Dictionary<string, LinkNextGen> Chain { get; private set; }

        public MarkovNextGen(string _filename) // You can provide a custom filename
        {
            filename = _filename;
            Chain = this.ReadChain();

        }
        public MarkovNextGen()
        {
            filename = "markov.pdo";
            Chain = this.ReadChain();
        }

        public Dictionary<string, LinkNextGen> ReadChain()
        {
            if (File.Exists(filename))
            {
                var contents = File.ReadAllText(filename);
                return JsonConvert.DeserializeObject<Dictionary<string, LinkNextGen>>(contents);
            }
            else
            {
                Console.WriteLine("_Warn: No PDO detected");
                Console.WriteLine("Resol: Returning blank Dictionary<string, LinkNextGen>");
                Console.WriteLine(Environment.NewLine);
                return new Dictionary<string, LinkNextGen>();
            }
        }

        public void WriteChain()
        {
            var contents = JsonConvert.SerializeObject(Chain, Formatting.Indented);
            File.WriteAllText(filename, contents);
        }

        // Adds link to chain, serializes to PDO
        public void AddToChain(string word, IEnumerable<string> link)
        {
            if (!String.IsNullOrWhiteSpace(word))   // Ensure no empty entries
            {
                if (Chain.ContainsKey(word)) // Entry exists, merge
                {
                    Chain[word].AddAfter(link); // Add to existing link
                }
                else
                {
                    var lnk = new LinkNextGen(link); // Add new link
                    Chain.Add(word, lnk);
                }
            }
            WriteChain();   // Serialize and record
        }

        public void AddToChain(string s)    // Add full string to text to chain as links
        {
            // Consistent casing and treat linebreaks as spaces
            var lower = s.ToLower().Replace(Environment.NewLine, " ").Trim(' ');
            // Remove all characters other than letters and spaces
            var cleancopy = Regex.Replace(lower, "[^a-z ]+", "", RegexOptions.Compiled);
            var words = cleancopy.Split(' ');

            if (words.Length >= 2)  // Don't record single words  
            {
                for (int i = 0; i < words.Length - 1; i++)
                {
                    var key = words[i];             // The word
                    var value = words[i + 1];       // Next word
                    var after = new List<string>();
                    after.Add(value);
                    AddToChain(key, after);         // Add link to chain
                }
            }
        }

        public void AddToChain(Dictionary<string, LinkNextGen> from)    // Merge with current chain
        {
            Chain = MarkovUtility.Merge(from, Chain);
            WriteChain();
        }

        public void PrintChain()
        {
            foreach (KeyValuePair<string, LinkNextGen> kvp in Chain)
            {
                Console.WriteLine(kvp.Key);
                Console.WriteLine("\t" + kvp.Value);
            }
        }
        
        // Generate with fixed length and predefined starting word
        public String Generate(int length, string start)
        {
            var keys = Chain.Keys.ToList();
            var _word = start;
            string genChain = _word;
            // Start at 1 since genChain already contains _word
            for (int i = 1; i < length; i++)    // Don't actually need i
            {
                if (Chain.ContainsKey(_word))   // Entry exists for _word
                {
                    if (Chain[_word].After.Count > 0)   // This should never return false
                    {
                        _word = Chain[_word].RandomAfter();
                        genChain += " " + _word;
                    }
                }
                else    // No entry for _word, pick a new starting word
                {
                    _word = keys[randy.Next(0, keys.Count)];
                    genChain += ", " + _word;   // Add a comma since it's a new starting point
                }
            }
            return genChain;
        }

        // Generate with fixed length and random starting word
        public String Generate(int length) // Generate based on # of words and random starting point
        {
            var keys = Chain.Keys.ToList();
            var _word = keys[randy.Next(0, keys.Count)];
            return Generate(length, _word);
        }

        // Generate with automatic length and predefined starting word
        public String Generate(string start)
        {
            var keys = Chain.Keys.ToList();
            var _word = start;
            string genChain = _word;
            var i = 0;  // Break at 50 so we don't get infinite loops
            while (Chain.ContainsKey(_word) && i < 50)
            {
                if (Chain[_word].After.Count > 0)   // Otherwise empty entry
                {
                    _word = Chain[_word].RandomAfter();
                    genChain += " " + _word;
                }
                i++;
            }
            return genChain;
        }

        // Generate with automatic length and random starting word
        public String Generate()
        {
            var keys = Chain.Keys.ToList();
            var _word = keys[randy.Next(0, keys.Count)];
            return Generate(_word);
        }

        // TODO add generator based on starting word input & number of sentences (with natural length through detecting punctuation)

    }

    public static class MarkovUtility
    {
        // Non-persistent
        public static Dictionary<string, LinkNextGen> Merge(Dictionary<string, LinkNextGen> from, Dictionary<string, LinkNextGen> target)
        {
            foreach (KeyValuePair<string, LinkNextGen> kvp in from)
            {
                if (target.ContainsKey(kvp.Key))    // Entry exists in target, merge
                {
                    target[kvp.Key].AddAfter(kvp.Value.After);
                }
                else                                // Entry doesn't exist, create one
                {
                    var _link = new LinkNextGen(kvp.Value.After);
                    target.Add(kvp.Key, _link);
                }
            }
            return new Dictionary<string, LinkNextGen>(target);
        }

        // Non-persistent
        public static Dictionary<string, LinkNextGen> MergeFrom(string from, string target) // Merges two pdo files into a new chain
        {
            if (File.Exists(from) && File.Exists(target))
            {
                var jsonfrom = File.ReadAllText(from);
                var jsontarget = File.ReadAllText(target);

                var _from = JsonConvert.DeserializeObject<Dictionary<string, LinkNextGen>>(jsonfrom);
                var _target = JsonConvert.DeserializeObject<Dictionary<string, LinkNextGen>>(jsontarget);

                return Merge(_from, _target);
            }
            else
            {
                Console.WriteLine("Error: One or more of the target files does not exist");
                Console.WriteLine("Resol: Returning blank Dictionary<string, LinkNextGen>");
                Console.WriteLine(Environment.NewLine);
                return new Dictionary<string, LinkNextGen>();
            }
        }

        // Persistent merge
        public static void MergeTo(string from, string target) // Merges one pdo file into another
        {
            if (File.Exists(from) && File.Exists(target))
            {
                var merged = MergeFrom(from, target);
                var jsontarget = JsonConvert.SerializeObject(merged, Formatting.Indented);
                File.WriteAllText(target, jsontarget);
            }
            else
            {
                Console.WriteLine("Error: One or more of the target files does not exist.");
                Console.WriteLine("Resol: Ignoring invalid MergeTo request");
                Console.WriteLine(Environment.NewLine);
            }
        }

        // Static add-to-chain to avoid rapidly serializing/deserializing during training
        public static Dictionary<string, LinkNextGen> LinkToChain(Dictionary<string, LinkNextGen> chain, string word, IEnumerable<string> link)
        {
            var _chain = new Dictionary<string, LinkNextGen>(chain);
            if (!String.IsNullOrWhiteSpace(word))
            {
                if (_chain.ContainsKey(word))
                {
                    _chain[word].AddAfter(link);
                }
                else
                {
                    var lnk = new LinkNextGen(link);
                    _chain.Add(word, lnk);
                }
            }
            return _chain;
        }

        public static Dictionary<string, LinkNextGen> LinkToChain(string s)
        {
            // Consistent casing and treat linebreaks as spaces
            var lower = s.ToLower().Replace(Environment.NewLine, " ").Trim(' ');
            // Remove all characters other than letters and spaces
            var cleancopy = Regex.Replace(lower, "[^a-z ]+", "", RegexOptions.Compiled);
            var words = cleancopy.Split(' ');
            var chain = new Dictionary<string, LinkNextGen>();
            if (words.Length >= 2)  // Don't record single words  
            {
                for (int i = 0; i < words.Length - 1; i++)
                {
                    var key = words[i];             // The word
                    var value = words[i + 1];       // Next word
                    var after = new List<string>();
                    after.Add(value);
                    chain = LinkToChain(chain, key, after);         // Add link to chain
                }
            }
            return chain;
        }
    }
}