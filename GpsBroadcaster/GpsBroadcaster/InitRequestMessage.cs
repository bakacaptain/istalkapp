using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerNetworkConnections;

namespace GpsBroadcaster
{
    public class InitRequestMessage : AbstractMessage
    {
        public String Username { get; private set; }

        public InitRequestMessage(String username)
        {
            Username = username;
        }

        public override string ToXML()
        {
            return "<InitRequestMessage><Username>" + Username + "</Username></InitRequestMessage>";
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
