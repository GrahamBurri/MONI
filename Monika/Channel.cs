// Serializeable classes
// #GiveCSharpRecordTypes!
using System;

namespace Monika
{
    public class ChannelInfo
    {
        public string ServerName { get; set; }
        public string ChannelName { get; set; }
        public UInt64 Id { get; set; }

        public ChannelInfo()
        {
            // Do Nothing
        }

        public ChannelInfo(string server, string channel, UInt64 id)
        {
            ServerName = server;
            ChannelName = channel;
            Id = id;
        }
    }
}