using System;
using System.Collections;
using System.Collections.Generic;

namespace HashDictionary
{
    class HashTable<K, V> : IDictionary<K, V> where K : IComparable<K>
    {
        //Gabriele and I went through this part and I got the thumbs up, nothing much has changed here.
        class Type
        {
            public int Hash;
            public K key;
            public V value;
            internal Type(int hash, K Key, V Value)
            {
                Hash = hash;
                key = Key;
                value = Value;
            }
        }

        class Enumerator : IEnumerator<KeyValuePair<K, V>>
        {
            List<KeyValuePair<K, V>> data;
            int Position;
            public Enumerator(List<KeyValuePair<K, V>> keyValues)
            {
                data = keyValues;
                Position = -1;
            }
            public KeyValuePair<K, V> Current => data[Position];

            object IEnumerator.Current => data[Position];

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {

                if (Position < data.Count - 1)
                {
                    Position++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                Position = -1;
            }
        }

        LinkedList<Type>[] list;
        int AmountOfItems;
        int Steps;

        public HashTable()
        {
            list = new LinkedList<Type>[10001];
            Steps = (int)(((long)Math.Pow(2, 32)) / list.Length);
        }
        public V this[K key]
        {
            get
            {
                Type FoundElement = Find(key);
                if (FoundElement != null)
                {
                    return FoundElement.value;
                }
                else
                    throw new NotImplementedException();
            }
            set
            {
                Type FoundElement = Find(key);
                if (FoundElement != null)
                {
                    FoundElement.value = value;
                }
                else
                {
                    Add(key, value);
                }

            }
        }

        public ICollection<K> Keys
        {
            get
            {
                List<K> AllKeys = new List<K>();
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] != null)
                    {
                        foreach (Type item in list[i])
                        {
                            AllKeys.Add(item.key);
                        }
                    }

                }
                return AllKeys;
            }
        }

        public ICollection<V> Values
        {
            get
            {
                List<V> AllValues = new List<V>();
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] != null)
                    {
                        foreach (Type item in list[i])
                        {
                            AllValues.Add(item.value);
                        }
                    }

                }
                return AllValues;
            }
        }

        public int Count
        {
            get
            {
                return AmountOfItems;
            }
            private set
            {
                AmountOfItems = value;
            }
        }

        public bool IsReadOnly => false;

        public void Add(K key, V value)
        {

            if (!ContainsKey(key))
            {
                int HashCode = key.GetHashCode();
                int LocationInArray = GetLocationInArray(HashCode);
                //if that particular part of the list is empty, create a new linked list there
                if (list[LocationInArray] == null)
                {
                    list[LocationInArray] = new LinkedList<Type>();
                }
                list[LocationInArray].AddLast(new Type(HashCode, key, value));
                AmountOfItems++;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            if (list != null)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    list[i] = null;
                }
                Count = 0;
            }
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            int HashCode = item.Key.GetHashCode();
            int i = GetLocationInArray(HashCode);
            if (list[i] != null)
            {
                foreach (var Item in list[i])
                {
                    if (item.Equals(Item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool ContainsKey(K key)
        {
            int HashCode = key.GetHashCode();
            int i = GetLocationInArray(HashCode);
            if (list[i] != null)
            {
                foreach (var item in list[i])
                {
                    if (item.key.CompareTo(key) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            if (list != null)
            {
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] != null)
                    {
                        foreach (var item in list[i])
                        {
                            list.CopyTo(array, arrayIndex);
                            arrayIndex++;
                        }
                    }

                }
            }
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            //List<KeyValuePair<K, V>> Data = new List<KeyValuePair<K, V>>();
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] != null)
                {
                    foreach (var item in list[i])
                    {
                        yield return new KeyValuePair<K, V>(item.key, item.value);
                    }
                }

            }
            //return new Enumerator(Data);
        }

        public bool Remove(K key)
        {
            int HashCode = key.GetHashCode();
            int i = GetLocationInArray(HashCode);
            if (list[i] != null)
            {
                foreach (var item in list[i])
                {
                    if (item.key.CompareTo(key) == 0)
                    {
                        list[i].Remove(item);
                        AmountOfItems--;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(K key, out V value)
        {
            int HashCode = key.GetHashCode();
            int i = GetLocationInArray(HashCode);
            if (list[i] != null)
            {
                foreach (var item in list[i])
                {
                    if (item.key.CompareTo(key) == 0)
                    {
                        value = item.value;
                        return true;
                    }
                }
            }
            value = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        Type Find(K key)
        {
            int HashCode = key.GetHashCode();
            int LocationInArray = GetLocationInArray(HashCode);
            if (list[LocationInArray] != null)
            {
                foreach (var item in list[LocationInArray])
                {
                    if (item.Hash == HashCode && item.key.CompareTo(key) == 0)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        int GetLocationInArray(int hash)
        {
            return (int)Math.Floor((decimal)((long)hash - (long)int.MinValue) / (decimal)Steps);
        }
    }
}
