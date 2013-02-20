package net.startingpoint.socket.framework;

public interface OnConnectingListener {
	public void onConnectionEstablished(TCPConnection connection);
	public void onConnectionNotEstablished(TCPConnection connection, Exception e);
}
