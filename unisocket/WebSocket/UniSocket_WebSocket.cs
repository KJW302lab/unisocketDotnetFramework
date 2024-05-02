using System;
using System.Collections.Generic;
using WebSocketSharp;

namespace LAB302
{
    public class UniSocket_WebSocket : UniSocket
    {
        private WebSocket _socket;
        
        public UniSocket_WebSocket(WebSocket acceptedSocket, string endPoint)
        {
            NetworkMethod = NetworkMethod.WEB_SOCKET;
            
            _socket = acceptedSocket;
    
            _socket.OnMessage += OnMessage;
    
            RemoteEndPoint = endPoint;
        }
    
        void OnMessage(object? sender, MessageEventArgs args)
        {
            var data = args.RawData;
    
            if (data == null)
            {
                Errors.PrintError($"Receive Failed : WebSocket OnMessaged Data is null.");
                return;
            }
    
            try
            {
                RaiseReceiveEvent( data.Length, ReceiveBuffer.Write(data));
            }
            catch (Exception e)
            {
                Errors.PrintError($"Receive Failed : {e}");
            }
        }
        
        protected override void SendBuffer(List<ArraySegment<byte>> bufferList)
        {
            int count = 0;
            
            bufferList.ForEach(buffer =>
            {
                _socket.Send(buffer.Array);
                count += buffer.Count;
            });
            
            RaiseSendEvent(count);
        }
    
        protected override bool IsConnected()
        {
            return _socket.IsAlive;
        }
    
        protected override void SelfDisconnect()
        {
            _socket.Close();
        }
    }
}