using Application.Helpers.BST;

namespace Application.Services.DataConsumerService
{
    public sealed class PriceNode : Node<double>
    {
        public long LastUpdateTs { get; private set; }

        public PriceNode(double value, long lastUpdateTs) : base(value)
        {
            Value = value;
            LastUpdateTs = lastUpdateTs;
        }

        public void UpdateTimestamp(long ts)
        {
            LastUpdateTs = ts;
        }
    }
}