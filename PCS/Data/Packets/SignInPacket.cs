using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS.Data.Packets
{
    class SignInPacket : Packet<AuthenticationInfos>
    {
        public SignInPacket(AuthenticationInfos authenticationInfos) : base(authenticationInfos, Flags.MemberSignIn) { }

        protected override string[] GetAttributes()
        {
            return new string[]
            {
                Item.MemberId.ToString(CultureInfo.InvariantCulture),
                Item.Password
            };
        }
    }
}
