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

        public Message GetMessage()
        {
            string channelTitle = m_attributes[0];

            string username = m_attributes[1];
            int id = Convert.ToInt32(m_attributes[2], CultureInfo.CurrentCulture);
            var dateTime = DateTime.FromFileTime(Convert.ToInt64(m_attributes[3], CultureInfo.CurrentCulture));
            string text = m_attributes[4];
            var attachedFiles = new List<Resource>();

            foreach (string attachedFile in m_attributes.Skip(5))
                if (!string.IsNullOrEmpty(attachedFile)) // DOLATER: Remove that and instead don't take after end of text in Split
                    attachedFiles.Add(new Resource(ftpUri: 
                        new Uri(attachedFile)));

            var author = new Member(username, id);

            return new Message(text, channelTitle, dateTime, author, attachedFiles);
        }

        public static string FromMessage(Message message)
        {
            var attributes = new string[] {
                 message.ChannelTitle,
                 message.Author.Username,
                 message.Author.ID.ToString(CultureInfo.CurrentCulture),
                 message.DateTime.ToFileTime().ToString(CultureInfo.CurrentCulture),
                 message.Text 
            };

            if (message.AttachedResources != null) // If there are any resource, add them
                attributes = attributes.Concat(from resourceUri in message.AttachedResources 
                                               select resourceUri.FtpUri.AbsoluteUri).ToArray();

            return CreateDataPacket(attributes);
        }

        public static string FromMember(Member member)
        {
            return CreateDataPacket(new string[] {
                member.Username,
                member.ID.ToString(CultureInfo.CurrentCulture) 
            });
        }

        private static string CreateDataPacket(string[] attributes)
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
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
