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
        public List<string> AttachedResources { get; }

        public Message(string text, string channelTitle, DateTime dateTime, Member author, List<string> attachedFiles)
        {
            Text = text;
            ChannelTitle = channelTitle;
            Author = author;
            DateTime = dateTime;
            AttachedResources = attachedFiles;

            if (HasEmptyField()) throw new MessageEmptyFieldException(Messages.Exceptions.MessageEmptyField);
        }

        public bool HasEmptyField()
            => string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(ChannelTitle);

        public bool HasNoResource()
            => AttachedResources == null || AttachedResources.Count == 0;

        public override string ToString()
        {
            string resources = string.Empty;
            if (AttachedResources != null)
                foreach (string attachedResource in AttachedResources)
                    resources += attachedResource + " ; ";

            return $"Message from {Author} in {ChannelTitle} at {DateTime}: {Text}" 
                + (HasNoResource() ? "" : "\nResources: " + resources);
        }
    }
}
