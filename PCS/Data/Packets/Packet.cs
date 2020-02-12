using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCS
{
    internal abstract class Packet
    {
        public string Flag { get; }

        public Packet(string flag)
        {
            Flag = flag;
        }

        public static Packet FromTextData(string textData)
        {
            var attributes = Split(textData);
            string headerFlag = attributes[0];

            attributes = attributes.Skip(1).Take(attributes.Length - 1).ToArray(); // Delete the flagHeader
                                                       
            switch (headerFlag)
            {
                case Flags.MemberSignIn:
                    return PacketObjects.GetMemberSignIn(attributes);
                case Flags.BroadcastMessage:
                    return PacketObjects.GetBroadcastMessage(attributes);
                case Flags.Message:
                    return PacketObjects.GetMessage(attributes);
                case Flags.ClientDisconnect:
                    return PacketObjects.GetDisconnect();
                default:
                    throw new Exception(Messages.Exceptions.NotRecognizedDataPacket);
            }
        }

        public string GetTextData()
        {
            string result = Flag;

            string[] attributes = GetAttributes();

            if (attributes != null) // Is not only a flag like Disconnect Packet
            {
                CreatePacket();
            }

            return result;

            void CreatePacket()
            {
                result += ControlChars.StartOfText;

                for (int i = 0; true; i++)
                {
                    result += attributes[i];

                    if (i == attributes.Length - 1)
                        break;

                    result += ControlChars.EndOfTransBlock;
                }

                result += ControlChars.EndOfText;
            }
        }

        protected abstract string[] GetAttributes();

        private static string[] Split(string textData) // TODO REDO
        {
            return textData.Split(new char[] { ControlChars.StartOfText, ControlChars.EndOfText, ControlChars.EndOfTransBlock, ControlChars.EndOfTransmission },
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
