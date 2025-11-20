using System.Collections;


namespace MuniConnect.Data
{
    public class IssueLinkedList<T> : IEnumerable<T>
    {
        private Node<T> head;
        public void Add(T data)
        {
            Node<T> newNode = new Node<T>(data);
            if (head == null)
                head = newNode;
            else
            {
                Node<T> current = head;
                while (current.Next != null)
                    current = current.Next;

                current.Next = newNode;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node<T> current = head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

