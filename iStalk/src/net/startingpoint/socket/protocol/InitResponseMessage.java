package net.startingpoint.socket.protocol;

import net.startingpoint.socket.framework.AbstractMessage;

public class InitResponseMessage extends AbstractMessage {

	public final boolean IS_ACCEPTED;
	
	public InitResponseMessage(boolean isAccepted) {
		IS_ACCEPTED = isAccepted;
	}
	
	@Override
	public String toXml() {
		StringBuffer buffer = new StringBuffer();
		return buffer.append("<InitResponseMessage>")
			  .append("<IsAccepted>")
			  .append(IS_ACCEPTED)
			  .append("</IsAccepted>")
			  .append("</InitResponseMessage>").toString();
	}

}
