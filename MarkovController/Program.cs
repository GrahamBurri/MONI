using System;
using MarkovController;

namespace MarkovController
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Markov.buildChain();
            Markov.purgeDuplicates();
            Markov.printChain();
            Console.WriteLine("-------------------------------");
            Console.WriteLine(Markov.generate(4));
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        }
    }
}
