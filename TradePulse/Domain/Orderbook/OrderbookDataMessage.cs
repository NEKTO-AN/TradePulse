using Domain.Orderbook.Exchange.WebSocket;
using Newtonsoft.Json;

namespace Domain.Orderbook
{
    public class OrderbookDataMessage : OrderbookResponse
    {
        [JsonProperty("lp")]
        public double LastPrice { get; set; }

        public OrderbookDataMessage(long timestamp, DataModel data) : base(timestamp, data)
        {

        }
    }
}