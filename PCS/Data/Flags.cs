using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    internal static class Flags
    {
        public const char StartOfText = (char)2;
        public const char EndOfText = (char)3;
        public const char EndOfTransmission = (char)4;
        public const char EndOfTransBlock = (char)23;

        public const string Message = "TX";
        public const string MemberSignIn = "SI";
        public const string ClientDisconnect = "DC";

        public static DataPacketType GetDataPacketType(string flag)
        {
            switch (flag)
            {
                case MemberSignIn: return DataPacketType.MemberSignIn;
                case Message: return DataPacketType.Message;
                case ClientDisconnect: return DataPacketType.ClientDisconnect;
                default:
                    throw new DataPacketException(Messages.Exceptions.NotRecognizedDataPacket);
            }
        }

        public static string GetFlag(DataPacketType dataPacketType)
        {
            switch (dataPacketType)
            {
                case DataPacketType.MemberSignIn:
                    return MemberSignIn;
                case DataPacketType.Message:
                    return Message;
                case DataPacketType.ClientDisconnect:
                    return ClientDisconnect;
            }

            return null; // Impossible, it's just to please the compiler.
        }
    }
}