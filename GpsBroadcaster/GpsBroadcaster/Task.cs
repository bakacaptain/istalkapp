using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Threading;

namespace GpsBroadcaster
{
    public enum TaskHandler
    {
        Dispatcher = 0x00,
        DispatcherAsync = 0x01,
        Background = 0x02,
        ThisThread = 0x03
    }

    /// <summary>
    /// A class that represents a unit of work to be done. At the choice of the user, 
    /// this can either be executed synchronously on the current thread or the GUI thread,
    /// or be executed asynchronously on a background worker. In either case, it will be 
    /// able to report back on the GUI thread when the work is done. And if you choose to 
    /// use an asynchronous operation, then it can also report back progress.
    /// </summary>
    public abstract class Task
    {
        private event RunWorkerCompletedEventHandler _runWorkerCompleted;
        private event ProgressChangedEventHandler _progressChanged;

        private BackgroundWorker Worker { get; set; }
        private TaskHandler TaskHandler { get; set; }
        private Dispatcher Dispatcher { get; set; }
        public Boolean IsCompleted { get; private set; }

        /// <summary>
        /// Event to be called when the task is reporting progress.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged
        {
            // Can only report progress as "ThisThread" and "Background"
            // It only makes sense if "ThisThread" is not the GUI thread itself.
            add
            {
                if (TaskHandler == TaskHandler.Background)
                {
                    Worker.ProgressChanged += value;
                }
                else if (TaskHandler == TaskHandler.ThisThread)
                {
                    _progressChanged += value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot report progress for synchronous operation");
                }
            }
            remove
            {
                if (TaskHandler == TaskHandler.Background)
                {
                    Worker.ProgressChanged -= value;
                }
                else if (TaskHandler == TaskHandler.ThisThread)
                {
                    _progressChanged -= value;
                }
                else
                {
                    throw new InvalidOperationException("Cannot report progress for synchronous operation");
                }
            }
        }

        /// <summary>
        /// Event to be called when the task completed.
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted
        {
            add
            {
                if (TaskHandler == TaskHandler.Background)
                {
                    Worker.RunWorkerCompleted += value;
                }
                else
                {
                    _runWorkerCompleted += value;
                }
            }
            remove
            {
                if (TaskHandler == TaskHandler.Background)
                {
                    Worker.RunWorkerCompleted -= value;
                }
                else
                {
                    _runWorkerCompleted -= value;
                }
            }
        }

        /// <summary>
        /// Boolean indicating whether or not the task has been cancelled from a call to
        /// Cancel(). It is up to the creater of the task to actually stop it from running.
        /// </summary>
        protected Boolean IsCancelled
        {
            // Cancelled only works for Background tasks.
            get
            {
                if (TaskHandler == TaskHandler.Background)
                {
                    return Worker.CancellationPending;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Constructor for the task.
        /// </summary>
        /// <param name="controller">The controller usually represents a controller class for the application.
        /// In addition, this class must be a DispatcherObject associated with the GUI thread.</param>
        /// <param name="handler">Is an enum representing how this task should be executed.</param>
        public Task(Dispatcher dispatcher, TaskHandler handler)
        {
            if (dispatcher == null)
                throw new NullReferenceException("Arguments cannot be null.");

            IsCompleted = false;
            Dispatcher = dispatcher;
            TaskHandler = handler;

            if (TaskHandler == TaskHandler.Background)
                Worker = new BackgroundWorker();
        }

        /// <summary>
        /// This method cancels the task. This only works for TaskHandlers of type background.
        /// </summary>
        public void Cancel()
        {
            if (TaskHandler == TaskHandler.Background)
            {
                Worker.CancelAsync();
            }
            else
            {
                throw new InvalidOperationException("Cannot cancel non-background task.");
            }
        }

        /// <summary>
        /// Method to start running this task asynchronously. This can only be used if the
        /// enum TaskHandler.Background was specified to the constructor of the class.
        /// </summary>
        /// <param name="parameters">All the object arguments</param>
        public void BeginExecute(params Object[] parameters)
        {
            // End if wrong task type for async operation.
            if (TaskHandler == TaskHandler.Dispatcher || TaskHandler == TaskHandler.ThisThread)
                throw new InvalidOperationException("This task must be run synchronously");

            if (IsCompleted)
                throw new InvalidOperationException("This task has already been executed");

            // Run asynchronously on GUI thread
            if (TaskHandler == TaskHandler.DispatcherAsync)
            {
                DoWorkEventArgs args = new DoWorkEventArgs(parameters);
                Dispatcher.BeginInvoke((Action)delegate()
                {
                    try
                    {
                        Execute(this, args);
                        IsCompleted = true;
                        if (_runWorkerCompleted != null)
                        {
                            _runWorkerCompleted(this, new RunWorkerCompletedEventArgs(args.Result, null, args.Cancel));
                        }
                    }
                    catch (Exception e)
                    {
                        if (_runWorkerCompleted != null)
                        {
                            _runWorkerCompleted(this, new RunWorkerCompletedEventArgs(args.Result, e, args.Cancel));
                        }
                    }
                });
            }
            else if (Dispatcher.CheckAccess()) // Check if thread has access to GUI. (BackgroundWorker must be started from GUI thread)
            {
                Worker.WorkerReportsProgress = true;
                Worker.WorkerSupportsCancellation = true;
                Worker.DoWork += Execute;
                Worker.RunWorkerCompleted += Completed;
                Worker.RunWorkerAsync(parameters);
            }
            else // Rerun this method from GUI thread
            {
                Dispatcher.BeginInvoke((Action)delegate() { BeginExecute(parameters); });
            }
        }

        private void Completed(Object sender, RunWorkerCompletedEventArgs args)
        {
            IsCompleted = true;
        }

        /// <summary>
        /// Method to start running this task synchronously, either on the current thread 
        /// or on the GUI thread. This can only be used if the enum TaskHandler.Dispatcher or
        /// TaskHandler.ThisThread was specified to the constructor.
        /// </summary>
        /// <param name="parameters"></param>
        public void Execute(params Object[] parameters)
        {
            // End if wrong task type for sync operation.
            if (TaskHandler == TaskHandler.Background || TaskHandler == TaskHandler.DispatcherAsync)
                throw new InvalidOperationException("This task must be run asynchronously");

            if (IsCompleted)
                throw new InvalidOperationException("This task has already been executed");

        // Run on this thread, ThisThread is specified or if this thread has access to GUI.
            else if (Dispatcher.CheckAccess() || TaskHandler == TaskHandler.ThisThread)
            {
                DoWorkEventArgs args = new DoWorkEventArgs(parameters);
                try
                {
                    Execute(this, args);
                    IsCompleted = true;
                    if (_runWorkerCompleted != null && TaskHandler != TaskHandler.ThisThread)
                    {
                        _runWorkerCompleted(this, new RunWorkerCompletedEventArgs(args.Result, null, args.Cancel));
                    }
                    else if (_runWorkerCompleted != null)
                    {
                        Dispatcher.Invoke((Action)delegate() { _runWorkerCompleted(this, new RunWorkerCompletedEventArgs(args.Result, null, args.Cancel)); });
                    }
                }
                catch (Exception e)
                {
                    if (_runWorkerCompleted != null && TaskHandler != TaskHandler.ThisThread)
                    {
                        _runWorkerCompleted(this, new RunWorkerCompletedEventArgs(args.Result, e, args.Cancel));
                    }
                    else if (_runWorkerCompleted != null)
                    {
                        Dispatcher.Invoke((Action)delegate() { _runWorkerCompleted(this, new RunWorkerCompletedEventArgs(args.Result, e, args.Cancel)); });
                    }
                }
            }
            else // Rerun this method SYNCHRONOUSLY on GUI thread
            {
                Dispatcher.Invoke((Action)delegate() { Execute(parameters); });
            }
        }

        /// <summary>
        /// Method used to report the progress being done by the task.
        /// </summary>
        /// <param name="percent">An Int32 representing the progress.</param>
        /// <param name="userState">An arbitary userState object to describe the state of the operation.</param>
        protected void ReportProgress(Int32 percent, Object userState)
        {
            if (TaskHandler == TaskHandler.Background)
            {
                Worker.ReportProgress(percent, userState);
            }
            else if (TaskHandler == TaskHandler.ThisThread && !Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke((Action)delegate() { _progressChanged(this, new ProgressChangedEventArgs(percent, userState)); });
            }
            else
            {
                throw new InvalidOperationException("Cannot report progress for synchronous operation");
            }
        }

        /// <summary>
        /// Abstract method to be overridden that represents the work being done by the task.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected abstract void Execute(Object sender, DoWorkEventArgs args);
    }
}
