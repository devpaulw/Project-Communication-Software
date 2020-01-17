using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class Message
    {
        public Member Author { get; }
        public string Text { get; }

        public Message(Member author, string text)
        {
            Author = author;
            Text = text;
        }

        public byte[] GetBytes() // TODO: Make an interface
        {
            return Encoding.UTF8.GetBytes(GetDataFlag() + '\0');
        }

        public string GetDataFlag()
        {
            return $"{Text};:!{Author.GetDataFlag()}";
        }
    }
}
