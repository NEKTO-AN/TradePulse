using Application.Helpers.BST;

namespace Application.Services.DataConsumerService
{
    public class TimestampNode : Node<long>
    {
        public PriceNode? PriceNode { get; set; }

        private TimestampNode(long value) : base(value)
        {
            Value = value;
        }

        public static PriceNode Init(long ts, double price)
        {
            TimestampNode _ts = new(ts);
            PriceNode _price = new(price, _ts);
            _ts.PriceNode = _price;
            return _price;
        }
    }
}