using System;
using System.Net;
using System.Net.Sockets;

namespace LAB302
{
    public class UniSocket_TcpConnector : UniSocketConnector
    {
        private Socket? _connectSocket;

        public UniSocket_TcpConnector(string ipAddress, int port, Action<UniSocket> connectedSocket)
        {
            try
            {
                IPEndPoint endPoint = new(IPAddress.Parse(ipAddress), port);

                _connectSocket = new(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                SocketAsyncEventArgs args = new();
                args.RemoteEndPoint = endPoint;
                args.UserToken = connectedSocket;
                args.Completed += OnConnectCompleted;
            
                bool pending = _connectSocket.ConnectAsync(args);
                if (pending == false)
                    OnConnectCompleted(null, args);
            }
            catch (Exception e)
            {
                Errors.PrintError($"{e}");
            }
        }

        void OnConnectCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                Errors.PrintError($"{args.SocketError}");
                return;
            }

            var connectedSocket = args.ConnectSocket;

            if (connectedSocket == null)
            {
                Errors.PrintError($"There is no ConnectSocket in args : {args.AcceptSocket}");
                return;
            }

            var callback = (Action<UniSocket>)args.UserToken!;
        
            callback.Invoke(new UniSocket_Tcp(connectedSocket));
        }
    }
}