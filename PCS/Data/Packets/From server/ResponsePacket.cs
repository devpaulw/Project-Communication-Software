using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS.Data.Packets
{
    class ResponsePacket : Packet<Response>
    {
        public ResponsePacket(Response response) : base(response, Flags.Response) { }

        protected override string[] GetAttributes()
        {
            return new[]
                {
                    ((int)Item.Code).ToString(CultureInfo.InvariantCulture),
                    Convert.ToInt32(Item.Succeeded).ToString(CultureInfo.InvariantCulture)
                };
        }
    }
}
