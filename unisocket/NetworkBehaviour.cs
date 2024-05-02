using System;

namespace LAB302
{
    public abstract class NetworkBehaviour
    {
        protected UniSocket connectedSocket;

        public bool Connected => connectedSocket != null && connectedSocket.Connected;

        public void Initialize(UniSocket socket)
        {
            connectedSocket = socket;

            connectedSocket.SendEvent += OnSend;
            connectedSocket.ReceiveEvent += OnReceive;

            OnConnected();
        }
    
        public void Send(ArraySegment<byte> buffer)
        {
            connectedSocket.Send(buffer);
        }

        public void Disconnect()
        {
            connectedSocket.Disconnect();
        
            OnDisconnected();
        }



        #region LifeCycle
        public virtual void OnConnected()
        {
        
        }

        public virtual void OnSend(int bytes)
        {
        
        }

        public virtual void OnReceive(byte[] buffer)
        {
        
        }

        public virtual void OnDisconnected()
        {
        
        }
        #endregion
    }
}