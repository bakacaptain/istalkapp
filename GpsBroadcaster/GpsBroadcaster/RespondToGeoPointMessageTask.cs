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
    class RespondToGeoPointMessageTask : Task
    {
        public ClientConnection ClientConnection { get; private set; }
        public ClientConnectionList ClientConnectionList { get; private set; }
        public ObservableCollection<UserLocation> UserLocations { get; private set; }
        public GeoPointMessage GeoPointMessage { get; private set; }

        public RespondToGeoPointMessageTask(Dispatcher disp, TaskHandler handler, ClientConnectionList list, ClientConnection client, ObservableCollection<UserLocation> locs, GeoPointMessage msg)
            : base(disp, handler)
        {
            ClientConnectionList = list;
            ClientConnection = client;
            GeoPointMessage = msg;
            UserLocations = locs;
        }

        protected override void Execute(object sender, System.ComponentModel.DoWorkEventArgs args)
        {
            UserLocation loc = new UserLocation(GeoPointMessage.UserLocation.Username);
            lock (UserLocations)
            {
                Int32 index = UserLocations.IndexOf(loc);
                if (index > -1)
                {
                    UserLocations.RemoveAt(index);
                    UserLocations.Insert(index, GeoPointMessage.UserLocation);
                    lock (ClientConnectionList)
                    {
                        ClientConnectionList.NotifyAllAuthenticatedClientsExcept(ClientConnection, GeoPointMessage); // notify other observers
                    }
                }
                else
                {
                    UserLocations.Add(GeoPointMessage.UserLocation);
                    lock (ClientConnectionList)
                    {
                        ClientConnectionList.NotifyAllAuthenticatedClientsExcept(ClientConnection, GeoPointMessage); // notify other observers
                    }
                }
            }

            args.Result = ClientConnection.ClientID.Username + " broadcasted his location, other clients were notified";
        }
    }
}
