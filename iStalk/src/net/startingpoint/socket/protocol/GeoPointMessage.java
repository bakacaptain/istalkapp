package net.startingpoint.socket.protocol;

import net.startingpoint.UserLocation;
import net.startingpoint.socket.framework.AbstractMessage;

public class GeoPointMessage extends AbstractMessage {
	
	public final UserLocation USER_LOCATION;
	
	public GeoPointMessage(UserLocation userLocation) {
		USER_LOCATION = userLocation;
	}
	
	@Override
	public String toXml() {
		StringBuffer buffer = new StringBuffer();
		return buffer.append("<GeoPointMessage>")
			  .append(USER_LOCATION.toXml())
			  .append("</GeoPointMessage>").toString();
	}

}
