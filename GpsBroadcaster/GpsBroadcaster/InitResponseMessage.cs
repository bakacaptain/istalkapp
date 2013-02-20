using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerNetworkConnections;

namespace GpsBroadcaster
{
    public class InitResponseMessage : AbstractMessage
    {
        public Boolean IsAccepted { get; private set; }

        public InitResponseMessage(Boolean isAccepted)
        {
            IsAccepted = isAccepted;
        }

        public override string ToXML()
        {
            return "<InitResponseMessage><IsAccepted>" + IsAccepted + "</IsAccepted></InitResponseMessage>";
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
