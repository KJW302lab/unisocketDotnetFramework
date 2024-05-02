using System;
using System.Collections.Generic;

namespace LAB302
{
    public abstract partial class UniSocket
    {
        public NetworkMethod NetworkMethod { get; protected set; }
        
        protected ReceiveBuffer ReceiveBuffer = new ReceiveBuffer(65535);
        
        private object _lock = new object();
        private Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        private List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        
        protected abstract void SendBuffer(List<ArraySegment<byte>> bufferList);
        protected abstract bool IsConnected();
        protected abstract void SelfDisconnect();
        
        public bool Connected => IsConnected();

        private readonly List<IConnectCallback>    _connectCallbacks = new List<IConnectCallback>();
        private readonly List<IDisconnectCallback> _disconnectCallbacks = new List<IDisconnectCallback>();
        private readonly List<ISendCallback>       _sendCallbacks = new List<ISendCallback>();
        private readonly List<IReceiveCallback>    _receiveCallbacks = new List<IReceiveCallback>();

        public event Action ConnectEvent;
        public event Action DisconnectEvent;
        public event Action<byte[]> ReceiveEvent;
        public event Action<int> SendEvent; 

        public void Send(ArraySegment<byte> buffer)
        {
            lock (_lock)
            {
                if (Connected == false)
                {
                    Errors.PrintFailure($"Send Failed at {RemoteEndPoint} ", FailureType.CONNECTION_DISCONNECTED);
                    return;
                }
            
                _sendQueue.Enqueue(buffer);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
        }

        void RegisterSend()
        {
            if (Connected == false)
                return;

            while (_sendQueue.Count > 0)
            {
                var buffer = _sendQueue.Dequeue();
                if (buffer.Array != null)
                    _pendingList.Add(buffer);
            }

            try
            {
                SendBuffer(_pendingList);
            }
            catch (Exception e)
            {
                Errors.PrintError($"{e}");
            }

            finally
            {
                _pendingList.Clear();
            }
        }

        protected void RaiseConnectEvent()
        {
            ConnectEvent?.Invoke();
            _connectCallbacks.ForEach(callback => callback.OnConnect());
        }

        protected void RaiseDisconnectEvent()
        {
            DisconnectEvent?.Invoke();
            _disconnectCallbacks.ForEach(callback => callback.OnDisconnected());
        }

        protected void RaiseReceiveEvent(int transferred, bool succeed)
        {
            try
            {
                if (succeed == false)
                {
                    Errors.PrintError($"Receive Failed : Receive Buffer not enough");
                    return;
                }

                var readSegment = ReceiveBuffer.ReadSegment;

                byte[] arr = new byte[transferred];

                for (int i = 0; i < transferred; i++)
                    if (readSegment.Array != null)
                        arr[i] = readSegment.Array[i];

                ReceiveEvent?.Invoke(arr);
                _receiveCallbacks.ForEach(callback => callback.OnReceive(arr));
            }
            catch (Exception e)
            {
                Errors.PrintError($"{e}");
            }
        }

        protected void RaiseSendEvent(int transferred)
        {
            SendEvent?.Invoke(transferred);
            _sendCallbacks.ForEach(callback => callback.OnSend(transferred));
        }

        public void Disconnect()
        {
            lock (_lock)
            {
                if (Connected == false)
                    return;
            
                SelfDisconnect();
                
                RaiseDisconnectEvent();
            }
        }

        public void AddConnectCallback(IConnectCallback callback)
        {
            if (callback == null)
                return;
            
            if (_connectCallbacks.Contains(callback) == false)
                _connectCallbacks.Add(callback);
        }
        
        public void AddDisconnectCallback(IDisconnectCallback callback)
        {
            if (callback == null)
                return;
            
            if (_disconnectCallbacks.Contains(callback) == false)
                _disconnectCallbacks.Add(callback);
        }
        
        public void AddSendCallback(ISendCallback callback)
        {
            if (callback == null)
                return;
            
            if (_sendCallbacks.Contains(callback) == false)
                _sendCallbacks.Add(callback);
        }
        
        public void AddReceiveCallback(IReceiveCallback callback)
        {
            if (callback == null)
                return;
            
            if (_receiveCallbacks.Contains(callback) == false)
                _receiveCallbacks.Add(callback);
        }
    }
}