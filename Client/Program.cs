using System;
using LAB302;

namespace Client
{
    static class Program
    {
        static void Main()
        {
            SocketInitializer initializer = new SocketInitializer(NetworkMethod.TCP, "10.34.221.32", 7777);
            UniSocket.Connect(initializer, socket =>
            {
                Console.WriteLine(socket.RemoteEndPoint);
            });
        }
    }
}