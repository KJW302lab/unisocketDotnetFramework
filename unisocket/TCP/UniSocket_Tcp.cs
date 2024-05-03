using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace LAB302
{
    public class UniSocket_Tcp : UniSocket
    {
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        private readonly SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

        public UniSocket_Tcp(Socket acceptedSocket)
        {
            NetworkMethod = NetworkMethod.TCP;
            
            _socket = acceptedSocket;

            RemoteEndPoint = _socket.RemoteEndPoint.ToString();

            _sendArgs.Completed += OnSendSegment;
            _recvArgs.Completed += OnReceiveSegment;
            RegisterReceive();
        }

        void RegisterReceive()
        {
            ReceiveBuffer.Clear();
            var segment = ReceiveBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (pending == false)
                    OnReceiveSegment(null, _recvArgs);
            }
            catch (Exception e)
            {
                e.Print();
                Disconnect();
            }
        }

        void OnReceiveSegment(object sender, SocketAsyncEventArgs args)
        {
            var error = args.SocketError;

            try
            {
                if (error != SocketError.Success || args.BytesTransferred <= 0)
                    throw new UniSocketErrors(args.SocketError);

                int transferred = args.BytesTransferred;
                
                ReceiveBuffer.Write(transferred);
                RaiseReceiveEvent(transferred);
                ReceiveBuffer.Read(transferred);
                RegisterReceive();
            }
            catch (UniSocketErrors e)
            {
                e.Print();
                Disconnect();
            }
        }

        protected override void SendBuffer(List<ArraySegment<byte>> bufferList)
        {
            try
            {
                List<ArraySegment<byte>> list = new List<ArraySegment<byte>>();
                list.AddRange(bufferList);

                _sendArgs.BufferList = list;

                bool pending = _socket.SendAsync(_sendArgs);
                if (pending == false)
                    OnSendSegment(null, _sendArgs);
            }
            catch (Exception e)
            {
                e.Print();
                Disconnect();
            }
        }

        void OnSendSegment(object sender, SocketAsyncEventArgs args)
        {
            var error = args.SocketError;
            var transferred = args.BytesTransferred;

            try
            {
                if (transferred > 0 && error == SocketError.Success)
                    RaiseSendEvent(transferred);
                else
                    throw new UniSocketErrors(FailureType.SEND_ERROR,
                        $"Transferred data : {transferred} SocketError : {args.SocketError}");
            }
            catch (UniSocketErrors e)
            {
                e.Print();
                Disconnect();
            }
        }

        protected override bool IsConnected()
        {
            return _socket.Connected;
        }

        protected override void SelfDisconnect()
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
    }
}