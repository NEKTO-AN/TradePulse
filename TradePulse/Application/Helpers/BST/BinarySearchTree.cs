namespace Application.Helpers.BST
{
    public class BinarySearchTree<T> where T : struct, IComparable
    {
        private Node<T>? _node;

        public void Insert(T value)
        {
            _node = InsertValue(_node, value);
        }

        public Node<T>? Search(T value)
        {
            return Search(_node, value);
        }

        public bool Remove(T value)
        {
            Node<T>? node = Remove(_node, value);
            if (node != null)
                _node = node;

            return node is not null;
        }

        public void Print()
        {
            Print(_node);

            Console.WriteLine();
        }

        private Node<T> InsertValue(Node<T>? node, T value)
        {
            if (node is null)
            {
                return new Node<T>(value);
            }

            if (value.CompareTo(node.Value) < 0)
            {
                node.Left = InsertValue(node.Left, value);
            }

            if (value.CompareTo(node.Value) > 0)
            {
                node.Right = InsertValue(node.Right, value);
            }

            return node;
        }

        private Node<T>? Search(Node<T>? node, T value)
        {
            if (node is null)
                return null;

            if (value.CompareTo(node.Value) == 0)
                return node;

            if (value.CompareTo(node.Value) > 0)
            {
                return Search(node.Right, value);
            }

            return Search(node.Left, value);
        }

        private Node<T>? Remove(Node<T>? node, T value)
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


        private void Print(Node<T>? node)
        {
            if (node?.Left is not null)
                Print(node.Left);

            Console.Write(" + " + node?.Value + " + ");

            if (node?.Right is not null)
                Print(node.Right);
        }
    }
}