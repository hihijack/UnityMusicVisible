using System;

public class WAV
{
	public float[] LeftChannel
	{
		get;
		internal set;
	}

	public float[] RightChannel
	{
		get;
		internal set;
	}

	public int ChannelCount
	{
		get;
		internal set;
	}

	public int SampleCount
	{
		get;
		internal set;
	}

	public int Frequency
	{
		get;
		internal set;
	}

	private static float bytesToFloat(byte firstByte, byte secondByte)
	{
		return (float)((short)((int)secondByte << 8 | (int)firstByte)) / 32768f;
	}

	private static int bytesToInt(byte[] bytes, int offset = 0)
	{
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			num |= (int)bytes[offset + i] << i * 8;
		}
		return num;
	}

	public WAV(byte[] wav)
	{
		this.ChannelCount = (int)wav[22];
		this.Frequency = WAV.bytesToInt(wav, 24);
		int i = 12;
		while (wav[i] != 100 || wav[i + 1] != 97 || wav[i + 2] != 116 || wav[i + 3] != 97)
		{
			i += 4;
			int num = (int)wav[i] + (int)wav[i + 1] * 256 + (int)wav[i + 2] * 65536 + (int)wav[i + 3] * 16777216;
			i += 4 + num;
		}
		i += 8;
		this.SampleCount = (wav.Length - i) / 2;
		if (this.ChannelCount == 2)
		{
			this.SampleCount /= 2;
		}
		this.LeftChannel = new float[this.SampleCount];
		if (this.ChannelCount == 2)
		{
			this.RightChannel = new float[this.SampleCount];
		}
		else
		{
			this.RightChannel = null;
		}
		int num2 = 0;
		while (i < wav.Length)
		{
			this.LeftChannel[num2] = WAV.bytesToFloat(wav[i], wav[i + 1]);
			i += 2;
			if (this.ChannelCount == 2)
			{
				this.RightChannel[num2] = WAV.bytesToFloat(wav[i], wav[i + 1]);
				i += 2;
			}
			num2++;
		}
	}

	public override string ToString()
	{
		return string.Format("[WAV: LeftChannel={0}, RightChannel={1}, ChannelCount={2}, SampleCount={3}, Frequency={4}]", new object[]
		{
			this.LeftChannel,
			this.RightChannel,
			this.ChannelCount,
			this.SampleCount,
			this.Frequency
		});
	}
}
