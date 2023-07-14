using CSCore.DSP;
using System;
using System.Drawing;

namespace WinformsVisualization.Visualization
{
	public class VoicePrint3DSpectrum : SpectrumBase
	{
		private readonly GradientCalculator _colorCalculator;

		private bool _isInitialized;

		public Color[] Colors
		{
			get
			{
				return this._colorCalculator.Colors;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					throw new ArgumentException("value");
				}
				this._colorCalculator.Colors = value;
			}
		}

		public int PointCount
		{
			get
			{
				return this.SpectrumResolution;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.SpectrumResolution = value;
				this.UpdateFrequencyMapping();
			}
		}

		public VoicePrint3DSpectrum(FftSize fftSize)
		{
			this._colorCalculator = new GradientCalculator();
			this.Colors = new Color[]
			{
				Color.Black,
				Color.Blue,
				Color.Cyan,
				Color.Lime,
				Color.Yellow,
				Color.Red
			};
			base.FftSize = fftSize;
		}

		public bool CreateVoicePrint3D(Graphics graphics, RectangleF clipRectangle, float xPos, Color background, float lineThickness = 1f)
		{
			if (!this._isInitialized)
			{
				this.UpdateFrequencyMapping();
				this._isInitialized = true;
			}
			float[] fftBuffer = new float[(int)base.FftSize];
			if (base.SpectrumProvider.GetFftData(fftBuffer, this))
			{
				SpectrumBase.SpectrumPointData[] array = this.CalculateSpectrumPoints(1.0, fftBuffer);
				using (Pen pen = new Pen(background, lineThickness))
				{
					float num = clipRectangle.Y + clipRectangle.Height;
					for (int i = 0; i < array.Length; i++)
					{
						SpectrumBase.SpectrumPointData spectrumPointData = array[i];
						float x = clipRectangle.X + xPos;
						float num2 = clipRectangle.Height / (float)array.Length;
						pen.Color = this._colorCalculator.GetColor((float)spectrumPointData.Value);
						PointF pt = new PointF(x, num);
						PointF pt2 = new PointF(x, num - num2);
						graphics.DrawLine(pen, pt, pt2);
						num -= num2;
					}
				}
				return true;
			}
			return false;
		}
	}
}
