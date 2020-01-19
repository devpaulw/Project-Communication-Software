using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    internal static class Flags
    {
        public const char EndOfText = (char)3;
        public const char EndOfTransmission = (char)4;

        // TODO Continue this concept!
        //public const string Message = "M";
        //public const string Connect = "C";
        //public const string Disconnect = "D";

        public static string[] Split(string textData)
        {
            return textData.Split(new char[] { EndOfText, EndOfTransmission },
                StringSplitOptions.None);
        }
    }

    //enum DataType
    //{
    //    Connection, Disconnection, ServerMessage, ClientMessage
    //}
}