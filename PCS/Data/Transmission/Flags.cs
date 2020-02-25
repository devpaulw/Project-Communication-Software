using System;
using System.Collections.Generic;
using System.Text;

namespace PCS.Data
{
    internal static class Flags
    {
        public const string SendableMessage = "TX";
        public const string BroadcastMessage = "BM";
        public const string MemberSignIn = "SI";
        public const string ClientDisconnect = "DC";
        public const string Response = "RS";
        public const string Request = "RQ";
        public const string MessageDelivery = "MD";
    }
}