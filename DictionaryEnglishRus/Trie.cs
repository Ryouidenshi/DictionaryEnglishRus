using System.Collections.Generic;

namespace AlgorithmsProject
{
    public class Trie<T>
    {
        public Node<T> Root = new Node<T>(null, "");

        public void Add(string key, T value)
        {
            Node<T> node = GetNode(key);

            if (node != null)
            {
                node.Value = value;
                node.IsWord = true;
                return;
            }

            string additiveKey = "";
            node = Root;

            for (int i = 0; i < key.Length; i++)
            {
                additiveKey += key[i];
                node = Add(additiveKey, Root);
            }
            node.Value = value;
            node.IsWord = true;
        }

        private Node<T> Add(string key, Node<T> node)
        {
            if (node.Key == key)
                return node;

            if (node.Childs == null || node.Childs.Count == 0)
            {
                node.AddChild(new Node<T>(node, key));
                return node.Childs[0];
            }

            for (int i = 0; i < node.Childs.Count; i++)
                if (key.StartsWith(node.Childs[i].Key))
                    return Add(key, node.Childs[i]);

            node.AddChild(new Node<T>(node, key));
            return node.Childs[node.Childs.Count - 1];
        }


        public void Remove(string key)
        {
            Node<T> node = GetNode(key);

            if (node == null || node == Root)
                return;

            node.IsWord = false;
            node.Value = default;

            if (node.Childs != null && node.Childs.Count > 0)
                return;

            while (node.Parent != Root && !node.IsWord)
            {
                if (node.Childs != null && node.Childs.Count > 0)
                    break;

                node.Parent.Childs.Remove(node);
                node = node.Parent;
            }
        }

        public bool ContainsValue(string key)
        {
            Node<T> node = GetNode(key);

            if (node == null)
                return false;

            return node.IsWord;
        }


        public Node<T> GetNode(string key)
        {
            return GetNode(key, Root);
        }

        private Node<T> GetNode(string key, Node<T> node)
        {
            if (node.Key == key)
                return node;

            if (node.Childs == null || node.Childs.Count == 0)
                return null;

            for (int i = 0; i < node.Childs.Count; i++)
                if (key.StartsWith(node.Childs[i].Key))
                    return GetNode(key, node.Childs[i]);

            return null;
        }


        public T GetValue(string key)
        {
            return GetNode(key).Value;
        }
    }

    public class Node<T>
    {
        public string Key { get; set; }
        public T Value { get; set; }
        public bool IsWord { get; set; }
        public Node<T> Parent { get; set; }
        public List<Node<T>> Childs { get; set; }

        public Node(Node<T> parent, string key)
        {
            Parent = parent;
            Key = key;
        }


        public void AddChild(Node<T> node)
        {
            if (Childs == null)
                Childs = new List<Node<T>>();

            if (!Childs.Contains(node))
                Childs.Add(node);
        }
    }
}