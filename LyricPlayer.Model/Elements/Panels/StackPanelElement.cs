using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameOverlay.Drawing;

namespace LyricPlayer.Model.Elements
{
   public class StackPanelElement : RenderElement
    {
        public StackPanelItemRenderRotation ItemsRenderRotation { set; get; }
        public bool AutoSize { set; get; } = true;
    }
}
