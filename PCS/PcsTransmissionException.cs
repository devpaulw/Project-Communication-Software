using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{

    [Serializable]
    public class PcsTransmissionException : SystemException
    {
        public PcsTransmissionException() { }
        public PcsTransmissionException(string message) : base(message) { }
        public PcsTransmissionException(string message, Exception inner) : base(message, inner) { }
        protected PcsTransmissionException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
