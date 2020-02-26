using PCS.Data.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace PCS.Data
{
    class BroadcastDeliveryResponse : Response
    {
        public List<BroadcastMessage> Broadcasts { get; set; }

        public BroadcastDeliveryResponse(bool succeeded, List<BroadcastMessage> broadcasts) 
            : base(ResponseCode.MessageDelivery, succeeded)
        {

        }

        internal override string[] GetAdditionalAttributes()
        {
            return (from broadcast in Broadcasts
                   selec broadcast.BaseMessage.ChannelName and broadcast.Author.Username)
                   .ToArray(); // UNDONE
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
