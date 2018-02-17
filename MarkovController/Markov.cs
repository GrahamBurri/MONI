using System;
using System.IO;
using System.Collections.Generic;
using MarkovController;

public static class Markov // Welcome to the shitshow
{
    private static String currentDirectory = Directory.GetCurrentDirectory();
    public static List<Link> chain = new List<Link>();
    public static Boolean buildChain()
    {
        // TODO make this read words based on spaces and punctuation (IE "The Rain in Spain" is seperated to "The" "Rain" "In" "Spain)
        String[] lines = System.IO.File.ReadAllLines(currentDirectory + "/datapool.txt");
        for(int i = 0; i < lines.Length; i++)
        {
            Link chainLink = null;

            if(i != 0 && i != lines.Length - 1)
                chainLink = new Link(lines[i], lines[i - 1], lines[i + 1]);
            else if (i == 0 && i != lines.Length - 1)
                chainLink = new Link(lines[i], null, lines[i + 1]);
            else if (i != 0 && i == lines.Length - 1)
                chainLink = new Link(lines[i], lines[i - 1], null);
            if(chainLink != null)
                chain.Add(chainLink);
        }
        return true;
    }
    public static Boolean addToDataFile(String addString)
    {
        Console.WriteLine("Added " + addString + " to the datapool at line <line>"); // TODO add line function
        return true;
    }
    public static Boolean purgeDuplicates() // THE EMPEROR CLEANSES (this function will kill your pc)
    {
        List<Link> replacementList = new List<Link>();
        for(int i = 0; i < chain.Count; i++)
        {
            if(i == 0)
            {
                replacementList.Add(chain[i]);
            }
            else
            {
                for(int g = 0; g < replacementList.Count; g++)
                {
                    if(replacementList[g].word.Equals(chain[i].word))
                    {
                        replacementList[g].addBefore(chain[i].before);
                        replacementList[g].addAfter(chain[i].after);
                        replacementList[g].frequency++;
                    }
                    else
                    {
                        replacementList.Add(chain[i]);
                    }
                }
            }
        }
        chain.Clear();
        for(int f = 0; f < replacementList.Count; f++)
        {
            chain.Add(replacementList[f]);
        }
        return true;
    }
    public static void printChain()
    {
        for(int i = 0; i < chain.Count; i++)
        {
            Console.WriteLine(chain[i].word);
        }
    }
    public static String generate(int length) // Generate based on a predefined number of words and a random starting word
    {
        Random randy = new Random();
        String c = chain[randy.Next(0, chain.Count)].word;
        String genChain = "";
        for(int i = 0; i < length; i++)
        {
            for(int g = 0; g < length; g++)
            {
                foreach(Link ch in chain)
                {
                    if(ch.word.Equals(c))
                    {
                        genChain = genChain + " " + c;
                        c = ch.getRandomAfter();
                    }
                }
            }
        }
        return genChain;
    }
    // TODO add generator based on starting word input (with and without length parameter)

    // TODO add generator based on starting word input & number of sentences (with natural length through detecting punctuation)
}
