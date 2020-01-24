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

        public static string TryGetText(string textData)
        {
            string[] infos = Flags.Split(textData);

            string header = infos[0];

            if (header != Flags.Text)
                return null; // Failed

            string text = infos[1];

            return text;
        }

        public static Message TryGetMessage(string textData)
        {
            string[] infos = Flags.Split(textData);

            string header = infos[0];

            if (header != Flags.Message)
                return null; // Failed

            string username = infos[1];
            int id = Convert.ToInt32(infos[2], CultureInfo.CurrentCulture);
            string text = infos[3];

            var author = new Member(username, id); // TODO use member from text data

            return new Message(author, text);
        }

        public static bool TryDisconnect(string textData)
        {
            if (textData == Flags.Disconnection)
                return true;
            else return false;
        }
    }
}
