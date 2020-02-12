using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS
{
    class SignInPacket : Packet
    {
        public Member Member { get; set; }

        public SignInPacket(Member member) : base(Flags.MemberSignIn)
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
    }
}
