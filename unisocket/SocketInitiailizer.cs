using System.Net;

namespace LAB302
{
    public enum NetworkMethod
    {
        TCP,
        WEB_SOCKET,
    }
    
    public struct SocketInitializer
    {
        public NetworkMethod NetworkMethod { get; private set; }
    
        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public string ServiceName { get; private set; }

        private IPAddress _address;

        public SocketInitializer(NetworkMethod method, string ipAddress, int port)
        {
            if (IPAddress.TryParse(ipAddress, out var address) == false)
                Errors.PrintError($"{address} is invalid IpAddress.", true);

            _address = address;
            NetworkMethod = method;
            IpAddress = ipAddress;
            Port = port;
            ServiceName = "/";
        }
    
        public SocketInitializer(NetworkMethod method, string ipAddress, int port, string serviceName)
        {
            if (IPAddress.TryParse(ipAddress, out var address) == false)
                Errors.PrintError($"{address} is invalid IpAddress.", true);

            _address = address;
            NetworkMethod = method;
            IpAddress = ipAddress;
            Port = port;

            if (serviceName.StartsWith("/") == false)
                serviceName = "/" + serviceName;
        
            ServiceName = serviceName;
        }
    }
}