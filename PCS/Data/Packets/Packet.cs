using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCS
{
    abstract class Packet
    {
        //protected TransmissionPacket

        //private TransmissionPacket(PacketType type)
        //{

        //}

        public PacketType Type { get; }

        protected Packet(PacketType type, string[] attributes)
        {
            Type = type;
            SetAttributes(attributes);
        }

        public static Packet FromTextData(string textData)
        {
            var attributes = Split(textData);

            string headerFlag = attributes[0];
            var type = Flags.GetDataPacketType(headerFlag);

            attributes = attributes.Skip(1).Take(attributes.Length - 1).ToArray(); // Delete the flagHeader
                                                       
            switch (type)
            {
                case PacketType.MemberSignIn:
                    return new SignInPacket(new Member());
            }
        }

        public string GetTextData()
        {
            string result = string.Empty;
            result += Flags.GetFlag(Type);

            string[] attributes = GetAttributes();

            if (attributes != null) // Is not only a flag like Disconnect Packet
            {
                result += Flags.StartOfText;

                for (int i = 0; true; i++)
                {
                    result += attributes[i];

                    if (i == attributes.Length - 1)
                        break;

                    result += Flags.EndOfTransBlock;
                }

                result += Flags.EndOfText;
            }

            return result;
        }

        protected abstract string[] GetAttributes();
        protected abstract void SetAttributes(string[] attributes);

        private static string[] Split(string textData)
        {
            return textData.Split(new char[] { Flags.StartOfText, Flags.EndOfText, Flags.EndOfTransBlock, Flags.EndOfTransmission },
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
