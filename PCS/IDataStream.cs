using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    interface IDataStream
    {
        byte[] GetBytes();
        string GetDataFlag();
    }
}
