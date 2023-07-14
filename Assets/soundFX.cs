using CSCore;
using CSCore.Codecs.WAV;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using UnityEngine;
using UnityEngine.UI;
using WinformsVisualization.Visualization;

public class soundFX : MonoBehaviour
{
	private const int SAMPLE_SIZE = 2048;

	public GameObject toggleObj;

	public float rms;

	public float db;

	public float pitch;

	public float vModifier = 50f;

	public float maxScale = 25f;

	public float keepPercentage = 0.5f;

	public float smoothSpeed = 10f;

	public bool useMicroPhone;

	public string selectDevice;

	public int minFreq = 5;

	public int maxFreq = 4500;

	public int barSpacing;

	public bool logScale = true;

	public bool isAverage;

	public float highScaleAverage = 2f;

	public float highScaleNotAverage = 3f;

	public float[] barData;

	private LineSpectrum lineSpectrum;

	private WasapiCapture capture;

	private WaveWriter writer;

	private FftSize fftSize;

	private float[] fftBuffer;

	private SingleBlockNotificationStream notificationSource;

	private BasicSpectrumProvider spectrumProvider;

	private IWaveSource finalSource;

	public float[] sValue;

	private int amnVisual = 4;

	private AudioSource source;

	private float[] samples;

	private float[] spectrum;

	private float sampleRate;

	private void Start()
	{
		this.source = base.GetComponent<AudioSource>();
		this.samples = new float[2048];
		this.spectrum = new float[2048];
		this.sampleRate = (float)AudioSettings.outputSampleRate;
		this.sValue = new float[this.amnVisual];
		this.SetupAudioSource();
		this.SetupGlobalAudio();
		this.SetupCScore();
		if (Microphone.devices.Length != 0)
		{
//			GameObject gameObject = this.toggleObj.transform.parent.gameObject;
//			RectTransform component = gameObject.GetComponent<RectTransform>();
//			for (int i = 0; i < Microphone.devices.Length; i++)
//			{
//				GameObject gameObject2 = Object.Instantiate<GameObject>(this.toggleObj, new Vector3(0f, 0f, 0f), Quaternion.identity);
//				Toggle toggleItem = gameObject2.GetComponent<Toggle>();
//				gameObject2.transform.SetParent(gameObject.transform);
//				gameObject2.transform.localScale = (Vector3.one);
//				gameObject2.transform.localPosition = (new Vector3(this.toggleObj.transform.localPosition.x, 9f - (float)(i + 1) * 35f, 0f));
//				gameObject2.transform.Find("Label").gameObject.GetComponent<Text>().text = (Microphone.devices[i]);
//				toggleItem.onValueChanged.AddListener(delegate(bool ifselect)
//				{
//					if (ifselect)
//					{
//						this.changeMicrophone(toggleItem);
//					}
//				});
//				gameObject2.gameObject.SetActive(true);
//				if (i == 0 && this.selectDevice != "")
//				{
//					toggleItem.isOn = (true);
//					this.selectDevice = Microphone.devices[i].ToString();
//				}
//			}
//			component.sizeDelta = (new Vector2(component.sizeDelta.x, (float)Microphone.devices.Length * 42f));
		}
	}

	private void OnApplicationQuit()
	{
		if (base.enabled)
		{
			this.capture.Stop();
			this.capture.Dispose();
		}
	}

	private void Update()
	{
		if (this.useMicroPhone)
		{
			this.updateValue();
			if (!Microphone.IsRecording(this.selectDevice))
			{
				this.source.clip = (Microphone.Start(this.selectDevice, true, 1, AudioSettings.outputSampleRate));
				while (Microphone.GetPosition(this.selectDevice) <= 0)
				{
				}
				this.source.Play();
				Debug.Log("play");
				return;
			}
		}
		else
		{
			this.updateWASAPI_Value();
			if (Microphone.IsRecording(this.selectDevice))
			{
				this.source.clip = (null);
				Microphone.End(this.selectDevice);
			}
		}
	}

	public void changeMicrophone(Toggle item)
	{
		string text = item.transform.Find("Label").gameObject.GetComponent<Text>().text;
		Debug.Log(text);
		if (Microphone.IsRecording(this.selectDevice))
		{
			Microphone.End(this.selectDevice);
		}
		this.selectDevice = text;
		this.source.clip = (null);
		this.source.clip = (Microphone.Start(this.selectDevice, true, 1, AudioSettings.outputSampleRate));
		while (Microphone.GetPosition(this.selectDevice) <= 0)
		{
		}
		this.source.Play();
	}

	private void SetupCScore()
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
			BarCount = this.amnVisual,
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
		var obj = this.sValue;
		lock (obj)
		{
			this.lineSpectrum.BarCount = this.amnVisual;
			if (this.amnVisual != this.sValue.Length)
			{
				this.sValue = new float[this.amnVisual];
			}
		}
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

	private void updateWASAPI_Value()
	{
		int num = this.sValue.Length;
		float[] fFtData = this.GetFFtData();
		
		if (fFtData == null)
		{
			return;
		}
		
		float[] obj = this.sValue;
		lock (obj)
		{
			int num2 = 0;
			while (num2 < num && num2 < fFtData.Length)
			{
				this.sValue[num2] = fFtData[num2] / 150f;
				num2++;
			}
			int num3 = 0;
			while (num3 < num && num3 < fFtData.Length)
			{
				if (this.lineSpectrum.UseAverage)
				{
					this.sValue[num3] = this.sValue[num3] + this.highScaleAverage * Mathf.Sqrt((float)num3 / ((float)num + 0f)) * this.sValue[num3];
				}
				else
				{
					this.sValue[num3] = this.sValue[num3] + this.highScaleNotAverage * Mathf.Sqrt((float)num3 / ((float)num + 0f)) * this.sValue[num3];
				}
				num3++;
			}
		}
	}

	private void SetupAudioSource()
	{
		this.source.bypassEffects = (true);
		this.source.bypassListenerEffects = (true);
		this.source.bypassReverbZones = (true);
		this.source.priority = (0);
		this.source.pitch = (1f);
	}

	private void SetupGlobalAudio()
	{
		AudioConfiguration configuration = AudioSettings.GetConfiguration();
		configuration.dspBufferSize = 2048;
		configuration.sampleRate = 44100;
		configuration.numVirtualVoices = 1;
		configuration.numRealVoices = 1;
		configuration.speakerMode = AudioSpeakerMode.Stereo;
		AudioSettings.Reset(configuration);
	}

	private void updateValue()
	{
		this.source.GetOutputData(this.samples, 0);
		int i = 0;
		float num = 0f;
		while (i < 2048)
		{
			num = this.samples[i] * this.samples[i];
			i++;
		}
		this.rms = Mathf.Sqrt(num / 2048f);
		this.db = 20f * Mathf.Log10(this.rms / 0.1f);
		this.source.GetSpectrumData(this.spectrum, 0, FFTWindow.Blackman);
		float num2 = 0f;
		int num3 = 0;
		for (i = 0; i < 2048; i++)
		{
			if (this.spectrum[i] > num2 && this.spectrum[i] > 0f)
			{
				num2 = this.spectrum[i];
				num3 = i;
			}
		}
		float num4 = (float)num3;
		if (num3 > 0 && num3 < 2047)
		{
			float num5 = this.spectrum[num3 - 1] / this.spectrum[num3];
			float num6 = this.spectrum[num3 + 1] / this.spectrum[num3];
			num4 += 0.5f * (num6 * num6 - num5 * num5);
		}
		this.pitch = num4 * (this.sampleRate / 2f) / 2048f;
		int j = 0;
		int num7 = 0;
		int num8 = (int)(2048f * this.keepPercentage) / this.amnVisual;
		while (j < this.amnVisual)
		{
			int k = 0;
			float num9 = 0f;
			while (k < num8)
			{
				num9 += this.spectrum[num7];
				num7++;
				k++;
			}
			float num10 = num9 / (float)num8 * this.vModifier;
			this.sValue[j] -= Time.deltaTime * this.smoothSpeed;
			if (this.sValue[j] < num10)
			{
				this.sValue[j] = num10;
			}
			if (this.sValue[j] > this.maxScale)
			{
				this.sValue[j] = this.maxScale;
			}
			j++;
		}
	}

	private void AnalyzeSound()
	{
	}
}
