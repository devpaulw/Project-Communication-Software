using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{

    [Serializable]
    public class MessageEmptyFieldException : Exception
    {
        public MessageEmptyFieldException() { }
        public MessageEmptyFieldException(string message) : base(message) { }
        public MessageEmptyFieldException(string message, Exception inner) : base(message, inner) { }
        protected MessageEmptyFieldException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
