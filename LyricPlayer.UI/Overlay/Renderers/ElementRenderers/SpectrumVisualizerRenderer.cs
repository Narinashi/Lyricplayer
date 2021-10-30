using GameOverlay.Drawing;
using GameOverlay.Windows;
using LyricPlayer.Model;
using LyricPlayer.Model.Elements;
using LyricPlayer.Model.Elements.Enums;
using LyricPlayer.MusicPlayer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer.UI.Overlay.Renderers.ElementRenderers
{
	class SpectrumVisualizerRenderer : ElementRenderer<SpectrumVisualizer>
	{
		TrackInfo CurrentlyPlaying;
		static Dictionary<TrackInfo, List<TimeSpectrumData>> VisualizersData { set; get; } = new Dictionary<TrackInfo, List<TimeSpectrumData>>();
		static Dictionary<SpectrumVisualizer, IBrush> Brushes { set; get; } = new Dictionary<SpectrumVisualizer, IBrush>();
		static readonly Dictionary<SpectrumVisualizationType, Action<SpectrumVisualizer, double, float, Graphics, IBrush>> SpectrumDrawers = new Dictionary<SpectrumVisualizationType, Action<SpectrumVisualizer, double, float, Graphics, IBrush>>
		{
			{SpectrumVisualizationType.Bottom, DrawAtBottom },
			{SpectrumVisualizationType.Top, DrawAtTop },
			{SpectrumVisualizationType.Left, DrawAtLeft },
			{SpectrumVisualizationType.Right, DrawAtRight },
		};

		protected override void InternalRender(SpectrumVisualizer element, AudioPlayer audioPlayer, DrawGraphicsEventArgs renderArgs)
		{
			if (audioPlayer.CurrentlyPlaying != CurrentlyPlaying)
			{
				CurrentlyPlaying = audioPlayer.CurrentlyPlaying;
				Initialize(audioPlayer, element);
			}

			if (!VisualizersData.TryGetValue(audioPlayer.CurrentlyPlaying, out List<TimeSpectrumData> spectrumData))
				return;

			if (!(spectrumData?.Any() ?? false))
				return;

			var currentTime = audioPlayer.CurrentTime.TotalMilliseconds;
			TimeSpectrumData playingSpectrumPart = null;

			for (int i = 0; i < spectrumData.Count - 1; i++)
				if (spectrumData[i + 1].Time > currentTime && spectrumData[i].Time <= currentTime)
				{
					playingSpectrumPart = spectrumData[i];
					break;
				}
			playingSpectrumPart = playingSpectrumPart ?? spectrumData[spectrumData.Count - 1];

			var gfx = renderArgs.Graphics;

			foreach (var drawer in SpectrumDrawers)
			{
				if ((drawer.Key & element.VisualizationType) == 0)
					continue;

				float startingPosition = element.BandSpace;
				if (element.VisualizationType == SpectrumVisualizationType.Bottom ||
					element.VisualizationType == SpectrumVisualizationType.Top)
					startingPosition += element.Location.X;
				else
					startingPosition += element.Location.Y;

				for (int index = 0; index < playingSpectrumPart.SpectrumData.Length; index++)
				{
					var band = playingSpectrumPart.SpectrumData[index];
					if (!Brushes.TryGetValue(element, out IBrush brush))
					{
						brush = gfx.CreateSolidBrush(element.BandColor.ToOverlayColor());
						Brushes.Add(element, brush);
					}

					drawer.Value(element, band * element.Multiplier, startingPosition, gfx, brush);
					startingPosition += element.BandWidth + element.BandSpace;
				}
			}
		}

		private void Initialize(AudioPlayer audioPlayer, SpectrumVisualizer element)
		{
			Task.Run(() =>
			{
				var track = audioPlayer.CurrentlyPlaying;
				var callerElement = element;
				var result = audioPlayer.CalculateSpecterumData(callerElement.BandCount);
				lock (VisualizersData)
				{
					if (!VisualizersData.TryGetValue(track, out List<TimeSpectrumData> data))
						VisualizersData.Add(track, result);
					else
						VisualizersData[track] = result;
				}
			});
		}

		private static void DrawAtBottom(SpectrumVisualizer element, double band, float startingPosition, Graphics gfx, IBrush brush)
		{
			if (element.BandRadius > 0)
				gfx.FillRoundedRectangle(brush, new RoundedRectangle(startingPosition, (float)((1 - band) * element.Size.Y), startingPosition + element.BandWidth, element.Size.Y + element.BandRadius, element.BandRadius));
			else
				gfx.FillRectangle(brush, new Rectangle(startingPosition, (float)((1 - band) * element.Size.Y), startingPosition + element.BandWidth, element.Size.Y));
		}

		private static void DrawAtTop(SpectrumVisualizer element, double band, float startingPosition, Graphics gfx, IBrush brush)
		{
			if (element.BandRadius > 0)
				gfx.FillRoundedRectangle(brush, new RoundedRectangle(startingPosition, (float)(band * element.Size.Y), startingPosition + element.BandWidth, -element.BandRadius, element.BandRadius));
			else
				gfx.FillRectangle(brush, new Rectangle(startingPosition, (float)(band * element.Size.Y), startingPosition + element.BandWidth, 0));
		}

		private static void DrawAtRight(SpectrumVisualizer element, double band, float startingPosition, Graphics gfx, IBrush brush)
		{
			if (element.BandRadius > 0)
				gfx.FillRoundedRectangle(brush, new RoundedRectangle(element.Size.X + element.BandRadius, startingPosition + element.BandWidth, (float)((1 - band) * element.Size.X), startingPosition, element.BandRadius));
			else
				gfx.FillRectangle(brush, new Rectangle(element.Size.X, startingPosition + element.BandWidth, (float)((1 - band) * element.Size.X), startingPosition));
		}

		private static void DrawAtLeft(SpectrumVisualizer element, double band, float startingPosition, Graphics gfx, IBrush brush)
		{
			if (element.BandRadius > 0)
				gfx.FillRoundedRectangle(brush, new RoundedRectangle(-element.BandRadius, startingPosition + element.BandWidth, (float)(band * element.Size.X), startingPosition, element.BandRadius));
			else
				gfx.FillRectangle(brush, new Rectangle(0, startingPosition + element.BandWidth, (float)(band * element.Size.X), startingPosition));

		}

		public override void Setup(Graphics gfx)
		{

		}

		public override void Destroy(Graphics gfx)
		{
			Dispose();

		}

		public override void Dispose()
		{
			VisualizersData.Clear();
			foreach (var item in Brushes)
				item.Value.Brush.Dispose();
		}
	}
}
