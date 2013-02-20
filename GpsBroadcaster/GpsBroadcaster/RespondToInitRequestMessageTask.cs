using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using ServerNetworkConnections;
using System.Collections.Concurrent;

namespace GpsBroadcaster
{
    class RespondToInitRequestMessage : Task
    {
        public ClientConnection ClientConnection { get; private set; }
        public ClientConnectionList ClientConnectionList { get; private set; }
        public InitRequestMessage InitRequestMessage { get; private set; }

        public RespondToInitRequestMessage(Dispatcher disp, TaskHandler handler, ClientConnectionList list, ClientConnection client, InitRequestMessage msg)
            : base(disp, handler)
        {
            ClientConnectionList = list;
            ClientConnection = client;
            InitRequestMessage = msg;
        }

        protected override void Execute(object sender, System.ComponentModel.DoWorkEventArgs args)
        {
            lock (ClientConnectionList)
            {
                if (!ClientConnectionList.ContainsClientID((ClientID)InitRequestMessage.Username))
                {
                    ClientConnection.ClientID.Username = InitRequestMessage.Username;
                    ClientConnection.Notify(new InitResponseMessage(true));
                    args.Result = ClientConnection.RemoteEndPoint.ToString() + " logged on as '" + InitRequestMessage.Username + "'";
                }
                else
                {
                    ClientConnection.Notify(new InitResponseMessage(false));
                    args.Result = ClientConnection.RemoteEndPoint.ToString() + " attempted to log on as '" + InitRequestMessage.Username + "' but was denied.";
                }
            }

        }
    }
}
