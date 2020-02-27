using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS.Data
{
    class BroadcastDeliveryRequest : Request
    {
        public int Start { get; set; }
        public int End { get; set; }
        public string ChannelName { get; set; }

        public BroadcastDeliveryRequest(int start, int end, string channelName) : base(RequestCode.BroadcastDelivery)
        {
            Start = start;
            End = end;
            ChannelName = channelName;
        }

        internal override string[] GetAttributes()
        {
            return new[] { 
                Start.ToString(CultureInfo.InvariantCulture), 
                End.ToString(CultureInfo.InvariantCulture), 
                ChannelName };
        }

        public static Request FromAttributes(string[] attributes)
        {
            return new BroadcastDeliveryRequest(
                int.Parse(attributes[0], CultureInfo.InvariantCulture),
                int.Parse(attributes[1], CultureInfo.InvariantCulture),
                attributes[2]);
        }
    }
}
