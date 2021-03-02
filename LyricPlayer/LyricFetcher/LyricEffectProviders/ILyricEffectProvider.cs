using LyricPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.LyricFetcher.LyricEffectProviders
{
    interface ILyricEffectProvider
    {
        void AddEffects(TrackLyric trackLyric);
    }
}
