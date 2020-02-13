using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    class ToDoListPacket : Packet
    {
        public ToDoList ToDoList { get; set; }

        public ToDoListPacket(ToDoList toDoList) : base(Flags.ToDoList)
        {
            ToDoList = toDoList ?? throw new ArgumentNullException(nameof(toDoList));
        }

        protected override string[] GetAttributes()
        {
            return new string[] { ToDoList.Name };
        }
    }
}
