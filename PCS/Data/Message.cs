using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PCS
{
    public class Message
    {
        public string Text { get; set; }
        public string ChannelName { get; set; }
        public Member Author { get; set; }
        public DateTime DateTime { get; set; }
        public List<Resource> AttachedResources { get; }

        public Message(string text, string channelTitle, DateTime dateTime, Member author, List<Resource> attachedFiles)
        {
            Text = text;
            ChannelName = channelTitle;
            Author = author;
            DateTime = dateTime;
            AttachedResources = attachedFiles;

            if (HasEmptyField) throw new MessageEmptyFieldException(Messages.Exceptions.MessageEmptyField);
        }

        public bool HasEmptyField
            => string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(ChannelName);

        public bool HasNoResource
            => AttachedResources == null || AttachedResources.Count == 0;

        public override string ToString()
        {
            string resources = string.Empty;
            if (AttachedResources != null)
                foreach (Resource attachedResource in AttachedResources)
                    resources += attachedResource.RemoteFileName + " ; ";

            return $"Message from {Author} in {ChannelName} at {DateTime}: {Text}" 
                + (HasNoResource ? "" : "\nResources: " + resources);
        }
    }
}
