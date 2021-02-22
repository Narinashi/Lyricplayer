using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LyricPlayer.UI.Overlay.EffectPlayers
{
    static class EffectPlayerResolver
    {
        public static Dictionary<Type, EffectPlayer> EffectPlayers { set; get; }
        static EffectPlayerResolver()
        {
            Refresh();
        }

        public static void Refresh()
        {
            EffectPlayers = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => !x.IsAbstract && x.IsClass && x.IsSubclassOf(typeof(EffectPlayer)))
                .ToDictionary(x => x.BaseType.GenericTypeArguments[0], x => Activator.CreateInstance(x) as EffectPlayer);
        }
    }
}
