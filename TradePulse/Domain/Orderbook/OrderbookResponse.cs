using Newtonsoft.Json;

namespace Domain.Orderbook
{
    public class OrderbookResponse
    {
        [JsonProperty("ts")]
        public long Timestamp { get; set; }

        [JsonProperty("data")]
        public DataModel? Data { get; set; }

        public OrderbookResponse(long timestamp, DataModel data)
		{
            Timestamp = timestamp;
            Data = data;
		}


        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.None);


        public class DataModel
        {
            [JsonProperty("s")]
            public string Symbol { get; set; }

            [JsonProperty("b")]
            public double[][] Bids { get; set; }

            [JsonProperty("a")]
            public double[][] Asks { get; set; }

            [JsonProperty("u")]
            public long UpdateId { get; set; }

            [JsonProperty("seq")]
            public long CrossSequence { get; set; }

            public DataModel(string symbol, double[][] bids, double[][] asks, long updateId, long crossSequence)
            {
                Symbol = symbol;
                Bids = bids;
                Asks = asks;
                UpdateId = updateId;
                CrossSequence = crossSequence;
            }
        }
	}
}

