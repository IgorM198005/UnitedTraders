using System.IO;
using System.Net.Sockets;

namespace MetricExchangeTools
{
    public interface IErrorProvider
    {        
        bool TryCapture(IOException e, bool notWriteAborted);
        void WriteException(SocketException e);
    }
}
