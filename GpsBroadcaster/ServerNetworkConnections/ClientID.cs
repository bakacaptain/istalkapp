using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net;
using System.ComponentModel;

namespace ServerNetworkConnections
{
    /// <summary>
    /// Class representing an ID for a client connection.
    /// 
    /// A connection is identified in one of two ways: Either by a given EndPoint
    /// or by a given username.
    /// 
    /// The application has been implemented in such a way that if the username 
    /// is not null, the given client is authenticated to the server.
    /// </summary>
    public class ClientID : IXmlable, INotifyPropertyChanged
    {
        #region Fields
        private String username;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for ClientID.
        /// </summary>
        /// <param name="username">A String with the username that identifies the Client.</param>
        public ClientID(String username)
        {
            this.username = username;
        }

        public ClientID()
        {
            this.username = null;
        }
        #endregion

        #region Properties

        /// <summary>
        /// The Username representing the ClientID.
        /// </summary>
        public String Username
        {
            get { return username; }
            set 
            { 
                username = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Username"));
            }
        }

        public Boolean IsAuthenticated
        {
            get { return Username != null; }
        }
        #endregion

        #region PublicMethods
        /// <summary>
        /// Operator that allows you to cast a String to a ClientID.
        /// </summary>
        /// <param name="id">This is the String with the username that should be used by the ClientID.</param>
        /// <returns>A ClientID using the id as the Username.</returns>
        public static explicit operator ClientID(String id)
        {
            return new ClientID(id);
        }

        /// <summary>
        /// Equals method that returns true if the usernames are the same. If they are not the same 
        /// the EndPoints will be compared instead, if those are the same, true will be returned.
        /// 
        /// If none of this is the case, false is returned.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ClientID))
            {
                return false;
            }

            ClientID other = (ClientID)obj;

            if (this.username != null && other.username != null && this.username.Equals(other.username))
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (this.username != null)
            {
                return this.username.GetHashCode();
            }
            return 0;
        }
        #endregion

        public string ToXML()
        {
            return new StringBuilder().Append("<Username>")
                                      .Append(username)
                                      .Append("</Username>").ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
