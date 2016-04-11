using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using GHIElectronics.NETMF.Hardware;
using GHIElectronics.NETMF.FEZ;
using Microsoft.SPOT;

namespace LazyAmbientLight.Fez
{
    public class Program
    {
        readonly PWM _red = new PWM((PWM.Pin)FEZ_Pin.PWM.Di5);
        readonly PWM _green = new PWM((PWM.Pin)FEZ_Pin.PWM.Di6);
        readonly PWM _blue = new PWM((PWM.Pin)FEZ_Pin.PWM.Di8);
        private static SerialPort _serialPort;


        public static void Main()
        {
            // Blink board LED
            
            bool ledState = false;
            var program = new Program();


            program.RunTest();

            program.StartReading();
           Thread.Sleep(Timeout.Infinite);
        }

        private  void StartReading()
        {
            _serialPort = new SerialPort("COM1", 9600);
            _serialPort.Open();
            _serialPort.DataReceived += UART_DataReceived;

        }

        private void UART_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // read the data
            byte[] rxData = new byte[10];
            var readCount = _serialPort.Read(rxData, 0, 3);
            if (readCount != 3)
            {
                Debug.Print("F:WrongSize");
            }
            else
            {
                Debug.Print("F:ColorRecieved");
                PwmHelper.Set(_red, rxData[0]);
                PwmHelper.Set(_green, rxData[1]);
                PwmHelper.Set(_blue, rxData[2]);
                
            }
            Thread.Sleep(5);
            Debug.Print("F:Write one value");
            _serialPort.Write(new byte[] { 112 }, 0, 1);
            _serialPort.Flush();
            Thread.Sleep(5);
        }

        
        private void RunTest()
        {
            Reset();
            Test(_red);
            Test(_green);
            Test(_blue);
            Reset();

        }

        private static void Test(PWM pwm)
        {
            pwm.Set(true);
            Thread.Sleep(500);
            pwm.Set(false);
        }

        private void Reset()
        {
            _red.Set(false);
            _green.Set(false);
            _blue.Set(false);
        }
    }

    public  static class PwmHelper {

        public static void Set(PWM pwm, byte dutyCycle)
        {
            byte cycle = NumericHelper.Map(dutyCycle, 0, 255, 0, 100);
            pwm.Set(10000, cycle);
        }
    }

    public static class NumericHelper
    {
        public static byte Map(byte input, byte fromMin, byte fromMax, byte  toMin, byte toMax)
        {
            byte inputc = Contrain(input, fromMin, fromMax);
            double pos = ((double)inputc - fromMin) / (fromMax - fromMin);
            return (byte) (((toMax - toMin) * pos) + toMin);

        }

        private static byte Contrain( byte input, byte fromMin, byte fromMax)
        {
            return (byte)System.Math.Min((byte)System.Math.Max(input, fromMin), fromMax);
        }
    }

    
}

