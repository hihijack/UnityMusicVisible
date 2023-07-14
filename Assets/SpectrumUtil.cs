using CSCore;
using CSCore.Codecs.WAV;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using WinformsVisualization.Visualization;

namespace DefaultNamespace
{
    public class SpectrumUtil
    {
        #region 单例
        private static SpectrumUtil _inst;
        public static SpectrumUtil Inst
        {
            get
            {
                if (_inst == null)
                {
                    _inst = new SpectrumUtil();
                }

                return _inst;
            }
        }
        #endregion

        private WasapiCapture capture;

        private WaveWriter writer;

        private FftSize fftSize;

        private float[] fftBuffer;

        private SingleBlockNotificationStream notificationSource;

        private BasicSpectrumProvider spectrumProvider;

        private IWaveSource finalSource;
        
        private LineSpectrum lineSpectrum;
        
        private int amnVisual = 4;
                
        public int minFreq = 5;

        public int maxFreq = 4500;

        public int barSpacing;

        public bool logScale = true;
        
        public void Init(int amnVisual)
        {
            this.capture = new WasapiLoopbackCapture();
            this.capture.Initialize();
            IWaveSource arg_BB_0 = new SoundInSource(this.capture);
            this.fftSize = FftSize.Fft2048;
            this.fftBuffer = new float[(int)this.fftSize];
            this.spectrumProvider = new BasicSpectrumProvider(this.capture.WaveFormat.Channels, this.capture.WaveFormat.SampleRate, this.fftSize);
            this.lineSpectrum = new LineSpectrum(this.fftSize)
            {
                SpectrumProvider = this.spectrumProvider,
                UseAverage = true,
                BarCount = amnVisual,
                BarSpacing = 2.0,
                IsXLogScale = false,
                ScalingStrategy = ScalingStrategy.Linear
            };
		
            //每次读取块后，触发SingleBlockRead事件。
            SingleBlockNotificationStream singleBlockNotificationStream = new SingleBlockNotificationStream(arg_BB_0.ToSampleSource());
            singleBlockNotificationStream.SingleBlockRead += NotificationSource_SingleBlockRead;
            this.finalSource = singleBlockNotificationStream.ToWaveSource();
            this.capture.DataAvailable += Capture_DataAvailable;
            this.capture.Start();
        }
        
        private void Capture_DataAvailable(object sender, DataAvailableEventArgs e)
        {
            this.finalSource.Read(e.Data, e.Offset, e.ByteCount);
        }

        private void NotificationSource_SingleBlockRead(object sender, SingleBlockReadEventArgs e)
        {
            this.spectrumProvider.Add(e.Left, e.Right);
        }
        
        public float[] GetFFtData()
        {
            if (this.spectrumProvider.IsNewDataAvailable)
            {
                this.lineSpectrum.MinimumFrequency = this.minFreq;
                this.lineSpectrum.MaximumFrequency = this.maxFreq;
                this.lineSpectrum.IsXLogScale = this.logScale;
                this.lineSpectrum.BarSpacing = (double)this.barSpacing;
                this.lineSpectrum.SpectrumProvider.GetFftData(this.fftBuffer, this);
                return this.lineSpectrum.GetSpectrumPoints(100f, this.fftBuffer);
            }
            return null;
        }
    }
}