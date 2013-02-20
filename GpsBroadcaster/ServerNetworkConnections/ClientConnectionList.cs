using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ServerNetworkConnections
{
    public class ClientConnectionList : ObservableCollection<ClientConnection>
    {
        #region PublicMethods

        public ClientConnection this[ClientID id]
        {
            get
            {
                return this.First(x => x.ClientID == id);
            }
        }

        public void NotifyAllClients(AbstractMessage msg)
        {
            foreach (ClientConnection con in this)
            {
                con.Notify(msg);
            }
        }

        public void NotifyAllAuthenticatedClients(AbstractMessage msg)
        {
            foreach (ClientConnection con in this.Where(x => x.ClientID.IsAuthenticated))
            {
                con.Notify(msg);
            }
        }

        public void NotifyAllAuthenticatedClientsExcept(ClientConnection client, AbstractMessage msg)
        {
            foreach (ClientConnection con in this.Where(x => x.ClientID.IsAuthenticated && !x.Equals(client)))
            {
                con.Notify(msg);
            }
        }

        public Boolean ContainsClientID(ClientID id)
        {
            return this.Any(x => x.ClientID.Equals(id));
        }

        #endregion
    }
}
