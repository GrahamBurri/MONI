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
        private static string path = Directory.GetCurrentDirectory();
        private string currentEmotion = (path + "\\default.jpg");
        private Discord.WebSocket.DiscordSocketClient client;

        public EmotionManager(DiscordSocketClient c)
        {
            client = c;
        }
        public String getState()
        {
            return currentEmotion;
        }
        public Discord.Image getStateImage()
        {
            Discord.Image image = new Discord.Image(File.OpenWrite(currentEmotion));
            return image;
        }
        private String setStateImage(String path)
        {
            return path;
        }
        public void setState(String emotion)
        {
            currentEmotion = emotion;
        }
        public String processMessage(String inputMessage) // add some emotion to a message
        {
            return inputMessage;
        }
        public async Task updateAvatar()
        {
            
            var user = client.CurrentUser;
            await user.ModifyAsync(x => {
                x.Avatar = getStateImage();
            });
        }
    }
}
