using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.ComponentModel;

namespace ServerNetworkConnections
{
    /// <summary>
    /// Class that is able to listen for incoming TCP requests.
    /// </summary>
    public class ClientConnectionListener : INotifyPropertyChanged
    {
        #region Delegates
        public delegate void OnInitialConnectionFailed(Object sender, InitialConnectionFailedEventArgs args);
        public delegate void OnInitialConnectionSuccess(Object sender, InitialConnectionSuccessEventArgs args);
        #endregion

        #region Events
        // Event called when an incomming initial connection fails
        public event OnInitialConnectionFailed InitialConnectionFailed;
        // Event called when a tcpconnection attaches itself.
        public event OnInitialConnectionSuccess InitialConnectionSuccess;
        // Event called when a message is received.
        public event ClientConnection.OnMessageReceived MessageReceived;
        // Event called when a connection fails.
        public event ClientConnection.OnConnectionFailed ConnectionFailed;
        // Event called when a corrupted message was received.
        public event ClientConnection.OnMessageFailed MessageFailed;
        // Event that informs when an important property on this object changes
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Fields
        // The underlying class listening for TCPConnections.
        private TCPServerConnector listener;
        // The thread used to listen.
        private Thread thr;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for ClientConnectionListener.
        /// </summary>
        /// <param name="port">This is a UInt16 representing the port to listen to.</param>
        /// <param name="MessageReceived">This is a delegate representing the callback to be fired when a 
        /// ClientConnection connected through this listener is sends a message</param>
        /// <param name="ConnectionFailed">This is a delegate representing the callback to be fired when a 
        /// ClientConnection connected through this listener fails</param>
        /// <param name="InitialConnectionFailure">This is a delegate representing the callback to fired 
        /// when an client tried to connect, but for whatever reason, failed in doing so</param>
        /// <param name="InitialConnectionSuccess">This is a delegate representing the callback to be fired 
        /// when a client connected through this listener</param>
        /// <param name="MessageFailed">This is a delegate representing the callback to be fired when a 
        /// ClientConnection connected through this listener sends a corrupted message</param>
        public ClientConnectionListener(UInt16 port, AbstractMessageParser parser)
        {
            this.listener = new TCPServerConnector(port, parser);
        }
        #endregion

        #region Properties
        /// <summary>
        /// The port that the ClientConnectionListener listens to.
        /// </summary>
        public UInt16 Port
        {
            get { return listener.Port; }
            set
            {
                if (IsAccepting)
                {
                    StopAccepting();
                    listener.Port = value;
                    StartAccepting();

                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Port"));
                }
                else
                {
                    listener.Port = value;

                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Port"));
                }
            }
        }

        /// <summary>
        /// A Boolean    indicating whether or not the ClientConnectionListener is currently accepting.
        /// </summary>
        public Boolean IsAccepting
        {
            get { return thr != null && thr.IsAlive; }
        }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Method to make the ClientConnectionListener start listening.
        /// </summary>
        public void StartAccepting()
        {
            if (!IsAccepting)
            {
                listener.Start();
                thr = new Thread(Go);
                thr.Priority = ThreadPriority.Highest;
                thr.IsBackground = true;
                thr.Start();

                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsAccepting"));
            }
        }

        /// <summary>
        /// Method to make the ClientConnectionListener stop listening.
        /// </summary>
        public void StopAccepting()
        {
            if (IsAccepting)
            {
                thr.Abort();
                listener.Stop();
                thr = null;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsAccepting"));
            }
        }
        #endregion

        #region ListeningMethod
        /// <summary>
        /// Method listening for incoming connections.
        /// </summary>
        private void Go()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        TCPConnection con = listener.waitForConnection();
                        ClientConnection client = new ClientConnection(con);
                        client.MessageFailed += MessageFailed;
                        client.MessageReceived += MessageReceived;
                        client.ConnectionFailed += ConnectionFailed;

                        if (InitialConnectionSuccess != null)
                        {
                            InitialConnectionSuccessEventArgs args = new InitialConnectionSuccessEventArgs();
                            args.Client = client;
                            InitialConnectionSuccess(this, args);
                        }
                    }
                    catch (SocketException e)
                    {
                        if (InitialConnectionFailed != null)
                        {
                            InitialConnectionFailedEventArgs args = new InitialConnectionFailedEventArgs();
                            args.Cause = e;
                            InitialConnectionFailed(this, args);
                        }
                    }
                    catch (InvalidOperationException e)
                    {
                        if (InitialConnectionFailed != null)
                        {
                            InitialConnectionFailedEventArgs args = new InitialConnectionFailedEventArgs();
                            args.Cause = e;
                            InitialConnectionFailed(this, args);
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // do something ?
            }
        }
        #endregion

        #region EventArgsDefinitions
        /// <summary>
        /// EventArgs class for when an initial connection fails.
        /// </summary>
        public class InitialConnectionFailedEventArgs : EventArgs
        {
            public Exception Cause { get; set; }
        }

        /// <summary>
        /// EventArgs class for when a ClientConnection is connected.
        /// </summary>
        public class InitialConnectionSuccessEventArgs : EventArgs
        {
            public ClientConnection Client { get; set; }
        }
        #endregion
    }
}
