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
        public const string ClientSigningIn = "SI";
        public const string ClientDisconnection = "DC";
    }
}