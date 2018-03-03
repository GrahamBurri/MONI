using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Monika.Emotions
{
    public class EmotionManager
    {
        public String Emotion { get; set; } = "default.jpg";
        public DiscordSocketClient Client { private get; set; }

        public EmotionManager(DiscordSocketClient c)
        {
            Client = c;
        }
        public static String ProcessMessage(String inputMessage) // add some emotion to a message
        {
            // Should return an emotion filename
            return inputMessage;
        }

        public async Task UpdateAvatar()
        {
            var user = Client.CurrentUser;
            if (File.Exists(Emotion))
            {
                using (var fs = File.OpenRead(Emotion))
                {
                    var img = new Discord.Image(fs);
                    await user.ModifyAsync(x =>
                    {
                        x.Avatar = img;
                    });
                }
            }
        }

        // Overload so they can set avatar in one call
        public async Task UpdateAvatar(string emotion)
        {
            Emotion = emotion;
            await UpdateAvatar();
        }
    }
}
