using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PCS
{
    class DataPacket
    {
        private readonly string[] m_attributes;

        public PacketType Type { get; }
        public object Object { get; }

        //public DataPacket(string textData)
        //{

        //}

        //public DataPacket(string textData, DataPacketType type)
        //{
        //    Type = type;
        //    m_attributes = Split(textData);
        //}

        public DataPacket(PacketType type, object @object)
        {
            Object = @object;
            Type = type;

            m_attributes = GetObjectAttributes();

            string[] GetObjectAttributes()
            {
                switch (Type)
                {
                    case PacketType.MemberSignIn:
                        return DataPacketAttributes.FromMemberSignIn(@object as Member);
                    case PacketType.Message:
                        return  DataPacketAttributes.FromMessage(@object as Message);
                    default:
                        return null;
                }
            }
        }

        public DataPacket(PacketType type)
        {
            Type = type;
        }

        private DataPacket(PacketType type, string[] attributes)
        {
            Type = type;
            m_attributes = attributes;

            switch (type)
            {
                case PacketType.MemberSignIn:
                    Object = DataPacketObjects.GetMemberSignIn(m_attributes);
                    break;
                case PacketType.Message:
                    Object = DataPacketObjects.GetMessage(m_attributes);
                    break;
            }
        }

        public static DataPacket FromTextData(string textData)
        {
            var attributes = Split(textData);

            string headerFlag = attributes[0];
            var type = Flags.GetDataPacketType(headerFlag);

            attributes = attributes.Skip(1).Take(attributes.Length - 1).ToArray(); // Delete the flagHeader
                                                                                   // TODO Check when no attributes
            return new DataPacket(type, attributes);
        }

        public string GetTextData()
        {
            string result = string.Empty;
            result += Flags.GetFlag(Type);

            if (m_attributes != null) // Is not only a flag like Disconnect Packet
            {
                result += Flags.StartOfText;

                for (int i = 0; true; i++)
                {
                    result += m_attributes[i];

                    if (i == m_attributes.Length - 1)
                        break;

                    result += Flags.EndOfTransBlock;
                }

                result += Flags.EndOfText;
            }

            return result;
        }

        private static string[] Split(string textData)
        {
            return textData.Split(new char[] { Flags.StartOfText, Flags.EndOfText, Flags.EndOfTransBlock, Flags.EndOfTransmission },
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
