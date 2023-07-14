using CSCore;
using CSCore.DSP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WinformsVisualization.Visualization
{
	public class SpectrumBase : INotifyPropertyChanged
	{
		[DebuggerDisplay("{Value}")]
		protected struct SpectrumPointData
		{
			public int SpectrumPointIndex;

			public double Value;
		}

		private const int ScaleFactorLinear = 9;

		protected const int ScaleFactorSqr = 2;

		protected const double MinDbValue = -90.0;

		protected const double MaxDbValue = 0.0;

		protected const double DbScale = 90.0;

		private int _fftSize;

		private bool _isXLogScale;

		private int _maxFftIndex;

		private int _maximumFrequency = 20000;

		private int _maximumFrequencyIndex;

		private int _minimumFrequency = 20;

		private int _minimumFrequencyIndex;

		private ScalingStrategy _scalingStrategy;

		private int[] _spectrumIndexMax;

		private int[] _spectrumLogScaleIndexMax;

		private ISpectrumProvider _spectrumProvider;

		protected int SpectrumResolution;

		private bool _useAverage;

		[method: CompilerGenerated]
		[CompilerGenerated]
		public event PropertyChangedEventHandler PropertyChanged;

		public int MaximumFrequency
		{
			get
			{
				return this._maximumFrequency;
			}
			set
			{
				if (value <= this.MinimumFrequency)
				{
					throw new ArgumentOutOfRangeException("value", "Value must not be less or equal the MinimumFrequency.");
				}
				this._maximumFrequency = value;
				this.UpdateFrequencyMapping();
				this.RaisePropertyChanged("MaximumFrequency");
			}
		}

		public int MinimumFrequency
		{
			get
			{
				return this._minimumFrequency;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._minimumFrequency = value;
				this.UpdateFrequencyMapping();
				this.RaisePropertyChanged("MinimumFrequency");
			}
		}

		[Browsable(false)]
		public ISpectrumProvider SpectrumProvider
		{
			get
			{
				return this._spectrumProvider;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._spectrumProvider = value;
				this.RaisePropertyChanged("SpectrumProvider");
			}
		}

		public bool IsXLogScale
		{
			get
			{
				return this._isXLogScale;
			}
			set
			{
				this._isXLogScale = value;
				this.UpdateFrequencyMapping();
				this.RaisePropertyChanged("IsXLogScale");
			}
		}

		public ScalingStrategy ScalingStrategy
		{
			get
			{
				return this._scalingStrategy;
			}
			set
			{
				this._scalingStrategy = value;
				this.RaisePropertyChanged("ScalingStrategy");
			}
		}

		public bool UseAverage
		{
			get
			{
				return this._useAverage;
			}
			set
			{
				this._useAverage = value;
				this.RaisePropertyChanged("UseAverage");
			}
		}

		[Browsable(false)]
		public FftSize FftSize
		{
			get
			{
				return (FftSize)this._fftSize;
			}
			protected set
			{
				if ((int)Math.Log((double)value, 2.0) % 1 != 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._fftSize = (int)value;
				this._maxFftIndex = this._fftSize / 2 - 1;
				this.RaisePropertyChanged("FFTSize");
			}
		}

		protected virtual void UpdateFrequencyMapping()
		{
			this._maximumFrequencyIndex = Math.Min(this._spectrumProvider.GetFftBandIndex((float)this.MaximumFrequency) + 1, this._maxFftIndex);
			this._minimumFrequencyIndex = Math.Min(this._spectrumProvider.GetFftBandIndex((float)this.MinimumFrequency), this._maxFftIndex);
			int spectrumResolution = this.SpectrumResolution;
			int num = this._maximumFrequencyIndex - this._minimumFrequencyIndex;
			double num2 = Math.Round((double)num / (double)spectrumResolution, 3);
			this._spectrumIndexMax = this._spectrumIndexMax.CheckBuffer((long)spectrumResolution, true);
			this._spectrumLogScaleIndexMax = this._spectrumLogScaleIndexMax.CheckBuffer((long)spectrumResolution, true);
			double num3 = Math.Log((double)spectrumResolution, (double)spectrumResolution);
			for (int i = 1; i < spectrumResolution; i++)
			{
				int num4 = (int)((num3 - Math.Log((double)(spectrumResolution + 1 - i), (double)(spectrumResolution + 1))) * (double)num) + this._minimumFrequencyIndex;
				this._spectrumIndexMax[i - 1] = this._minimumFrequencyIndex + (int)((double)i * num2);
				this._spectrumLogScaleIndexMax[i - 1] = num4;
			}
			if (spectrumResolution > 0)
			{
				this._spectrumIndexMax[this._spectrumIndexMax.Length - 1] = (this._spectrumLogScaleIndexMax[this._spectrumLogScaleIndexMax.Length - 1] = this._maximumFrequencyIndex);
			}
		}

		protected virtual SpectrumBase.SpectrumPointData[] CalculateSpectrumPoints(double maxValue, float[] fftBuffer)
		{
			List<SpectrumBase.SpectrumPointData> list = new List<SpectrumBase.SpectrumPointData>();
			double val = 0.0;
			double num = 0.0;
			double num2 = 0.0;
			int num3 = 0;
			for (int i = this._minimumFrequencyIndex; i <= this._maximumFrequencyIndex; i++)
			{
				switch (this.ScalingStrategy)
				{
				case ScalingStrategy.Decibel:
					val = (20.0 * Math.Log10((double)fftBuffer[i]) - -90.0) / 90.0 * maxValue;
					break;
				case ScalingStrategy.Linear:
					val = (double)(fftBuffer[i] * 9f) * maxValue;
					break;
				case ScalingStrategy.Sqrt:
					val = Math.Sqrt((double)fftBuffer[i]) * 2.0 * maxValue;
					break;
				}
				bool flag = true;
				num = Math.Max(0.0, Math.Max(val, num));
				while (num3 <= this._spectrumIndexMax.Length - 1 && i == (this.IsXLogScale ? this._spectrumLogScaleIndexMax[num3] : this._spectrumIndexMax[num3]))
				{
					if (!flag)
					{
						num = num2;
					}
					if (num > maxValue)
					{
						num = maxValue;
					}
					if (this._useAverage && num3 > 0)
					{
						num = (num2 + num) / 2.0;
					}
					list.Add(new SpectrumBase.SpectrumPointData
					{
						SpectrumPointIndex = num3,
						Value = num
					});
					num2 = num;
					num = 0.0;
					num3++;
					flag = false;
				}
			}
			return list.ToArray();
		}

		protected void RaisePropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null && !string.IsNullOrEmpty(propertyName))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
