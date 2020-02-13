using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class TaskList
    {
        public string Name { get; set; }
        public bool HasEmptyField
            => string.IsNullOrEmpty(Name);
        public List<Task> Tasks { get; }
        public TaskList(string name)
        {
            Name = name;

            if (HasEmptyField)
                throw new Exception(Messages.Exceptions.MessageEmptyField);
        }
    }
}
