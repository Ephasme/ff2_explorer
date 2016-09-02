using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bioware.GFF.Composite
{
    public abstract class GffComposite : GffComponent, IList<GffComponent>
    {
        protected readonly List<GffComponent> Childs;

        protected GffComposite(string label, GffType type) : base(label, type)
        {
            Childs = new List<GffComponent>();
        }

        public int IndexOf(GffComponent item)
        {
            return Childs.IndexOf(item);
        }

        public void Insert(int index, GffComponent item)
        {
            Childs.Insert(index, item);
            item.Owner = this;
        }

        public void RemoveAt(int index)
        {
            Childs[index].Owner = null;
            Childs.RemoveAt(index);
        }

        public GffComponent this[int index]
        {
            get { return Childs[index]; }
            set { Childs[index] = value; }
        }

        public void Add(GffComponent item)
        {
            Childs.Add(item);
            item.Owner = this;
        }

        public void Clear()
        {
            foreach (var c in Childs)
            {
                c.Owner = null;
            }
            Childs.Clear();
        }

        public bool Contains(GffComponent item)
        {
            return Childs.Contains(item);
        }

        public void CopyTo(GffComponent[] array, int arrayIndex)
        {
            Childs.CopyTo(array, arrayIndex);
        }

        public int Count => Childs.Count;

        public bool IsReadOnly => ((ICollection<GffComponent>) Childs).IsReadOnly;

        public bool Remove(GffComponent item)
        {
            if (!Childs.Remove(item)) return false;
            item.Owner = null;
            return true;
        }

        public IEnumerator<GffComponent> GetEnumerator()
        {
            return Childs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Childs.GetEnumerator();
        }

        protected GffComponent SelectComponent(string label)
        {
            return Childs.Single(item => item.Label == label);
        }

        //public List<GComponent> SelectAll(string label) {
        //    return new List<GComponent>(childs.Where((item) => { return item.Label == label; }));

        //}
    }
}