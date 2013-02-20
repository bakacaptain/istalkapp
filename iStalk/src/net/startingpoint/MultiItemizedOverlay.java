package net.startingpoint;

import java.util.List;

import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Matrix;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;

import com.google.android.maps.Overlay;

public class MultiItemizedOverlay {
	
	private final MyItemizedOverlay FEMALE_GRAY, FEMALE_PURPLE, FEMALE_DARK_BLUE,
	FEMALE_LIGHT_BLUE, FEMALE_TURK, FEMALE_GREEN, FEMALE_YELLOW, FEMALE_ORANGE, FEMALE_RED;
	
	private final MyItemizedOverlay MALE_GRAY, MALE_PURPLE, MALE_DARK_BLUE,
	MALE_LIGHT_BLUE, MALE_TURK, MALE_GREEN, MALE_YELLOW, MALE_ORANGE, MALE_RED;
	
	public MultiItemizedOverlay(Resources res) {
		FEMALE_GRAY = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_GRAY, GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE, res));
		FEMALE_PURPLE = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_PURPLE, GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE, res));
		FEMALE_DARK_BLUE = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE, GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE, res));
		FEMALE_LIGHT_BLUE = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_LIGHTBLUE, GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE, res));
		FEMALE_TURK = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_TURKISH, GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE, res));
		FEMALE_GREEN = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_GREEN, GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE, res));
		FEMALE_YELLOW = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_YELLOW, GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE, res));
		FEMALE_ORANGE = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_ORANGE, GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE, res));
		FEMALE_RED = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_RED, GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE, res));
		
		MALE_GRAY = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_GRAY, GlobalConstants.GENDER_OVERLAYIMAGE_MALE, res));
		MALE_PURPLE = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_PURPLE, GlobalConstants.GENDER_OVERLAYIMAGE_MALE, res));
		MALE_DARK_BLUE = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE, GlobalConstants.GENDER_OVERLAYIMAGE_MALE, res));
		MALE_LIGHT_BLUE = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_LIGHTBLUE, GlobalConstants.GENDER_OVERLAYIMAGE_MALE, res));
		MALE_TURK = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_TURKISH, GlobalConstants.GENDER_OVERLAYIMAGE_MALE, res));
		MALE_GREEN = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_GREEN, GlobalConstants.GENDER_OVERLAYIMAGE_MALE, res));
		MALE_YELLOW = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_YELLOW, GlobalConstants.GENDER_OVERLAYIMAGE_MALE, res));
		MALE_ORANGE = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_ORANGE, GlobalConstants.GENDER_OVERLAYIMAGE_MALE, res));
		MALE_RED = new MyItemizedOverlay(bitmapOverlayer(GlobalConstants.BG_OVERLAYERIMAGE_RED, GlobalConstants.GENDER_OVERLAYIMAGE_MALE, res));
	}
	
	public void setOnTapListener(OnTapListener listener) {
		FEMALE_GRAY.setOnTapListener(listener);
		FEMALE_PURPLE.setOnTapListener(listener);
		FEMALE_DARK_BLUE.setOnTapListener(listener);
		FEMALE_LIGHT_BLUE.setOnTapListener(listener);
		FEMALE_TURK.setOnTapListener(listener);
		FEMALE_GREEN.setOnTapListener(listener);
		FEMALE_YELLOW.setOnTapListener(listener);
		FEMALE_ORANGE.setOnTapListener(listener);
		FEMALE_RED.setOnTapListener(listener);
		
		MALE_GRAY.setOnTapListener(listener);
		MALE_PURPLE.setOnTapListener(listener);
		MALE_DARK_BLUE.setOnTapListener(listener);
		MALE_LIGHT_BLUE.setOnTapListener(listener);
		MALE_TURK.setOnTapListener(listener);
		MALE_GREEN.setOnTapListener(listener);
		MALE_YELLOW.setOnTapListener(listener);
		MALE_ORANGE.setOnTapListener(listener);
		MALE_RED.setOnTapListener(listener);
	}
	
	public void removeOnTapListener(OnTapListener listener) {
		FEMALE_GRAY.removeOnTapListener(listener);
		FEMALE_PURPLE.removeOnTapListener(listener);
		FEMALE_DARK_BLUE.removeOnTapListener(listener);
		FEMALE_LIGHT_BLUE.removeOnTapListener(listener);
		FEMALE_TURK.removeOnTapListener(listener);
		FEMALE_GREEN.removeOnTapListener(listener);
		FEMALE_YELLOW.removeOnTapListener(listener);
		FEMALE_ORANGE.removeOnTapListener(listener);
		FEMALE_RED.removeOnTapListener(listener);
		
		MALE_GRAY.removeOnTapListener(listener);
		MALE_PURPLE.removeOnTapListener(listener);
		MALE_DARK_BLUE.removeOnTapListener(listener);
		MALE_LIGHT_BLUE.removeOnTapListener(listener);
		MALE_TURK.removeOnTapListener(listener);
		MALE_GREEN.removeOnTapListener(listener);
		MALE_YELLOW.removeOnTapListener(listener);
		MALE_ORANGE.removeOnTapListener(listener);
		MALE_RED.removeOnTapListener(listener);
	}
	
	public void addAllOverlays(List<Overlay> list) {
		list.add(FEMALE_GRAY);
		list.add(FEMALE_PURPLE);
		list.add(FEMALE_DARK_BLUE);
		list.add(FEMALE_LIGHT_BLUE);
		list.add(FEMALE_TURK);
		list.add(FEMALE_GREEN);
		list.add(FEMALE_YELLOW);
		list.add(FEMALE_ORANGE);
		list.add(FEMALE_RED);

		list.add(MALE_GRAY);
		list.add(MALE_PURPLE);
		list.add(MALE_DARK_BLUE);
		list.add(MALE_LIGHT_BLUE);
		list.add(MALE_TURK);
		list.add(MALE_GREEN);
		list.add(MALE_YELLOW);
		list.add(MALE_ORANGE);
		list.add(MALE_RED);
	}
	
	public void replace(UserLocation loc) {
		switch(loc.BG_COLOUR | loc.GENDER) {
		case GlobalConstants.BG_OVERLAYERIMAGE_GRAY | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_GRAY.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_PURPLE | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_PURPLE.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_DARK_BLUE.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_LIGHTBLUE | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_LIGHT_BLUE.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_TURKISH | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_TURK.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_GREEN | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_GREEN.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_YELLOW | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_YELLOW.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_ORANGE | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_ORANGE.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_RED | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_RED.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_GRAY | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_GRAY.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_PURPLE | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_PURPLE.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_DARK_BLUE.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_LIGHTBLUE | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_LIGHT_BLUE.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_TURKISH | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_TURK.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_GREEN | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_GREEN.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_YELLOW | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_YELLOW.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_ORANGE | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_ORANGE.replace(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_RED | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_RED.replace(loc);
			break;
		}
	}
	
	public void remove(UserLocation loc) {
		switch(loc.BG_COLOUR | loc.GENDER) {
		case GlobalConstants.BG_OVERLAYERIMAGE_GRAY | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_GRAY.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_PURPLE | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_PURPLE.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_DARK_BLUE.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_LIGHTBLUE | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_LIGHT_BLUE.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_TURKISH | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_TURK.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_GREEN | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_GREEN.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_YELLOW | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_YELLOW.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_ORANGE | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_ORANGE.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_RED | GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE:
			FEMALE_RED.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_GRAY | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_GRAY.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_PURPLE | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_PURPLE.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_DARK_BLUE.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_LIGHTBLUE | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_LIGHT_BLUE.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_TURKISH | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_TURK.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_GREEN | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_GREEN.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_YELLOW | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_YELLOW.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_ORANGE | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_ORANGE.remove(loc);
			break;

		case GlobalConstants.BG_OVERLAYERIMAGE_RED | GlobalConstants.GENDER_OVERLAYIMAGE_MALE:
			MALE_RED.remove(loc);
			break;
		}
	}
	
	public void clear() {
		FEMALE_GRAY.clear();
		FEMALE_PURPLE.clear();
		FEMALE_DARK_BLUE.clear();
		FEMALE_LIGHT_BLUE.clear();
		FEMALE_TURK.clear();
		FEMALE_GREEN.clear();
		FEMALE_YELLOW.clear();
		FEMALE_ORANGE.clear();
		FEMALE_RED.clear();
		
		MALE_GRAY.clear();
		MALE_PURPLE.clear();
		MALE_DARK_BLUE.clear();
		MALE_LIGHT_BLUE.clear();
		MALE_TURK.clear();
		MALE_GREEN.clear();
		MALE_YELLOW.clear();
		MALE_ORANGE.clear();
		MALE_RED.clear();
	}
	
	/**
	 * Finds the corresponding image for gender and background, and then creates an overlayered drawable
	 * @param bg
	 * @param gender
	 * @return
	 */
	private Drawable bitmapOverlayer(int bg,int gender, Resources resources){
		
		Bitmap bgImagelayer = null, genderImagelayer = null;
		
		if(gender==GlobalConstants.GENDER_OVERLAYIMAGE_FEMALE){
			genderImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_icon_woman);
		}else{
			genderImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_icon_man);
		}
		
		switch(bg){
		case GlobalConstants.BG_OVERLAYERIMAGE_GRAY:
			bgImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_bg_gry);
			break;
		case GlobalConstants.BG_OVERLAYERIMAGE_PURPLE:
			bgImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_bg_pur);
			break;
		case GlobalConstants.BG_OVERLAYERIMAGE_DARKBLUE:
			bgImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_bg_dbl);
			break;
		case GlobalConstants.BG_OVERLAYERIMAGE_LIGHTBLUE:
			bgImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_bg_lbl);
			break;
		case GlobalConstants.BG_OVERLAYERIMAGE_TURKISH:
			bgImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_bg_tur);
			break;
		case GlobalConstants.BG_OVERLAYERIMAGE_GREEN:
			bgImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_bg_gre);
			break;
		case GlobalConstants.BG_OVERLAYERIMAGE_YELLOW:
			bgImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_bg_yel);
			break;
		case GlobalConstants.BG_OVERLAYERIMAGE_RED:
			bgImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_bg_red);
			break;
		default:
			//Orange
			bgImagelayer = BitmapFactory.decodeResource(resources, R.drawable.geopoint_bg_ora);
			break;
		}
		
		// Create overlayed bitmap containing background and figure
		Bitmap bmOverlay = Bitmap.createBitmap(bgImagelayer.getWidth(), bgImagelayer.getHeight(), bgImagelayer.getConfig());
		Canvas canvas = new Canvas(bmOverlay);
		canvas.drawBitmap(bgImagelayer, new Matrix(), null);
		canvas.drawBitmap(genderImagelayer, new Matrix(), null);
		
		// Make the bitmap into a drawable
		Drawable icon = new BitmapDrawable(bmOverlay);
		return icon;
	}
}
