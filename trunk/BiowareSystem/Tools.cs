using System;
using System.Collections.Generic;

namespace Bioware.Tools {
    public static class HexaManip {
        public static string ByteArrayToString(byte[] buf) {
            string str = string.Empty;
            foreach (byte b in buf) {
                str += String.Format("{0:X}", b).PadLeft(2, '0').ToUpper();
            }
            return str;
        }
        public static byte[] StringToByteArray(String hex) {
            int NumberChars = hex.Length;
            if (hex.Length%2 != 0) {
                throw new ApplicationException("Invalid hexadecimal value.");
            }
            var bytes = new byte[NumberChars/2];
            for (int i = 0; i < NumberChars; i += 2) {
                bytes[i/2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
    public class DoubleDictionary<K, V> {
        private readonly Dictionary<V, K> dic_inv;
        private readonly Dictionary<K, V> dic_reg;

        public DoubleDictionary() {
            if (typeof (K) == typeof (V)) {
                throw new ArgumentException();
            }
            dic_reg = new Dictionary<K, V>();
            dic_inv = new Dictionary<V, K>();
        }

        public int Count {
            get { return dic_reg.Count; }
        }
        public Dictionary<K, V> Regular {
            get { return dic_reg; }
        }
        public Dictionary<V, K> Inverse {
            get { return dic_inv; }
        }

        public V this[K key] {
            get { return dic_reg[key]; }
            set { dic_reg[key] = value; }
        }
        public K this[V val] {
            get { return dic_inv[val]; }
            set { dic_inv[val] = value; }
        }
        public void Add(K key, V value) {
            dic_reg.Add(key, value);
            dic_inv.Add(value, key);
        }
    }
}