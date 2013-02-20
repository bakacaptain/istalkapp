using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerNetworkConnections;

namespace GpsBroadcaster
{
    public class MultipleGeoPointResponseMessage : AbstractMessage
    {
        public List<UserLocation> UserLocations { get; private set; }

        public MultipleGeoPointResponseMessage(List<UserLocation> userLocations)
        {
            UserLocations = userLocations;
        }

        public override string ToXML()
        {
            String text = "<MultipleGeoPointResponseMessage><UserLocations>";
            foreach (UserLocation loc in UserLocations)
            {
                text += loc.ToXML();
            }
            text += "</UserLocations></MultipleGeoPointResponseMessage>";

            return text;
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
