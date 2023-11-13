using MongoDB.Bson.Serialization.Attributes;

namespace Domain.PumpDumpSnapshot
{
    public class PumpDumpSnapshot
    {
        [BsonElement("_id")]
        public Guid Id { get; set; }

        [BsonElement("symbol")]
        public string Symbol { get; set; }

        [BsonElement("type")]
        public PumpAndDumpType Type { get; set; }

        [BsonElement("price")]
        public Data<double> Price { get; set; }

        [BsonElement("time")]
        public Data<long> Time { get; set; }

        public PumpDumpSnapshot(Guid id, string symbol, PumpAndDumpType type, Data<double> price, Data<long> time)
        {
            Id = id;
            Symbol = symbol;
            Type = type;
            Price = price;
            Time = time;
        }

        public static PumpDumpSnapshot Create(string symbol, PumpAndDumpType type, Data<double> price, Data<long> time)
        {
            return new(Guid.NewGuid(), symbol, type, price, time);
        }

        public class Data<T>
        {
            [BsonElement("max")]
            public T Max { get; set; }

            [BsonElement("min")]
            public T Min { get; set; }

            public Data(T max, T min)
            {
                Max = max;
                Min = min;
            }
        }
    }
}