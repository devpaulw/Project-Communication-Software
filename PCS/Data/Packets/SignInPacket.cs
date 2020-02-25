using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS.Data.Packets
{
    class SignInPacket : Packet
    {
        public AuthenticationInfos AuthenticationInfos { get; set; }

        public SignInPacket(AuthenticationInfos authenticationInfos) : base(Flags.MemberSignIn)
        {
            AuthenticationInfos = authenticationInfos;
        }

        protected override string[] GetAttributes()
        {
            return new string[]
            {
                AuthenticationInfos.MemberId.ToString(CultureInfo.InvariantCulture),
                AuthenticationInfos.Password
            };
        }
    }
}
