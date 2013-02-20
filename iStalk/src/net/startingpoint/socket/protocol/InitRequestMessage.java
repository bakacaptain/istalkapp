package net.startingpoint.socket.protocol;

import net.startingpoint.socket.framework.AbstractMessage;

public class InitRequestMessage extends AbstractMessage {

	public final String USERNAME;
	
	public InitRequestMessage(String username) {
		USERNAME = username;
	}
	
	@Override
	public String toXml() {
		StringBuffer buffer = new StringBuffer();
		return buffer.append("<InitRequestMessage>")
			  .append("<Username>")
			  .append(USERNAME)
			  .append("</Username>")
			  .append("</InitRequestMessage>").toString();
	}

}
