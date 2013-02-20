using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ServerNetworkConnections
{
    /// <summary>
    /// Class that encapsulates the workings of a TcpListener
    /// </summary>
    public class TCPServerConnector
    {
        #region Fields
        // A API class used to listen for TcpClients
        private TcpListener listener;
        // The port used by the listener.
        private UInt16 port;
        // XML Parser
        private AbstractMessageParser parser;
        // Is accepting?
        private Boolean isAccepting;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for TCPServerConnector
        /// </summary>
        /// <param name="port">A UInt16 with the port.</param>
        public TCPServerConnector(UInt16 port, AbstractMessageParser parser)
        {
            listener = new TcpListener(IPAddress.Any, port);
            this.port = port;
            this.parser = parser;
            this.isAccepting = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The port the server is listening to.
        /// </summary>
        public UInt16 Port 
        {
            get { return port; }
            set
            {
                if (isAccepting)
                    throw new InvalidOperationException("Don't change port while listening.");

                port = value;
                listener = new TcpListener(IPAddress.Any, port);
            }
        }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Method to make the listener start listening
        /// </summary>
        public void Start()
        {
            listener.Start();
            isAccepting = true;
        }

        /// <summary>
        /// Method to make the listening stop listening
        /// </summary>
        public void Stop()
        {
            listener.Stop();
            isAccepting = false;
        }

        /// <summary>
        /// A blocking method that listens for an incoming TCP connection and returns it.
        /// </summary>
        /// <returns>A TCP connection encapsulated in a TCPConnection class</returns>
        public TCPConnection waitForConnection()
        {
            TcpClient client = listener.AcceptTcpClient();
            return new TCPConnection(client, parser);
        }
        #endregion
    }
}
