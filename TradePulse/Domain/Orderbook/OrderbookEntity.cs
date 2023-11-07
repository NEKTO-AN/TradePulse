using Newtonsoft.Json;

namespace Domain.Orderbook
{
    public class Orderbook
	{
		[JsonProperty("ts")]
		public long Timestamp { get; set; }

		[JsonProperty("symbol")]
		public string Symbol { get; set; }

		[JsonProperty("data")]
		public DataModel Data { get; set; }

		public Orderbook(long timestamp, string symbol, DataModel data)
		{
			Timestamp = timestamp;
			Symbol = symbol;
            Data = data;
        }


        public class DataModel
        {
            [JsonProperty("bids")]
            public double[][] Bids { get; set; }

            [JsonProperty("asks")]
            public double[][] Asks { get; set; }

            [JsonProperty("seq")]
            public long CrossSequence { get; set; }

            public DataModel(double[][] bids, double[][] asks, long crossSequence)
            {
                Bids = bids;
                Asks = asks;
                CrossSequence = crossSequence;
            }
        }
    }
}

