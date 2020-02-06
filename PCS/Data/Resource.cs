using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class Resource
    {
        public string LocalPath { get; set; }
        public string RemoteFileName { get; set; }
        public ResourceType Type { get; set; }

        public Resource(string localPath = null, string remoteFileName = null, ResourceType? type = null)
        {
            LocalPath = localPath;
            RemoteFileName = remoteFileName;
            if (type != null) Type = (ResourceType)type;
        }
    }
}
