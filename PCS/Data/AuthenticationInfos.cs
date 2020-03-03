using System;
using System.Collections.Generic;
using System.Text;

namespace PCS.Data
{
    public class AuthenticationInfos
    {
        public int MemberId { get; set; }
        public string Password { get; set; } // DOLATER Use secured string

        public AuthenticationInfos(int memberId, string password)
        {
            MemberId = memberId;
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }
    }
}
