package net.startingpoint.socket.protocol;

import net.startingpoint.UserLocation;
import net.startingpoint.socket.framework.AbstractMessage;

public class RemoveGeoPointMessage extends AbstractMessage {

	public final UserLocation USER_LOCATION;
	
	public RemoveGeoPointMessage(UserLocation userLoc) {
		USER_LOCATION = userLoc;
	}
	
	@Override
	public String toXml() {
		StringBuffer buffer = new StringBuffer();
		return buffer.append("<RemoveGeoPointMessage>")
			  .append(USER_LOCATION.toXml())
			  .append("</RemoveGeoPointMessage>").toString();
	}

}
