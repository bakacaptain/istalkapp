using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerNetworkConnections;

namespace GpsBroadcaster
{
    public class GeoPoint : IXmlable
    {
        public Int32 Latitude { get; private set; }
        public Int32 Longtitude { get; private set; }

        public GeoPoint(Int32 latitude, Int32 longtitude)
        {
            Latitude = latitude;
            Longtitude = longtitude;
        }

        public string ToXML()
        {
            return "<GeoPoint><Latitude>" +
                Latitude +
                "</Latitude><Longtitude>" +
                Longtitude +
                "</Longtitude></GeoPoint>";
        }
    }
}
