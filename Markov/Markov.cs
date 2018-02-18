using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace MarkovGenerator
{
    public class Markov
    {
        private string directory;
        private string filename;
        public List<Link> Chain { get; private set; }

        public Markov(string _filename) // You can provide a custom filename
        {
            directory = Directory.GetCurrentDirectory();
            filename = _filename;
            Chain = this.ReadChain();

        }
        public Markov()
        {
            directory = Directory.GetCurrentDirectory();
            filename = "markov.pdo";
            Chain = this.ReadChain();
        }

        public List<Link> ReadChain()
        {
            if (File.Exists(filename))
            {
                var contents = File.ReadAllText(filename);
                return JsonConvert.DeserializeObject<List<Link>>(contents);
            }
            else
            {
                return new List<Link>();
            }
        }

        public void WriteChain()
        {
            var contents = JsonConvert.SerializeObject(Chain, Formatting.Indented);
            File.WriteAllText(filename, contents);
        }

        public void AddToChain(Link link)   // Adds the link to the chain, then serializes to file
        {
            if (link.Word != String.Empty)  // Ensures no empty entries
            {
                var duplicates = Chain.Where(x => x.Word == link.Word).ToList();
                if (duplicates.Count > 0)   // If an entry exists, merge them
                {
                    var i = Chain.IndexOf(duplicates[0]);
                    Chain[i].AddBefore(link.Before);
                    Chain[i].AddAfter(link.After);
                }
                else                        // Otherwise create a new one
                {
                    Chain.Add(link);
                }
                WriteChain();               // Commit to pdo file
            }
        }

        public void AddToChain(string s)    // Add a full string of text to chain as links
        {
            var lower = s.ToLower();
            // Remove all characters other than letters and spaces
            var cleancopy = Regex.Replace(lower, "[^a-z ]+", "", RegexOptions.Compiled);
            var words = cleancopy.Split(' ');
            if (words.Length > 2)
            {
                var first = new Link(words.First());
                first.AddAfter(words[1]);
                AddToChain(first);
                for (int i = 1; i < words.Length - 1; i++)
                {
                    var link = new Link(words[i], words[i - 1], words[i + 1]);
                    AddToChain(link);
                }
                var last = new Link(words.Last());
                last.AddBefore(words[words.Length - 2]);
                AddToChain(last);
            }
            else if (words.Length == 2)
            {
                var first = new Link(words.First());
                first.AddAfter(words.Last());
                AddToChain(first);
            }
        }

        // Doesn't work but we don't need it anymore anyway
        /*
        public void Purge()
        {
            List<Link> flagged = new List<Link>();
            for (int i = 0; i < Chain.Count; i++)
            {
                var _word = Chain[i].Word;
                var copy = new List<Link>(Chain);
                copy.RemoveAt(i);
                List<Link> duplinks = copy.Where(x => x.Word == _word).ToList();
                foreach (Link duplicate in duplinks)
                {
                    flagged.Add(duplicate);
                    Chain[i].AddBefore(duplicate.Before);
                    Chain[i].AddAfter(duplicate.After);
                    Chain[i].Frequency++;
                }
            }

            foreach (Link garbage in flagged)
            {
                Chain.Remove(garbage);
            }
            WriteChain();
        }
        */

        public void PrintChain()
        {
            foreach (Link link in Chain)
            {
                Console.WriteLine(link.Word);
            }
        }

        public String Generate(int length) // Generate based on given number of words and random starting word
        {
            Random randy = new Random();
            string _word = Chain[randy.Next(0, Chain.Count)].Word;
            string genChain = String.Empty;
            for (int i = 0; i < length; i++) // You originally had two layers of this i think?
            {
                var links = Chain.Where(x => x.Word == _word).ToList();
                foreach (Link lnk in links)
                {
                    genChain += _word + " ";
                    _word = lnk.RandomAfter();
                }
            }
            return genChain;
        }
        // TODO add generator based on starting word input (with and without length parameter)

        // TODO add generator based on starting word input & number of sentences (with natural length through detecting punctuation)

    }
}