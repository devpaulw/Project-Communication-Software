using PCS.Data.Packets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PCS.Data
{
    class Response
    {
        public RequestCode Code { get; }
        public bool Succeeded { get; }

        public Response(RequestCode code, bool succeeded)
        {
            Code = code;
            Succeeded = succeeded;
        }

        internal virtual string[] GetAdditionalAttributes()
            => null;
    }
}
