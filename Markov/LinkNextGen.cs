using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkovGenerator
{
    public class LinkNextGen // Basically a glorified List<string>
    {
        private Random randy = new Random();

        public List<string> After { get; set; }

        public LinkNextGen()
        {
            After = new List<string>();
        }

        public LinkNextGen(IEnumerable<string> after)
        {
            After = new List<string>(after);
        }

        public void AddAfter(string s)
        {
            After.Add(s);
        }

        public void AddAfter(IEnumerable<string> lst)
        {
            foreach (var s in lst)
            {
                this.AddAfter(s);
            }
        }

        public string RandomAfter()
        {
            if (After.Count > 0)
            {
                var i = randy.Next(0, After.Count);
                return After[i];
            }
            else // Empty list
            {
                return String.Empty;
            }
        }
    }
}
