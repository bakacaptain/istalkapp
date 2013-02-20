using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ServerNetworkConnections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Concurrent;

namespace GpsBroadcaster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MainWindowViewModel MainWindowViewModel { get; set; }
        public ClientConnectionList UserList { get; set; }
        public ObservableCollection<ActionMessage> Actions { get; set; }
        public ClientConnectionListener Listener { get; set; }
        public ObservableCollection<UserLocation> UserLocations { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // create model
            UserLocations = new ObservableCollection<UserLocation>();
            UserList = new ClientConnectionList();
            Actions = new ObservableCollection<ActionMessage>();
            Listener = new ClientConnectionListener(50000, new XmlMessageParser()); // add parser
            Listener.InitialConnectionSuccess += OnInitialConnectionSuccess;
            Listener.InitialConnectionFailed += OnInitialConnectionFailed;
            Listener.ConnectionFailed += OnConnectionFailed;
            Listener.MessageReceived += OnMessageReceived;
            Listener.MessageFailed += OnMessageFailed;

            // create view model
            MainWindowViewModel = new MainWindowViewModel();
            MainWindowViewModel.UserList = UserList;
            MainWindowViewModel.Actions = Actions;
            MainWindowViewModel.UserLocations = UserLocations;
            
            // create view
            MainWindow = new MainWindow();
            MainWindow.DataContext = MainWindowViewModel;
            MainWindow.Show();

            // start listening
            Listener.StartAccepting();

            UpdaterTask task = new UpdaterTask(
                Dispatcher,
                TaskHandler.Background,
                UserList,
                UserLocations);

            task.ProgressChanged += TaskProgressReport;
            task.BeginExecute();
        }

        // ******************************************************************************************************* //
        //                                          Callback Methods
        // ******************************************************************************************************* //

        public void TaskCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false && e.Result != null)
            {
                if (Actions.Count >= 100)
                    Actions.RemoveAt(0);
                Actions.Add(e.Result.ToString());
            }
            else if (e.Cancelled == true && e.Result != null)
            {
                if (Actions.Count >= 100)
                    Actions.RemoveAt(0);
                Actions.Add(e.Result.ToString());
            }
            else if (e.Error != null)
            {
                if (Actions.Count >= 100)
                    Actions.RemoveAt(0);
                Actions.Add(e.Error.Message);
            }
        }

        private void TaskProgressReport(Object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is String)
            {
                String msg = e.UserState as String;

                if (Actions.Count >= 100)
                    Actions.RemoveAt(0);
                Actions.Add((String)e.UserState);
            }
        }

        // ******************************************************************************************************* //
        //                                          Event interaction
        // ******************************************************************************************************* //

        public void OnInitialConnectionSuccess(Object sender, ClientConnectionListener.InitialConnectionSuccessEventArgs args)
        {
            AddClientToUserListTask task = new AddClientToUserListTask(
                Dispatcher,
                TaskHandler.DispatcherAsync,
                UserList,
                args.Client);

            task.RunWorkerCompleted += TaskCompleted;
            task.BeginExecute();
        }

        public void OnInitialConnectionFailed(Object sender, ClientConnectionListener.InitialConnectionFailedEventArgs args)
        {
        }

        public void OnConnectionFailed(Object sender, ClientConnection.ConnectionFailedEventArgs args)
        {
            RemoveClientFromUserListTask task = new RemoveClientFromUserListTask(
                Dispatcher,
                TaskHandler.DispatcherAsync,
                UserList,
                sender as ClientConnection,
                UserLocations);

            task.RunWorkerCompleted += TaskCompleted;
            task.BeginExecute();
        }

        public void OnMessageReceived(Object sender, ClientConnection.MessageReceivedEventArgs args)
        {
            ClientConnection client = sender as ClientConnection;
            if (args.Message is InitRequestMessage)
            {
                RespondToInitRequestMessage task = new RespondToInitRequestMessage(
                    Dispatcher,
                    TaskHandler.DispatcherAsync,
                    UserList,
                    client,
                    args.Message as InitRequestMessage);

                task.RunWorkerCompleted += TaskCompleted;
                task.BeginExecute();
            }
            else if (args.Message is GeoPointMessage)
            {
                RespondToGeoPointMessageTask task = new RespondToGeoPointMessageTask(
                    Dispatcher,
                    TaskHandler.DispatcherAsync,
                    UserList,
                    client,
                    UserLocations,
                    args.Message as GeoPointMessage);

                task.RunWorkerCompleted += TaskCompleted;
                task.BeginExecute();
            }
            else if (args.Message is MultipleGeoPointRequestMessage)
            {
                MultipleGeoPointResponseMessage msg = null;
                lock (UserLocations)
                {
                    msg = new MultipleGeoPointResponseMessage(UserLocations.ToList());
                }
                client.Notify(msg);
            }
        }

        public void OnMessageFailed(Object sender, ClientConnection.MessageFailedEventArgs args)
        {
        }
    }
}
