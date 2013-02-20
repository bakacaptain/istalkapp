using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerNetworkConnections;

namespace GpsBroadcaster
{
    public class GeoPointMessage : AbstractMessage
    {
        public UserLocation UserLocation { get; private set; }

        public GeoPointMessage(UserLocation userLocation)
        {
            UserLocation = userLocation;
        }

        public override string ToXML()
        {
            return "<GeoPointMessage>" + UserLocation.ToXML() + "</GeoPointMessage>";
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
