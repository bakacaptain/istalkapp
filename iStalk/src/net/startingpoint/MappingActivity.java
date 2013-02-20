package net.startingpoint;

import net.startingpoint.socket.protocol.GeoPointMessage;
import net.startingpoint.socket.protocol.MultipleGeoPointResponseMessage;
import net.startingpoint.socket.protocol.RemoveGeoPointMessage;
import android.app.AlertDialog;
import android.app.Dialog;
import android.app.Service;
import android.content.ComponentName;
import android.content.DialogInterface;
import android.content.DialogInterface.OnClickListener;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.Handler;
import android.os.IBinder;
import android.view.ContextMenu;
import android.view.ContextMenu.ContextMenuInfo;
import android.view.Menu;
import android.view.View;

import com.google.android.maps.MapActivity;
import com.google.android.maps.MapView;

public class MappingActivity extends MapActivity {

	/*
	 * *********************************************************************
	 * Constants
	 * *********************************************************************
	 */

	public static final int IDENTIFIER = 0x0003;

	/*
	 * *********************************************************************
	 * Fields
	 * *********************************************************************
	 */

	private MultiItemizedOverlay mMapItemOverlay;
	private MapView mMapView;
	private GpsNConnectionService mService;

	/*
	 * *********************************************************************
	 * Life cycle callbacks
	 * *********************************************************************
	 */

	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.mapping);
		mMapView = (MapView) findViewById(R.id.mapview);
		mMapView.setBuiltInZoomControls(true);

		mMapItemOverlay = new MultiItemizedOverlay(this.getResources());
		mMapItemOverlay.setOnTapListener(mTapListener);
		mMapItemOverlay.addAllOverlays(mMapView.getOverlays());

		Intent i = new Intent(this, GpsNConnectionService.class);
		i.putExtra(GlobalConstants.IDENTIFIER_NAME, IDENTIFIER);
		bindService(i, mServiceConnection, Service.BIND_AUTO_CREATE);
	}

	@Override
	protected void onStart() {
		super.onStart();
	}

	@Override
	protected void onRestart() {
		super.onRestart();
	}

	@Override
	protected void onPause() {
		super.onPause();
	}

	@Override
	protected void onStop() {
		super.onStop();
	}

	@Override
	protected void onDestroy() {
		super.onDestroy();

		unbindService(mServiceConnection);
	}

	/*
	 * *********************************************************************
	 * User Interaction
	 * *********************************************************************
	 */

	@Override
	protected boolean isRouteDisplayed() {
		return false;
	}

	/*
	 * *********************************************************************
	 * 							Dialog creations
	 * *********************************************************************
	 */
    
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
    	return super.onCreateOptionsMenu(menu);
    }
    
    @Override
    public boolean onPrepareOptionsMenu(Menu menu) {
    	return super.onPrepareOptionsMenu(menu);
    }
    
    @Override
    public void onCreateContextMenu(ContextMenu menu, View v,
    		ContextMenuInfo menuInfo) {
    	super.onCreateContextMenu(menu, v, menuInfo);
    }
    
    @Override
    protected Dialog onCreateDialog(int id) {
    	return super.onCreateDialog(id);
    }
    
    @Override
    protected void onPrepareDialog(int id, Dialog dialog) {
    	super.onPrepareDialog(id, dialog);
    }

	/*
	 * *********************************************************************
	 * Handler/Dispatcher (easy access to GUI thread)
	 * *********************************************************************
	 */

	private Handler mHandler = new Handler() {

		@Override
		public void handleMessage(android.os.Message msg) {
			switch (msg.what) {
			case GlobalConstants.GEO_POINT_RECEIVED:
				GeoPointMessage message = (GeoPointMessage) msg.obj;
				mMapItemOverlay.replace(message.USER_LOCATION);
				mMapView.invalidate();
				break;

			case GlobalConstants.MULTIPLE_GEO_POINT_RECEIVED:
				MultipleGeoPointResponseMessage message2 = (MultipleGeoPointResponseMessage) msg.obj;
				mMapItemOverlay.clear();
				for (UserLocation loc : message2.USER_LOCATIONS) {
					mMapItemOverlay.replace(loc);
				}
				mMapView.invalidate();
				break;
				
			case GlobalConstants.GEO_POINT_REMOVE:
				RemoveGeoPointMessage message3 = (RemoveGeoPointMessage) msg.obj;
				mMapItemOverlay.remove(message3.USER_LOCATION);
				mMapView.invalidate();
				break;
				
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
	 * Service Connection
	 * *********************************************************************
	 */

	private ServiceConnection mServiceConnection = new ServiceConnection() {

		@Override
		public void onServiceDisconnected(ComponentName c) {
			mService = null;
		}

		@Override
		public void onServiceConnected(ComponentName c, IBinder b) {
			GpsNConnectionService.LocalBinder binder = (GpsNConnectionService.LocalBinder) b;
			mService = binder.bindMappingActivity(mHandler);

			mService.requestAllPositions();
		}
	};

	public boolean isConnectedToService() {
		return mService != null;
	}

	private OnTapListener mTapListener = new OnTapListener() {

		@Override
		public void onTap(MyOverlayItem item) {
			
	    	AlertDialog.Builder builder = new AlertDialog.Builder(MappingActivity.this);
	    	builder.setMessage("This is " + item.getTitle() + ".")
	    	.setCancelable(false)
	    	.setPositiveButton("Okay", new OnClickListener() {
				
				@Override
				public void onClick(DialogInterface dialog, int which) {
					dialog.dismiss();
				}
			});
	    	AlertDialog d = builder.create();
	    	d.show();
		}
	};
}
