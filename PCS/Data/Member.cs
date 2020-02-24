using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCS
{
    public class Member : IEquatable<Member>
    {
        public string Username { get; set; }
        public int ID { get; set; }
        public ConnectionState ConnectionState { get; set; } // TODO Find a solution to handle better this variable

        public Member(string username, int id)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            ID = id;
        }

        public static Member Unknown {
            get {
                return new Member("Unknown member", -1);
            }
        }

        public override string ToString()
        {
            return "Member " + Username + " (ID " + ID + ")";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Member);
        }

        public bool Equals(Member other)
        {
            return other != null &&
                   ID == other.ID;
        }

        public override int GetHashCode()
        {
            var hashCode = 1275858721;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Member left, Member right)
        {
            if (left is null || right is null)
                return false;

            return left.ID == right.ID;
        }

        public static bool operator !=(Member left, Member right)
        {
            return !(left == right);
        }
    }
}
