using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCS
{
    public class Member
    {
        public string Username { get; set; }
        public int ID { get; set; }

        public Member(string username, int id)
        {
            Username = username;
            ID = id;
        }

        public byte[] GetBytes()
        {
            string formattedUsername = $"MBR::UN:{Username};ID:{ID};;\0";

            return Encoding.ASCII.GetBytes(formattedUsername);
        }

        public override string ToString()
        {
            return "Member " + Username + " (ID " + ID + ")";
        }
    }
}
