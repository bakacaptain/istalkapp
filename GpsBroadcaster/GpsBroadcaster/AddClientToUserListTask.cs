using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ServerNetworkConnections;

namespace GpsBroadcaster
{
    class AddClientToUserListTask : Task
    {
        public ClientConnection ClientConnection { get; private set; }
        public ClientConnectionList ClientConnectionList { get; private set; }

        public AddClientToUserListTask(Dispatcher disp, TaskHandler handler, ClientConnectionList list, ClientConnection client)
            : base(disp, handler)
        {
            ClientConnectionList = list;
            ClientConnection = client;
        }

        protected override void Execute(object sender, System.ComponentModel.DoWorkEventArgs args)
        {
            lock (ClientConnectionList)
            {
                ClientConnectionList.Add(ClientConnection);
            }
            ClientConnection.Start();

            args.Result = ClientConnection.RemoteEndPoint.ToString() + " connected to the server";
        }
    }
}
