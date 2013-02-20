using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net;

namespace ServerNetworkConnections
{
    /// <summary>
    /// Class that encapsulates a socket into a more convenient unit from which you can write
    /// AbstractMessage to and read AbstractMessages from
    /// </summary>
    public class TCPConnection
    {
        #region Fields
        private TcpClient client;
        private NetworkStream stream;
        private StreamWriter writer;
        private StreamReader reader;
        private AbstractMessageParser parser;
        private EndPoint localEndPoint;
        private EndPoint remoteEndPoint;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the TCPConnection to the given hostname / port.
        /// </summary>
        /// <param name="hostname">A String with the hostname of the server</param>
        /// <param name="port">A UInt16 with the port of the server.</param>
        public TCPConnection(String hostname, UInt16 port, AbstractMessageParser parser)
        {
            this.client = new TcpClient(hostname, port);
            this.stream = client.GetStream();
            this.writer = new StreamWriter(stream);
            this.reader = new StreamReader(stream);
            this.parser = parser;
            this.localEndPoint = client.Client.LocalEndPoint;
            this.remoteEndPoint = client.Client.RemoteEndPoint;
        }

        /// <summary>
        /// Initializes the TCPConnection with the given TcpClient.
        /// </summary>
        /// <param name="client">The TcpClient to the server.</param>
        public TCPConnection(TcpClient client, AbstractMessageParser parser)
        {
            this.client = client;
            this.stream = client.GetStream();
            this.writer = new StreamWriter(stream);
            this.reader = new StreamReader(stream);
            this.parser = parser;
            this.localEndPoint = client.Client.LocalEndPoint;
            this.remoteEndPoint = client.Client.RemoteEndPoint;
        }
        #endregion

        #region Properties
        /// <summary>
        /// The RemoteEndPoint the TCPConnection is connected to.
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get { return remoteEndPoint; }
        }

        /// <summary>
        /// The LocalEndPoint of this TCPConnection.
        /// </summary>
        public EndPoint LocalEndPoint
        {
            get { return localEndPoint; }
        }

        /// <summary>
        /// A Boolean indicating whether or not we are connected to the server.
        /// </summary>
        public Boolean IsConnected
        {
            get { return client != null && client.Connected; }
        }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Closes the TCPConnection.
        /// </summary>
        public void Close()
        {
            stream.Close();
            client.Close();
        }

        /// <summary>
        /// Writes the given AbstractMessage into the TCPConnection.
        /// </summary>
        /// <param name="msg"></param>
        public void Write(AbstractMessage msg)
        {
            String text = msg.ToXML();
            writer.WriteLine(text);
            writer.Flush();
        }

        /// <summary>
        /// Reads an AbstractMessage from the TCPConnection.
        /// 
        /// Will throw an XmlException if an invalid Xml message was received.
        /// </summary>
        /// <returns>The AbstractMessage that was read.</returns>
        public AbstractMessage Read()
        {
            return parser.Parse(reader.ReadLine());
        }
        #endregion
    }
}
