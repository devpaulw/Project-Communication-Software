using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    [Serializable]
    public class DataPacketException : Exception // TODO Maybe deprecate these exceptions, because it's useless with the Exception classic
    {
        public DataPacketException() : base() { }
        public DataPacketException(string message) : base(message) { }
        public DataPacketException(string message, Exception inner) : base(message, inner) { }
        protected DataPacketException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
