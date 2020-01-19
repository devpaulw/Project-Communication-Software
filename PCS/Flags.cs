using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    internal static class Flags
    {
        public const char EndOfText = (char)3;
        public const char EndOfTransmission = (char)4;

        public const string Connect = "C";
        public const string Disconnect = "D";
    }
}