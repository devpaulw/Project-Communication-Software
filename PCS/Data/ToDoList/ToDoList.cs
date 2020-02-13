using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class ToDoList
    {
        public string Name { get; set; } = "NewToDoList";
        public bool HasEmptyField
            => string.IsNullOrEmpty(Name);
        public List<TaskList> Lists { get; }
        public ToDoList()
        {
            if (HasEmptyField)
                throw new Exception(Messages.Exceptions.MessageEmptyField);
        }
    }
}
