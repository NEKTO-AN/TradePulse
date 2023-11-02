using System.Net.WebSockets;
using Application.Behaviors.Socket;
using Domain.Exchange;
using Microsoft.Extensions.Configuration;

namespace Application.Behaviors.Exchange
{
    public class ExchangeWebSocketBehavior
    {
        private readonly ClientWebSocket ws = new();
        private readonly WebSocketBehavior webSocket;

        public ExchangeWebSocketBehavior(IConfiguration configuration)
		{
            webSocket = new(
                handler: new WebSocketHandler(ws),
                url: configuration["BYBIT_SOCKET_URL_PUBLIC_LINEAR"] ?? throw new Exception(),
                receiveBufferSize: 1024 * 32);
        }


		public async Task OrderbookAsync(string[] topics, Func<string, Task> onMessageReceived, CancellationToken cancellationToken)
        {
            //const string symbol = "OPUSDT";

            await webSocket.ConnectAsync(cancellationToken);
            await webSocket.SendAsync(new WebSocketFilters("subscribe", topics).ToString(), cancellationToken);


            webSocket.OnMessageReceived(onMessageReceived, cancellationToken);
        }
	}
}

