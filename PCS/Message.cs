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

        public string GetData()
        {
            return Author.GetData() + Flags.EndOfText + Text;
        }
    }
}
