using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS
{
    static class DataPacketAttributes
    {
        public static string[] FromMessage(Message message)
        {
            return new string[] {
                 message.ChannelName,
                 message.Author.Username,
                 message.Author.ID.ToString(CultureInfo.CurrentCulture),
                 message.DateTime.ToFileTime().ToString(CultureInfo.CurrentCulture),
                 message.Text
            };
        }

        public static string[] FromMemberSignIn(Member member)
        {
            return new string[] {
                member.Username,
                member.ID.ToString(CultureInfo.CurrentCulture) };
        }
    }
}
