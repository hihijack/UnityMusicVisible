using CSCore.DSP;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace WinformsVisualization.Visualization
{
	public class LineSpectrum : SpectrumBase
	{
		private int _barCount;

		private double _barSpacing;

		private double _barWidth;

		private Size _currentSize;

		[Browsable(false)]
		public double BarWidth
		{
			get
			{
				return this._barWidth;
			}
		}

		public double BarSpacing
		{
			get
			{
				return this._barSpacing;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._barSpacing = value;
				this.UpdateFrequencyMapping();
				base.RaisePropertyChanged("BarSpacing");
				base.RaisePropertyChanged("BarWidth");
			}
		}

		public int BarCount
		{
			get
			{
				return this._barCount;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._barCount = value;
				this.SpectrumResolution = value;
				this.UpdateFrequencyMapping();
				base.RaisePropertyChanged("BarCount");
				base.RaisePropertyChanged("BarWidth");
			}
		}

		[Browsable(false)]
		public Size CurrentSize
		{
			get
			{
				return this._currentSize;
			}
			protected set
			{
				this._currentSize = value;
				base.RaisePropertyChanged("CurrentSize");
			}
		}

		public LineSpectrum(FftSize fftSize)
		{
			base.FftSize = fftSize;
		}

		public Bitmap CreateSpectrumLine(Size size, Brush brush, Color background, bool highQuality)
		{
			if (!this.UpdateFrequencyMappingIfNessesary(size))
			{
				return null;
			}
			float[] fftBuffer = new float[(int)base.FftSize];
			if (base.SpectrumProvider.GetFftData(fftBuffer, this))
			{
				using (Pen pen = new Pen(brush, (float)this._barWidth))
				{
					Bitmap bitmap = new Bitmap(size.Width, size.Height);
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						this.PrepareGraphics(graphics, highQuality);
						graphics.Clear(background);
						this.CreateSpectrumLineInternal(graphics, pen, fftBuffer, size);
					}
					return bitmap;
				}
			}
			return null;
		}

		public Bitmap CreateSpectrumLine(Size size, Color color1, Color color2, Color background, bool highQuality)
		{
			if (!this.UpdateFrequencyMappingIfNessesary(size))
			{
				return null;
			}
			Bitmap result;
			using (Brush brush = new LinearGradientBrush(new RectangleF(0f, 0f, (float)this._barWidth, (float)size.Height), color2, color1, LinearGradientMode.Vertical))
			{
				result = this.CreateSpectrumLine(size, brush, background, highQuality);
			}
			return result;
		}

		public float[] GetSpectrumPoints(float height, float[] fftBuffer)
		{
			SpectrumBase.SpectrumPointData[] array = this.CalculateSpectrumPoints((double)height, fftBuffer);
			float[] array2 = new float[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = (float)array[i].Value;
			}
			return array2;
		}

		private void CreateSpectrumLineInternal(Graphics graphics, Pen pen, float[] fftBuffer, Size size)
		{
			int height = size.Height;
			SpectrumBase.SpectrumPointData[] array = this.CalculateSpectrumPoints((double)height, fftBuffer);
			for (int i = 0; i < array.Length; i++)
			{
				SpectrumBase.SpectrumPointData spectrumPointData = array[i];
				int spectrumPointIndex = spectrumPointData.SpectrumPointIndex;
				double num = this.BarSpacing * (double)(spectrumPointIndex + 1) + this._barWidth * (double)spectrumPointIndex + this._barWidth / 2.0;
				PointF pt = new PointF((float)num, (float)height);
				PointF pt2 = new PointF((float)num, (float)height - (float)spectrumPointData.Value - 1f);
				graphics.DrawLine(pen, pt, pt2);
			}
		}

		protected override void UpdateFrequencyMapping()
		{
			this._barWidth = Math.Max(((double)this._currentSize.Width - this.BarSpacing * (double)(this.BarCount + 1)) / (double)this.BarCount, 1E-05);
			base.UpdateFrequencyMapping();
		}

		private bool UpdateFrequencyMappingIfNessesary(Size newSize)
		{
			if (newSize != this.CurrentSize)
			{
				this.CurrentSize = newSize;
				this.UpdateFrequencyMapping();
			}
			return newSize.Width > 0 && newSize.Height > 0;
		}

		private void PrepareGraphics(Graphics graphics, bool highQuality)
		{
			if (highQuality)
			{
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.CompositingQuality = CompositingQuality.AssumeLinear;
				graphics.PixelOffsetMode = PixelOffsetMode.Default;
				graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
				return;
			}
			graphics.SmoothingMode = SmoothingMode.HighSpeed;
			graphics.CompositingQuality = CompositingQuality.HighSpeed;
			graphics.PixelOffsetMode = PixelOffsetMode.None;
			graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
		}
	}
}
