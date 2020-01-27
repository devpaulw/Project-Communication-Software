using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    [Serializable]
    public class UnknownDataPacketException : Exception
    {
        private const string DefaultMessage = "This data packet is not recognized";

        public UnknownDataPacketException() : base(DefaultMessage) { } // TODO: Handle Data Tables, what is it?
        public UnknownDataPacketException(string message) : base(message) { }
        public UnknownDataPacketException(string message, Exception inner) : base(message, inner) { }
        protected UnknownDataPacketException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
