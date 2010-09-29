using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tools {
    public class DoubleDictionary<K, V> {

        private Dictionary<K, V> dic_reg;
        private Dictionary<V, K> dic_inv;

        public DoubleDictionary() {
            if (typeof(K) == typeof(V)) {
                throw new ApplicationException("Classes ambigues.");
            }
            dic_reg = new Dictionary<K, V>();
            dic_inv = new Dictionary<V, K>();
        }

        public void Add(K key, V value) {
            dic_reg.Add(key, value);
            dic_inv.Add(value, key);
        }

        public int Count {
            get {
                return dic_reg.Count;
            }
        }
        public Dictionary<K, V> Regular {
            get {
                return dic_reg;
            }
        }
        public Dictionary<V, K> Inverse {
            get {
                return dic_inv;
            }
        }

        public V this[K key] {
            get {
                return dic_reg[key];
            }
            set {
                dic_reg[key] = value;
            }
        }
        public K this[V val] {
            get {
                return dic_inv[val];
            }
            set {
                dic_inv[val] = value;
            }
        }
    }
}
