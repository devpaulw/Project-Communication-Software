using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PCS.Data.Packets
{
    class RequestPacket : Packet<Request>
    {
        public RequestPacket(Request request) : base(request, Flags.Request)
        {
        }

        protected override string[] GetAttributes() => new[]
                {
                    ((int)Item.Code).ToString(CultureInfo.CurrentCulture)
                }
                .Concat(Item.GetAdditionalAttributes())
                .ToArray();
    }
}
