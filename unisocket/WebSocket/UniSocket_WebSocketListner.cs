using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace LAB302
{
    public class UniSocket_WebSocketListener : UniSocketListener
    {
        private WebSocketServer _ws;

        public UniSocket_WebSocketListener(string ipAddress, int port, string serviceName, Action<UniSocket> acceptedSocket)
        {
            try
            {
                string url = $"ws://{ipAddress}:{port}";

                _ws = new WebSocketServer(url);

                void Callback(WebSocket socket, string endPoint) => acceptedSocket.Invoke(new UniSocket_WebSocket(socket, endPoint));

                _ws.AddWebSocketService(serviceName, ()=> new UniSocket_WebSocketServer_Service(Callback));
            
                _ws.Start();

                Console.WriteLine($"Server : {url}{serviceName} listening...");
            }
            catch (Exception e)
            {
                Errors.PrintError($"{e}");
            }
        }
    }

    public class UniSocket_WebSocketServer_Service : WebSocketBehavior
    {
        private readonly Action<WebSocket, string> _acceptHandler;

        public UniSocket_WebSocketServer_Service(Action<WebSocket, string> onAccepted)
        {
            _acceptHandler = onAccepted;
        }
    
        protected override void OnOpen()
        {
            _acceptHandler?.Invoke(Context.WebSocket, Context.UserEndPoint.ToString());
        }
    }
}