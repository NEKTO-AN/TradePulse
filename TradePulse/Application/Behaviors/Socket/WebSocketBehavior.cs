using System.Net.WebSockets;
using System.Text;
using Application.Abstractions;

namespace Application.Behaviors.Socket
{
    public class WebSocketBehavior : IDisposable
    {
        private readonly IWebSocketHandler handler;
        private readonly List<Func<string, Task>> onMessageReceivedFunctions;
        private readonly List<CancellationTokenRegistration> onMessageReceivedCancellationTokenRegistrations;
        private CancellationTokenSource loopCancellationTokenSource;
        private readonly Uri url;
        private readonly int receiveBufferSize;

        public WebSocketBehavior(IWebSocketHandler handler, string url, int receiveBufferSize = 8192)
        {
            this.handler = handler;
            this.url = new Uri(url);
            this.receiveBufferSize = receiveBufferSize;
            onMessageReceivedFunctions = new List<Func<string, Task>>();
            onMessageReceivedCancellationTokenRegistrations = new List<CancellationTokenRegistration>();

            loopCancellationTokenSource = new();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            if (handler.State != WebSocketState.Open)
            {
                loopCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                await handler.ConnectAsync(url, cancellationToken);
                await Task.Factory.StartNew(() => ReceiveLoop(loopCancellationTokenSource.Token, receiveBufferSize), loopCancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            loopCancellationTokenSource?.Cancel();

            if (handler.State == WebSocketState.Open)
            {
                await handler.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
                await handler.CloseAsync(WebSocketCloseStatus.NormalClosure, null, cancellationToken);
            }
        }

        public void OnMessageReceived(Func<string, Task> onMessageReceived, CancellationToken cancellationToken)
        {
            onMessageReceivedFunctions.Add(onMessageReceived);

            if (cancellationToken != CancellationToken.None)
            {
                CancellationTokenRegistration reg = cancellationToken.Register(() =>
                    onMessageReceivedFunctions.Remove(onMessageReceived));

                onMessageReceivedCancellationTokenRegistrations.Add(reg);
            }
        }

        public async Task SendAsync(string message, CancellationToken cancellationToken)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(message);

            await handler.SendAsync(new ArraySegment<byte>(byteArray), WebSocketMessageType.Text, true, cancellationToken);
        }

        public void Dispose()
        {
            DisconnectAsync(CancellationToken.None).Wait();

            handler.Dispose();

            onMessageReceivedCancellationTokenRegistrations.ForEach(ct => ct.Dispose());

            loopCancellationTokenSource.Dispose();
        }

        private async Task ReceiveLoop(CancellationToken cancellationToken, int receiveBufferSize = 8192)
        {
            WebSocketReceiveResult? receiveResult = null;
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[receiveBufferSize]);
                    receiveResult = await handler.ReceiveAsync(buffer, cancellationToken);

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }

                    string content = Encoding.UTF8.GetString(buffer.ToArray(), buffer.Offset, buffer.Count);
                    onMessageReceivedFunctions.ForEach(omrf => omrf(content));
                }
            }
            catch (TaskCanceledException)
            {
                await DisconnectAsync(CancellationToken.None);
            }
        }
    }
}

