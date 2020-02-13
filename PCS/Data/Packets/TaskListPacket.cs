using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    class TaskListPacket : Packet
    {
        public TaskList TaskList { get; set; }

        public TaskListPacket(TaskList taskList) : base(Flags.TaskList)
        {
            TaskList = taskList ?? throw new ArgumentNullException(nameof(taskList));
        }

        protected override string[] GetAttributes()
        {
            return new string[] { TaskList.Name };
        }
    }
}
