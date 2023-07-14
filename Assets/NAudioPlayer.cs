using NAudio.Wave;
using System;
using System.IO;
using UnityEngine;

public static class NAudioPlayer
{
	public static AudioClip FromMp3Data(byte[] data)
	{
		WAV wAV = new WAV(NAudioPlayer.AudioMemStream(WaveFormatConversionStream.CreatePcmStream(new Mp3FileReader(new MemoryStream(data)))).ToArray());
		Debug.Log(wAV);
		AudioClip expr_3E = AudioClip.Create("testSound", wAV.SampleCount, 1, wAV.Frequency, false);
		expr_3E.SetData(wAV.LeftChannel, 0);
		return expr_3E;
	}

	private static MemoryStream AudioMemStream(WaveStream waveStream)
	{
		MemoryStream memoryStream = new MemoryStream();
		using (WaveFileWriter waveFileWriter = new WaveFileWriter(memoryStream, waveStream.WaveFormat))
		{
			byte[] array = new byte[waveStream.Length];
			waveStream.Position = 0L;
			waveStream.Read(array, 0, Convert.ToInt32(waveStream.Length));
			waveFileWriter.Write(array, 0, array.Length);
			waveFileWriter.Flush();
		}
		return memoryStream;
	}
}
