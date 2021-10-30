using CSCore;
using CSCore.Codecs;
using CSCore.DSP;
using CSCore.Streams;
using LyricPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LyricPlayer.MusicPlayer.AudioAnalyse
{
	public class RawSpectrumDataProvider
	{
		SpectrumBase Spectrum { set; get; }
		BasicSpectrumProvider SpectrumProvider { set; get; }
		SingleBlockNotificationStream NotificationStream { set; get; }
		public void LoadFile(string filePath, int resoulution = 9, FftSize fftSize = FftSize.Fft256)
		{
			var source = CodecFactory.Instance.GetCodec(filePath).ToSampleSource();

			NotificationStream = new SingleBlockNotificationStream(source);
			NotificationStream.SingleBlockRead += (s, a) =>
			{
				SpectrumProvider.Add(a.Left, a.Right);
			};

			SpectrumProvider = new BasicSpectrumProvider(source.WaveFormat.Channels, source.WaveFormat.SampleRate, fftSize);
			Spectrum = new SpectrumBase()
			{
				SpectrumProvider = SpectrumProvider,
				SpectrumResolution = resoulution,
				MaximumFrequency = 20000,
				MinimumFrequency = 20,
				ScalingStrategy = ScalingStrategy.Linear,
				UseAverage = true,
			};
		}

		public List<TimeSpectrumData> GenerateSpectrumDataSafeWay(int stepDurationMs = 13, int smoothnessWindow = 5)
		{
			var streamLength = NotificationStream.Length;
			var samples = new float[2];
			int samplesRead = -1;
			var result = new List<TimeSpectrumData>();

			NotificationStream.Position = 0;
			while (NotificationStream.Position < streamLength)
			{
				var lastTimeRead = NotificationStream.GetMilliseconds(NotificationStream.Position);

				while (NotificationStream.GetMilliseconds(NotificationStream.Position) - lastTimeRead < stepDurationMs & samplesRead != 0)
					samplesRead = NotificationStream.Read(samples, 0, samples.Length);

				if (samplesRead == 0)
					break;

				result.Add(GetTimeSpectrumData());
			}

			SmoothDataAcrossTime(result, smoothnessWindow);
			return result;
		}

		public List<TimeSpectrumData> GenerateSpectrumDataNewWay(int stepDurationMs = 10, int smoothnessWindow = 3)
		{
			var streamLength = NotificationStream.Length;
			var samples = new float[32 * 1024];
			int samplesRead = -1, totalSamplesRead = 0;

			var result = new List<TimeSpectrumData>();

			NotificationStream.Position = 0;
			while (NotificationStream.Position < streamLength)
			{
				var lastTimeRead = NotificationStream.GetMilliseconds(NotificationStream.Position);

				while (NotificationStream.GetMilliseconds(NotificationStream.Position) - lastTimeRead < stepDurationMs & samplesRead != 0)
					samplesRead = NotificationStream.Read(samples, 0, samples.Length);

				for (int i = 0; i < samplesRead; i += NotificationStream.WaveFormat.Channels)
				{
					SpectrumProvider.Add(samples[i], NotificationStream.WaveFormat.Channels > 1 ? samples[i + 1] : 0);

					if (NotificationStream.GetMilliseconds(totalSamplesRead + i) - lastTimeRead >= stepDurationMs)
					{
						result.Add(GetTimeSpectrumData());
						lastTimeRead = NotificationStream.GetMilliseconds(totalSamplesRead + i);
					}
				}

				totalSamplesRead += samplesRead;
				if (samplesRead == 0)
					break;
			}

			SmoothDataAcrossTime(result, smoothnessWindow);
			return result;
		}

		private void SmoothDataAcrossTime(List<TimeSpectrumData> spectrumData, int smoothnessWindow)
		{
			if (smoothnessWindow % 2 == 0)
				smoothnessWindow++;

			if (smoothnessWindow > 1)
			{
				for (int dataIndex = smoothnessWindow / 2; dataIndex < spectrumData.Count - smoothnessWindow / 2; dataIndex++)
					for (int band = 0; band < spectrumData[dataIndex].SpectrumData.Length; band++)
					{
						double sum = 0;
						int divider = 0;
						for (int windowIndex = -smoothnessWindow / 2; windowIndex <= smoothnessWindow / 2 && dataIndex + windowIndex < spectrumData.Count; windowIndex++)
						{
							sum += spectrumData[dataIndex + windowIndex].SpectrumData[band] * (smoothnessWindow - Math.Abs(windowIndex));
							divider += smoothnessWindow - Math.Abs(windowIndex);
						}

						spectrumData[dataIndex].SpectrumData[band] = sum / divider;
					}
			}

			var maxValues = new double[spectrumData[0].SpectrumData.Length];

			for (int band = 0; band < maxValues.Length; band++)
				for (int index = 0; index < spectrumData.Count; index++)
					if (spectrumData[index].SpectrumData[band] > maxValues[band])
						maxValues[band] = spectrumData[index].SpectrumData[band];

			for (int i = 0; i < spectrumData.Count; i++)
				for (int band = 0; band < spectrumData[i].SpectrumData.Length; band++)
					spectrumData[i].SpectrumData[band] /= maxValues[band];
		}

		//private void SmoothDataAcross

		private TimeSpectrumData GetTimeSpectrumData()
		{
			var fftBuffer = new float[(int)Spectrum.FftSize];
			if (!Spectrum.SpectrumProvider.GetFftData(fftBuffer, this))
				throw new Exception("idk wtf happened");

			var data = Spectrum.CalculateSpectrumPoints(1000000, fftBuffer).Select(x => x.Value);

			return new TimeSpectrumData
			{
				SpectrumData = data.ToArray(),
				Time = NotificationStream.GetMilliseconds(NotificationStream.Position)
			};
		}

	}
}
