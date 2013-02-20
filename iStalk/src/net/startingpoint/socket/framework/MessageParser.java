package net.startingpoint.socket.framework;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.nio.charset.Charset;
import java.util.ArrayList;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.parsers.SAXParser;
import javax.xml.parsers.SAXParserFactory;

import net.startingpoint.GlobalConstants;
import net.startingpoint.UserLocation;
import net.startingpoint.socket.protocol.GeoPointMessage;
import net.startingpoint.socket.protocol.InitRequestMessage;
import net.startingpoint.socket.protocol.InitResponseMessage;
import net.startingpoint.socket.protocol.MultipleGeoPointRequestMessage;
import net.startingpoint.socket.protocol.MultipleGeoPointResponseMessage;
import net.startingpoint.socket.protocol.RemoveGeoPointMessage;

import org.xml.sax.Attributes;
import org.xml.sax.SAXException;
import org.xml.sax.SAXParseException;
import org.xml.sax.helpers.DefaultHandler;

import com.google.android.maps.GeoPoint;

public class MessageParser extends DefaultHandler {
	
	private SAXParserFactory factory;
	
	public MessageParser() {
		factory = SAXParserFactory.newInstance();
	}
	
	public AbstractMessage parse(String text) throws SAXException, IOException {
		SAXParser parser = null;
		try {
			parser = factory.newSAXParser();
		} catch (ParserConfigurationException e) { }
		
		parser.parse(new ByteArrayInputStream(text.getBytes(Charset.availableCharsets().get("UTF-8"))), this);
		
		if(msg != null) {
			return msg;
		} else {
			throw new SAXParseException("Invalid XML format...", null);
		}
	}
	
	private AbstractMessage msg;
	private String currentString;
	private String element;
	
	private ArrayList<UserLocation> list;
	private UserLocation userLocation;
	private String username;
	private boolean isAccepted;
	private int latitude, longtitude;
	private int gender, bgColor;
	private GeoPoint geoPoint;
	
	@Override
	public void startDocument() throws SAXException {
		msg = null;
		currentString = "";
		element = "";
		isAccepted = false;
		latitude = 0;
		longtitude = 0;
		geoPoint = null;
		list = null;
		gender = GlobalConstants.GENDER_OVERLAYIMAGE_MALE;
		bgColor = GlobalConstants.BG_OVERLAYERIMAGE_ORANGE;
	}
	
	@Override
	public void startElement(String uri, String localName, String qName, Attributes attributes) throws SAXException
	{
		currentString = "";
		element = qName;
		if(element.equals("UserLocations")) {
			list = new ArrayList<UserLocation>();
		}
	}

	@Override
	public void endElement(String uri, String localName, String qName)
	{
		if(qName.equals("InitRequestMessage")) {
			msg = new InitRequestMessage(username);
		} else if(qName.equals("InitResponseMessage")) {
			msg = new InitResponseMessage(isAccepted);
		} else if(qName.equals("GeoPointMessage")) {
			msg = new GeoPointMessage(userLocation);
		} else if(qName.equals("MultipleGeoPointRequestMessage")) {
			msg = new MultipleGeoPointRequestMessage();
		} else if(qName.equals("MultipleGeoPointResponseMessage")) {
			msg = new MultipleGeoPointResponseMessage(list);
		} else if(qName.equals("IsAccepted")) {
			isAccepted = Boolean.parseBoolean(currentString);
		} else if(qName.equals("Latitude")) {
			latitude = Integer.parseInt(currentString);
		} else if(qName.equals("Longtitude")) {
			longtitude = Integer.parseInt(currentString);
		} else if(qName.equals("GeoPoint")) {
			geoPoint = new GeoPoint(latitude, longtitude);
		} else if(qName.equals("UserLocation")) {
			if(list != null) {
				list.add(new UserLocation(username, geoPoint, gender, bgColor));
			} else {
				userLocation = new UserLocation(username, geoPoint, gender, bgColor);
			}
		} else if(qName.equals("Username")) {
			username = currentString;
		} else if(qName.equals("RemoveGeoPointMessage")) {
			msg = new RemoveGeoPointMessage(userLocation);
		} else if(qName.equals("Gender")) {
			gender = Integer.parseInt(currentString);
		} else if(qName.equals("BgColour")) {
			bgColor = Integer.parseInt(currentString);
		}
	}

	@Override
	public void characters(char[] ch, int start, int length)
	{
		String s = new String(ch, start, length);
		currentString += s;
	}
}
