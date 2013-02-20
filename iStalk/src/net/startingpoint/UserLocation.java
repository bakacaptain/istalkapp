package net.startingpoint;


import net.startingpoint.socket.framework.AbstractMessage;

import com.google.android.maps.GeoPoint;

public class UserLocation extends AbstractMessage {

	public final String USERNAME;
	public final GeoPoint GEO_POINT;
	public final int GENDER, BG_COLOUR;
	
	public UserLocation(String username, GeoPoint geoPoint, 
			int gender, int bgColor) {
		USERNAME = username;
		GEO_POINT = geoPoint;
		GENDER = gender;
		BG_COLOUR = bgColor;
	}
	
	@Override
	public String toXml() {
		StringBuffer buffer = new StringBuffer();
		return buffer.append("<UserLocation>")
			  .append("<Username>")
			  .append(USERNAME)
			  .append("</Username>")
			  .append("<Gender>")
			  .append(GENDER)
			  .append("</Gender>")
			  .append("<BgColour>")
			  .append(BG_COLOUR)
			  .append("</BgColour>")
			  .append("<GeoPoint>")
			  .append("<Latitude>")
			  .append(GEO_POINT.getLatitudeE6())
			  .append("</Latitude>")
			  .append("<Longtitude>")
			  .append(GEO_POINT.getLongitudeE6())
			  .append("</Longtitude>")
			  .append("</GeoPoint>")
			  .append("</UserLocation>").toString();
	}

}
