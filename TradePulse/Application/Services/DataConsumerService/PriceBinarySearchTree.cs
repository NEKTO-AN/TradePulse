using Application.Helpers.BST;

namespace Application.Services.DataConsumerService
{
    public class PriceBinarySearchTree
    {
        private PriceNode? _priceNode;
        private readonly TimeSpan _nodeLifetime;
        private readonly Queue<(double price, long timestamp)> timeframe = new(100_000);

        public PriceNode MinPrice { get; private set; }
        public PriceNode MaxPrice { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeLifetime"></param>
        public PriceBinarySearchTree(TimeSpan nodeLifetime)
        {
            _nodeLifetime = nodeLifetime;
            MinPrice = MaxPrice = new(0, 0);
        }

        public void Insert(double value, long timestamp)
        {
            _priceNode = InsertValue(_priceNode, value, timestamp);
            timeframe.Enqueue((price: value, timestamp: timestamp));

            while (true)
            {
                if (timeframe.Last().timestamp - timeframe.First().timestamp <= _nodeLifetime.TotalMilliseconds)
                {
                    break;
                }

                (double price, long timestamp) timeframeData = timeframe.Dequeue();
                PriceNode? newNode = Remove(_priceNode, timeframeData.price, timeframeData.timestamp);
                if (newNode == null)
                    continue;

                _priceNode = newNode;
            }

            MinPrice = GetMinPrice(_priceNode) ?? MinPrice;
            MaxPrice = GetMaxPrice(_priceNode) ?? MaxPrice;
        }

        public PriceNode? SearchPrice(double price)
        {
            return (PriceNode?)Search(_priceNode, price);
        }
        

        private PriceNode InsertValue(PriceNode? node, double price, long timestamp)
        {
            if (node is null)
            {
                return new(price, timestamp);
            }

            if (price < node.Value)
            {
                node.Left = InsertValue((PriceNode?)node.Left, price, timestamp);
            }
            else if (price > node.Value)
            {
                node.Right = InsertValue((PriceNode?)node.Right, price, timestamp);
            }
            else
            {
                node.UpdateTimestamp(timestamp);
            }

            return node;
        }

        private PriceNode InsertValue(PriceNode? node, PriceNode newNode)
        {
            if (node is null)
            {
                return newNode;
            }

            if (newNode.Value < node.Value)
            {
                node.Left = InsertValue((PriceNode?)node.Left, newNode);
            }
            else if (newNode.Value > node.Value)
            {
                node.Right = InsertValue((PriceNode?)node.Right, newNode);
            }
            else
            {
                node.UpdateTimestamp(newNode.LastUpdateTs);
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


        private PriceNode? Remove(PriceNode? node, double value, long updateTime)
        {
            if (node is null)
            {
                return null;
            }

            if (value.CompareTo(node.Value) < 0)
            {
                node.Left = Remove((PriceNode?)node.Left, value, updateTime);
                return node;
            }
            else if (value.CompareTo(node.Value) > 0)
            {
                node.Right = Remove((PriceNode?)node.Right, value, updateTime);
                return node;
            }
            else
            {
                if (node.LastUpdateTs != updateTime)
                {
                    return node;
                }
                if (node.Left is not null)
                {
                    if (node.Right == null)
                    {
                        return (PriceNode?)node.Left;
                    }
                    else if (node.Left == null)
                    {
                        return (PriceNode?)node.Right;
                    }
                    else
                    {
                        return InsertValue((PriceNode?)node.Right, (PriceNode)node.Left);
                    }
                }
                else
                {
                    return (PriceNode?)node.Right;
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

#endregion

    }
}