using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PCS
{
    // In another approach, this class could be not be static, and contain a type "DataType" and easily get data with object function, but that's less clear
    // SDNMSG LAST: -> I DID IT!

    internal class DataPacket
    {
        private string[] m_attributes;

        public DataPacketType Type { get; }

        public DataPacket(string textData)
        {
            m_attributes = Split(textData);

            string flag = m_attributes[0];

            var type = Flags.GetDataPacketType(flag);
            if (type != null)
                Type = (DataPacketType)type;
            else // If the Data Packet has a wrong flag header
                throw new UnknownDataPacketException();
        }

        public Member GetSignedInMember()
        {
            //TODO An exception WrongDataPacket
            string username = m_attributes[1];
            int id = Convert.ToInt32(m_attributes[2], CultureInfo.CurrentCulture);

            return new Member(username, id);
        }

        public Message GetClientMessage()
        {
            string channelTitle = m_attributes[1];
            string text = m_attributes[2];

            return new Message(text, channelTitle);
        }

        public Message GetServerMessage()
        {
            string channelTitle = m_attributes[1];
            string username = m_attributes[2];
            int id = Convert.ToInt32(m_attributes[3], CultureInfo.CurrentCulture);
            var dateTime = DateTime.FromFileTime(Convert.ToInt64(m_attributes[4], CultureInfo.CurrentCulture));
            string text = m_attributes[5];

            var author = new Member(username, id); // TODO use member from text data

            return new Message(text, channelTitle, dateTime, author);
        }

        public static string FromMessage(Message message, bool knownAuthor) // Of a Message
        {
            if (knownAuthor)
                return CreateDataPacket(message.ChannelTitle,
                    message.Text);
            else
                return CreateDataPacket(message.ChannelTitle,
                    FromMember(message.Author),
                    message.DateTime.ToFileTime().ToString(CultureInfo.CurrentCulture),
                    message.Text);
        }

        public static string FromMember(Member member) // Of a Member
        {
            return CreateDataPacket(member.Username,
                member.ID.ToString(CultureInfo.CurrentCulture));
        }

        private static string CreateDataPacket(params string[] attributes)
        {
            string result = string.Empty;

            for (int i = 0; true; i++)
            {
                result += attributes[i];

                if (i == attributes.Length - 1)
                    break;

                result += Flags.EndOfText;
            }

            return result;
        }

        private static string[] Split(string textData)
        {
            return textData.Split(new char[] { Flags.EndOfText, Flags.EndOfTransmission },
                StringSplitOptions.None);
        }
    }
}
