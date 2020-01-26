using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS
{
    // In another approach, this class could be not be static, and contain a type "DataType" and easily get data with object function, but that's less clear
    // TODO: DO IT! It avoids doing 4 times the same algorithm!
    internal static class DataPacket
    {
        public static string GetDataPacket(this Message message, bool knownAuthor) // Of a Message
        {
            if (!knownAuthor) 
                return message.ChannelTitle + Flags.EndOfText + 
                    message.Author.GetDataPacket() + Flags.EndOfText + 
                    message.Text;
            else 
                return message.ChannelTitle + Flags.EndOfText +
                    message.Text;
        }

        public static string GetDataPacket(this Member member) // Of a Member
        {
            return member.Username + Flags.EndOfText + member.ID;
        }

        public static Member TryGetSignedInMember(string textData)
        {
            string[] infos = Split(textData);
            string header = infos[0];

            if (header != Flags.ClientSigningIn)
                return null; // Failed

            string username = infos[1];
            int id = Convert.ToInt32(infos[2], CultureInfo.CurrentCulture);

            return new Member(username, id);
        }
        
        public static Message TryGetClientMessage(string textData)
        {
            string[] infos = Split(textData);

            string header = infos[0];

            if (header != Flags.ClientMessage)
                return null; // Failed

            string channelTitle = infos[1];
            string text = infos[2];

            return new Message(text, channelTitle, DateTime.Now);
        }

        public static Message TryGetMessage(string textData)
        {
            string[] infos = Split(textData);

            string header = infos[0];

            if (header != Flags.ServerMessage)
                return null; // Failed

            string channelTitle = infos[1];
            string username = infos[2];
            int id = Convert.ToInt32(infos[3], CultureInfo.CurrentCulture);
            string text = infos[4];

            var author = new Member(username, id); // TODO use member from text data

            return new Message(text, channelTitle, DateTime.Now, author);
        }

        public static bool WishDisconnect(string textData)
        {
            var infos = Split(textData);

            string header = infos[0];

            if (header == Flags.ClientDisconnection)
                return true;
            else return false;
        }

        private static string[] Split(string textData)
        {
            return textData.Split(new char[] { Flags.EndOfText, Flags.EndOfTransmission },
                StringSplitOptions.None);
        }
    }
}
