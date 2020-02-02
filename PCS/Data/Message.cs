using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class Message
    {
        public string Text { get; set; }
        public string ChannelTitle { get; set; }
        public Member Author { get; set; }
        public DateTime DateTime { get; set; }
        public IEnumerable<string> AttachedFiles { get; set; }

        public Message(string text, string channelTitle, DateTime dateTime, Member author, IEnumerable<string> attachedFiles)
        {
            Text = text;
            ChannelTitle = channelTitle;
            
            Author = author;
            DateTime = dateTime;
            AttachedFiles = attachedFiles;
        }

        public override string ToString()
        {
            return $"Message from {Author} in {ChannelTitle} at {DateTime}: {Text}";
        }
    }
}
