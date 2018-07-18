using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MetricExchangeTools;

namespace Monitor
{
    internal sealed class Session
    {
        private readonly byte[] _bytes;
        private readonly NetworkStream _stream;
        private readonly TcpClient _client;
        private readonly IErrorProvider _errorProvider;

        public Session(TcpClient client, IErrorProvider errorProvider)
        {
            _errorProvider = errorProvider ?? throw new ArgumentNullException(nameof(errorProvider));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _stream = client.GetStream();
            _bytes = new byte[sizeof(int)];
        }

        public void BeginRead()
        {
            ThreadPool.QueueUserWorkItem(DoBeginRead);
        }

        private void DoBeginRead(object state)
        {
            bool forceClose = true;

            try
            {
                var ar = _stream.BeginRead(_bytes, 0, _bytes.Length, null, null);
                ThreadPool.RegisterWaitForSingleObject(ar.AsyncWaitHandle, EndRead, ar, TimeSpan.FromSeconds(6), true);
                forceClose = false;
            }
            catch (IOException e)
            {
                if (!_errorProvider.TryCapture(e, true))
                {
                    throw;
                }
            }
            finally
            {
                if (forceClose)
                {
                    _stream.Close();
                    _client.Close();
                }                    
            }                        
        }

        private void EndRead(object state, bool timeOut)
        {
            bool forceClose = true;

            try
            {
                if (!(timeOut || _stream.EndRead((IAsyncResult)state) == 0))
                {
                    Console.WriteLine(
                        $"{_client.Client.RemoteEndPoint} {DateTime.Now:dd.MM.yyyy-HH:mm:ss.fff} {BitConverter.ToInt32(_bytes, 0)}");

                    forceClose = false;
                }
            }
            catch (IOException e)
            {
                if (!_errorProvider.TryCapture(e, true))
                {
                    throw;
                }
            }
            finally
            {
                if (forceClose)
                {
                    _stream.Close();
                    _client.Close();
                }                    
            }

            if (!forceClose) DoBeginRead(null);
        }        
    }
}
