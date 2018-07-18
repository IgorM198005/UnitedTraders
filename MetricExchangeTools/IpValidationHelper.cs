using System;
using System.Globalization;
using System.Net;


namespace MetricExchangeTools
{
    public static class IpValidationHelper
    {
        public static IPEndPoint Parse(string[] args)
        {            
            if (args.Length < 2)
            {
                throw new ArgumentException("указано недостаточно параметров, ожидаются - [ip-адрес] [порт]");
            }
            if (!IPAddress.TryParse(args[0], out var ipAddress))
            {
                throw new ArgumentException($"значение не является IP адресом '{args[0]}'");
            }
            if (!UInt16.TryParse(args[1], NumberStyles.None, CultureInfo.InvariantCulture, out var port))
            {
                throw new ArgumentException($"значение не является номером порта '{args[1]}'");
            }
            
            return new IPEndPoint(ipAddress, port);
        }
    }
}
