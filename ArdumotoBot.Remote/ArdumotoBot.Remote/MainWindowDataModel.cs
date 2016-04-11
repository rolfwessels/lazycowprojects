using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Timer = System.Timers.Timer;

namespace ArdumotoBot.Remote
{
    public class MainWindowDataModel : NotifyPropertyChangedBase
    {
        private readonly IDispatcher _dispatcher;
        private ObservableCollection<string> _serialPorts;
        private string _serialPortSelected;
        private bool _isConnected;
        private int _baudRate;
        private SerialPort _serialPort;
        private Timer _portRefreshTimer;
        private int _speed;
        private int _direction;
        private readonly Timer _writeTimer;
        private bool _controlPadOpen;
        private bool _connectionSettingsOpen;
        private string _loggedCommandSent;
        private string _loggedCommandRecieved;
        private Thread _readThread;

        public MainWindowDataModel(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _loggedCommandRecieved = "";
            _loggedCommandSent = "";
            
            _serialPorts = new ObservableCollection<string>();
            RefreshPorts();
            ConnectCommand = new RelayCommand<object>(ConnectExecute,CanConnect);
            DisconnectCommand = new RelayCommand<object>(DisconnectExecute, CanDisconnect);
            StopCommand = new RelayCommand<object>(StopExecute, CanStop);
            IsConnected = false;
            BaudRate = 9600;
            

            //refresh ports every 15 min
            _portRefreshTimer = new Timer(15 * 1000);
            _portRefreshTimer.Elapsed += (x,e)=> RefreshPorts();
            _portRefreshTimer.Start();

            _writeTimer = new Timer(500);
            _writeTimer.Elapsed += (x, e) => SendValues();
            _writeTimer.Start();
        }

        private bool CanStop(object obj)
        {
            return _isConnected ;
        }

        private void StopExecute(object obj)
        {
            Speed = 0;
            Direction = 0;
        }


        private bool CanDisconnect(object obj)
        {
            return IsConnected;
        }

        private void DisconnectExecute(object obj)
        {
            if (_serialPort != null)
            {
                // Close the port
                try
                {
                    _serialPort.Close();
                }
                finally
                {
                    _serialPort = null;
                    IsConnected = false;
                }
            }
        }


        private bool CanConnect(object obj)
        {
            return !IsConnected && SerialPortSelected != null;
        }

        private void ConnectExecute(object obj)
        {
            if (SerialPortSelected != null)
            {
                try
                {
                    _serialPort = new SerialPort(SerialPortSelected, _baudRate, Parity.None, 8, StopBits.One);
                    _serialPort.Open();
                    
                    IsConnected = true;
                }
                catch (Exception e)
                {
                   ShowException(e);
                }
            }
        }

        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand StopCommand { get; set; }

        private void RefreshPorts()
        {
            var portNames = SerialPort.GetPortNames();
            var currentSerials = _serialPorts.ToArray();
            foreach (var portName in portNames)
            {
                if (!_serialPorts.Contains(portName))
                {
                    string nameOfPort = portName;
                    _dispatcher.Invoke(() => _serialPorts.Add(nameOfPort));
                }
            }
            //remove unwanted
            foreach (var serialPort in currentSerials)
            {
                string port = serialPort;
                if (portNames.Count(x => x == port) == 0)
                {
                    _dispatcher.Invoke(() => _serialPorts.Remove(port));
                    
                }
            }
            _dispatcher.Invoke(() =>
                                   {
                                       if (_serialPorts.Count == 0)
                                       {
                                           SerialPortSelected = null;
                                       }
                                       else if ((SerialPortSelected == null || !_serialPorts.Contains(SerialPortSelected)) && _serialPorts.Count > 0)
                                       {
                                           SerialPortSelected = _serialPorts.First();
                                       }
                                   });
            
        }

        public ObservableCollection<string> SerialPorts
        {
            get { return _serialPorts; }
        }

        public string SerialPortSelected
        {
            get { return _serialPortSelected; }
            set { SetField(ref _serialPortSelected, value, () => SerialPortSelected); }
        }

        public int BaudRate
        {
            get { return _baudRate; }
            set { SetField(ref _baudRate, value, () => BaudRate); }
        }

        public int Speed
        {
            get { return _speed; }
            set { SetField(ref _speed, value, () => Speed);
               
            }
        }

        private void SendValues()
        {
            if (IsConnected)
            {
                try
                {
                    if (IsDirty)
                    {
                        var asString = GetValuesAsString();
                        _serialPort.Write(asString);
                        LoggedCommandSent = asString + Environment.NewLine + LoggedCommandSent;
                        IsDirty = false;
                    }
                }
                catch (Exception e)
                {
                    ShowException(e);
                    DisconnectExecute(null);
                }
            }
        }

        public void Read()
        {
            
            while (IsConnected && _serialPort != null && _serialPort.IsOpen)
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    LoggedCommandRecieved = message.Trim() + Environment.NewLine + LoggedCommandRecieved;
                    Thread.Sleep(100);
                }
                catch (TimeoutException) { }
                catch (Exception e)
                {
                    LoggedCommandRecieved += e.Message + Environment.NewLine + LoggedCommandRecieved;
                }
            }
        }


        private void ShowException(Exception exception)
        {
            MessageBox.Show("Exception: " + exception.Message);
        }

        private string GetValuesAsString()
        {  
            return "?S" + (Speed) + "D"+(Direction)+";";
        }

        public int Direction
        {
            get { return _direction; }
            set { SetField(ref _direction, value, () => Direction);}
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                SetField(ref _isConnected, value, () => IsConnected);
                ConnectionSettingsOpen = !value;
                ControlPadOpen = value;
                if (value)
                {
                    _readThread = new Thread(Read);
                    _readThread.Start();
                }
                else if (_readThread != null && _readThread.IsAlive)
                {
                    _readThread.Join();
                    LoggedCommandSent = "Stopped" + Environment.NewLine + LoggedCommandSent;
                }
            }
        }

        public bool ConnectionSettingsOpen
        {
            get { return _connectionSettingsOpen; }
            set { SetField(ref _connectionSettingsOpen, value, () => ConnectionSettingsOpen, false); }
        }

        public bool ControlPadOpen
        {
            get { return _controlPadOpen; }
            set { SetField(ref _controlPadOpen, value, () => ControlPadOpen, false); }
        }

        public string LoggedCommandSent
        {
            get { return _loggedCommandSent; }
            set { SetField(ref _loggedCommandSent, value, () => LoggedCommandSent, false); }
        }

        public string LoggedCommandRecieved
        {
            get { return _loggedCommandRecieved; }
            set { SetField(ref _loggedCommandRecieved, value, () => LoggedCommandRecieved,false); }
        }
    }

  
}