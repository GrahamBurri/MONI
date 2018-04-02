using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monika.IdentityController
{
    public class Personality
    {

        public DiscordSocketClient Client { private get; set; }
        public Dictionary<String, String> EmotionDictionary { get; private set; }
        public Dictionary<String, String> ResponseDictionary { get; private set; }
        public String CurrentEmotion { get; private set; }
        private string CurrentDirectory
        {
            get
            {
                return Directory.GetCurrentDirectory();
            }
            set
            {
                Directory.SetCurrentDirectory(value);
            }
        }

        public Personality(DiscordSocketClient inputClient)
        {
            Client = inputClient;
        }
        public SocketMessage getResponse(SocketMessage msg) // Parse a message and create a response based on the personality
        {
            // if the found response includes [user] replace it with the sender's name
            // if msg is null, assume that this is not in response to a particular message and return a non [user] tagged message
            // If the response includes an [emotion] set the bot's emotion to the respective emotion if it exists
            return null;
        }
        public async Task SetAvatar(string avatar) // TODO update this to properly read files; move?
        {
            var user = Client.CurrentUser;
            if (File.Exists(avatar))
            {
                using (var fs = File.OpenRead(avatar))
                {
                    var img = new Discord.Image(fs);
                    await Task.Delay(21); // This should keep Discord from limiting the bot
                    await user.ModifyAsync(x =>
                    {
                        x.Avatar = img;
                    });
                }
            }
        }

    }

    public class Character
    {
        public string Name { get; set; }
        public string Personality { get; set; }
        public string Avatar { get; set; }
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
                var serfile = destination + String.Format(@"\{}.ini", name);
                var json = File.ReadAllText(serfile);
                return JsonConvert.DeserializeObject<Character>(json);
            }
            else
            {
                // throw new IOException("This character already exists locally");
                Console.WriteLine("Error: This character already exists locally");
                Console.WriteLine("Resol: Returning blank Character instance");
                return new Character();
            }
        }
    }

}
