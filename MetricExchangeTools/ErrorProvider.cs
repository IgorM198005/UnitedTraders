using System;
using System.IO;
using System.Net.Sockets;


namespace MetricExchangeTools
{
    public sealed class ErrorProvider : IErrorProvider
    {
        public bool TryCapture(IOException e, bool notWriteAborted)
        {
            if (e.InnerException is SocketException socketException)
            {
                if (!(notWriteAborted && IsAborted(socketException.SocketErrorCode)))
                {
                    WriteException(socketException);
                }                
                return true;
            }

            return false;
        }
        public void WriteException(SocketException e)
        {
            Console.WriteLine($"{e.Message} ({e.ErrorCode})");
        }
        private static bool IsAborted(SocketError error)
        {
            return error == SocketError.ConnectionAborted ||
                   error == SocketError.OperationAborted ||
                   error == SocketError.ConnectionReset;
        }
    }
}
