using System;
using System.Collections.Generic;
using System.Linq;
using static System.String;

namespace Bioware
{
    public static class HexaManip
    {
        public static string ByteArrayToString(byte[] buf)
        {
            return buf.Aggregate(Empty, (current, b) => current + $"{b:X}".PadLeft(2, '0').ToUpper());
        }

        public static byte[] StringToByteArray(string hex)
        {
            var numberChars = hex.Length;
            if (hex.Length%2 != 0)
            {
                throw new ApplicationException("Invalid hexadecimal value.");
            }
            var bytes = new byte[numberChars/2];
            for (var i = 0; i < numberChars; i += 2)
            {
                bytes[i/2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }

    public class DoubleDictionary<K, V>
    {
        public DoubleDictionary()
        {
            if (typeof (K) == typeof (V))
            {
                throw new ArgumentException();
            }
            Regular = new Dictionary<K, V>();
            Inverse = new Dictionary<V, K>();
        }

        public int Count => Regular.Count;

        public Dictionary<K, V> Regular { get; }

        public Dictionary<V, K> Inverse { get; }

        public V this[K key]
        {
            get { return Regular[key]; }
            set { Regular[key] = value; }
        }

        public K this[V val]
        {
            get { return Inverse[val]; }
            set { Inverse[val] = value; }
        }

        public void Add(K key, V value)
        {
            Regular.Add(key, value);
            Inverse.Add(value, key);
        }
    }
}