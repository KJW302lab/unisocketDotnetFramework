﻿using System;
using System.Collections.Generic;

namespace LAB302
{
    public abstract partial class UniSocket
    {
        public string RemoteEndPoint { get; protected set; }
        
        private static List<UniSocketListener>  _listeners = new List<UniSocketListener>();
        private static List<UniSocketConnector> _connectors = new List<UniSocketConnector>();

        public static void Listen(SocketInitializer initializer, Action<UniSocket> acceptedSocket)
        {
            if (initializer.NetworkMethod == NetworkMethod.TCP)
            {
                UniSocket_TcpListener listener = new UniSocket_TcpListener(initializer.IpAddress, initializer.Port, acceptedSocket);
                _listeners.Add(listener);
            }
            else
            {
                UniSocket_WebSocketListener listener = new UniSocket_WebSocketListener(initializer.IpAddress, initializer.Port,
                    initializer.ServiceName, acceptedSocket);
                _listeners.Add(listener);
            }
        }

        public static void Connect(SocketInitializer initializer, Action<UniSocket> connectedSocket)
        {
            if (initializer.NetworkMethod == NetworkMethod.TCP)
            {
                UniSocket_TcpConnector connector = new UniSocket_TcpConnector(initializer.IpAddress, initializer.Port, connectedSocket);
                _connectors.Add(connector);
            }
            else
            {
                UniSocket_WebSocketConnector connector = new UniSocket_WebSocketConnector(initializer.IpAddress, initializer.Port,
                    initializer.ServiceName, connectedSocket);
                
                _connectors.Add(connector);
            }
        }
    }
}