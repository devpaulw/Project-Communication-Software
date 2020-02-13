using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class Task
    {
        public string Name { get; set; }
        public bool HasEmptyField
            => string.IsNullOrEmpty(Name);
        public Task(string name)
        {
            Name = name;

            if (HasEmptyField)
                throw new Exception(Messages.Exceptions.MessageEmptyField);
        }
    }
}
