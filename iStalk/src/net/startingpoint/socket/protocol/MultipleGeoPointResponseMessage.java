package net.startingpoint.socket.protocol;

import java.util.List;

import net.startingpoint.UserLocation;
import net.startingpoint.socket.framework.AbstractMessage;

public class MultipleGeoPointResponseMessage extends AbstractMessage {
	
	public final List<UserLocation> USER_LOCATIONS;
	
	public MultipleGeoPointResponseMessage(List<UserLocation> userLocations) {
		USER_LOCATIONS = userLocations;
	}
	
	@Override
	public String toXml() {
		StringBuffer buffer = new StringBuffer();
		
		buffer.append("<MultipleGeoPointResponseMessage><UserLocations>");
		for(UserLocation loc : USER_LOCATIONS) {
			buffer.append(loc.toXml());
		}
		buffer.append("</UserLocations></MultipleGeoPointResponseMessage>");
		
		return buffer.toString();
	}

}
