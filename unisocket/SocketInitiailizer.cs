using System;
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
            try
            {
                if (IPAddress.TryParse(ipAddress, out var address) == false)
                    throw new UniSocketErrors(FailureType.IP_PARSE_ERROR, $"{ipAddress}");

                _address = address;
                NetworkMethod = method;
                IpAddress = ipAddress;
                Port = port;
                ServiceName = "/";
            }
            catch (UniSocketErrors e)
            {
                e.Print();
                throw;
            }
        }
    
        public SocketInitializer(NetworkMethod method, string ipAddress, int port, string serviceName)
        {
            try
            {
                if (IPAddress.TryParse(ipAddress, out var address) == false)
                    throw new UniSocketErrors(FailureType.IP_PARSE_ERROR, $"{ipAddress}");

                _address = address;
                NetworkMethod = method;
                IpAddress = ipAddress;
                Port = port;

                if (serviceName.StartsWith("/") == false)
                    serviceName = "/" + serviceName;
        
                ServiceName = serviceName;
            }
            catch (UniSocketErrors e)
            {
                e.Print();
                throw;
            }
        }
    }
}