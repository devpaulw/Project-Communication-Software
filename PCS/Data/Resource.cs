using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class Resource
    {
        public string LocalPath { get; set; }
        public Uri FtpUri { get; set; }
        public ResourceType Type { get; set; }

        public Resource(string localPath = null, Uri ftpUri = null, ResourceType? type = null)
        {
            LocalPath = localPath;
            FtpUri = ftpUri;
            if (type != null) Type = (ResourceType)type;
        }
    }
}
