package net.startingpoint.socket.framework;

public interface ConnectionCallbacks {
	public void onMessageReceived(ClientConnection sender, AbstractMessage message);
	public void onConnectionFailed(ClientConnection sender);
	public void onMessageFailed(ClientConnection sender, Exception e);
}
