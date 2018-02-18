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

        public void PrintChain()
        {
            foreach (KeyValuePair<string, LinkNextGen> kvp in Chain)
            {
                Console.WriteLine(kvp.Key);
                Console.WriteLine("\t" + kvp.Value);
            }
        }

        public String Generate(int length, string start)
        {
            var keys = Chain.Keys.ToList();
            var _word = start;
            string genChain = _word;
            // Start at 1 since genChain already contains _word
            for (int i = 1; i < length; i++)    // Don't actuall need i
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

        public String Generate(int length) // Generate based on # of words and random starting point
        {
            Random randy = new Random();
            var keys = Chain.Keys.ToList();
            var _word = keys[randy.Next(0, keys.Count)];
            return Generate(length, _word);
        }

        // TODO add generator based on starting word input & number of sentences (with natural length through detecting punctuation)

    }
}