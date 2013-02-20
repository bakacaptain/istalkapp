using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ServerNetworkConnections;
using System.Collections.ObjectModel;
using System.Threading;

namespace GpsBroadcaster
{
    class UpdaterTask : Task
    {
        public ClientConnectionList ClientConnectionList { get; private set; }
        public ObservableCollection<UserLocation> UserLocations { get; private set; }

        public UpdaterTask(Dispatcher disp, TaskHandler handler, ClientConnectionList list, ObservableCollection<UserLocation> userLocations)
            : base(disp, handler)
        {
            ClientConnectionList = list;
            UserLocations = userLocations;
        }

        protected override void Execute(object sender, System.ComponentModel.DoWorkEventArgs args)
        {
            while (true)
            {
                Thread.Sleep(15000);
                if (ClientConnectionList.Count > 0 && UserLocations.Count > 0)
                {
                    List<UserLocation> list = null;
                    lock (UserLocations)
                    {
                        list = UserLocations.ToList();
                    }
                    lock (ClientConnectionList)
                    {
                        ClientConnectionList.NotifyAllAuthenticatedClients(new MultipleGeoPointResponseMessage(list));
                    }
                    ReportProgress(0, "Clients were notified of eachother's position");
                }
            }
        }
    }
}
