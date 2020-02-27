using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PCS.Data.Packets
{
    class ResponsePacket : Packet<Response>
    {
        public ResponsePacket(Response response) : base(response, Flags.Response) { }

        public override string[] GetAttributes()
        {
            return new[]
                {
                    ((int)Item.Code).ToString(CultureInfo.InvariantCulture),
                    Convert.ToInt32(Item.Succeeded).ToString(CultureInfo.InvariantCulture)
                }
            .Concat(Item.GetAdditionalAttributes() ?? Array.Empty<string>())
            .ToArray();
        }

        public static Packet FromAttributes(string[] attributes)
        {
            RequestCode responseCode = (RequestCode)Convert.ToInt32(attributes[0], CultureInfo.CurrentCulture);
            bool succeeded = Convert.ToBoolean(Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

            if (responseCode == RequestCode.BroadcastDelivery) // TODO Better handle
            {
                return new ResponsePacket(BroadcastDeliveryResponse.FromAttributes(succeeded, attributes.Skip(2).ToArray()));
            }

            return new ResponsePacket(new Response(responseCode, succeeded));
        }
    }
}
