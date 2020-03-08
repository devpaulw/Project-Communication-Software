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
        public Channel Channel { get; set; }

        public BroadcastDeliveryRequest(int start, int end, Channel channel) : base(RequestCode.BroadcastDelivery)
        {
            Start = start;
            End = end;
            Channel = channel;
        }

        internal override string[] GetAttributes()
        {
            return new[] { 
                Start.ToString(CultureInfo.InvariantCulture), 
                End.ToString(CultureInfo.InvariantCulture), 
                Channel.Name };
        }

        public static Request FromAttributes(string[] attributes)
        {
            return new BroadcastDeliveryRequest(
                int.Parse(attributes[0], CultureInfo.InvariantCulture),
                int.Parse(attributes[1], CultureInfo.InvariantCulture),
                new Channel(attributes[2]));
        }
    }
}
