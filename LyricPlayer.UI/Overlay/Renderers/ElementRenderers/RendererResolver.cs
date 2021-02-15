using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
    static class RendererResolver
    {
        public static Dictionary<Type, IElementRenderer> Renderers { get; private set; }
        static RendererResolver()
        {
            Refresh();
        }
        public static void Refresh()
        {
            Renderers = Assembly.GetExecutingAssembly().GetTypes()
               .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(ElementRenderer)))
               .ToDictionary(x => x.BaseType.GenericTypeArguments[0], a => Activator.CreateInstance(a) as IElementRenderer);
        }
    }
}
