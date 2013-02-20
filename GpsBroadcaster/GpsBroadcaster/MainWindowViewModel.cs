using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using ServerNetworkConnections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace GpsBroadcaster
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ClientConnectionList _userList;
        private ObservableCollection<ActionMessage> _actions;
        private ObservableCollection<UserLocation> _userLocations;
        private ICommand _closeCommand;

        public ClientConnectionList UserList
        {
            get { return _userList; }
            set
            {
                _userList = value;
                RaisePropertyChanged("UserList");
            }
        }

        public ObservableCollection<ActionMessage> Actions
        {
            get { return _actions; }
            set
            {
                _actions = value;
                RaisePropertyChanged("Actions");
            }
        }

        public ObservableCollection<UserLocation> UserLocations
        {
            get { return _userLocations; }
            set
            {
                _userLocations = value;
                RaisePropertyChanged("UserLocations");
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(param => this.CloseExecute());

                return _closeCommand;
            }
        }

        public void CloseExecute()
        {
            App.Current.Shutdown(0);
        }

        protected void RaisePropertyChanged(String prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
