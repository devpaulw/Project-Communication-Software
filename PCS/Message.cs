using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class Message : IDataStream
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
            return PcsServerHost.Encoding.GetBytes(GetDataFlag() + (char)4);
        }

        public string GetDataFlag()
        {
            return $"{Text}{(char)3}{Author.GetDataFlag()}";
        }
    }
}
