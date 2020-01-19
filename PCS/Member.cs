using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCS
{
    public class Member : IDataStream
    {
        public string Username { get; set; }
        public int ID { get; set; }

        public Member(string username, int id)
        {
            Username = username;
            ID = id;
        }

        public string GetData()
        {
            return Username + Flags.EndOfText + ID;
        }

        public override string ToString()
        {
            return "Member " + Username + " (ID " + ID + ")";
        }
    }
}
