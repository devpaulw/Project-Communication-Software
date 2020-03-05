using System;
using System.Collections.Generic;
using System.Text;

namespace PCS.Data
{
    static class ControlChars
    {
        public const char StartOfText = (char)2; 
        public const char EndOfText = (char)3;
        public const char EndOfTransmission = (char)4;
        public const char EndOfTransBlock = (char)23;
    }
}
