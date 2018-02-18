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

        public void AddToChain(Link link) // Adds the link to the chain, then serializes to file
        {
            if (link.Word != String.Empty)
            {
                var duplicates = Chain.Where(x => x.Word == link.Word).ToList();
                if (duplicates.Count > 0)
                {
                    var i = Chain.IndexOf(duplicates[0]);
                    Chain[i].AddBefore(link.Before);
                    Chain[i].AddAfter(link.After);
                }
                else
                {
                    Chain.Add(link);
                }
                WriteChain();
            }
        }

        public void AddToChain(string s)
        {
            var lower = s.ToLower();
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

        public void SlowPurge()
        {
            List<Link> flagged = new List<Link>();
            for (int i = 0; i < Chain.Count; i++)
            {
                foreach (Link duplink in Chain)
                {
                    // Ensure it's a duplicate (aka same word but not same index)
                    if ((Chain.IndexOf(duplink) != i) && duplink.Word == Chain[i].Word)
                    {
                        flagged.Add(duplink);
                        Chain[i].AddBefore(duplink.Before);
                        Chain[i].AddAfter(duplink.After);
                        Chain[i].Frequency++;
                    }
                }
            }

            foreach (Link garbage in flagged)
            {
                Chain.Remove(garbage);
            }
        }

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