using PCS.Data.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;

namespace PCS.Data
{
    class BroadcastDeliveryResponse : Response
    {
        public BroadcastMessage[] BroadcastMessages { get; set; }

        public BroadcastDeliveryResponse(bool succeeded, BroadcastMessage[] broadcastMessages)
            : base(RequestCode.BroadcastDelivery, succeeded)
        {
            BroadcastMessages = broadcastMessages;
        }

        internal override string[] GetAdditionalAttributes()
        {
            string[] ret = Array.Empty<string>();
            foreach (var broadcast in BroadcastMessages)
            {
                ret = ret .Concat(new BroadcastMessagePacket(broadcast).GetAttributes()).ToArray();
            }
            return ret;
        }

        public static Response FromAttributes(bool succeeded, string[] attributes)
        {
            List<BroadcastMessage> broadcastMessages = new List<BroadcastMessage>();
            for (int i = 0; i < attributes.Length; i += 6) // DOLATER a better approach to know what the length (5 yet)
            {
                broadcastMessages.Add(BroadcastMessagePacket.FromAttributes(attributes.Skip(i).ToArray()).Item);
            }
            return new BroadcastDeliveryResponse(succeeded, broadcastMessages.ToArray());
        }
    }

    //class BroadcastDeliveryPacket : Packet<>
    //{
    //    public BroadcastDeliveryPacket(List<BroadcastMessage> broadcasts) : base(broadcasts, Flags.MessageDelivery)
    //    {
    //    }

    //    protected override string[] GetAttributes()
    //    {

    //    }
    //}
}
