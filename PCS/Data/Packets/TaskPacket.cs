using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    class TaskPacket : Packet
    {
        public TaskPacket() : base(Flags.Task)
        {

        }

        protected override string[] GetAttributes()
        {
            return new string[] {"voiture"};
        }
    }
}
