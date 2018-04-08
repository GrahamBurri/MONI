using System;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Monika
{
    public class Character
    {
        public string Name { get; set; }
        public string Personality { get; set; }
        public string Avatar { get; set; }
        public Dictionary<string, string> Emotions { get; set; }
        public Dictionary<string, string> PdoFiles { get; set; }
        public List<string> Responses { get; set; }
    }

    public static class CharacterCreator
    {
        // Unzips the .chr and adds it to the Data folder, returning the deserialized character
        public static Character CreateCharacter(string charfilepath)
        {
            var name = Path.GetFileNameWithoutExtension(charfilepath);
            var destination = @".\Data\" + name;
            if (!Directory.Exists(@".\Data\" + name))
            {
                ZipFile.ExtractToDirectory(charfilepath, destination);
                var serfile = String.Format(@".\Data\{0}\{0}.pdo", name);
                var json = File.ReadAllText(serfile);
                return JsonConvert.DeserializeObject<Character>(json);
            }
            else
            {
                // throw new IOException("This character already exists locally");
                Console.WriteLine("Error: This character already exists locally");
                return new Character();
            }
        }

        public static Character Default
        {
            get
            {
                var character = new Character();
                character.Name = "Sayori";
                character.Emotions = new Dictionary<string, string>();
                character.Emotions.Add("default", "default.jpg");
                character.PdoFiles = new Dictionary<string, string>();
                character.PdoFiles.Add("default", "markov.pdo");
                character.Avatar = character.Emotions["default"];
                character.Personality = character.Emotions["default"];
                return character;
            }
        }
    }
}
