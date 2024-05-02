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

    public ArraySegment<byte> Reserve()
    {
        return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize);
    }

    public bool Write(byte[] array)
    {
        if (_buffer.Count < array.Length)
        {
            Errors.PrintError($"Data to write is too large than receive buffer. (buffer size : {_buffer.Count} data size : {array.Length}");
            return false;
        }

        if (FreeSize < array.Length)
        {
            if (DataSize + array.Length > _buffer.Count)
            {
                Errors.PrintError($"Data to write is too large than receive buffer's FreeSize.");
                return false;
            }
            
            Clear();
        }

        Array.Copy(
            sourceArray: array,
            sourceIndex: 0,
            destinationArray: _buffer.Array,
            destinationIndex: _buffer.Offset + _writePos,
            length: array.Length
        );

        _writePos += array.Length;

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