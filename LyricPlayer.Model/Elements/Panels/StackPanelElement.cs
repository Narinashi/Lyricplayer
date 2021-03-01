using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.Model.Elements
{
   public class StackPanelElement : RenderElement
    {
        public StackPanelItemRenderRotation ItemsOrientation { set; get; }
        public bool AutoSize { set; get; } = true;
    }
}
