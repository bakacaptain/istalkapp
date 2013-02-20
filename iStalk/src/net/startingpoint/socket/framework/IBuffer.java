package net.startingpoint.socket.framework;

/**
 * An interface representing methods to be implemented for a buffer
 * 
 * @version v.100508
 *
 * @param <T>
 *           is the Class associated with the Buffer. It will be the Objects
 *           that it stores
 */
public interface IBuffer<T>
{
	/**
	 * Put an element into the buffer
	 * 
	 * @param element is the element you want to add
	 */
	void put(T element);
	
	/**
	 * Takes an element out of the buffer
	 * 
	 * @return the element taken out of it
	 */
	T take();
	
	/**
	 * Looks at what the next element to be returned is
	 * 
	 * @return the next element to be returned
	 */
	T first();
	
	/**
	 * Method to check whether or not the buffer is empty
	 * 
	 * @return a boolean representing whether or not the buffer is empty
	 */
	boolean isEmpty();
	
	/**
	 * Method to count the number of elements inside the buffer
	 * 
	 * @return an int with the number of elements
	 */
	int count();
	
	/**
	 * notifies all suspended thread working on the buffer
	 */
	void notification();
}
