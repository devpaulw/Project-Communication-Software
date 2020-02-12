using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    class DisconnectPacket : Packet
    {
        public DisconnectPacket() : base(Flags.ClientDisconnect) { } // TODO Discontinue PacketType and use only Flags string (simpler)

        protected override string[] GetAttributes()
        {
            return null;
        }
    }
}
