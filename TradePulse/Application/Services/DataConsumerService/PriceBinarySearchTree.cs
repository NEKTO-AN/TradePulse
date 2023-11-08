using Application.Helpers.BST;

namespace Application.Services.DataConsumerService
{
    public class PriceBinarySearchTree
    {
        private PriceNode? _priceNode;

        public void Insert(double value, long timestamp)
        {
            _priceNode = InsertValue(_priceNode, value, timestamp);
        }

        public PriceNode? SearchPrice(double price)
        {
            return (PriceNode?)Search(_priceNode, price);
        }
        public TimestampNode? SearchTimestamp(long timestamp)
        {
            return (TimestampNode?)Search(_priceNode?.TimestampNode, timestamp);
        }

        private PriceNode InsertValue(PriceNode? node, double price, long timestamp)
        {
            if (node is null)
            {
                return TimestampNode.Init(timestamp, price);
            }

            if (price < node.Value)
            {
                node.Left = InsertValue((PriceNode?)node.Left, price, timestamp);
            }

            if (price > node.Value)
            {
                node.Right = InsertValue((PriceNode?)node.Right, price, timestamp);
            }

            return node;
        }

        private Node<T>? Search<T>(Node<T>? node, T value) where T : struct, IComparable
        {
            if (node is null)
                return null;

            if (node.Value.CompareTo(value) == 0)
                return node;

            if (value.CompareTo(node.Value) > 0)
            {
                return Search(node.Right, value);
            }

            return Search(node.Left, value);
        }
    }
}