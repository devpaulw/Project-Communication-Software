using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    abstract class TransmissionPacket
    {
        //protected TransmissionPacket

        //private TransmissionPacket(PacketType type)
        //{

        //}

        public PacketType Type { get; }

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
    }
}
