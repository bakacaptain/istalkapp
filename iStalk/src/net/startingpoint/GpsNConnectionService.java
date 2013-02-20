package net.startingpoint;

import java.io.IOException;
import java.net.UnknownHostException;

import net.startingpoint.socket.framework.AbstractMessage;
import net.startingpoint.socket.framework.ClientConnection;
import net.startingpoint.socket.framework.ConnectionCallbacks;
import net.startingpoint.socket.framework.TCPConnection;
import net.startingpoint.socket.protocol.GeoPointMessage;
import net.startingpoint.socket.protocol.InitRequestMessage;
import net.startingpoint.socket.protocol.InitResponseMessage;
import net.startingpoint.socket.protocol.MultipleGeoPointRequestMessage;
import net.startingpoint.socket.protocol.MultipleGeoPointResponseMessage;
import net.startingpoint.socket.protocol.RemoveGeoPointMessage;
import android.app.Service;
import android.content.Intent;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Binder;
import android.os.Bundle;
import android.os.Handler;
import android.os.HandlerThread;
import android.os.IBinder;
import android.util.Log;

import com.google.android.maps.GeoPoint;

public class GpsNConnectionService extends Service {
	
	/*
	 * *********************************************************************
	 * 							Fields
	 * *********************************************************************
	 */

	private LocalBinder mBinder = new LocalBinder();
	private Handler mIStalkActivityClient;
	private Handler mMappingActivityClient;
	private Handler mMainMenuActivityClient;
	private LocationManager mLocationManager;
	private ClientConnection mConnection;
	private GeoPoint mCurrentPosition;
	
	private int gender = GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE, 
				bgColor = GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE;

	/*
	 * *********************************************************************
	 * 						Life cycle callback
	 * *********************************************************************
	 */
	
	@Override
	public void onCreate() {
		super.onCreate();
		
		mLocationManager = (LocationManager)getSystemService(LOCATION_SERVICE);
		mLocationManager.requestLocationUpdates(LocationManager.NETWORK_PROVIDER, 0, 2.0F, mLocationListener);
		mLocationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, 0, 2.0F, mLocationListener);
	}
	
	@Override
	public void onDestroy() {
		super.onDestroy();
		
		mLocationManager.removeUpdates(mLocationListener);
		if(isConnected()) {
			disconnect();
		}
	}

	/*
	 * *********************************************************************
	 * 						Binding callback
	 * *********************************************************************
	 */
	
	@Override
	public boolean onUnbind(Intent intent) {
		int id = intent.getIntExtra(GlobalConstants.IDENTIFIER_NAME, -1);
		
		if(isBoundToIStalkActivity() && id == IStalkActivity.IDENTIFIER) {
			mIStalkActivityClient = null;
		} else if(isBoundToMappingActivity() && id == MappingActivity.IDENTIFIER) {
			mMappingActivityClient = null;
		} else if(isBoundToMainMenuActivity() && id == MainMenuActivity.IDENTIFIER) {
			mMainMenuActivityClient = null;
		}
		
		return super.onUnbind(intent); // no rebind callback
	}
	
	@Override
	public IBinder onBind(Intent intent) {
		return mBinder;
	}

	/*
	 * *********************************************************************
	 * 						Binder declaration
	 * *********************************************************************
	 */
	
	public class LocalBinder extends Binder {
		
		public GpsNConnectionService bindIStalkActivity(Handler client) {
			mIStalkActivityClient = client;
			return GpsNConnectionService.this;
		}
		
		public GpsNConnectionService bindMappingActivity(Handler client) {
			mMappingActivityClient = client;
			return GpsNConnectionService.this;
		}
		
		public GpsNConnectionService bindMainMenuActivity(Handler client) {
			mMainMenuActivityClient = client;
			return GpsNConnectionService.this;
		}
	}
	
	private boolean isBoundToIStalkActivity() {
		return mIStalkActivityClient != null;
	}
	
	private boolean isBoundToMappingActivity() {
		return mMappingActivityClient != null;
	}
	
	private boolean isBoundToMainMenuActivity() {
		return mMainMenuActivityClient != null;
	}

	/*
	 * *********************************************************************
	 * 							Listeners
	 * *********************************************************************
	 */
	
	private LocationListener mLocationListener = new LocationListener() {
		
		@Override
		public void onStatusChanged(String provider, int status, Bundle extras) {
		}
		
		@Override
		public void onProviderEnabled(String provider) {
		}
		
		@Override
		public void onProviderDisabled(String provider) {
		}
		
		@Override
		public void onLocationChanged(Location location) {
			
			if(isConnected()) {
				mCurrentPosition = new GeoPoint((int)(location.getLatitude() * 1e6),
						(int)(location.getLongitude() * 1e6));
				
				GeoPointMessage msg = new GeoPointMessage(new UserLocation(getUsername(), mCurrentPosition, 
						gender, bgColor));
				
				mMappingActivityClient.obtainMessage(GlobalConstants.GEO_POINT_RECEIVED, msg).sendToTarget();
				mConnection.notify(msg);
			}
			
		}
	};
	
	private ConnectionCallbacks mConnectionListener = new ConnectionCallbacks() {
		
		@Override
		public void onMessageReceived(ClientConnection sender, AbstractMessage message) {
			
			if(message instanceof InitResponseMessage) {
				InitResponseMessage msg = (InitResponseMessage)message;
				
				isAccepted = msg.IS_ACCEPTED;
				timedOut = false;
				synchronized (GpsNConnectionService.this) {
					GpsNConnectionService.this.notify();
				}
				
			} else if(message instanceof GeoPointMessage) {
				GeoPointMessage msg = (GeoPointMessage)message;
				
				if(isBoundToMappingActivity()) {
					mMappingActivityClient.obtainMessage(GlobalConstants.GEO_POINT_RECEIVED, msg).sendToTarget();
				}
				
			} else if(message instanceof MultipleGeoPointResponseMessage) {
				MultipleGeoPointResponseMessage msg = (MultipleGeoPointResponseMessage)message;
				
				if(isBoundToMappingActivity()) {
					mMappingActivityClient.obtainMessage(GlobalConstants.MULTIPLE_GEO_POINT_RECEIVED, msg).sendToTarget();
				}
			} else if(message instanceof RemoveGeoPointMessage) {
				RemoveGeoPointMessage msg = (RemoveGeoPointMessage)message;
				
				if(isBoundToMappingActivity()) {
					mMappingActivityClient.obtainMessage(GlobalConstants.GEO_POINT_REMOVE, msg).sendToTarget();
				}
			}
		}
		
		@Override
		public void onMessageFailed(ClientConnection sender, Exception e) {
			// not used
		}
		
		@Override
		public void onConnectionFailed(ClientConnection sender) {
			
			mConnection = null;
			gender = GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE;
			bgColor = GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE;
			if(isBoundToMappingActivity()) {
				mMappingActivityClient.obtainMessage(GlobalConstants.CONNECTION_LOST).sendToTarget();
			}
			
			if(isBoundToMainMenuActivity()) {
				mMainMenuActivityClient.obtainMessage(GlobalConstants.CONNECTION_LOST).sendToTarget();
			}
		}
	};

	/*
	 * *********************************************************************
	 * 							Client Methods
	 * *********************************************************************
	 */
	
	public void cancelConnect() {
		if(con != null) {
			con.close(); // close connection (will interrupt an attempt to connect)
			synchronized (GpsNConnectionService.this) {
				GpsNConnectionService.this.notify(); // notify thread
			}
		}
	}
	
	private boolean timedOut;
	private boolean isAccepted;
	private TCPConnection con;
	
	public void connectAsync(final String username, final String hostname, final int port, final int gender, final int bgColor) {
		
		if(!isConnected()) {
			HandlerThread connector = new HandlerThread("connector", HandlerThread.NORM_PRIORITY) {
				@Override
				public void run() {
					
					timedOut = true;
					isAccepted = false;
					con = new TCPConnection();
					try {
						con.open(hostname, port);
					} catch (UnknownHostException e) {
						Log.e("Exception", e.getMessage(), e);
					} catch (IOException e) {
						Log.e("Exception", e.getMessage(), e);
					}
					
					if(con.isConnected()) {
						mConnection = new ClientConnection(con, mConnectionListener);
						mConnection.start();
						
						synchronized (GpsNConnectionService.this) {
							mConnection.notify(new InitRequestMessage(username));
							try {
								GpsNConnectionService.this.wait(GlobalConstants.TIME_OUT);
							} catch (InterruptedException e) {
								
							}
						}
						
						if(!timedOut) {
							if(isAccepted) {
								mConnection.getId().setUsername(username);
								GpsNConnectionService.this.gender = gender;
								GpsNConnectionService.this.bgColor = bgColor;
								
								if(isBoundToIStalkActivity()) {
									mIStalkActivityClient.obtainMessage(GlobalConstants.CONNECTION_ESTABLISH_SUCCESS).sendToTarget();
								} else {
									disconnect();
								}
								
							} else {
								disconnect();
								if(isBoundToIStalkActivity()) {
									mIStalkActivityClient.obtainMessage(GlobalConstants.CONNECTION_ESTABLISH_BAD_USERNAME, username).sendToTarget();
								}
							}
							
						} else {
							disconnect();
							if(isBoundToIStalkActivity()) {
								mIStalkActivityClient.obtainMessage(GlobalConstants.CONNECTION_ESTABLISH_FAILURE).sendToTarget();
							}
						}
						
					} else {
						disconnect();
						if(isBoundToIStalkActivity()) {
							mIStalkActivityClient.obtainMessage(GlobalConstants.CONNECTION_ESTABLISH_FAILURE).sendToTarget();
						}
					}
				}
			};
			connector.start();
		} else {
			throw new RuntimeException("Is already connected.");
		}
	}
	
	public boolean disconnect() {
		if(mConnection != null) {
			mConnection.stop();
			mConnection = null;
			gender = GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE;
			bgColor = GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE;
			return true;
		}
		return false;
	}
	
	public boolean isConnected() {
		return mConnection != null && mConnection.isConnected();
	}
	
	public boolean isLoggedOn() {
		return isConnected() && mConnection.getId().getUsername() != null;
	}
	
	public String getUsername() {
		if(mConnection != null && isConnected()) {
			return mConnection.getId().getUsername();
		} else {
			throw new RuntimeException("Not connected");
		}
	}
	
	public void requestAllPositions() {
		if(isConnected()) {
			mConnection.notify(new MultipleGeoPointRequestMessage());
		}
	}
	
	public int getGender() {
		return gender;
	}

	public int getBgColor() {
		return bgColor;
	}
}
