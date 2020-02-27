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

        public override string[] GetAttributes() => new[]
                {
                    ((int)Item.Code).ToString(CultureInfo.CurrentCulture)
                }
                .Concat(Item.GetAttributes() ?? Array.Empty<string>())
                .ToArray();

        public static Packet FromAttributes(string[] attributes)
        {
            RequestCode requestCode = (RequestCode)Convert.ToInt32(attributes[0], CultureInfo.CurrentCulture);
            attributes = attributes.Skip(1).ToArray();
            Request request = null;

            switch (requestCode)
            {
                case RequestCode.SignIn:
                    request = SignInRequest.FromAttributes(attributes);
                    break;
                case RequestCode.DeleteMessage:
                    request = DeleteMessageRequest.FromAttributes(attributes);
                    break;
                case RequestCode.ModifyMessage:
                    request = ModifyMessageRequest.FromAttributes(attributes);
                    break;
                case RequestCode.BroadcastDelivery:
                    request = BroadcastDeliveryRequest.FromAttributes(attributes);
                    break;
            }

            return new RequestPacket(request);
        }
    }
}
