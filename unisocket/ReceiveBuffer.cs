using System;

namespace LAB302
{
    public class ReceiveBuffer
    {
        private ArraySegment<byte> _buffer;
        private int _readPos;
        private int _writePos;

        public ReceiveBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize => _writePos - _readPos;
        public int FreeSize => _buffer.Count - _writePos;
        
        public ArraySegment<byte> ReadSegment => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize);
        public ArraySegment<byte> WriteSegment => new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize);

        public void Clear()
        {
            int dataSize = DataSize;
            
            if (dataSize == 0)
                _readPos = _writePos = 0;
            
            else
            {
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public void Copy(byte[] array)
        {
            if (Write(array.Length) == false)
                return;

            Array.Copy(array, 0, _buffer.Array, _buffer.Offset + _readPos, array.Length);
        }

        public bool Write(int bytesToWrite)
        {
            if (bytesToWrite > FreeSize)
                return false;

            _writePos += bytesToWrite;

            return true;
        }

        public bool Read(int bytesToRead)
        {
            if (bytesToRead > DataSize)
                return false;

            _readPos += bytesToRead;
            return true;
        }
    }
}