using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkovGenerator
{
    public class Link
    {
        private Random randy = new Random();

        public List<string> Before { get; set; } = new List<string>();
        public List<string> After { get; set; } = new List<string>();
        public string Word { get; set; }
        public int Frequency { get; set; }

        public Link()
        {
            Word = String.Empty;
            Frequency = 0;
        }

        public Link(string _word) // Beginning or end of file
        {
            Word = _word;
            Frequency = 0;
        }
        public Link(string _word, string _before, string _after)
        {
            Word = _word;
            Before.Add(_before);
            After.Add(_after);
            Frequency = 0;
        }

        public void AddBefore(string s)
        {
            Before.Add(s);
        }

        public void AddBefore(IEnumerable<string> lst)
        {
            foreach (string s in lst)
            {
                Before.Add(s);
            }
        }

        public void AddAfter(string s)
        {
            After.Add(s);
        }

        public void AddAfter(IEnumerable<string> lst)
        {
            foreach (string s in lst)
            {
                After.Add(s);
            }
        }

        public String RandomBefore()
        {
            try
            {
                var i = randy.Next(0, After.Count);
                return After[i];
            }
            catch // Empty list
            {
                return "~empty";
            }
        }

        public String RandomAfter()
        {
            try
            {
                var i = randy.Next(0, After.Count);
                return After[i];
            }
            catch
            {
                return "~empty";
            }
        }
    }
}
