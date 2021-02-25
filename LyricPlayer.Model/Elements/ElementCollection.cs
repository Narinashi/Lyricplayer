using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LyricPlayer.Model.Elements
{
    public class ElementCollection : ICollection<RenderElement>
    {
        public RenderElement Parent { set; get; }
        Collection<RenderElement> Collection { set; get; }

        public int Count => Collection.Count;

        public bool IsReadOnly => false;

        object _lock = new object();
        public ElementCollection()
        {
            Collection = new Collection<RenderElement>();
        }
        public ElementCollection(RenderElement parent) : this()
        {
            Parent = parent;
        }

        public void Add(RenderElement element)
        {
            element.ParentElement = Parent;
            lock (_lock)
            { Collection.Add(element); }
        }
        public void Add(IEnumerable<RenderElement> elements)
        {
            lock (_lock)
            {
                foreach(var e in elements)
                {
                    e.ParentElement = Parent;
                    Collection.Add(e);
                }
            }
        }
        public bool Remove(RenderElement element)
        {
            element.ParentElement = null;
            lock (_lock)
            { return Collection.Remove(element); }
        }
        public void RemoveAt(int index)
        {
            var item = Collection[index];
            item.ParentElement = null;
            lock (_lock) { Collection.RemoveAt(index); }
        }
        public void Clear()
        {
            lock (_lock)
            { Collection.Clear(); }
        }

        public void Replace(List<RenderElement> elements)
        {
            lock (_lock)
            {
                for (int i = 0; i < Collection.Count; i++)
                    if (!elements.Contains(Collection[i]))
                    {
                        Collection[i].ParentElement = null;
                        Collection.RemoveAt(i);
                        i--;
                    }

                foreach (var item in elements)
                {
                    item.ParentElement = Parent;
                    Collection.Add(item);
                }
            }
        }

        public bool Contains(RenderElement item)
        {
            return Collection.Contains(item);
        }

        public void CopyTo(RenderElement[] array, int arrayIndex)
        { Collection.CopyTo(array, arrayIndex); }

        public IEnumerator<RenderElement> GetEnumerator()
        { lock (_lock) { return Collection.GetEnumerator(); } }

        IEnumerator IEnumerable.GetEnumerator()
        { lock (_lock) { return Collection.GetEnumerator(); } }
    }
}
