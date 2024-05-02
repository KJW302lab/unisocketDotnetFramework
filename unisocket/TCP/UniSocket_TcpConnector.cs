using System;
using System.Net;
using System.Net.Sockets;

namespace LAB302
{
    public class UniSocket_TcpConnector : UniSocketConnector
    {
        private Socket _connectSocket;

        public UniSocket_TcpConnector(string ipAddress, int port, Action<UniSocket> connectedSocket)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

                _connectSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
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

        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                Errors.PrintError($"{args.SocketError}");
                return;
            }

            var callback = (Action<UniSocket>)args.UserToken;
        
            callback.Invoke(new UniSocket_Tcp(_connectSocket));
        }
    }
}