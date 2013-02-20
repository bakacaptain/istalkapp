using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServerNetworkConnections
{
    public abstract class AbstractMessageParser
    {
        public abstract AbstractMessage Parse(String stream);
    }
}
