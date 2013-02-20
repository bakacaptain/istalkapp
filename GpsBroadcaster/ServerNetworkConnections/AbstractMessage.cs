using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using CommonUtilities;

namespace ServerNetworkConnections
{
    /// <summary>
    /// Base class for all messages that can be sent over network in this application
    /// </summary>
    [Serializable()]
    public abstract class AbstractMessage : IXmlable, ISerializable
    {
        public const String XML_HEADER = "<?xml version=\"1.0\"?>";

        public abstract string ToXML();
        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);
    }
}
