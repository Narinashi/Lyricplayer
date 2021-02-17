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
        public bool Remove(RenderElement element)
        {
            element.ParentElement = null;
            lock (_lock)
            { return Collection.Remove(element); }
        }
        public void Clear()
        {
            lock (_lock)
            { Collection.Clear(); }
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
