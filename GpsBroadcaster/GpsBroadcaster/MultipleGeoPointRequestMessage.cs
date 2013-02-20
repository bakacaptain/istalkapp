using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerNetworkConnections;

namespace GpsBroadcaster
{
    public class MultipleGeoPointRequestMessage : AbstractMessage
    {
        public override string ToXML()
        {
            return "<MultipleGeoPointRequestMessage></MultipleGeoPointRequestMessage>";
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
