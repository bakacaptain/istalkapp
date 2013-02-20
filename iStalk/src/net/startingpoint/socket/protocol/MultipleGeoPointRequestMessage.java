package net.startingpoint.socket.protocol;

import net.startingpoint.socket.framework.AbstractMessage;

public class MultipleGeoPointRequestMessage extends AbstractMessage {
	
	public MultipleGeoPointRequestMessage() {
	}
	
	@Override
	public String toXml() {
		return "<MultipleGeoPointRequestMessage></MultipleGeoPointRequestMessage>";
	}

}
