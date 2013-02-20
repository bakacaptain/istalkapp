package net.startingpoint.socket.framework;


public class ClientID  {
	private String username;
	
	public ClientID(String username) {
		this.username = username;
	}
	
	public ClientID() {
		this.username = null;
	}
	
	public String getUsername() {
		return username;
	}
	
	public void setUsername(String username) {
		this.username = username;
	}
	
	public boolean isAuthenticated() {
		return username != null;
	}
	
	public boolean equals(Object obj) {
		if(!(obj instanceof ClientID)) {
			return false;
		}
		
		ClientID other = (ClientID)obj;
		
		if(this.username != null && other.username != null && username.equals(other.username)) {
			return true;
		}
		return false;
	}
}
