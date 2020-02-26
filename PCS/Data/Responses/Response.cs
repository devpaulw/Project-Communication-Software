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
        public ResponseCode Code { get; }
        public bool Succeeded { get; }

        public Response(ResponseCode code, bool succeeded)
        {
            Code = code;
            Succeeded = succeeded;
        }

        internal virtual string[] GetAdditionalAttributes()
            => null;
    }
}
