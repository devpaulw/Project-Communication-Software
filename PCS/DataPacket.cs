using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS
{
    // In another approach, this class could be not be static, and contain a type "DataType" and easily get data with object function, but that's less clear
    internal static class DataPacket
    {
        public static Member TryGetSignedInMember(string textData)
        {
            string[] infos = Flags.Split(textData);

            string header = infos[0];

            if (header != Flags.SigningIn)
                return null; // Failed

            string username = infos[1];
            int id = Convert.ToInt32(infos[2], CultureInfo.CurrentCulture);

            return new Member(username, id);
        }

        public static Message TryGetClientMessage(string textData)
        {
            string[] infos = Flags.Split(textData);

            string header = infos[0];

            if (header != Flags.Text)
                return null; // Failed

            string channelTitle = infos[1];
            string text = infos[2];

            return new Message(text, channelTitle, DateTime.Now);
        }

        public static Message TryGetMessage(string textData)
        {
            string[] infos = Flags.Split(textData);

            string header = infos[0];

            if (header != Flags.Message)
                return null; // Failed

            string channelTitle = infos[1];
            string username = infos[2];
            int id = Convert.ToInt32(infos[3], CultureInfo.CurrentCulture);
            string text = infos[4];

            var author = new Member(username, id); // TODO use member from text data

            return new Message(text, channelTitle, DateTime.Now, author);
        }

        public static bool TryDisconnect(string textData)
        {
            var infos = Flags.Split(textData);

            if (infos[0] == Flags.Disconnection)
                return true;
            else return false;
        }
    }
}
