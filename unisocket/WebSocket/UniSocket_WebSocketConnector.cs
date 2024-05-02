using System;
using WebSocketSharp;

namespace LAB302
{
    public class UniSocket_WebSocketConnector : UniSocketConnector
    {
        private WebSocket _ws;

        public UniSocket_WebSocketConnector(string ipAddress, int port, string serviceName, Action<UniSocket> connectedSocket)
        {
            try
            {
                string url = $"ws://{ipAddress}:{port}{serviceName}";

                _ws = new WebSocket(url);
            
                _ws.OnOpen += (sender, args) => connectedSocket.Invoke(new UniSocket_WebSocket(_ws, url));

                _ws.Connect();
            }
            catch (Exception e)
            {
                Errors.PrintError($"{e}");
            }
        }
    }
}