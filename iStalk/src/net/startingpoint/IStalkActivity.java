package net.startingpoint;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.app.ProgressDialog;
import android.app.Service;
import android.content.ComponentName;
import android.content.DialogInterface;
import android.content.DialogInterface.OnCancelListener;
import android.content.Intent;
import android.content.ServiceConnection;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.os.Handler;
import android.os.IBinder;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.ContextMenu;
import android.view.ContextMenu.ContextMenuInfo;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.Toast;

public class IStalkActivity extends Activity {
	
	/*
	 * *********************************************************************
	 * 								Constants
	 * *********************************************************************
	 */
	
	public static final int IDENTIFIER = 0x0001;
	public static final int PROGRESS_DIALOG = 0x0001, COLOUR_DIALOG = 0x003, GENDER_DIALOG = 0x009;
	
	private static final String USERNAME_ID = "username",
								HOST_ADDRESS_ID = "hostaddress",
								HOST_PORT_ID = "hostport",
								USER_GENDER_ID = "usergender", 
								USER_COLOUR_ID = "userbgcolour";
	
	private static final String SHARED_PREFS = "susersettings", 
								SHARED_USERNAME = "susername", 
								SHARED_ADDRESS = "shostaddress", 
								SHARED_PORT = "shostport", 
								SHARED_GENDER = "susergender", 
								SHARED_COLOUR = "susercolour", 
								SHARED_SAVE = "susersave";

	private final String[] colourItems = {"Grey","Purple","Dark Blue","Light Blue","Turkish","Green","Yellow","Orange","Red"};
	private final String[] genderItems = {"Male","Female"};
	private int tempColour = 1,tempGender = 0;
	
	/*
	 * *********************************************************************
	 * 								Fields
	 * *********************************************************************
	 */
	
	private GpsNConnectionService mService;
	private String mUsername,mHostAddress;
	private int mHostPort;
	
	private EditText editUsername,editHost,editPort;
	private Button btnConnect;
	private CheckBox chkSaved;
	
	/*
	 * *********************************************************************
	 * 							Life cycle callback
	 * *********************************************************************
	 */
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        
        this.initUI();
        this.addListeners();
        
        if(savedInstanceState != null) {
        	mUsername = savedInstanceState.getString(USERNAME_ID);
        	mHostAddress = savedInstanceState.getString(HOST_ADDRESS_ID);
        	mHostPort = savedInstanceState.getInt(HOST_PORT_ID);
        	
        	tempColour = savedInstanceState.getInt(USER_COLOUR_ID);
        	tempGender = savedInstanceState.getInt(USER_GENDER_ID);
        	
        	editUsername.setText(mUsername);
        	editHost.setText(mHostAddress);
        	editPort.setText(Integer.toString(mHostPort));
        } else {
        	SharedPreferences settings = getSharedPreferences(SHARED_PREFS, 0);
    		chkSaved.setChecked(settings.getBoolean(SHARED_SAVE, false));
    		if(settings.getBoolean(SHARED_SAVE, false)){
    			ToastMe("It was not saved? LOL");
        		mUsername = settings.getString(SHARED_USERNAME, "");
        		mHostAddress = settings.getString(SHARED_ADDRESS, "");
        		mHostPort = settings.getInt(SHARED_PORT, 0);
        		
        		tempGender = settings.getInt(SHARED_GENDER, 0);
        		tempColour = settings.getInt(SHARED_COLOUR, 1);
        		
        		editUsername.setText(mUsername);
            	editHost.setText(mHostAddress);
            	editPort.setText(Integer.toString(mHostPort));
            	
    		}
    }
        
        Intent i = new Intent(IStalkActivity.this, GpsNConnectionService.class);
        i.putExtra(GlobalConstants.IDENTIFIER_NAME, IDENTIFIER);
        bindService(i, mServiceConnection, Service.BIND_AUTO_CREATE);
    }
    
    @Override
    protected void onRestart() {
    	super.onRestart();
    }
    
    @Override
    protected void onStart() {
    	super.onStart();
    }
    
    @Override
    protected void onResume() {
    	super.onResume();
    }
    
    @Override
    protected void onPause() {
    	super.onPause();
    }
    
    @Override
    protected void onStop() {
    	super.onStop();
    	
    	SharedPreferences settings = getSharedPreferences(SHARED_PREFS, 0);
    	SharedPreferences.Editor editor = settings.edit();
    	editor.putBoolean(SHARED_SAVE, chkSaved.isChecked());
    	editor.putString(SHARED_USERNAME, editUsername.getText().toString());
    	editor.putString(SHARED_ADDRESS, editHost.getText().toString());
    	editor.putInt(SHARED_PORT, mHostPort);
    	editor.putInt(SHARED_GENDER, tempGender);
    	editor.putInt(SHARED_COLOUR, tempColour);
    	
    	// Needs a commit ofc!
    	editor.commit();
    }
    
    @Override
    protected void onDestroy() {
    	super.onDestroy();
        unbindService(mServiceConnection);
    }
    
    @Override
    protected void onSaveInstanceState(Bundle outState) {
    	super.onSaveInstanceState(outState);
    	
    	outState.putString(USERNAME_ID, editUsername.getText().toString());
    	outState.putString(HOST_ADDRESS_ID, editHost.getText().toString());
    	outState.putInt(HOST_ADDRESS_ID, mHostPort);
    	outState.putInt(USER_COLOUR_ID, tempColour);
    	outState.putInt(USER_GENDER_ID, tempGender);
    }

	/*
	 * *********************************************************************
	 * 							Dialog creations
	 * *********************************************************************
	 */
    
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
    	MenuInflater inflater = getMenuInflater();
    	inflater.inflate(R.menu.mainmenuitems, menu);
    	return true;
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
    	switch(id) {
    	case PROGRESS_DIALOG:
    		ProgressDialog progressDialog = new ProgressDialog(this);
    		progressDialog.setIndeterminate(true);
    		progressDialog.setButton("Cancel", new Dialog.OnClickListener() {
				@Override
				public void onClick(DialogInterface dialog, int which) {
					dialog.cancel();
				}
			});
    		progressDialog.setOnCancelListener(new OnCancelListener() {
				
				@Override
				public void onCancel(DialogInterface dialog) {
					dialog.dismiss();
					mService.cancelConnect();
				}
			});
    		progressDialog.setMessage("Please wait...");
    		return progressDialog;
    	case COLOUR_DIALOG:
    		AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.setTitle("Choose your own background colour:");
			builder.setSingleChoiceItems(colourItems, tempColour, new DialogInterface.OnClickListener() {
				@Override
				public void onClick(DialogInterface dialog, int which) {
					tempColour = which;
					
				}
			});
			AlertDialog alert = builder.create();
    		return alert;
    	case GENDER_DIALOG:
    		AlertDialog.Builder bobthebuilder = new AlertDialog.Builder(this);
    		bobthebuilder.setTitle("Choose your gender:");
    		bobthebuilder.setSingleChoiceItems(genderItems, tempGender, new DialogInterface.OnClickListener() {
				@Override
				public void onClick(DialogInterface dialog, int which) {
					tempGender = which;
					
				}
			});
			AlertDialog bobalert = bobthebuilder.create();
    		return bobalert;
    	default:
        	return super.onCreateDialog(id);
    	}
    }
    
    @Override
    protected void onPrepareDialog(int id, Dialog dialog) {
    	super.onPrepareDialog(id, dialog);
    }

	/*
	 * *********************************************************************
	 * 							User interaction
	 * *********************************************************************
	 */
    
    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
    	switch(item.getItemId()){
    		case R.id.menu_bgcolour:
    			showDialog(COLOUR_DIALOG);
    			break;
    		case R.id.menu_userGender:
    			showDialog(GENDER_DIALOG);
    			break;
    		default:
    			break;
    	}
    	
    	
    	return true;
    }
    
    @Override
    public boolean onContextItemSelected(MenuItem item) {
    	return super.onContextItemSelected(item);
    }
    
    // additional handler classes

	/*
	 * *********************************************************************
	 * 				Handler/Dispatcher (easy access to GUI thread)
	 * *********************************************************************
	 */
    
    private Handler mHandler = new Handler() {
    	
    	@Override
    	public void handleMessage(android.os.Message msg) {
    		
    		switch(msg.what) {
    		case GlobalConstants.CONNECTION_ESTABLISH_FAILURE:
    			dismissDialog(PROGRESS_DIALOG);
    	        Toast.makeText(IStalkActivity.this, "Could not contact server.", Toast.LENGTH_LONG).show();
    			break;
    			
    		case GlobalConstants.CONNECTION_ESTABLISH_SUCCESS:
    			dismissDialog(PROGRESS_DIALOG);
				Intent i = new Intent(IStalkActivity.this,MainMenuActivity.class);
				startActivity(i);
    			break;
    			
    		case GlobalConstants.CONNECTION_ESTABLISH_BAD_USERNAME:
    			dismissDialog(PROGRESS_DIALOG);
    	        Toast.makeText(IStalkActivity.this, "Username already in use.", Toast.LENGTH_LONG).show();
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
			mService = binder.bindIStalkActivity(mHandler);
		}
	};
	
	private boolean isConnectedToService() {
		return mService != null;
	}
	
	/*
	 * *********************************************************************
	 * 						Sam's GUI
	 * *********************************************************************
	 */
	
	/**
	 * initializes the GUI
	 */
	private void initUI(){
		editUsername = (EditText) findViewById(R.id.editUsername);
		editHost = (EditText) findViewById(R.id.editHost);
		editPort = (EditText) findViewById(R.id.editPort);
		btnConnect = (Button) findViewById(R.id.btnConnect);
		chkSaved = (CheckBox) findViewById(R.id.chkSaved);
	}
	
	/**
	 * adds the respective listeners to the controls
	 */
	private void addListeners(){
		editPort.addTextChangedListener(hostPortWatcher);
		btnConnect.setOnClickListener(connectListener);
	}
	
	/**
	 * Listener for changing to the main/map activity
	 */
	OnClickListener connectListener = new OnClickListener(){
		@Override
		public void onClick(View v) {
			mUsername = editUsername.getText().toString();
			mHostAddress = editHost.getText().toString();
			if(mUsername.equals("")){
				ToastMe("Username Missing");
			}else if(mHostAddress.equals("")){
				ToastMe("Host Missing");
			}else if(mHostPort>65535||mHostPort<0){
				ToastMe("Invalid Port number");
			}else{
				ToastMe("U:"+mUsername+"\nH:"+mHostAddress+"\nP:"+mHostPort);
				//TODO Create intent and go to the map or main screen
				if(isConnectedToService()) {
					mService.connectAsync(mUsername, mHostAddress, mHostPort, sGenderToConstant(tempGender), sColourToConstant(tempColour));
					showDialog(PROGRESS_DIALOG);
				}
			}
		}
	};
	
	/**
	 * Listener for checking valid port
	 */
	TextWatcher hostPortWatcher = new TextWatcher(){

		@Override
		public void afterTextChanged(Editable arg0) {
		}

		@Override
		public void beforeTextChanged(CharSequence arg0, int arg1, int arg2,
				int arg3) {
		}

		@Override
		public void onTextChanged(CharSequence s, int start, int before,
				int count) {
			
			try {
				int temp = Integer.parseInt(s.toString());
				if(temp>65535||temp<0 )
					ToastMe("Invalid Port Number");
				else
					mHostPort = temp;
			} catch (Exception e) {
			}
		}
		
	};
	
	/**
	 * Toast Wrapper to make it easier to make toasts. Nom nom nom :)
	 * @param text
	 */
	public void ToastMe(String text){
		Toast.makeText(IStalkActivity.this, text, Toast.LENGTH_SHORT).show();
	}
	
	/**
	 * Returns the constant value for the colourItems array
	 * @param index
	 * @return
	 */
	private int sColourToConstant(int index){
		switch(index){
		case 0:
			return GlobalConstants.BG_OVERLAYERIMAGE_GRAY;
		case 1:
			return GlobalConstants.BG_OVERLAYERIMAGE_PURPLE;
		case 2:
			return GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE;
		case 3:
			return GlobalConstants.BG_OVERLAYERIMAGE_LIGHTBLUE;
		case 4:
			return GlobalConstants.BG_OVERLAYERIMAGE_TURKISH;
		case 5:
			return GlobalConstants.BG_OVERLAYERIMAGE_GREEN;
		case 6:
			return GlobalConstants.BG_OVERLAYERIMAGE_YELLOW;
		case 7:
			return GlobalConstants.BG_OVERLAYERIMAGE_ORANGE;
		case 8:
			return GlobalConstants.BG_OVERLAYERIMAGE_RED;
		default:
			return -1;
		}
	}
	
	/**
	 * Returns the constant value for the genderItems array
	 * @param index
	 * @return
	 */
	private int sGenderToConstant(int index){
		switch(index){
			case 0:
				return GlobalConstants.GENDER_OVERLAYIMAGE_MALE;
			case 1:
				return GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE;
			default:
				return -1;
		}
	}
}