using Application.Helpers.BST;

namespace Application.Services.DataConsumerService
{
    public class PriceBinarySearchTree
    {
        private PriceNode? _priceNode;
        private readonly TimeSpan _nodeLifetime;
        private readonly Queue<(double price, long timestamp)> timeframe = new(100_000);

        public double MinPrice { get; private set; }
        public double MaxPrice { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeLifetime"></param>
        public PriceBinarySearchTree(TimeSpan nodeLifetime)
        {
            _nodeLifetime = nodeLifetime;
        }

        public void Insert(double value, long timestamp)
        {
            _priceNode = InsertValue(_priceNode, value, timestamp);

            while (true)
            {
                long? maxTs = GetMaxTimestamp(_priceNode.TimestampNode)?.Value;
                long? minTs = GetMinTimestamp(_priceNode.TimestampNode)?.Value;
                if (minTs == null || maxTs == null || (maxTs - minTs) < _nodeLifetime.TotalMilliseconds)
                {
                    break;
                }

                bool result = RemoveTimestamp(minTs!.Value);
                if (!result)
                    break;
            }

            MinPrice = GetMinPrice(_priceNode)?.Value ?? MinPrice;
            MaxPrice = GetMaxPrice(_priceNode)?.Value ?? MaxPrice;
        }

        public PriceNode? SearchPrice(double price)
        {
            return (PriceNode?)Search(_priceNode, price);
        }
        public TimestampNode? SearchTimestamp(long timestamp)
        {
            return (TimestampNode?)Search(_priceNode?.TimestampNode, timestamp);
        }
        public bool RemoveTimestamp(long timestamp)
        {
            TimestampNode? findNode = (TimestampNode?)Search(_priceNode?.TimestampNode, timestamp);
            if (findNode is null)
                return false;
            else if (findNode.PriceNode is null)
                throw new Exception();

            PriceNode? priceNode = (PriceNode?)Remove(_priceNode, findNode.PriceNode.Value);
            if (priceNode is null)
                return false;

            priceNode.TimestampNode = (TimestampNode?)Remove(priceNode.TimestampNode, timestamp) ?? throw new Exception();
            _priceNode = priceNode;

            return true;
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



        private Node<T>? Remove<T>(Node<T>? node, T value) where T : struct, IComparable
        {
            if (node is null)
            {
                return null;
            }

            if (value.CompareTo(node.Value) < 0)
            {
                node.Left = Remove(node.Left, value);
                return node;
            }
            else if (value.CompareTo(node.Value) > 0)
            {
                node.Right = Remove(node.Right, value);
                return node;
            }
            else
            {
                if (node.Left is not null)
                {
                    return node.Left.Right = node.Right;
                }
                else
                {
                    return node.Right;
                }
            }
        }

#region GET_VALUES

        private PriceNode? GetMinPrice(PriceNode? node)
        {
            if (node?.Left is not null)
            {
                return GetMinPrice((PriceNode)node.Left);
            }

            return node;
        }

        private PriceNode? GetMaxPrice(PriceNode? node)
        {
            if (node?.Right is not null)
            {
                return GetMaxPrice((PriceNode)node.Right);
            }

            return node;
        }

        private TimestampNode? GetMinTimestamp(TimestampNode? node)
        {
            if (node?.Left is not null)
            {
                return GetMinTimestamp((TimestampNode)node.Left);
            }

            return node;
        }

        private TimestampNode? GetMaxTimestamp(TimestampNode? node)
        {
            if (node?.Right is not null)
            {
                return GetMaxTimestamp((TimestampNode)node.Right);
            }

            return node;
        }

#endregion

    }
}