using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CommonUtilities
{
    /// <summary>
    /// Synchronized Buffer class working as a queue.
    /// If the queue is empty when a thread tries to take out an element, it will wait until there is.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Buffer<T>
    {
        // the underlying queue used by the buffer
        private Queue<T> queue;

        /// <summary>
        /// Constructor for Buffer
        /// </summary>
        public Buffer()
        {
            queue = new Queue<T>();
        }

        /// <summary>
        /// Retrieves the number of messages in the buffer.
        /// </summary>
        public int Count
        {
            get 
            {
                Monitor.Enter(this);        // lock while working
                int res = queue.Count;
                Monitor.Exit(this);         // release lock when done
                return res;
            }
        }

        /// <summary>
        /// Checks if the buffer is empty.
        /// </summary>
        public Boolean IsEmpty
        {
            get
            {
                Monitor.Enter(this);        // lock while working
                Boolean res = queue.Count == 0;
                Monitor.Exit(this);         // release lock when done
                return res;
            }
        }

        /// <summary>
        /// The first element of the buffer.
        /// </summary>
        public T First
        {
            get
            {
                Monitor.Enter(this);        // lock while working
                T element = queue.Peek();
                Monitor.Exit(this);         // release lock when done
                return element;
            }
        }

        /// <summary>
        /// Adds an element to the back of the buffer.
        /// </summary>
        /// <param name="element">The element to be added</param>
        public void Put(T element)
        {
            Monitor.Enter(this);            // lock while working
            queue.Enqueue(element);
            Monitor.PulseAll(this);         // warn waiting potential threads that queue is not empty
            Monitor.Exit(this);             // release lock when done
        }

        /// <summary>
        /// Takes out the first element placed into the queue after FIFO principle.
        /// </summary>
        /// <returns>The element that was first added.</returns>
        public T Take()
        {
            Monitor.Enter(this);            // lock while working
            while (queue.Count == 0)
            {
                Monitor.Wait(this);         // if nothing in queue, wait until there is
            }

            T element = queue.Dequeue();
            Monitor.Exit(this);             // release lock when done
            return element;
        }
    }
}
