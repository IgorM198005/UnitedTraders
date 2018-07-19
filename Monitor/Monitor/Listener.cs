using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MetricExchangeTools;

namespace Monitor
{
    internal sealed class Listener
    {
        private readonly IPEndPoint _endPoint;
        private TcpListener _tcpListener;
        private readonly IErrorProvider _errorProvider;
        private readonly TimeSpan _interval;

        public Listener(IPEndPoint endPoint, IErrorProvider errorProvider, TimeSpan interval)
        {
            _interval = interval;
            _errorProvider = errorProvider ?? throw new ArgumentNullException(nameof(errorProvider));
            _endPoint = endPoint ?? throw new ArgumentNullException(nameof(endPoint));                        
        }

        public void Start()
        {
            _tcpListener = new TcpListener(_endPoint);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(AcceptTcpClientCallback, null);
        }

        private void AcceptTcpClientCallback(IAsyncResult ar)
        {
            var incomingClient = _tcpListener.EndAcceptTcpClient(ar);
            new Session(incomingClient, _errorProvider, _interval).BeginRead();
            _tcpListener.BeginAcceptTcpClient(AcceptTcpClientCallback, _tcpListener);
        }
    }
}
