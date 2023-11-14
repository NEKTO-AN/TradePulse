using Newtonsoft.Json;

namespace Domain.Exchange.WebSocket
{
    public class TickerResponse
    {
        [JsonProperty("ts")]
        public long Timestamp { get; set; }

        [JsonProperty("cs")]
        public long CrossSequence { get; set; }

        [JsonProperty("data")]
        public DataModel Data { get; set; }

        public TickerResponse(long timestamp, long crossSequence, DataModel data)
        {
            Timestamp = timestamp;
            CrossSequence = crossSequence;
            Data = data;
        }

        public class DataModel
        {
            [JsonProperty("symbol")]
            public string Symbol { get; set; }

            [JsonProperty("lastPrice")]
            public double LastPrice { get; set; }

            public DataModel(string symbol, double lastPrice)
            {
                Symbol = symbol;
                LastPrice = lastPrice;
            }
        }
    }
}