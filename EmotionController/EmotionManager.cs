using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace EmotionController
{
    public class EmotionManager
    {
        public String Emotion { get; set; }
        public Discord.WebSocket.DiscordSocketClient Client { private get; set; }

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
                await user.ModifyAsync(x => {
                    x.Avatar = Emotion.ToStateImage();
                });
            }
        }
    }

    public static class Extensions
    {
        // Need to check if file exists before call
        public static Discord.Image ToStateImage(this string s)
        {
            Discord.Image img;
            using (var fs = File.OpenRead(s))
            {
                img = new Discord.Image(fs);
            }
            return img;
        }
    }
}
