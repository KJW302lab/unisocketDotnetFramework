using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace LAB302
{
    public class UniSocket_Tcp : UniSocket
    {
        private readonly Socket _socket;
        private readonly SocketAsyncEventArgs _recvArgs = new();
        private readonly SocketAsyncEventArgs _sendArgs = new();

        public UniSocket_Tcp(Socket acceptedSocket)
        {
            NetworkMethod = NetworkMethod.TCP;
            
            _socket = acceptedSocket;

            RemoteEndPoint = _socket.RemoteEndPoint!.ToString();

            _sendArgs.Completed += OnSendSegment;
            _recvArgs.Completed += OnReceiveSegment;
            RegisterReceive();
        }

        void RegisterReceive()
        {
            ReceiveBuffer.Clear();
            var segment = ReceiveBuffer.Reserve();
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (pending == false)
                    OnReceiveSegment(null, _recvArgs);
            }
            catch (Exception e)
            {
                Errors.PrintError($"{e}");
            }
        }

        void OnReceiveSegment(object? sender, SocketAsyncEventArgs args)
        {
            var error = args.SocketError;

            try
            {
                if (error != SocketError.Success || args.BytesTransferred <= 0) return;
                RaiseReceiveEvent(args.BytesTransferred, true);
                RegisterReceive();
            }
            catch (Exception e)
            {
                Errors.PrintError($"Receive Failed : {e}");
            }
        }

        protected override void SendBuffer(List<ArraySegment<byte>> bufferList)
        {
            _sendArgs.BufferList = bufferList;

            bool pending = _socket.SendAsync(_sendArgs);
            if (pending == false)
                OnSendSegment(null, _sendArgs);
        }

        void OnSendSegment(object? sender, SocketAsyncEventArgs args)
        {
            var error = args.SocketError;
            var transferred = args.BytesTransferred;

            try
            {
                if (transferred > 0 && error == SocketError.Success)
                    RaiseSendEvent(transferred);
                else
                    Errors.PrintError($"Send Error : {args.SocketError}, TransferredBytes : {transferred}");
            }
            catch (Exception e)
            {
                Errors.PrintError($"Receive Failed : {e}");
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