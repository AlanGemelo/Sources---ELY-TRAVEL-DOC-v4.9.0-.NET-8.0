using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace ELY_TRAVEL_DOC
{
    public delegate void DelegateReadMrz(string MRZ);
    public class Scanner
    {
        // Init COM port
        private SerialPort _com = new SerialPort();

        private DelegateReadMrz _delegate_read_mrz;

        static readonly object _locker = new object();
        //static bool _received;

        private string _sn = "";
        private string _version = "";
        private string _nna = "";
        private string _product_info = "";
        private int scannerTime = 1000;

        public Scanner(DelegateReadMrz delegate_read_mrz)
        {
            // Init delegate handler
            _delegate_read_mrz = delegate_read_mrz;
        }

        public bool Connect(string port_name)
        {
            try
            {
                // First open the port
                //_com = new SerialPort(port_name, 9600, Parity.None, 8);
                //_com = new SerialPort(port_name, 19200, Parity.None, 8);
                _com = new SerialPort(port_name, 115200, Parity.None, 8);
                _com.Open();
                _com.DtrEnable = true;

                _com.ReadTimeout = 500;

                // Purge
                _com.DiscardInBuffer();
                _com.DiscardOutBuffer();

                _com.DataReceived += serialPort_DataReceived;

                return true;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error in Connect" + exc);
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {
                _com.Close();
                return true;
            }

            catch (Exception exc)
            {
                Console.WriteLine("Error in Disconnect" + exc);
                return false;
            }
        }

        public bool IsConnected()
        {
            return _com.IsOpen;
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Read MRZ with 1 sec timeout
            string mrz = ReadMRZ(1000);

            if (!String.IsNullOrEmpty(mrz))
                _delegate_read_mrz(mrz);
        }

        public void Inquire()
        {
            byte[] inquire = { 0x49, 0, 0 };
            try
            {
                _com.DiscardInBuffer();
                _com.Write(inquire, 0, 3);
            }
            catch
            { }
        }

        private string EBCD_toString(byte[] data, int offset, int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; ++i)
            {
                sb.Append(data[offset + i] >> 4);
                sb.Append(data[offset + i] & 0xF);
            }

            return sb.ToString();
        }

        private string ReadMRZ(int timeout)
        {
            int read;

            char cmd;
            byte[] response = new byte[256];
            int length;

            StringBuilder mrz = new StringBuilder();

            try
            {
                int i = 0;
                // Wait for a response from the scanner
                for (; i < timeout; i += 10)
                {
                    Thread.Sleep(10);
                    if (_com.BytesToRead > 2)
                    {
                        break;
                    }
                }

                // Check if a packet of data is available 
                if (_com.BytesToRead > 2)
                {
                    read = _com.Read(response, 0, 3);
                    if (read != 3)
                        throw new InvalidOperationException();

                    cmd = (char)response[0];
                    length = response[1] * 256 + response[2];

                    // Read the data
                    if (length > 0)
                    {
                        if (length > response.Length)
                        {
                            throw new Exception("Device communication error.");
                        }
                        else
                        {
                            for (; i < timeout; i += 10)
                            {
                                Thread.Sleep(10);
                                if (_com.BytesToRead >= length)
                                {
                                    break;
                                }
                            }

                            read = _com.Read(response, 0, length);

                            if (read != length)
                                throw new InvalidOperationException();
                        }
                    }

                    switch (cmd)
                    {
                        case 'I':
                            // Decode MRZ data 
                            mrz.Append(ASCIIEncoding.ASCII.GetString(response, 0, length));

                            // Reactivate automatic reading
                            if (_enableContinuousReading)
                            {
                                SetContinuousReading(true);
                            } else
                            {
                                SetContinuousReading(false);
                            }
                            break;

                        case 'V':
                            _version = ASCIIEncoding.ASCII.GetString(response, 0, length);
                            break;
                        case 'R':
                            _sn = ASCIIEncoding.ASCII.GetString(response, 0, length);
                            break;
                        case 'r':
                            _sn = EBCD_toString(response, 84, 4);
                            break;
                        case 'T':
                            _product_info = ASCIIEncoding.ASCII.GetString(response, 0, length);
                            break;
                        case 'W':
                            _nna = ASCIIEncoding.ASCII.GetString(response, 0, length);
                            break;
                        default:
                            break;
                    }

                    lock (_locker)
                    {
                        //_received = true;
                        Monitor.Pulse(_locker);
                    }
                }
            }
            catch
            { }

            return mrz.ToString().Replace("\r", "\r\n");
        }

        private void SetContinuousReading(bool state)
        {
            byte[] enable_reading = { 0x43, 0, 1, 0 };

            if (state)
            {
                enable_reading[3] = 1;
            }

            //_com.DiscardInBuffer();
            _com.Write(enable_reading, 0, 4);
        }

        private bool _enableContinuousReading = true;
        public bool EnableContinuousReading
        {
            get { return _enableContinuousReading; }
            set
            {
                SetContinuousReading(value);
                _enableContinuousReading = value;
            }
        }

        public string GetVersion()
        {
            byte[] get_version = { 0x56, 0, 0 };

            _com.DiscardInBuffer();
            _com.Write(get_version, 0, 3);

            bool lockTaken = false;
            try
            {
                _version = "";
                Monitor.TryEnter(_locker, 1000, ref lockTaken);
                if (!lockTaken) throw new TimeoutException();
                Monitor.Wait(_locker, scannerTime);// or compensate
                                                   // work here
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_locker);
            }

            return _version;
        }

        public string GetSerialNumber()
        {
            byte[] get_serial = { 0x52, 0, 0 };

            _com.DiscardInBuffer();
            _com.Write(get_serial, 0, 3);

            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_locker, 1000, ref lockTaken);
                if (!lockTaken) throw new TimeoutException();
                _sn = "";
                Monitor.Wait(_locker, scannerTime);
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_locker);
            }

            if (_sn == "")
            {
                byte[] get_serial2 = { 0x72, 0, 0 };
                // Read the full EEPROM

                _com.DiscardInBuffer();
                _com.Write(get_serial, 0, 3);

                lockTaken = false;
                try
                {
                    Monitor.TryEnter(_locker, 1000, ref lockTaken);
                    if (!lockTaken) throw new TimeoutException();
                    _sn = "";
                    Monitor.Wait(_locker, scannerTime);
                }
                finally
                {
                    if (lockTaken) Monitor.Exit(_locker);
                }
            }

            return _sn;
        }

        public string GetNnaVersion()
        {
            byte[] get_nna = { 0x57, 0, 0 };

            _com.DiscardInBuffer();
            _com.Write(get_nna, 0, 3);

            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_locker, 1000, ref lockTaken);
                if (!lockTaken) throw new TimeoutException();
                _nna = "";
                Monitor.Wait(_locker, scannerTime);
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_locker);
            }

            return _nna;
        }

        public string GetProductInformation()
        {
            byte[] get_product_info = { 0x54, 0, 0 };

            _com.DiscardInBuffer();
            _com.Write(get_product_info, 0, 3);

            bool lockTaken = false;
            try
            {
                Monitor.TryEnter(_locker, 1000, ref lockTaken);
                if (!lockTaken) throw new TimeoutException();
                _product_info = "";
                Monitor.Wait(_locker, scannerTime);
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_locker);
            }

            return _product_info;
        }
    }
}
