using System;
using System.Collections.Generic;
using System.Text;

namespace PCS.Data.Packets
{
    class DisconnectPacket : Packet
    {
        public DisconnectPacket() : base(Flags.ClientDisconnect) { }
    }
}
