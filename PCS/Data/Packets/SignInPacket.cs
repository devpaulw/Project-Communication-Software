using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS
{
    class SignInPacket : Packet
    {
        public Member Member { get; set; }

        public SignInPacket(Member member)
        {
            Member = member ?? throw new ArgumentNullException(nameof(member));
        }

        protected override string[] GetAttributes()
        {
            return new string[]
            {
                Member.Username,
                Member.ID.ToString(CultureInfo.InvariantCulture)
            };
        }

        protected override void SetAttributes(string[] attributes)
        {
            Member.Username = attributes[0];
            Member.ID = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
        }
    }
}
