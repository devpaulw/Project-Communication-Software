using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    class TaskPacket : Packet
    {
        public Task Task { get; set; }

        public TaskPacket(Task task) : base(Flags.Task)
        {
            Task = task ?? throw new ArgumentNullException(nameof(task));
        }

        protected override string[] GetAttributes()
        {
            return new string[] { Task.Name };
        }
    }
}
