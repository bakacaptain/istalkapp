using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerNetworkConnections;

namespace GpsBroadcaster
{
    public class UserLocation : IXmlable
    {
        public GeoPoint GeoPoint { get; private set; }
        public String Username { get; private set; }
        public int Gender { get; private set; }
        public int BgColour { get; private set; }

        public UserLocation(String username, GeoPoint geoPoint, int gender, int bgColour)
        {
            GeoPoint = geoPoint;
            Username = username;
            Gender = gender;
            BgColour = bgColour;
        }

        public UserLocation(String username)
        {
            Username = username;
        }

        public string ToXML()
        {
            return "<UserLocation><Username>" + Username + "</Username><Gender>" + Gender + "</Gender><BgColour>" + BgColour + "</BgColour>" + GeoPoint.ToXML() + "</UserLocation>";
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            UserLocation other = obj as UserLocation;

            return this.Username.Equals(other.Username);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return this.Username.GetHashCode();
        }
    }
}
