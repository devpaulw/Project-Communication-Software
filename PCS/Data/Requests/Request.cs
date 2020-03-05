using PCS.Data.Packets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PCS.Data
{
    abstract class Request
    {
        public RequestCode Code { get; }

        public Request(RequestCode code)
        {
            Code = code;
        }

        internal abstract string[] GetAttributes();
    }
}
