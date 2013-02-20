using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerNetworkConnections;
using System.Xml;
using System.IO;

namespace GpsBroadcaster
{
    public class XmlMessageParser : AbstractMessageParser
    {
        public override AbstractMessage Parse(String xml)
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(xml));
            AbstractMessage returnee = null;

            String element = "";
            String username = "";
            Boolean isAccepted = false;
            GeoPoint geoPoint = null;
            Int32 longtitude = 0;
            Int32 latitude = 0;
            Int32 gender = 0x0A0; // please set to initial gender => male
            Int32 bgColour = 0x0B7; // colour => orange
            List<UserLocation> userLocations = null;
            UserLocation userLocation = null;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.CDATA:
                            switch (element)
                            {
                                default:
                                    break;
                            }
                            break;

                        case XmlNodeType.Element:
                            element = reader.Name; // define last known element
                            switch (element)
                            {
                                case "UserLocations":
                                    userLocations = new List<UserLocation>();
                                    break;
                                default:
                                    break;
                            }
                            break;

                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case "InitRequestMessage":
                                    returnee = new InitRequestMessage(username);
                                    break;
                                case "InitResponseMessage":
                                    returnee = new InitResponseMessage(isAccepted);
                                    break;
                                case "GeoPointMessage":
                                    returnee = new GeoPointMessage(userLocation);
                                    break;
                                case "MultipleGeoPointRequestMessage":
                                    returnee = new MultipleGeoPointRequestMessage();
                                    break;
                                case "MultipleGeoPointResponseMessage":
                                    returnee = new MultipleGeoPointResponseMessage(userLocations);
                                    break;
                                case "RemoveGeoPointMessage":
                                    returnee = new RemoveGeoPointMessage(userLocation);
                                    break;
                                case "UserLocation":
                                    if (userLocations != null)
                                    {
                                        userLocations.Add(new UserLocation(username, geoPoint, gender, bgColour));
                                    }
                                    else
                                    {
                                        userLocation = new UserLocation(username, geoPoint, gender, bgColour);
                                    }
                                    break;
                                case "GeoPoint":
                                    geoPoint = new GeoPoint(latitude, longtitude);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            switch (element)
                            {
                                case "Username":
                                    username = reader.Value;
                                    break;
                                case "IsAccepted":
                                    isAccepted = Boolean.Parse(reader.Value);
                                    break;
                                case "Latitude":
                                    latitude = Int32.Parse(reader.Value);
                                    break;
                                case "Longtitude":
                                    longtitude = Int32.Parse(reader.Value);
                                    break;
                                case "Gender":
                                    gender = int.Parse(reader.Value);
                                    break;
                                case "BgColour":
                                    bgColour = int.Parse(reader.Value);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                throw new XmlException("Unrecognized XML Format...");
            }
            reader.Close();
            if (returnee != null)
            {
                return returnee;
            }
            else
            {
                throw new XmlException("Unrecognized XML Format...");
            }
        }
    }
}
