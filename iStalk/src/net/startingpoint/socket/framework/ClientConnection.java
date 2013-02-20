package net.startingpoint.socket.framework;

import java.net.InetAddress;

import org.xml.sax.SAXException;

import android.os.HandlerThread;


public class ClientConnection {
	
	private ConnectionCallbacks callbacks;
	
	private Buffer<AbstractMessage> buffer;
	private TCPConnection connection;
	private MessageReader readerThr;
	private MessageWriter writerThr;
	private ClientID id;
	
	private boolean terminating;
	private boolean hasStarted;
	
	public ClientConnection(TCPConnection connection, ConnectionCallbacks callbacks) {
		this.buffer = new Buffer<AbstractMessage>();
		this.callbacks = callbacks;
		this.connection = connection;
		this.id = new ClientID();
		
		terminating = false;
		hasStarted = false;
	}
	
	public boolean isConnected() {
		return connection.isConnected();
	}
	
	public ClientID getId() {
		return id;
	}
	
	public InetAddress getRemoteAddress() {
		return connection.getRemoteAddress();
	}
	
	public InetAddress getLocalAddress() {
		return connection.getLocalAddress();
	}
	
	public void start() {
		if(!hasStarted) {
			terminating = false;
			hasStarted = true;
			
			writerThr = new MessageWriter("writer", HandlerThread.NORM_PRIORITY);
			readerThr = new MessageReader("reader", HandlerThread.NORM_PRIORITY);
			writerThr.setDaemon(true);
			readerThr.setDaemon(true);
			
			readerThr.start();
			writerThr.start();
		}
	}
	
	public void stop() {
		try {
			terminating = true;
			hasStarted = false;
			connection.close();
			if(writerThr != null && readerThr != null) {
				writerThr.interrupt();
				readerThr.interrupt();
			}
		} catch(Exception e) {
			
		}
	}
	
	public void notify(AbstractMessage msg) {
		buffer.put(msg);
	}
	
	private synchronized void internalTerminationProcedure() {
		if(!terminating) { // <-- Important, otherwise might be called twice!
			terminating = true;
			stop();
			callbacks.onConnectionFailed(this);
		}
	}
	
	@Override
	public boolean equals(Object o) {
		if(!(o instanceof ClientConnection))
			return false;
		
		ClientConnection other = (ClientConnection)o;
		
		if(getId().isAuthenticated() && other.getId().isAuthenticated() && getId().getUsername().equals(other.getId().getUsername()))
			return true;
		else if(getRemoteAddress().equals(other.getRemoteAddress()))
			return true;
		
		return false;
	}
	
	private class MessageWriter extends HandlerThread {
		
		public MessageWriter(String name, int priority) {
			super(name, priority);
		}

		public void run() {
			try {
				while(true) {
					AbstractMessage msg = buffer.take();
					connection.write(msg);
				}
			} catch(Exception e) {
				Thread end = new HandlerThread("END") {
					
					@Override
					public void run() {
						internalTerminationProcedure();
					}
				};
				end.start();
			}
		}
	};
	
	private class MessageReader extends HandlerThread {
		
		public MessageReader(String name, int priority) {
			super(name, priority);
		}

		public void run() {
			try {
				while(true) {
					try {
						AbstractMessage msg = connection.read();
						
						callbacks.onMessageReceived(ClientConnection.this, msg);
					} catch (SAXException e) {
						callbacks.onMessageFailed(ClientConnection.this, e);
					}
				}
			} catch(Exception e) {
				Thread end = new HandlerThread("END") {
					@Override
					public void run() {
						internalTerminationProcedure();
					}
				};
				end.start();
			}
		}
	};
	
}
