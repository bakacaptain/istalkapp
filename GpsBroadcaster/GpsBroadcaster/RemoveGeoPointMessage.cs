using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerNetworkConnections;

namespace GpsBroadcaster
{
    public class RemoveGeoPointMessage : AbstractMessage
    {
        public UserLocation UserLocation { get; private set; }

        public RemoveGeoPointMessage(UserLocation userLocation)
        {
            UserLocation = userLocation;
        }

        public override string ToXML()
        {
            return "<RemoveGeoPointMessage>" + UserLocation.ToXML() + "</RemoveGeoPointMessage>";
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
