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
        static Monika.MonikaBot monikaBot;
        static MarkovNextGen markov;
        static void Main(string[] args)
        {
            
            monikaBot = Monika.InterConsole.getBot();
            // This is passing a null reference to the bot- why?

            printMenu();
        }
        public static async void printMenu()
        {
            markov = new MarkovNextGen();
            Console.WriteLine("What would you like to do?\n1. Generate a markov chain\n2. Change avatar\n3. Change nickname\n4. Cleanse PDO\n5. Say something\n6. Load preset PDO");
            String keyInput = Console.ReadLine();
            switch (keyInput)
            {
                default:

                    break;

                case "1":
                    Console.WriteLine("How long should the chain be?");
                    int chainLength = Convert.ToInt32(Console.ReadLine());
                    if (chainLength == Convert.ToInt32(chainLength))
                    {
                        Console.WriteLine(markov.Generate(chainLength));
                        printMenu();
                    }
                    else
                    {
                        Console.WriteLine("Enter a valid Int32 numerical value!");
                        printMenu();
                    }
                    break;

                case "2":
                    // TODO figure out how the fuck the async calls work
                    Console.WriteLine("Enter an image filename:");
                    String input = Directory.GetCurrentDirectory() + Console.ReadLine();
                    await monikaBot.ChangeAvatar(input);
                    printMenu();
                    break;

                case "3":
                    // TODO figure out how to change this
                    printMenu();
                    break;

                case "4":
                    // TODO add code to cleanse the PDO file
                    printMenu();
                    break;

                case "5":
                    //String message = Console.ReadLine();
                    //monikaBot.SendMessage(message);
                    printMenu();
                    break;
            }
        }
    }
}
