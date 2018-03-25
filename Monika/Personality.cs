using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monika.PersonalityController
{
    public class BotPersonality
    {

        public DiscordSocketClient Client { private get; set; }
        public Dictionary<String, String> emotionDictionary { get; private set; }
        public Dictionary<String, String> responseDictionary { get; private set; }

        public BotPersonality(DiscordSocketClient inputClient)
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
        public async Task SetAvatar(string avatar) // TODO update this to properly read files
        {
            var user = Client.CurrentUser;
            if (File.Exists(avatar))
            {
                using (var fs = File.OpenRead(avatar))
                {
                    var img = new Discord.Image(fs);
                    await user.ModifyAsync(x =>
                    {
                        x.Avatar = img;
                    });
                }
            }
        }
    }
}
