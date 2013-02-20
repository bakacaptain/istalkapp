package net.startingpoint;

import android.app.Activity;
import android.app.Service;
import android.content.ComponentName;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.Handler;
import android.os.IBinder;
import android.view.View;

public class MainMenuActivity extends Activity {
	
	/*
	 * *********************************************************************
	 * 								Constants
	 * *********************************************************************
	 */
	
	public static final int IDENTIFIER = 0x0002;
	
	/*
	 * *********************************************************************
	 * 								Fields
	 * *********************************************************************
	 */
	
	private GpsNConnectionService mService;
	
	/*
	 * *********************************************************************
	 * 							Life cycle callback
	 * *********************************************************************
	 */

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
        setContentView(R.layout.mainmenu);
        
        Intent i = new Intent(MainMenuActivity.this, GpsNConnectionService.class);
        i.putExtra(GlobalConstants.IDENTIFIER_NAME, IDENTIFIER);
        bindService(i, mServiceConnection, Service.BIND_AUTO_CREATE);
	}
	
	@Override
	protected void onDestroy() {
		super.onDestroy();
		
        unbindService(mServiceConnection);
	}

	/*
	 * *********************************************************************
	 * 							User interaction
	 * *********************************************************************
	 */
	
	public void onStartMapActivity(View view) {
        Intent i = new Intent(MainMenuActivity.this, MappingActivity.class);
        startActivity(i);
	}
	
	@Override
	public void onBackPressed() {
		mService.disconnect();
		super.onBackPressed();
	}

	/*
	 * *********************************************************************
	 * 				Handler/Dispatcher (easy access to GUI thread)
	 * *********************************************************************
	 */
    
    private Handler mHandler = new Handler() {
    	
    	@Override
    	public void handleMessage(android.os.Message msg) {
    		
    		switch(msg.what) {
    		case GlobalConstants.CONNECTION_LOST:
    			finish();
    			break;
    			
    		default:
    			break;
    		}
    	}
	};

	/*
	 * *********************************************************************
	 * 						Service Connection
	 * *********************************************************************
	 */
    
    private ServiceConnection mServiceConnection = new ServiceConnection() {
		
		@Override
		public void onServiceDisconnected(ComponentName c) {
			mService = null;
		}

		@Override
		public void onServiceConnected(ComponentName c, IBinder b) {
			GpsNConnectionService.LocalBinder binder = (GpsNConnectionService.LocalBinder)b;
			mService = binder.bindMainMenuActivity(mHandler);
		}
	};
	
	private boolean isConnectedToService() {
		return mService != null;
	}
	
	
}
