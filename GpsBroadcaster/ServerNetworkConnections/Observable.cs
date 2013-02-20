using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonUtilities;
using System.Threading;
using System.Net;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using System.Windows.Threading;

namespace ServerNetworkConnections
{
    /// <summary>
    /// Thread-safe collection class meant to hold key-pair values of ClientID and ClientConnections.
    /// </summary>
    public class Observable : DispatcherObject, IEnumerable<ClientConnection>, INotifyCollectionChanged
    {
        #region Fields
        // Dictionary holding all ClientConnections associated with their given ClientID.
        private List<MyKeyValuePair<ClientID, ClientConnection>> identifiedConnections;
        // Lock used to access the above dictionary. Used to make class thread-safe.
        private ReaderWriterLock identifiedLocker;

        // Dictionary holding all ClientConnections associated with the EndPoint
        private List<MyKeyValuePair<EndPoint, ClientConnection>> unidentifiedConnections;
        // Lock to access the above dictionary. Used to make class thread-safe
        private ReaderWriterLock unidentifiedLocker;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for Observable.
        /// </summary>
        public Observable()
        {
            identifiedLocker = new ReaderWriterLock();
            identifiedConnections = new List<MyKeyValuePair<ClientID, ClientConnection>>();
            unidentifiedLocker = new ReaderWriterLock();
            unidentifiedConnections = new List<MyKeyValuePair<EndPoint, ClientConnection>>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a collection of Strings representing all authenticated clients.
        /// Since each connected client is connected through a username, we are 
        /// just returning a collection of these usernames.
        /// </summary>
        /*public ClientConnection[] IdentifiedConnections
        {
            get
            {
                identifiedLocker.AcquireReaderLock(0);
                ClientConnection[] connections = new ClientConnection[identifiedConnections.Count];
                identifiedConnections.Values.CopyTo(connections, 0);
                identifiedLocker.ReleaseReaderLock();
                return connections;
            }
        }

        public ClientConnection[] UnidentifiedConnections
        {
            get
            {
                unidentifiedLocker.AcquireReaderLock(0);
                ClientConnection[] connections = new ClientConnection[unidentifiedConnections.Count];
                unidentifiedConnections.Values.CopyTo(connections, 0);
                unidentifiedLocker.ReleaseReaderLock();
                return connections;
            }
        }*/
        #endregion 

        #region PublicMethods
        /// <summary>
        /// Adds the given ClientConnection to the collection.
        /// </summary>
        /// <param name="con">Is the ClientConnection to be added</param>
        public void AddClient(ClientID id, ClientConnection con)
        {
            MyKeyValuePair<ClientID, ClientConnection> val = new MyKeyValuePair<ClientID, ClientConnection>(id, con);
            identifiedLocker.AcquireReaderLock(0);
            if (!identifiedConnections.Contains(val))
            {
                identifiedLocker.UpgradeToWriterLock(0);
                Int32 count = identifiedConnections.Count;
                identifiedConnections.Add(val);
                identifiedLocker.ReleaseLock();

                if (CollectionChanged != null && CheckAccess())
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, con, count));
                else if (CollectionChanged != null)
                    Dispatcher.BeginInvoke(CollectionChanged, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, con, count));
            }
            else
            {
                identifiedLocker.ReleaseLock();
                throw new InvalidOperationException("User with the given identifier already exists.");
            }
            
        }

        public void AddClient(EndPoint id, ClientConnection con)
        {
            unidentifiedLocker.AcquireWriterLock(0);
            Int32 count = unidentifiedConnections.Count;
            unidentifiedConnections.Add(new MyKeyValuePair<EndPoint, ClientConnection>(id, con));
            unidentifiedLocker.ReleaseWriterLock();

            if (CollectionChanged != null && CheckAccess())
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, con, identifiedConnections.Count + count));
            else if (CollectionChanged != null)
                Dispatcher.BeginInvoke(CollectionChanged, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, con, identifiedConnections.Count + count));
        }

        public ClientConnection RemoveClient(ClientConnection con)
        {
            if (con.ClientID.IsAuthenticated)
                return RemoveClient(con.ClientID);
            else
                return RemoveClient(con.RemoteEndPoint);
        }

        /// <summary>
        /// Removes the ClientConnection with the given id from the collection.
        /// </summary>
        /// <param name="id">Is the ClientID of ClientConnection to be removed.</param>
        /// <returns>Is the removed ClientConnection or null if none were removed.</returns>
        public ClientConnection RemoveClient(ClientID id)
        {
            identifiedLocker.AcquireReaderLock(0); // Start with reader lock
            Int32 index = identifiedConnections.IndexOf(new MyKeyValuePair<ClientID, ClientConnection>(id, null));
            if (index != -1)
            {
                identifiedLocker.UpgradeToWriterLock(0); // Now we need to remove, upgrade lock!
                ClientConnection ret = identifiedConnections[index].Value;
                identifiedConnections.RemoveAt(index);
                identifiedLocker.ReleaseLock();

                if (CollectionChanged != null && CheckAccess())
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ret, index));
                else if (CollectionChanged != null)
                    Dispatcher.BeginInvoke(CollectionChanged, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ret, index));

                return ret;
            }
            identifiedLocker.ReleaseLock(); // Release reader lock
            return null;
        }

        public ClientConnection RemoveClient(EndPoint id)
        {
            unidentifiedLocker.AcquireReaderLock(0); // Start with reader lock
            Int32 index = unidentifiedConnections.IndexOf(new MyKeyValuePair<EndPoint, ClientConnection>(id, null));
            if (index != -1)
            {
                unidentifiedLocker.UpgradeToWriterLock(0); // Now we need to remove, upgrade lock!
                ClientConnection ret = unidentifiedConnections[index].Value;
                unidentifiedConnections.RemoveAt(index);
                unidentifiedLocker.ReleaseLock(); // Release writer lock

                if (CollectionChanged != null && CheckAccess())
                    CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ret, identifiedConnections.Count + index));
                else if (CollectionChanged != null)
                    Dispatcher.BeginInvoke(CollectionChanged, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ret, identifiedConnections.Count + index));

                return ret;
            }
            unidentifiedLocker.ReleaseLock(); // Release reader lock
            return null;
        }

        /// <summary>
        /// Checks if a ClientConnection with the given ClientID exists in the collection.
        /// </summary>
        /// <param name="id">Is the ClientID that we should check if a ClientConnection exists for.</param>
        /// <returns>Is a boolean indicating whether or not the ClientConnection existed.</returns>
        public Boolean Contains(ClientID id)
        {
            identifiedLocker.AcquireReaderLock(0); // reader lock
            Boolean ret = identifiedConnections.Contains(new MyKeyValuePair<ClientID, ClientConnection>(id, null));
            identifiedLocker.ReleaseReaderLock(); // release lock
            return ret;
        }

        /// <summary>
        /// Gets the ClientConnection with the given ClientID.
        /// </summary>
        /// <param name="id">Is the CliendID of the ClientConnection to be retrieved.</param>
        /// <returns>The ClientConnection represented by the given ClientID, or null if it does not exist.</returns>
        public ClientConnection this[ClientID id]
        {
            get
            {
                ClientConnection ret = null;
                identifiedLocker.AcquireReaderLock(0);
                Int32 index = identifiedConnections.IndexOf(new MyKeyValuePair<ClientID, ClientConnection>(id, null));
                if (index != -1)
                    ret = identifiedConnections[index].Value;
                identifiedLocker.ReleaseReaderLock();
                return ret;
            }
        }

        public ClientConnection this[EndPoint id]
        {
            get
            {
                ClientConnection ret = null;
                Int32 index = unidentifiedConnections.IndexOf(new MyKeyValuePair<EndPoint, ClientConnection>(id, null));
                unidentifiedLocker.AcquireReaderLock(0);
                if (index != -1)
                    ret = unidentifiedConnections[index].Value;
                unidentifiedLocker.ReleaseReaderLock();
                return ret;
            }
        }

        /// <summary>
        /// Notifies all Clients with the given AbstractMessage.
        /// </summary>
        /// <param name="msg">Is the AbstractMessage all clients should be notified with.</param>
        public void NotifyAllClients(AbstractMessage msg)
        {
            identifiedLocker.AcquireReaderLock(0);
            foreach (MyKeyValuePair<ClientID, ClientConnection> con in identifiedConnections)
            {
                con.Value.Notify(msg);
            }
            identifiedLocker.ReleaseReaderLock();

            unidentifiedLocker.AcquireReaderLock(0);
            foreach (MyKeyValuePair<EndPoint, ClientConnection> con in unidentifiedConnections)
            {
                con.Value.Notify(msg);
            }
            unidentifiedLocker.ReleaseReaderLock();

        }

        /// <summary>
        /// Notifies all authenticated clients except the ClientConnection with the given ClientID.
        /// </summary>
        /// <param name="id">Is the ClientID of the ClientConnection that should not be notified.</param>
        /// <param name="msg">Is the AbstractMessage the  clients should be notified with.</param>
        public void NotifyAllAuthenticatedClientsExcept(ClientID id, AbstractMessage msg)
        {
            identifiedLocker.AcquireReaderLock(0);
            MyKeyValuePair<ClientID, ClientConnection> thisKey = new MyKeyValuePair<ClientID, ClientConnection>(id, null);
            foreach (MyKeyValuePair<ClientID, ClientConnection> val in identifiedConnections)
            {
                if (!val.Key.Equals(thisKey))
                {
                    val.Value.Notify(msg);
                }
            }
            identifiedLocker.ReleaseReaderLock();
        }

        /// <summary>
        /// Authenticates the given ClientConnecction with the given username.
        /// </summary>
        /// <param name="con">Is the ClientConnection to be authenticated.</param>
        /// <param name="username">Is the username to authenticate the ClientConnection with.</param>
        public void AuthenticateClientWithUsername(EndPoint id, String username)
        {
            unidentifiedLocker.AcquireWriterLock(0);
            ClientConnection con = unidentifiedConnections[unidentifiedConnections.IndexOf(new MyKeyValuePair<EndPoint, ClientConnection>(id, null))].Value;
            unidentifiedLocker.ReleaseWriterLock();

            con.ClientID.Username = username;

            identifiedLocker.AcquireWriterLock(0);
            identifiedConnections.Add(new MyKeyValuePair<ClientID, ClientConnection>(con.ClientID, con));
            identifiedLocker.ReleaseWriterLock();

            if (CollectionChanged != null && CheckAccess())
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            else if (CollectionChanged != null)
                Dispatcher.BeginInvoke(CollectionChanged, this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        
        /// <summary>
        /// Notifies the ClientConnection with the given ClientID with the given AbstractMessage.
        /// </summary>
        /// <param name="id">The ClientID of the ClientConnection to be notified.</param>
        /// <param name="msg">The AbstractMessage that the ClientConnection should be notified with.</param>
        public void NotifyClient(ClientID id, AbstractMessage msg)
        {
            identifiedLocker.AcquireReaderLock(0);
            identifiedConnections[identifiedConnections.IndexOf(new MyKeyValuePair<ClientID,ClientConnection>(id, null))].Value.Notify(msg);
            identifiedLocker.ReleaseReaderLock();
        }

        /// <summary>
        /// Notifies the ClientConnection with the given ClientIDs specified in the collection with the AbstractMessage.
        /// </summary>
        /// <param name="ids">Is an IEnumerable of ClientIDs containing the IDs of the ClientConnections to be notified of the AbstractMessage.</param>
        /// <param name="msg">The AbstractMessage the ClientConnections should be notified of.</param>
        public void NotifyClients(IEnumerable<ClientID> ids, AbstractMessage msg)
        {
            foreach (ClientID id in ids)
            {
                NotifyClient(id, msg);
            }
        }
        #endregion

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<ClientConnection> GetEnumerator()
        {
            return new MyEnumerator(identifiedConnections, unidentifiedConnections, identifiedLocker, unidentifiedLocker);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MyEnumerator(identifiedConnections, unidentifiedConnections, identifiedLocker, unidentifiedLocker);
        }

        private class MyEnumerator : IEnumerator<ClientConnection>
        {
            private IEnumerator first;
            private IEnumerator second;

            private ReaderWriterLock identifiedLocker;
            private ReaderWriterLock unidentifiedLocker;

            private Boolean useFirst = true;

            public MyEnumerator(List<MyKeyValuePair<ClientID, ClientConnection>> identified, List<MyKeyValuePair<EndPoint, ClientConnection>> unidentified,
                ReaderWriterLock identifiedLocker, ReaderWriterLock unidentifiedLocker)
            {
                this.identifiedLocker = identifiedLocker;
                this.unidentifiedLocker = unidentifiedLocker;

                identifiedLocker.AcquireReaderLock(0);
                unidentifiedLocker.AcquireReaderLock(0);

                ClientConnection[] f = new ClientConnection[identified.Count];
                Int32 i = 0;
                foreach (MyKeyValuePair<ClientID, ClientConnection> keyPair in identified)
                {
                    f[i++] = keyPair.Value;
                }
                first = f.GetEnumerator();

                ClientConnection[] s = new ClientConnection[unidentified.Count];
                i = 0;
                foreach (MyKeyValuePair<EndPoint, ClientConnection> keyPair in unidentified)
                {
                    f[i++] = keyPair.Value;
                }
                second = s.GetEnumerator();

                identifiedLocker.ReleaseReaderLock();
                unidentifiedLocker.ReleaseReaderLock();
            }

            public ClientConnection Current
            {
                get 
                {
                    if (useFirst)
                        return first.Current as ClientConnection;
                    else
                        return second.Current as ClientConnection;
                }
            }

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get
                {
                    if (useFirst)
                        return first.Current;
                    else
                        return second.Current;
                }
            }

            public bool MoveNext()
            {
                if (useFirst)
                {
                    useFirst = first.MoveNext();
                    if (useFirst)
                        return true;
                }

                return second.MoveNext();
            }

            public void Reset()
            {
                first.Reset();
                second.Reset();
            }
        }

        private class MyKeyValuePair<TKey, TValue>
        {
            public TKey Key { get; set; }
            public TValue Value { get; set; }

            public MyKeyValuePair(TKey key, TValue val)
            {
                Key = key;
                Value = val;
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                return Key.Equals((obj as MyKeyValuePair<TKey, TValue>).Key);
            }

            // override object.GetHashCode
            public override int GetHashCode()
            {
                return Key.GetHashCode();
            }
        }
    }
}
