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

        public event Action ConnectEvent;
        public event Action DisconnectEvent;
        public event Action<ArraySegment<byte>> ReceiveEvent;
        public event Action<int> SendEvent; 

        public void Send(ArraySegment<byte> buffer)
        {
            lock (_lock)
            {
                try
                {
                    if (Connected == false)
                        throw new UniSocketErrors(FailureType.SEND_ERROR, $"Disconnected from {RemoteEndPoint}");
            
                    _sendQueue.Enqueue(buffer);
                    if (_pendingList.Count == 0)
                        RegisterSend();
                }
                catch (UniSocketErrors e)
                {
                    e.Print();
                    Disconnect();
                }
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
                e.Print();
                Disconnect();
            }

            finally
            {
                _pendingList.Clear();
            }
        }
        
        protected int OnReceive(ArraySegment<byte> buffer)
        {
            int processLength = 0;

            UniSocketErrors.RaiseMsg($"offset : {buffer.Offset} count : {buffer.Count}");

            while (buffer.Count > 0)
            {
                // 최소한 헤더는 파싱할 수 있는지 확인
                if (buffer.Count < 2)
                    break;

                // 패킷이 완전체로 도착했는지 확인
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;
                
                UniSocketErrors.RaiseMsg($"dataSize : {dataSize}");
            
                // 여기까지 왔으면 패킷 조립 가능
                ArraySegment<byte> packet = new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize);
                RaiseReceiveEvent(packet);
                
                processLength += dataSize;

                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
                
                UniSocketErrors.RaiseMsg($"new offset : {buffer.Offset} count : {buffer.Count}");
            }
        
            return processLength;
        }

        protected void RaiseConnectEvent()
        {
            ConnectEvent?.Invoke();
        }

        protected void RaiseDisconnectEvent()
        {
            DisconnectEvent?.Invoke();
        }

        protected void RaiseReceiveEvent(ArraySegment<byte> buffer)
        {
            ReceiveEvent?.Invoke(buffer);
        }

        protected void RaiseSendEvent(int transferred)
        {
            SendEvent?.Invoke(transferred);
        }

        public void Disconnect()
        {
            lock (_lock)
            {
                SelfDisconnect();
                
                RaiseDisconnectEvent();
            }
        }
    }
}