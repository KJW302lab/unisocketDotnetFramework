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
    
        void OnMessage(object sender, MessageEventArgs args)
        {
            var data = args.RawData;

            try
            {
                if (data == null)
                    throw new UniSocketErrors(FailureType.RECEIVE_ERROR, $"received data is null");
                
                ReceiveBuffer.Copy(data);
                var proceed = OnReceive(ReceiveBuffer.ReadSegment);
                ReceiveBuffer.Read(proceed);
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
                int count = 0;
            
                bufferList.ForEach(buffer =>
                {
                    byte[] array = new byte[buffer.Count];
                    Array.Copy(buffer.Array, buffer.Offset, array, 0, buffer.Count);
                    _socket.Send(array);
                    count += buffer.Count;
                });
            
                RaiseSendEvent(count);
            }
            catch (Exception e)
            {
                e.Print();
                Disconnect();
            }
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