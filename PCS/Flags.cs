using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    internal static class Flags
    {
        public const char EndOfText = (char)3;
        public const char EndOfTransmission = (char)4;

        public const string ClientMessage = "TX";
        public const string ServerMessage = "MS";
        public const string ClientSignIn = "SI";
        public const string ClientDisconnect = "DC";

        public static DataPacketType? GetDataPacketType(string flag)
        {
            switch (flag)
            {
                case ClientSignIn: return DataPacketType.ClientSignIn;
                case ClientMessage: return DataPacketType.ClientMessage;
                case ClientDisconnect: return DataPacketType.ClientDisconnect;
                case ServerMessage: return DataPacketType.ServerMessage;
                default: return null;
            }
        }
    }
}