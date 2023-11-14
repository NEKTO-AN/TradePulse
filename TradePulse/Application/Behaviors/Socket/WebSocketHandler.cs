using System.Net.WebSockets;
using Application.Abstractions;

namespace Application.Behaviors.Socket
{
    public class WebSocketHandler : IWebSocketHandler
    {
        private readonly ClientWebSocket webSocket;

        public WebSocketHandler(ClientWebSocket clientWebSocket)
        {
            webSocket = clientWebSocket;
        }

        public WebSocketState State { get => webSocket.State; }

        public Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
            => webSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);

        public Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken)
            => webSocket.CloseOutputAsync(closeStatus, statusDescription, cancellationToken);

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
            => webSocket.ConnectAsync(uri, cancellationToken);

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
            => webSocket.ReceiveAsync(buffer, cancellationToken);

        public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
            => webSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
    }
}

