using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MetricExchangeTools;

namespace Sensor
{
    class Program
    {
        static void Main(string[] args)
        {
            var endPoint = IpValidationHelper.Parse(args);
            new MetricClient(endPoint, MetricEnvironment.Interval, new ErrorProvider()).Start();
            Console.ReadKey();
        }
    }    
}
