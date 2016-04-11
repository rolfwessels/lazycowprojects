using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using SystemColors = System.Drawing.SystemColors;

namespace LazyAmbientLight.Input
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Thread _thread;
        private bool _started;
        private System.Drawing.Color _dominantColor;
        private SerialPort _serialPort;
        private readonly Thread _threadS;
        private readonly object _setColor = new object();
        private bool _continue;
        private bool _stopConnection;
        private int SleepForColorInput;
        private int SleepSendingSerial;

        public MainWindow()
        {
            InitializeComponent();
            _started = true;
            _thread = new Thread(RunCapture);
            _thread.Start();

            _threadS = new Thread(SerialPortConnect);
            _threadS.Start();

            _dominantColor = System.Drawing.Color.FromArgb(0, 0, 0);
            SleepForColorInput = 100;
            SleepSendingSerial = 100;
        }

        public void SerialPortConnect()
        {
            while (_started)
            {
                if (_stopConnection)
                {
                    SetStatus("Connection Stoped");
                    Thread.Sleep(1000);
                    continue;
                }
                SetStatus("Lookup Serial port");
                string[] theSerialPortNames = System.IO.Ports.SerialPort.GetPortNames();
                if (theSerialPortNames.Length == 0)
                {
                    SetStatus("No COM found");
                }
                else
                {
                    string theSerialPortName = theSerialPortNames[0];
                    SetStatus("Connecting to " + theSerialPortName);
                    try
                    {
                        _serialPort = new SerialPort(theSerialPortName,9600);
                        _serialPort.ReceivedBytesThreshold = 1;
                        _serialPort.Open();
                        SetStatus("Connected to " + theSerialPortName);
                        _serialPort.DataReceived += Recieved;
                        _continue = true;
                        var lastSent = DateTime.Now;
                        while (_started && !_stopConnection)
                        {
                            Debug.Print("I:Waiting for input");


                           // while (!_continue && _started && (DateTime.Now - lastSent < TimeSpan.FromSeconds(1))) { Thread.Sleep(10); }

                            lock (_setColor)
                            {
                                Debug.Print("I:Writing color");
                                var buffer = new byte[] { 0xff, _dominantColor.R, _dominantColor.G, _dominantColor.B };
                                _serialPort.Write(buffer, 0, buffer.Length); 
                                _continue = false;
                                Thread.Sleep(SleepSendingSerial);
                                lastSent = DateTime.Now;
                            }
                        }
                        
                        _serialPort.Close();
                        SetStatus("Connection closed");
                    }
                    catch (Exception e)
                    {
                        SetStatus("Error:" + e.Message);
                    }
                    
                }
                Thread.Sleep(5000);
            }

            
        }

        private void Recieved(object sender, SerialDataReceivedEventArgs e)
        {
            _continue = true;
            Debug.Print("I:Input Recieved");
        }

        private void SetStatus(string lookup)
        {
            Debug.Print("I:" + lookup);
            Dispatcher.BeginInvoke(new Action(() => Status.Text = lookup)).Wait();      
        }

        public void RunCapture()
        {
            int count = 0;
            long avg = 0;

            double primaryScreenWidth = SystemParameters.PrimaryScreenWidth;
            double primaryScreenHeight = SystemParameters.PrimaryScreenHeight;
            int topMargin = 30;
            int bottomMargin = 60;
            while (_started)
            {
                
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var bitmapSource = CaptureScreenshot.CaptureBmp(new Rect(0, topMargin, primaryScreenWidth, primaryScreenHeight - bottomMargin));

                System.Drawing.Color color =CalculateAverageColor(bitmapSource);
                lock (_setColor)
                {
                    _dominantColor = color;
                }
                Dispatcher.BeginInvoke(new Action(CaptureScreenShot),null);
                
                stopwatch.Stop();
                avg += stopwatch.ElapsedMilliseconds;
                count++;
                //Speed test and cleanup
                if (count > 10)
                {
                    Dispatcher.BeginInvoke(new Action(() => CaptureTime.Text = string.Format("Time: {0} [{1}x{2}]", (avg / count), primaryScreenWidth, primaryScreenHeight))).Wait();
                    avg = 0;
                    count = 0;
                    GC.Collect(); 
                }
                Thread.Sleep(SleepForColorInput);
               
            }
        }

        private void CaptureScreenShot()
        {
            ColorDisplay.Background = new System.Windows.Media.SolidColorBrush(Color.FromRgb(_dominantColor.R, _dominantColor.G, _dominantColor.B));
        }

        private static System.Drawing.Color CalculateAverageColor(Bitmap bm)
        {
            int width = bm.Width;
            int height = bm.Height;
            int red = 0;
            int green = 0;
            int blue = 0;
            int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
            int dropped = 0; // keep track of dropped pixels
            long[] totals = new long[] { 0, 0, 0 };
            int bppModifier = bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

            BitmapData srcData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);
            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        red = p[idx + 2];
                        green = p[idx + 1];
                        blue = p[idx];
                        if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion)
                        {
                            totals[2] += red;
                            totals[1] += green;
                            totals[0] += blue;
                        }
                        else
                        {
                            dropped++;
                        }
                    }
                }
            }

            int count = width * height - dropped;
            int avgR = (int)(totals[2] / count);
            int avgG = (int)(totals[1] / count);
            int avgB = (int)(totals[0] / count);

            return System.Drawing.Color.FromArgb(avgR, avgG, avgB);
        }

        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!_started)
            {
                _started = true;
                _thread = new Thread(RunCapture);
                _thread.Start();
            }
            else
            {
                _started = false;
                _thread.Join();
            }
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_started)
            {
                _started = false;

                _thread.Join(TimeSpan.FromSeconds(1));
                _threadS.Join(TimeSpan.FromSeconds(1));

            }
            else
            {
                Console.Out.WriteLine("Ke");
            }
        }

        private void StopConnection(object sender, RoutedEventArgs e)
        {
            _stopConnection = !_stopConnection;
            Sc.Content = _stopConnection ? "S" : "X";
        }
    }
}
