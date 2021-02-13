using System.Collections.ObjectModel;

namespace LyricPlayer.Model.Elements
{
    public class ElementCollection : Collection<RenderElement>
    {
        public RenderElement Parent { set; get; }
        protected override void InsertItem(int index, RenderElement item)
        {
            item.ParentElement = Parent;
            base.InsertItem(index, item);
        }
        protected override void RemoveItem(int index)
        {
            Items[index].ParentElement = null;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, RenderElement item)
        {
            item.ParentElement = Parent;
            base.SetItem(index, item);
        }
    }
}
