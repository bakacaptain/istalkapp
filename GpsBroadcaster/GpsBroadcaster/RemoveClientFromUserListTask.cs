using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ServerNetworkConnections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace GpsBroadcaster
{
    class RemoveClientFromUserListTask : Task
    {
        public ClientConnection ClientConnection { get; private set; }
        public ClientConnectionList ClientConnectionList { get; private set; }
        public ObservableCollection<UserLocation> UserLocations { get; private set; }

        public RemoveClientFromUserListTask(Dispatcher disp, TaskHandler handler, ClientConnectionList list, ClientConnection client, ObservableCollection<UserLocation> locs)
            : base(disp, handler)
        {
            ClientConnectionList = list;
            ClientConnection = client;
            UserLocations = locs;
        }

        protected override void Execute(object sender, System.ComponentModel.DoWorkEventArgs args)
        {
            lock (ClientConnectionList)
            {
                ClientConnectionList.Remove(ClientConnection);
            }

            if (ClientConnection.ClientID.IsAuthenticated)
            {
                UserLocation loc = new UserLocation(ClientConnection.ClientID.Username);
                lock (UserLocations)
                {
                    Int32 index = UserLocations.IndexOf(loc);
                    if (index > -1)
                    {
                        loc = UserLocations[index];
                        UserLocations.Remove(loc);
                        lock (ClientConnectionList)
                        {
                            ClientConnectionList.NotifyAllAuthenticatedClients(new RemoveGeoPointMessage(loc));
                        }
                        args.Result = ClientConnection.ClientID.Username + " disconnected from server, other clients has been notified";
                    }
                    else
                    {
                        args.Result = ClientConnection.ClientID.Username + " disconnected from server";
                    }
                }
            }
            else
            {
                args.Result = ClientConnection.RemoteEndPoint.ToString() + " disconnected from server";
            }

        }
    }
}
