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

        public byte[] GetBytes()
        {
            return PcsServerHost.Encoding.GetBytes(GetDataFlag() + (char)4);
        }

        public string GetDataFlag()
        {
            return $"{Username}{(char)3}{ID}{(char)3}"; // DOLATER not clean
        }

        public static Member FromBytes(byte[] bytes, Encoding encoding) // UNDONE
        {
            string strMember = encoding.GetString(bytes);

            var infos = strMember.Split(new char[] { (char)3, (char)4 }, StringSplitOptions.RemoveEmptyEntries);

            return new Member(infos[0], Convert.ToInt32(infos[1]));
        }

        public override string ToString()
        {
            return "Member " + Username + " (ID " + ID + ")";
        }
    }
}
