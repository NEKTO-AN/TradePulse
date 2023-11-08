using Application.Helpers.BST;

namespace Application.Services.DataConsumerService
{
    public class PriceNode : Node<double>
    {
        public TimestampNode TimestampNode { get; set; }

        public PriceNode(double value, TimestampNode ts) : base(value)
        {
            Value = value;
            TimestampNode = ts;
        }
    }
}