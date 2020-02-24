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
            var attributes = Split(textData, out string headerFlag);
                                                       
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
                case Flags.Response:
                    return PacketObjects.GetResponse(attributes);
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

        private static string[] Split(string textData, out string headerFlag)
        {
            headerFlag = string.Empty;

            var ret = new List<string>();
            bool textStarted = false;

            foreach (char c in textData)
            {
                if (c == ControlChars.StartOfText)
                {
                    textStarted = true;
                    ret.Add(string.Empty);
                    continue;
                }
                else if (c == ControlChars.EndOfTransBlock)
                {
                    ret.Add(string.Empty);
                    continue;
                }
                else if (c == ControlChars.EndOfText || c == ControlChars.EndOfTransmission)
                    break;

                if (!textStarted)
                    headerFlag += c;
                else
                    ret[ret.Count - 1] += c;
            }

            return ret.ToArray();
        }
    }
}
