package net.startingpoint;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.OverlayItem;

public class MyOverlayItem extends OverlayItem {

	public MyOverlayItem(GeoPoint point, String title, String snippet) {
		super(point, title, snippet);
	}

	@Override
	public boolean equals(Object o) {
		if(!(o instanceof MyOverlayItem)) {
			return false;
		}
		
		MyOverlayItem other = (MyOverlayItem)o;
		
		return this.mTitle.equals(other.mTitle);
	}
}
