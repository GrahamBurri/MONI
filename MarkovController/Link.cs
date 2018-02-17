using System;
using System.Collections.Generic;
using System.Text;

namespace MarkovController
{
    public class Link
    {
        public String word;
        public List<String> before = new List<String>();
        public List<String> after = new List<String>();
        public int frequency;

        public Link(String w)
        {
            // Pass NULL for beginning or end of file (no words before/after)
            word = w;
        }
        public Link(String w, String b, String a)
        {
            // Pass NULL for beginning or end of file (no words before/after)
            word = w;
            before.Add(b);
            after.Add(a);
        }
        public String getWord()
        {
            return word;
        }
        public void addBefore(String b)
        {
            before.Add(b);
        }
        public void addAfter(String a)
        {
            before.Add(a);
        }
        public void addBefore(List<String> b)
        {
            for(int i = 0; i < b.Count; i++)
            {
                before.Add(b[i]);
            }
        }
        public void addAfter(List<String> a)
        {
            for (int i = 0; i < a.Count; i++)
            {
                after.Add(a[i]);
            }
        }
        public String getRandomBefore()
        {
            Random randy = new Random();
            return before[randy.Next(0, before.Count)];
        }
        public String getRandomAfter()
        {
            Random randy = new Random();
            return after[randy.Next(0, after.Count)];
        }
    }
}
