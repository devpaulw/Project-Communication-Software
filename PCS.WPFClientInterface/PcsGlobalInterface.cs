using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCS.WPFClientInterface
{
    internal static class PcsGlobalInterface
    {
        public static PcsAccessor Accessor { get; } = new PcsAccessor();
        public static string SelectedChannel { get; set; }
    }
}
