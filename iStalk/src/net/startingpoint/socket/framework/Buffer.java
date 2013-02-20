package net.startingpoint.socket.framework;

import java.util.Queue;
import java.util.concurrent.ConcurrentLinkedQueue;

/**
 * An object of this class represents a synchronized queue implemented as a
 * monitor
 * 
 * @version v.100508
 * 
 * @param <T>
 *           is the Class associated with the Buffer. It will be the Objects
 *           that it stores
 */
public class Buffer<T> implements IBuffer<T>
{
	private Queue<T> queue;

	/**
	 * Constructor for Buffer
	 * 
	 * @param user					is the AccountID that will be associated with the buffer
	 * @param controller			is the ServerController for the application
	 */
	public Buffer()
	{
		queue = new ConcurrentLinkedQueue<T>();
	}

	@Override
	public synchronized int count()
	{
		return queue.size();
	}

	@Override
	public synchronized boolean isEmpty()
	{
		return queue.isEmpty();
	}

	@Override
	public synchronized T first()
	{
		return queue.peek();
	}

	@Override
	public synchronized void put(T element)
	{
		queue.offer(element);

		notifyAll();
	}

	@Override
	public synchronized T take()
	{
		while (isEmpty())
		{
			try
			{
				wait();
			}
			catch (InterruptedException e)
			{
			}
		}

		T element = queue.remove();

		notifyAll();

		return element;
	}

	@Override
	public synchronized void notification()
	{
		notifyAll();
	}

}
