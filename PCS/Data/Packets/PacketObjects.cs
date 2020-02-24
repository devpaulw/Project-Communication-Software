using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS
{
    static class PacketObjects
    {
        public static SignInPacket GetMemberSignIn(string[] attributes)
        {
            string username = attributes[0];
            int id = Convert.ToInt32(attributes[1], CultureInfo.CurrentCulture);

            return new SignInPacket(new Member(username, id));
        }

        public static BroadcastMessagePacket GetBroadcastMessage(string[] attributes)
        {
            string channelTitle = attributes[0];

            string username = attributes[1];
            int id = Convert.ToInt32(attributes[2], CultureInfo.CurrentCulture);
            var dateTime = DateTime.FromFileTime(Convert.ToInt64(attributes[3], CultureInfo.CurrentCulture));
            string text = attributes[4];

            var author = new Member(username, id);

            return new BroadcastMessagePacket(new BroadcastMessage(0, new Message(text, channelTitle), dateTime, author)); // TODO Supply
        }

        public static MessagePacket GetMessage(string[] attributes)
        {
            string channelTitle = attributes[0];
            string text = attributes[1];

            return new MessagePacket(new Message(text, channelTitle));
        }

        public static DisconnectPacket GetDisconnect() => 
            new DisconnectPacket();

        public static ResponsePacket GetResponse(string[] attributes)
        {
            ResponseCode responseCode = (ResponseCode)Convert.ToInt32(attributes[0], CultureInfo.CurrentCulture);
            return new ResponsePacket(responseCode);
        }
    }
}
