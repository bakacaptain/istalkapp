package net.startingpoint;

import java.util.ArrayList;
import java.util.List;

import android.graphics.drawable.Drawable;

import com.google.android.maps.ItemizedOverlay;

public class MyItemizedOverlay extends ItemizedOverlay<MyOverlayItem> {

	private List<MyOverlayItem> mOverlays = new ArrayList<MyOverlayItem>();
	private List<OnTapListener> mTapListeners = new ArrayList<OnTapListener>();
	
	public MyItemizedOverlay(Drawable defaultMarker) {
		super(boundCenterBottom(defaultMarker));
		populate();
	}
	
	public void clear() {
		mOverlays.clear();
		populate();
	}

	@Override
	protected MyOverlayItem createItem(int i) {
		return mOverlays.get(i);
	}

	@Override
	public int size() {
		return mOverlays.size();
	}
	
	public void replace(UserLocation loc) {
		MyOverlayItem item = new MyOverlayItem(loc.GEO_POINT, loc.USERNAME, null);
		mOverlays.remove(item);
		mOverlays.add(item);
		populate();
	}
	
	public void remove(UserLocation loc) {
		MyOverlayItem item = new MyOverlayItem(loc.GEO_POINT, loc.USERNAME, null);
		mOverlays.remove(item);
		populate();
	}
	
	@Override
	protected boolean onTap(int index) {
		MyOverlayItem item = mOverlays.get(index);
		for (OnTapListener listener : mTapListeners) {
			listener.onTap(item);
		}
		return false;
	}
	
	public void setOnTapListener(OnTapListener listener) {
		mTapListeners.add(listener);
	}
	
	public void removeOnTapListener(OnTapListener listener) {
		mTapListeners.remove(listener);
	}
}
