using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PCS
{
    internal class DataPacket
    {
        private readonly string[] m_attributes;

        public DataPacketType Type { get; }

        public DataPacket(string textData)
        {
            var attributes = Split(textData);

            #region Getting type
            string headerFlag = attributes[0];

            var type = Flags.GetDataPacketType(headerFlag);
            if (type != null)
                Type = (DataPacketType)type;
            else // If the Data Packet has a wrong flag header
                throw new DataPacketException(Messages.Exceptions.NotRecognizedDataPacket);
            #endregion

            m_attributes = attributes.Skip(1).Take(attributes.Length - 1).ToArray(); // Delete the flagHeader
        }

        public DataPacket(string textData, DataPacketType type)
        {
            Type = type;
            m_attributes = Split(textData);
        }

        public Member GetMember()
        {
            string username = m_attributes[0];
            int id = Convert.ToInt32(m_attributes[1], CultureInfo.CurrentCulture);

            return new Member(username, id);
        }

        public ClientMessage GetClientMessage()
        {
            string channelTitle = m_attributes[0];
            string text = m_attributes[1];

            return new ClientMessage(text, channelTitle);
        }

        public ServerMessage GetServerMessage()
        {
            string channelTitle = m_attributes[0];

            string username = m_attributes[1];
            int id = Convert.ToInt32(m_attributes[2], CultureInfo.CurrentCulture);
            var dateTime = DateTime.FromFileTime(Convert.ToInt64(m_attributes[3], CultureInfo.CurrentCulture));
            string text = m_attributes[4];

            var author = new Member(username, id);

            return new ServerMessage(text, channelTitle, dateTime, author);
        }

        public static string FromClientMessage(ClientMessage message)
        {
            return CreateDataPacket(message.ChannelTitle,
                message.Text);
        }

        public static string FromServerMessage(ServerMessage message)
        {
            return CreateDataPacket(message.ChannelTitle,
                 message.Author.Username,
                 message.Author.ID.ToString(CultureInfo.CurrentCulture),
                 message.DateTime.ToFileTime().ToString(CultureInfo.CurrentCulture),
                 message.Text);
        }

        public static string FromMember(Member member)
        {
            return CreateDataPacket(member.Username,
                member.ID.ToString(CultureInfo.CurrentCulture));
        }

        private static string CreateDataPacket(params string[] attributes)
        {
            string result = string.Empty;
            result += Flags.StartOfText;

            for (int i = 0; true; i++)
            {
                result += attributes[i];

                if (i == attributes.Length - 1)
                    break;

                result += Flags.EndOfTransBlock;
            }

            result += Flags.EndOfText;

            return result;
        }

        private static string[] Split(string textData)
        {
            return textData.Split(new char[] { Flags.StartOfText, Flags.EndOfText, Flags.EndOfTransBlock, Flags.EndOfTransmission },
                StringSplitOptions.RemoveEmptyEntries); // WARNING, Cannot send void messages with that, get used of it
        }
    }
}
