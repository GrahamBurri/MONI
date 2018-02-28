using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MarkovGenerator;

namespace AdminController
{
    public class Program
    {
        static void Main(string[] args)
        {
            MarkovNextGen markov = new MarkovNextGen();
            Console.WriteLine(markov.Generate(15));
            Console.ReadKey();
        }
        public void printMenu()
        {

        }
    }
}
