using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Monika
{
    public class Personality
    {
        public DiscordSocketClient Client { get; private set; }
        public String CurrentAvatar { get; private set; }

        public Personality(DiscordSocketClient inputClient)
        {
            Client = inputClient;
        }

        // Doesn't do anything yet
        public string GenerateResponse(SocketMessage msg)
        {
            var author = msg.Author.Username;
            // Create response based on msg.content
            return String.Empty;
        }

        public SocketMessage getResponse(SocketMessage msg) // Parse a message and create a response based on the personality
        {
            // if the found response includes [user] replace it with the sender's name
            // if msg is null, assume that this is not in response to a particular message and return a non [user] tagged message
            // If the response includes an [emotion] set the bot's emotion to the respective emotion if it exists
            return null;
        }
        public async Task SetAvatar(string path)
        {
            var user = Client.CurrentUser;
            if (File.Exists(path))
            {
                using (var fs = File.OpenRead(path))
                {
                    var img = new Discord.Image(fs);
                    await Task.Delay(21); // This should keep Discord from rate limiting the bot
                    await user.ModifyAsync(x =>
                    {
                        x.Avatar = img;
                    });
                }
                CurrentAvatar = Path.GetFileName(path);
            }
        }
    }
}
