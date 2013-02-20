package net.startingpoint.socket.framework;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.ArrayList;
import java.util.List;

import org.xml.sax.SAXException;

import android.os.HandlerThread;


public class TCPConnection
{
	private Socket socket;
	private BufferedReader in;
	private BufferedWriter out;
	private MessageParser parser;
	public boolean opening;
	
	private List<OnConnectingListener> listeners = new ArrayList<OnConnectingListener>();
	
	public TCPConnection() {
		parser = new MessageParser();
		socket = new Socket();
		opening = false;
	}
	
	TCPConnection(Socket socket) throws UnknownHostException, IOException {
		opening = true;
		this.socket = socket;
		in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
		out = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
		parser = new MessageParser();
	}
	
	public void openAsync(String ip, int port) {
		if(!opening) {
			opening = true;
			Connector connector = new Connector(ip, port);
			connector.start();
		}
	}
	
	public void open(String ip, int port) throws UnknownHostException, IOException {
		if(!opening) {
			opening = true;
			socket.connect(new InetSocketAddress(ip, port));
			in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
			out = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
		}
	}
	
	public void close() {
		try {
			socket.close();
		} catch (IOException e) {
		}
	}
	
	public InetAddress getRemoteAddress() {
		return socket.getInetAddress();
	}
	
	public InetAddress getLocalAddress() {
		return socket.getLocalAddress();
	}
	
	public void write(AbstractMessage msg) throws IOException {
		out.write(msg.toXml() + "\n");
		out.flush();
	}
	
	public boolean isConnected() {
		return socket != null && socket.isConnected();
	}
	
	public AbstractMessage read() throws SAXException, IOException
	{
		return parser.parse(in.readLine());
	}
	
	public void setOnConnectingListener(OnConnectingListener listener) {
		listeners.add(listener);
	}
	
	public void removeOnConnectingListener(OnConnectingListener listener) {
		listeners.remove(listener);
	}
	
	private class Connector extends HandlerThread {
		
		private String hostname;
		private int port;

		public Connector(String hostname, int port) {
			super("connector", HandlerThread.NORM_PRIORITY);
			this.hostname = hostname;
			this.port = port;
		}
		
		@Override
		public void run() {
			try {
				socket.connect(new InetSocketAddress(hostname, port));
				in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
				out = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
				
				for(OnConnectingListener listener : listeners) {
					listener.onConnectionEstablished(TCPConnection.this);
				}
			} catch (Exception e) {
				for(OnConnectingListener listener : listeners) {
					listener.onConnectionNotEstablished(TCPConnection.this, e);
				}
			}
		}
	}
}
