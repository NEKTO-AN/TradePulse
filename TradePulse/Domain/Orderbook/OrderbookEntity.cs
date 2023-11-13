using Domain.Orderbook.Exchange.WebSocket;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.Orderbook
{
    public class Orderbook
	{
		[JsonProperty("ts")]
        [BsonElement("_id")]
		public long Timestamp { get; set; }

		[JsonProperty("symbol")]
        [BsonElement("symbol")]
        public string Symbol { get; set; }

		[JsonProperty("data")]
        [BsonElement("data")]
        public DataModel Data { get; set; }

		public Orderbook(long timestamp, string symbol, DataModel data)
		{
			Timestamp = timestamp;
			Symbol = symbol;
            Data = data;
        }

        public Orderbook(long timestamp, OrderbookResponse.DataModel dataModel)
        {
            Timestamp = timestamp;
            Symbol = dataModel.Symbol;
            Data = new(dataModel.Bids, dataModel.Asks, dataModel.CrossSequence);
        }


        public class DataModel
        {
            [JsonProperty("bids")]
            [BsonElement("bids")]
            public double[][] Bids { get; set; }

            [JsonProperty("asks")]
            [BsonElement("asks")]
            public double[][] Asks { get; set; }

            [JsonProperty("seq")]
            [BsonElement("seq")]
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

