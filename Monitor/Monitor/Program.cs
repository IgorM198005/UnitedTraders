using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MetricExchangeTools;

namespace Monitor
{
    class Program
    {
        static void Main(string[] args)
        {
            var endPoint = IpValidationHelper.Parse(args);
            new Listener(endPoint, new ErrorProvider()).Start();
            Console.ReadKey();
        }
    }
}
