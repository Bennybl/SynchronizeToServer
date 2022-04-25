using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace SynchronizeToServer
{
    class MetaData
    {
        public HashSet<string> FilesInFolder { get; set; }

        public DateTime LastInvocationTime { get; set; }
    }
}
