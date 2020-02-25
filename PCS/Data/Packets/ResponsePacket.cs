using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS.Data.Packets
{
    class ResponsePacket : Packet
    {
        public ResponseCode ResponseCode { get; set; }

        public ResponsePacket(ResponseCode responseCode) : base(Flags.Response)
        {
            ResponseCode = responseCode;
        }

        protected override string[] GetAttributes()
        {
            return new string[]
            {
                ((int)ResponseCode).ToString(CultureInfo.InvariantCulture)
            };
        }
    }
}
