using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    internal static class Flags
    {
        public const char EndOfText = (char)3;
        public const char EndOfTransmission = (char)4;

        public const string Text = "TX";
        public const string Message = "MS";
        public const string SigningIn = "SI";
        public const string Disconnection = "DC";

        public static string[] Split(string textData)
        {
            return textData.Split(new char[] { EndOfText, EndOfTransmission },
                StringSplitOptions.None);
        }
    }
}