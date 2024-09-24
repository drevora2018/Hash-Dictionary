using System;
using System.Collections;
using System.Collections.Generic;

namespace HashDictionary
{
    public class HashDictionary<TKey, TValue> : IDictionary<TKey, TValue> //Hashdictionary is implementing IDictionary interface for key and value
    {
        LinkedList<InternalType>[] _buckets; //
        int _bucketsStep;
        int _count;

        public HashDictionary()
        {
            _buckets = new LinkedList<InternalType>[65536]; //Bucket of array of linked list of internal type
            _bucketsStep = (int)(((long)Math.Pow(2, 32)) / _buckets.Length); //The step between each element of buckets array
            _count = 0;
        }

        public TValue this[TKey key]
        {
            get //When we get the city from the dictionary 
            {
                InternalType element = findNode(key);

                if (element != null)
                {
                    return element.Value;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            set //When we set or update new or excisting element
            {
                InternalType element = findNode(key);

                if (element != null)
                {
                    element.Value = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get // Get the list of keys in the collection
            {
                List<TKey> result = new List<TKey>();

                for(int i = 0; i < _buckets.Length; i++)
                {
                    if (_buckets[i] != null)
                    {
                        foreach(InternalType element in _buckets[i])
                        {
                            result.Add(element.Key);
                        }
                    }
                }

                return result;
            }
        }

        public ICollection<TValue> Values
        {
            get //Get the list of values
            {
                List<TValue> result = new List<TValue>();

                for (int i = 0; i < _buckets.Length; i++)
                {
                    if (_buckets[i] != null)
                    {
                        foreach (InternalType element in _buckets[i])
                        {
                            result.Add(element.Value);
                        }
                    }
                }

                return result;
            }
        }


        public int Count { get { return _count; } } //Returning number of elements

        public bool IsReadOnly { get { return false; } } //Always allowed to write 

        public void Add(TKey key, TValue value) //To add a new element to the collection in last
        {
            if (!ContainsKey(key))
            {
                int hash = key.GetHashCode();
                int location = getLocation(hash);

                if (_buckets[location] == null)
                {
                    _buckets[location] = new LinkedList<InternalType>();
                }

                _buckets[location].AddLast(new InternalType(hash, key, value));
                _count++;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item) //Another way to add an element
        {
            Add(item.Key, item.Value);
        }

        public void Clear() //Remove all the elements from the list
        {
            for(int i = 0; i < _buckets.Length; i++)
            {
                _buckets[i] = null;
            }
            _count = 0;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) //Check if an element exist in the list
        {
            InternalType element = findNode(item.Key);

            if(element!=null && element.Value.Equals(item.Value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ContainsKey(TKey key) //Check if the key exist in the list
        {
            InternalType element = findNode(key);

            if (element != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) //Copy all the elements from the list to the array
        {
            int pointer = 0;

            for(int i = 0; i < _buckets.Length; i++)
            {
                if (_buckets[i] != null)
                {
                    foreach(InternalType element in _buckets[i])
                    {
                        array[arrayIndex + pointer] = new KeyValuePair<TKey, TValue>(element.Key, element.Value);
                        pointer++;
                    }
                }
            }
        }

        public bool Remove(TKey key) //Remove an element from the list
        {
            int location = getLocation(key.GetHashCode());

            if (_buckets[location] != null)
            {
                InternalType element = findNode(key);
                if (element != null && _buckets[location].Remove(element))
                {
                    _count--;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) //Another method to remove the element
        {
            int location = getLocation(item.Key.GetHashCode());

            if (_buckets[location] != null)
            {
                InternalType element = findNode(item.Key);
                if (element.Value.Equals(item.Value))
                {
                    return _buckets[location].Remove(element);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value) //Get element from the disctionary
        {
            InternalType element = findNode(key);

            if (element != null)
            {
                value = element.Value;
                return true;
            }
            else
            {
                value = default(TValue);
                return false;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() //Get the class which support using of foreach statements
        {
            List<KeyValuePair<TKey, TValue>> data = new List<KeyValuePair<TKey, TValue>>();

            for(int i = 0; i < _buckets.Length; i++)
            {
                if (_buckets[i] != null)
                {
                    foreach(InternalType element in _buckets[i])
                    {
                        data.Add(new KeyValuePair<TKey, TValue>(element.Key, element.Value));
                    }
                }
            }

            return new Enumerator(data);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            List<KeyValuePair<TKey, TValue>> data = new List<KeyValuePair<TKey, TValue>>();

            for (int i = 0; i < _buckets.Length; i++)
            {
                if (_buckets[i] != null)
                {
                    foreach (InternalType element in _buckets[i])
                    {
                        data.Add(new KeyValuePair<TKey, TValue>(element.Key, element.Value));
                    }
                }
            }

            return new Enumerator(data);
        }

        private class InternalType // The hashfunction for the key and value. This is the type to be used internally in order to store the data
        {
            public int KeyHash;
            public TKey Key;
            public TValue Value;

            public InternalType(int KeyHash, TKey Key, TValue Value)
            {
                this.KeyHash = KeyHash;
                this.Key = Key;
                this.Value = Value;
            }
        }

        private class Enumerator : IEnumerator<KeyValuePair<TKey, TValue>> //Class for the enumerator
        {
            private List<KeyValuePair<TKey, TValue>> _data;
            private int _position;

            public Enumerator(List<KeyValuePair<TKey, TValue>> Data)
            {
                _data = Data;
                _position = -1;
            }

            public KeyValuePair<TKey, TValue> Current { get { return _data[_position]; } }

            object IEnumerator.Current { get { return _data[_position]; } }

            public void Dispose()
            {
                _data = null;
            }

            public bool MoveNext()
            {
                if (_position < _data.Count-1)
                {
                    _position++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                _position = -1;
            }
        }

        private int getLocation(int Hash) //Calculates in which array element the city is placed
        {
            return (int)Math.Floor((decimal)((long)Hash-(long)int.MinValue) / (decimal)_bucketsStep);
        }

        private InternalType findNode(TKey Key) //Search for element with hashcode and check if it is the correct one
        {
            int hash = Key.GetHashCode();
            int location = getLocation(hash);

            if (_buckets[location] != null)
            {
                foreach(InternalType element in _buckets[location])
                {
                    if (element.KeyHash == hash && element.Key.Equals(Key))
                    {
                        return element;
                    }
                }
            }

            return null;
        }
    }
}
