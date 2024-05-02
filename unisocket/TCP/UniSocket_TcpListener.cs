using System;
using System.Net;
using System.Net.Sockets;

namespace LAB302
{
    public class UniSocket_TcpListener : UniSocketListener
    {
        private readonly Socket?           _listenSocket;
        private readonly IPEndPoint        _endPoint;
        private readonly Action<UniSocket> _onAccetpted;
    
        public UniSocket_TcpListener(string ipAddress, int port, Action<UniSocket> acceptedSocket)
        {
            _onAccetpted = acceptedSocket;
            _endPoint = new(IPAddress.Parse(ipAddress), port);
            _listenSocket = new(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listenSocket.Bind(_endPoint);
            _listenSocket.Listen(100);
            
            Console.WriteLine($"Server : {_endPoint.Address} listening...");
            
            RegisterAccept();
        }
    
        void RegisterAccept()
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    SocketAsyncEventArgs args = new();
                    args.Completed += OnAcceptCompleted;
    
                    bool pending = _listenSocket.AcceptAsync(args);
                    if (pending == false)
                        OnAcceptCompleted(null, args);
                }
            }
            catch (Exception e)
            {
                Errors.PrintError($"{e}");
            }
        }
        
        void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                Errors.PrintError($"{args.SocketError}");
                return;
            }
    
            var acceptedSocket = args.AcceptSocket;
    
            if (acceptedSocket == null)
            {
                Errors.PrintError($"There is no AcceptSocket in args : {args.AcceptSocket}");
                return;
            }
    
            _onAccetpted?.Invoke(new UniSocket_Tcp(acceptedSocket));
            
            RegisterAccept();
        }
    }
}