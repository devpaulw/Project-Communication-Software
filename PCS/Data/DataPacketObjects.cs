﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS
{
    static class DataPacketObjects
    {
        public static Member GetMemberSignIn(string[] attributes)
        {
            string username = attributes[0];
            int id = Convert.ToInt32(attributes[1], CultureInfo.CurrentCulture);

            return new Member(username, id);
        }

        public static Message GetMessage(string[] attributes)
        {
            string channelTitle = attributes[0];

            string username = attributes[1];
            int id = Convert.ToInt32(attributes[2], CultureInfo.CurrentCulture);
            var dateTime = DateTime.FromFileTime(Convert.ToInt64(attributes[3], CultureInfo.CurrentCulture));
            string text = attributes[4];

            var author = new Member(username, id);

            return new Message(text, channelTitle, dateTime, author);
        }
    }
}
