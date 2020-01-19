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
            return Text + Flags.EndOfText + Author.GetData();
        }

        public static Message FromTextData(string textData)
        {
            var infos = Flags.Split(textData);
            var author = new Member(infos[1], Convert.ToInt32(infos[2])); // TODO use member from text data

            return new Message(author, infos[0]);
        }
    }
}
