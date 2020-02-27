using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS.Data.Packets
{
    class SignInRequest : Request
    {
        public AuthenticationInfos AuthenticationInfos { get; set; }

        public SignInRequest(AuthenticationInfos authenticationInfos) : base(RequestCode.SignIn)
        {
            AuthenticationInfos = authenticationInfos;
        }

        internal override string[] GetAttributes()
        {
            return new string[]
            {
                AuthenticationInfos.MemberId.ToString(CultureInfo.InvariantCulture),
                AuthenticationInfos.Password
            };
        }

        public static Request FromAttributes(string[] attributes)
        {
            int userId = int.Parse(attributes[0], CultureInfo.InvariantCulture);
            string password = attributes[1];

            return new SignInRequest(new AuthenticationInfos(userId, password));
        }
    }
}
