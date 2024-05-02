namespace LAB302
{
    public interface IConnectCallback
    {
        void OnConnect();
    }

    public interface IDisconnectCallback
    {
        void OnDisconnected();
    }

    public interface ISendCallback
    {
        void OnSend(int transferred);
    }

    public interface IReceiveCallback
    {
        void OnReceive(byte[] buffer);
    }
}