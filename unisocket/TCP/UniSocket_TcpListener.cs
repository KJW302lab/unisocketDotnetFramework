using System;
using System.Net;
using System.Net.Sockets;

namespace LAB302
{
    public class UniSocket_TcpListener : UniSocketListener
    {
        private readonly Socket            _listenSocket;
        private readonly IPEndPoint        _endPoint;
        private readonly Action<UniSocket> _onAccetpted;
    
        public UniSocket_TcpListener(string ipAddress, int port, Action<UniSocket> acceptedSocket)
        {
            _onAccetpted = acceptedSocket;
            _endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            _listenSocket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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
                    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                    args.Completed += OnAcceptCompleted;
    
                    bool pending = _listenSocket.AcceptAsync(args);
                    if (pending == false)
                        OnAcceptCompleted(null, args);
                }
            }
            catch (Exception e)
            {
                e.Print();
            }
        }
        
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            try
            {
                if (args.SocketError != SocketError.Success)
                    throw new UniSocketErrors(FailureType.ACCEPT_ERROR, args.SocketError);
    
                var acceptedSocket = args.AcceptSocket;

                if (acceptedSocket == null)
                    throw new UniSocketErrors(FailureType.ACCEPT_ERROR, $"Accept Socket is null.");
    
                _onAccetpted?.Invoke(new UniSocket_Tcp(acceptedSocket));
            
                RegisterAccept();
            }
            catch (UniSocketErrors e)
            {
                e.Print();
            }
        }
    }
}