using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LyricPlayer.LyricEffects;
using LyricPlayer.Models;
using GameOverlay.Windows;

namespace LyricPlayer.UI.Overlay.EffectPlayers
{
    public abstract class LyricEffectPlayerBase
    {
        public bool Initiated { set; get; } = false;

        public abstract void ApplyEffect(LyricHolder holder, LyricEffect effect, DrawGraphicsEventArgs drawEventArg);
    }

    public abstract class LyricEffectPlayerBase<T> : LyricEffectPlayerBase where T : LyricEffect
    {
        public override void ApplyEffect(LyricHolder holder, LyricEffect effect, DrawGraphicsEventArgs drawEventArg)
            => ApplyEffect(holder, (T)effect, drawEventArg);

        protected abstract void ApplyEffect(LyricHolder holder, T effect, DrawGraphicsEventArgs drawEventArg);
    }
}
