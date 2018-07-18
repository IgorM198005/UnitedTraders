using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using MetricExchangeTools;
using RegularTimer = System.Timers.Timer;

namespace Sensor
{
    internal sealed class MetricClient
    {
        private readonly AutoResetEvent _waitHandle;
        private readonly RegularTimer _timer;
        private readonly IPEndPoint _endPoint;
        private readonly IErrorProvider _errorProvider;

        public MetricClient(IPEndPoint endPoint, TimeSpan interval, IErrorProvider errorProvider)
        {
            _endPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));
            _errorProvider = errorProvider ?? throw new ArgumentNullException(nameof(errorProvider));
            _waitHandle = new AutoResetEvent(false);
            _timer = new RegularTimer(interval.TotalMilliseconds);
            _timer.Elapsed += (sender, e) => _waitHandle.Set();
        }

        public void Start()
        {
            new Thread(SendMetric) { IsBackground = true }.Start();
            _timer.Start();
        }

        private void SendMetric()
        {
            var metric = int.MinValue;            
            TcpClient tcpClient = null;
            NetworkStream networkStream = null;
            while (true)
            {
                try
                {
                    tcpClient = new TcpClient();
                    tcpClient.Connect(_endPoint);
                    networkStream = tcpClient.GetStream();
                    while (true)
                    {
                        var bytes = BitConverter.GetBytes(metric++);
                        networkStream.Write(bytes, 0, bytes.Length);
                        networkStream.Flush();
                        Console.WriteLine(
                            string.Format(CultureInfo.InvariantCulture,
                                "{0:dd.MM.yyyy HH:mm:ss.fff} {1}", DateTime.Now, metric));
                        _waitHandle.WaitOne();
                    }
                }
                catch (IOException ioException)
                {
                    if (!_errorProvider.TryCapture(ioException, false))
                    {
                        throw;
                    }
                }
                catch (SocketException socketException)
                {
                    _errorProvider.WriteException(socketException);
                }
                finally
                {
                    if (networkStream != null)
                    {
                        networkStream.Dispose();
                        networkStream = null;
                    }
                    if (tcpClient != null)
                    {
                        tcpClient.Close();
                        tcpClient = null;
                    }
                }
                _waitHandle.WaitOne();
            }
        }
    }
}
