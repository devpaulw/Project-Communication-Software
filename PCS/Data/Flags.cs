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

        public static PacketType GetDataPacketType(string flag)
        {
            switch (flag)
            {
                case MemberSignIn: return PacketType.MemberSignIn;
                case Message: return PacketType.Message;
                case ClientDisconnect: return PacketType.ClientDisconnect;
                default:
                    throw new DataPacketException(Messages.Exceptions.NotRecognizedDataPacket);
            }
        }

        public static string GetFlag(PacketType dataPacketType)
        {
            switch (dataPacketType)
            {
                case PacketType.MemberSignIn:
                    return MemberSignIn;
                case PacketType.Message:
                    return Message;
                case PacketType.ClientDisconnect:
                    return ClientDisconnect;
            }

            return null; // Impossible, it's just to please the compiler.
        }
    }
}