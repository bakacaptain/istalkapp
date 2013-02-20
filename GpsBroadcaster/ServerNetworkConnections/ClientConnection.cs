using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.Security;
using System.IO;
using CommonUtilities;
using System.Xml;
using System.Net;

namespace ServerNetworkConnections
{
    /// <summary>
    /// This class represents a connection to a remote host.
    /// 
    /// It is possible to notify this host with messages that extends upon the "AbstractMessage"
    /// class.
    /// 
    /// HOW TO USE:
    /// * Initialize the class with needed parameters (see constructor)
    /// * Call "Start()" method to start listening for incoming messages and start to write messages
    /// * Whenever a message is received from the server, the OnMessageReceived callback is called.
    /// * Whenever a message is received but in a corrupted format, the OnMessageFailed is called.
    /// * If the connection to the server fails, the OnConnectionFailed is called. The underlying 
    /// connection will automatically be closed, so you do not need to call the stop method if this 
    /// delegate is fired.
    /// * Writing to the server is non-blocking. All it does is to place the message into a buffer that is 
    /// then handled by another thread.
    /// </summary>
    public class ClientConnection
    {
        #region DelegateDeclarations
        public delegate void OnMessageReceived(Object sender, MessageReceivedEventArgs args);
        public delegate void OnConnectionFailed(Object sender, ConnectionFailedEventArgs args);
        public delegate void OnMessageFailed(Object sender, MessageFailedEventArgs args);
        #endregion

        #region Events
        // Event called when a message is received
        public event OnMessageReceived MessageReceived
        {
            add
            {
                reader.MessageReceived += value;
            }
            remove
            {
                reader.MessageReceived -= value;
            }
        }
        // Event called when the connection fails.
        public event OnConnectionFailed ConnectionFailed;
        // Event called when a corrupt message is received.
        public event OnMessageFailed MessageFailed
        {
            add
            {
                reader.MessageFailed += value;
            }
            remove
            {
                reader.MessageFailed -= value;
            }
        }
        #endregion

        #region Fields
        // Buffer in which all outgoing messages are placed prior to being sent
        private Buffer<AbstractMessage> buffer;
        // The underlying TCPConnection that all abstract messages are sent into
        private TCPConnection connection;
        // The MessageWriter responsible for taking messages from the buffer and writing them into the TCPConnection
        private MessageWriter writer;
        // The MessageReader responsible for reading messages from the TCPConnection and calling the appropriate delegates
        private MessageReader reader;
        // Threads that are reading and writing from the TCPConnection.
        private Thread writerThr;
        private Thread readerThr;
        // Booleans indicating the state of the ClientConnection.
        private Boolean terminating;
        private Boolean hasStarted;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for ClientConnection
        /// </summary>
        /// <param name="id">ClientID is an id representing this connection</param>
        /// <param name="connection">This is a TCPConnection representing the connection to the client</param>
        /// <param name="MessageReceived">This is a delegate representing the method to be called when a message is received</param>
        /// <param name="ConnectionFailed">This is a delegate representing the method to be called when the connection fails</param>
        /// <param name="MessageFailed">This is a delegate representing the method to be called when a corrupt message is received</param>
        public ClientConnection(TCPConnection connection)
        {
            this.buffer = new Buffer<AbstractMessage>();
            this.connection = connection;
            ClientID = new ClientID();

            terminating = false;
            hasStarted = false;
            writer = new MessageWriter(this, connection);
            reader = new MessageReader(this, connection);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Boolean indicating whether or not this ClientConnection is currently connected.
        /// </summary>
        public Boolean IsConnected
        {
            get { return connection.IsConnected; }
        }

        /// <summary>
        /// The ClientID attached to the ClientConnection.
        /// </summary>
        public ClientID ClientID { get; private set; }

        public EndPoint LocalEndPoint
        {
            get { return connection.LocalEndPoint; }
        }

        public EndPoint RemoteEndPoint
        {
            get { return connection.RemoteEndPoint; }
        }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Method that makes the connection start listening and writing messages to it's attached socket.
        /// </summary>
        public void Start()
        {
            if (!hasStarted)
            {
                terminating = false;
                hasStarted = true;
                writerThr = new Thread(writer.Go);
                readerThr = new Thread(reader.Go);
                writerThr.IsBackground = true;
                readerThr.IsBackground = true;
                writerThr.Priority = ThreadPriority.BelowNormal;
                readerThr.Priority = ThreadPriority.BelowNormal;

                writerThr.Start();
                readerThr.Start();
            }
        }

        /// <summary>
        /// Method that closes the connection.
        /// </summary>
        public void Stop()
        {
            terminating = true;
            hasStarted = false;
            connection.Close();
            if (writerThr != null && readerThr != null)
            {
                writerThr.Abort();
                readerThr.Abort();
            }
        }

        /// <summary>
        /// Method that places a message in a buffer to be sent to the server.
        /// 
        /// This is a non-blocking call.
        /// </summary>
        /// <param name="msg"></param>
        public void Notify(AbstractMessage msg)
        {
            buffer.Put(msg);
        }
        #endregion

        #region PrivateMethods
        /// <summary>
        /// Internal methods to terminate the ClientConnection when an unexpected failure happens.
        /// </summary>
        private void InternalTerminationProcedure()
        {
            lock (this)
            {
                if (!terminating)
                {
                    terminating = true;
                    Stop();
                    if (ConnectionFailed != null)
                    {
                        ConnectionFailed(this, ConnectionFailedEventArgs.Empty);
                    }
                }
            }
        }
        #endregion

        #region MessageReaderAndWriter
        /// <summary>
        /// Inner class used to handle the message writing into the given TCPConnection.
        /// 
        /// It will basically at all times try to read from it's attached buffer, and 
        /// write the message.
        /// </summary>
        private class MessageWriter
        {
            private ClientConnection inner;
            private TCPConnection connection;

            /// <summary>
            /// Constructor for MessageWriter.
            /// </summary>
            /// <param name="inner">This is the ClientConnection that this class is inside.</param>
            /// <param name="connection">This is the TCPConnection that the ClientConnection is connected to.</param>
            /// <param name="MessageFailed">A delegate representing the method to be called whenever this MessageWriter
            /// fails to write a message into it's underlying TCPConnection.</param>
            public MessageWriter(ClientConnection inner, TCPConnection connection)
            {
                this.inner = inner;
                this.connection = connection;
            }

            /// <summary>
            /// Endlessly repeating loop.
            /// </summary>
            public void Go()
            {
                try
                {
                    while (true)
                    {
                        AbstractMessage msg = inner.buffer.Take();
                        connection.Write(msg);
                    }
                }
                catch (Exception)
                {
                    new Thread(inner.InternalTerminationProcedure).Start();
                }
            }
        }

        /// <summary>
        /// Inner class used to handle the message reading from the given TCPConnection.
        /// 
        /// It will basically at all times try to read from it's the TCPConnection and 
        /// call the appropriate delegates passwed in the constructor.
        /// </summary>
        private class MessageReader
        {
            public event OnMessageReceived MessageReceived;
            public event OnMessageFailed MessageFailed;
            private ClientConnection inner;
            private TCPConnection connection;

            /// <summary>
            /// Constructor for MessageReader.
            /// </summary>
            /// <param name="inner">This is the ClientConnection that this class is inside.</param>
            /// <param name="connection">This is the TCPConnection that the ClientConnection is connected to.</param>
            /// <param name="MessageReceived">Delegate to be called whenever a message arrives from the underlying
            /// TCPConnection.</param>
            /// <param name="MessageFailed">Delegate to be called whenever a message was read incorrectly.</param>
            public MessageReader(ClientConnection inner, TCPConnection connection)
            {
                this.inner = inner;
                this.connection = connection;
            }

            /// <summary>
            /// Endlessly repeating loop!
            /// </summary>
            public void Go()
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            AbstractMessage msg = connection.Read();

                            if (MessageReceived != null)
                            {
                                MessageReceivedEventArgs args = new MessageReceivedEventArgs();
                                args.Message = msg;
                                MessageReceived(inner, args);
                            }
                        }
                        catch (XmlException e)
                        {
                            if (MessageFailed != null)
                            {
                                MessageFailedEventArgs args = new MessageFailedEventArgs();
                                args.Cause = e;
                                MessageFailed(inner, args);
                            }
                        }
                    }
                }
                catch (Exception) // any exception
                {
                    new Thread(inner.InternalTerminationProcedure).Start();
                }
            }
        }
        #endregion

        #region EventArgsDefinition
        /// <summary>
        /// EventArgs class used for the event of receiving a message.
        /// </summary>
        public class MessageReceivedEventArgs : EventArgs
        {
            public AbstractMessage Message { get; set; }
        }

        /// <summary>
        /// EventArgs class used for the event of a connection failure.
        /// </summary>
        public class ConnectionFailedEventArgs : EventArgs
        {
            static ConnectionFailedEventArgs()
            {
                Empty = new ConnectionFailedEventArgs();
            }

            public new static ConnectionFailedEventArgs Empty { get; private set; }
        }

        /// <summary>
        /// EventArgs used for the event of a corrupted message was received.
        /// </summary>
        public class MessageFailedEventArgs : EventArgs
        {
            public Exception Cause { get; set; }
        }
        #endregion
    }
}