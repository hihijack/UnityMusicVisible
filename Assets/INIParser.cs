using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class INIParser
{
	public int error;

	private object m_Lock = new object();

	private string m_FileName;

	private string m_iniString;

	private bool m_AutoFlush;

	private Dictionary<string, Dictionary<string, string>> m_Sections = new Dictionary<string, Dictionary<string, string>>();

	private Dictionary<string, Dictionary<string, string>> m_Modified = new Dictionary<string, Dictionary<string, string>>();

	private bool m_CacheModified;

	public string FileName
	{
		get
		{
			return this.m_FileName;
		}
	}

	public string iniString
	{
		get
		{
			return this.m_iniString;
		}
	}

	public void Open(string path)
	{
		this.m_FileName = path;
		if (File.Exists(this.m_FileName))
		{
			this.m_iniString = File.ReadAllText(this.m_FileName);
		}
		else
		{
			File.Create(this.m_FileName).Close();
			this.m_iniString = "";
		}
		this.Initialize(this.m_iniString, false);
	}

	public void Open(TextAsset name)
	{
		if (name == null)
		{
			this.error = 1;
			this.m_iniString = "";
			this.m_FileName = null;
			this.Initialize(this.m_iniString, false);
			return;
		}
		this.m_FileName = Application.dataPath + name.name;
		if (File.Exists(this.m_FileName))
		{
			this.m_iniString = File.ReadAllText(this.m_FileName);
		}
		else
		{
			this.m_iniString = name.text;
		}
		this.Initialize(this.m_iniString, false);
	}

	public void OpenFromString(string str)
	{
		this.m_FileName = null;
		this.Initialize(str, false);
	}

	public override string ToString()
	{
		return this.m_iniString;
	}

	private void Initialize(string iniString, bool AutoFlush)
	{
		this.m_iniString = iniString;
		this.m_AutoFlush = AutoFlush;
		this.Refresh();
	}

	public void Close()
	{
		object @lock = this.m_Lock;
		lock (@lock)
		{
			this.PerformFlush();
			this.m_FileName = null;
			this.m_iniString = null;
		}
	}

	private string ParseSectionName(string Line)
	{
		if (!Line.StartsWith("["))
		{
			return null;
		}
		if (!Line.EndsWith("]"))
		{
			return null;
		}
		if (Line.Length < 3)
		{
			return null;
		}
		return Line.Substring(1, Line.Length - 2);
	}

	private bool ParseKeyValuePair(string Line, ref string Key, ref string Value)
	{
		int num;
		if ((num = Line.IndexOf('=')) <= 0)
		{
			return false;
		}
		int num2 = Line.Length - num - 1;
		Key = Line.Substring(0, num).Trim();
		if (Key.Length <= 0)
		{
			return false;
		}
		Value = ((num2 > 0) ? Line.Substring(num + 1, num2).Trim() : "");
		return true;
	}

	private bool isComment(string Line)
	{
		string text = null;
		string text2 = null;
		return this.ParseSectionName(Line) == null && !this.ParseKeyValuePair(Line, ref text, ref text2);
	}

	private void Refresh()
	{
		object @lock = this.m_Lock;
		lock (@lock)
		{
			StringReader stringReader = null;
			try
			{
				this.m_Sections.Clear();
				this.m_Modified.Clear();
				stringReader = new StringReader(this.m_iniString);
				Dictionary<string, string> dictionary = null;
				string key = null;
				string value = null;
				string text;
				while ((text = stringReader.ReadLine()) != null)
				{
					text = text.Trim();
					string text2 = this.ParseSectionName(text);
					if (text2 != null)
					{
						if (this.m_Sections.ContainsKey(text2))
						{
							dictionary = null;
						}
						else
						{
							dictionary = new Dictionary<string, string>();
							this.m_Sections.Add(text2, dictionary);
						}
					}
					else if (dictionary != null && this.ParseKeyValuePair(text, ref key, ref value) && !dictionary.ContainsKey(key))
					{
						dictionary.Add(key, value);
					}
				}
			}
			finally
			{
				if (stringReader != null)
				{
					stringReader.Close();
				}
				stringReader = null;
			}
		}
	}

	private void PerformFlush()
	{
		if (!this.m_CacheModified)
		{
			return;
		}
		this.m_CacheModified = false;
		StringWriter stringWriter = new StringWriter();
		try
		{
			Dictionary<string, string> dictionary = null;
			Dictionary<string, string> dictionary2 = null;
			StringReader stringReader = null;
			try
			{
				stringReader = new StringReader(this.m_iniString);
				string text = null;
				string value = null;
				bool flag = true;
				bool flag2 = false;
				string key = null;
				string text2 = null;
				while (flag)
				{
					string text3 = stringReader.ReadLine();
					flag = (text3 != null);
					bool flag3;
					string text4;
					if (flag)
					{
						flag3 = true;
						text3 = text3.Trim();
						text4 = this.ParseSectionName(text3);
					}
					else
					{
						flag3 = false;
						text4 = null;
					}
					if (text4 != null || !flag)
					{
						if (dictionary != null && dictionary.Count > 0)
						{
							StringBuilder stringBuilder = stringWriter.GetStringBuilder();
							while (stringBuilder[stringBuilder.Length - 1] == '\n' || stringBuilder[stringBuilder.Length - 1] == '\r')
							{
								stringBuilder.Length--;
							}
							stringWriter.WriteLine();
							foreach (string current in dictionary.Keys)
							{
								if (dictionary.TryGetValue(current, out value))
								{
									stringWriter.Write(current);
									stringWriter.Write('=');
									stringWriter.WriteLine(value);
								}
							}
							stringWriter.WriteLine();
							dictionary.Clear();
						}
						if (flag && !this.m_Modified.TryGetValue(text4, out dictionary))
						{
							dictionary = null;
						}
					}
					else if (dictionary != null && this.ParseKeyValuePair(text3, ref text, ref value) && dictionary.TryGetValue(text, out value))
					{
						flag3 = false;
						dictionary.Remove(text);
						stringWriter.Write(text);
						stringWriter.Write('=');
						stringWriter.WriteLine(value);
					}
					if (flag3)
					{
						if (text4 != null)
						{
							if (!this.m_Sections.ContainsKey(text4))
							{
								flag2 = true;
								dictionary2 = null;
							}
							else
							{
								flag2 = false;
								this.m_Sections.TryGetValue(text4, out dictionary2);
							}
						}
						else if (dictionary2 != null && this.ParseKeyValuePair(text3, ref key, ref text2))
						{
							flag2 = !dictionary2.ContainsKey(key);
						}
					}
					if (flag3)
					{
						if (this.isComment(text3))
						{
							stringWriter.WriteLine(text3);
						}
						else if (!flag2)
						{
							stringWriter.WriteLine(text3);
						}
					}
				}
				stringReader.Close();
				stringReader = null;
			}
			finally
			{
				if (stringReader != null)
				{
					stringReader.Close();
				}
				stringReader = null;
			}
			foreach (KeyValuePair<string, Dictionary<string, string>> current2 in this.m_Modified)
			{
				dictionary = current2.Value;
				if (dictionary.Count > 0)
				{
					stringWriter.WriteLine();
					stringWriter.Write('[');
					stringWriter.Write(current2.Key);
					stringWriter.WriteLine(']');
					foreach (KeyValuePair<string, string> current3 in dictionary)
					{
						stringWriter.Write(current3.Key);
						stringWriter.Write('=');
						stringWriter.WriteLine(current3.Value);
					}
					dictionary.Clear();
				}
			}
			this.m_Modified.Clear();
			this.m_iniString = stringWriter.ToString();
			stringWriter.Close();
			stringWriter = null;
			if (this.m_FileName != null)
			{
				File.WriteAllText(this.m_FileName, this.m_iniString);
			}
		}
		finally
		{
			if (stringWriter != null)
			{
				stringWriter.Close();
			}
			stringWriter = null;
		}
	}

	public bool IsSectionExists(string SectionName)
	{
		return this.m_Sections.ContainsKey(SectionName);
	}

	public bool IsKeyExists(string SectionName, string Key)
	{
		if (this.m_Sections.ContainsKey(SectionName))
		{
			Dictionary<string, string> dictionary;
			this.m_Sections.TryGetValue(SectionName, out dictionary);
			return dictionary.ContainsKey(Key);
		}
		return false;
	}

	public void SectionDelete(string SectionName)
	{
		if (this.IsSectionExists(SectionName))
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				this.m_CacheModified = true;
				this.m_Sections.Remove(SectionName);
				this.m_Modified.Remove(SectionName);
				if (this.m_AutoFlush)
				{
					this.PerformFlush();
				}
			}
		}
	}

	public void KeyDelete(string SectionName, string Key)
	{
		if (this.IsKeyExists(SectionName, Key))
		{
			object @lock = this.m_Lock;
			lock (@lock)
			{
				this.m_CacheModified = true;
				Dictionary<string, string> dictionary;
				this.m_Sections.TryGetValue(SectionName, out dictionary);
				dictionary.Remove(Key);
				if (this.m_Modified.TryGetValue(SectionName, out dictionary))
				{
					dictionary.Remove(SectionName);
				}
				if (this.m_AutoFlush)
				{
					this.PerformFlush();
				}
			}
		}
	}

	public string ReadValue(string SectionName, string Key, string DefaultValue)
	{
		object @lock = this.m_Lock;
		string result;
		lock (@lock)
		{
			Dictionary<string, string> dictionary;
			string text;
			if (!this.m_Sections.TryGetValue(SectionName, out dictionary))
			{
				result = DefaultValue;
			}
			else if (!dictionary.TryGetValue(Key, out text))
			{
				result = DefaultValue;
			}
			else
			{
				result = text;
			}
		}
		return result;
	}

	public void WriteValue(string SectionName, string Key, string Value)
	{
		object @lock = this.m_Lock;
		lock (@lock)
		{
			this.m_CacheModified = true;
			Dictionary<string, string> dictionary;
			if (!this.m_Sections.TryGetValue(SectionName, out dictionary))
			{
				dictionary = new Dictionary<string, string>();
				this.m_Sections.Add(SectionName, dictionary);
			}
			if (dictionary.ContainsKey(Key))
			{
				dictionary.Remove(Key);
			}
			dictionary.Add(Key, Value);
			if (!this.m_Modified.TryGetValue(SectionName, out dictionary))
			{
				dictionary = new Dictionary<string, string>();
				this.m_Modified.Add(SectionName, dictionary);
			}
			if (dictionary.ContainsKey(Key))
			{
				dictionary.Remove(Key);
			}
			dictionary.Add(Key, Value);
			if (this.m_AutoFlush)
			{
				this.PerformFlush();
			}
		}
	}

	private string EncodeByteArray(byte[] Value)
	{
		if (Value == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Value.Length; i++)
		{
			string text = Convert.ToString(Value[i], 16);
			int length = text.Length;
			if (length > 2)
			{
				stringBuilder.Append(text.Substring(length - 2, 2));
			}
			else
			{
				if (length < 2)
				{
					stringBuilder.Append("0");
				}
				stringBuilder.Append(text);
			}
		}
		return stringBuilder.ToString();
	}

	private byte[] DecodeByteArray(string Value)
	{
		if (Value == null)
		{
			return null;
		}
		int num = Value.Length;
		if (num < 2)
		{
			return new byte[0];
		}
		num /= 2;
		byte[] array = new byte[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = Convert.ToByte(Value.Substring(i * 2, 2), 16);
		}
		return array;
	}

	public bool ReadValue(string SectionName, string Key, bool DefaultValue)
	{
		int num;
		if (int.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), out num))
		{
			return num != 0;
		}
		return DefaultValue;
	}

	public int ReadValue(string SectionName, string Key, int DefaultValue)
	{
		int result;
		if (int.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return DefaultValue;
	}

	public long ReadValue(string SectionName, string Key, long DefaultValue)
	{
		long result;
		if (long.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return DefaultValue;
	}

	public double ReadValue(string SectionName, string Key, double DefaultValue)
	{
		double result;
		if (double.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), NumberStyles.Any, CultureInfo.InvariantCulture, out result))
		{
			return result;
		}
		return DefaultValue;
	}

	public byte[] ReadValue(string SectionName, string Key, byte[] DefaultValue)
	{
		string value = this.ReadValue(SectionName, Key, this.EncodeByteArray(DefaultValue));
		byte[] result;
		try
		{
			result = this.DecodeByteArray(value);
		}
		catch (FormatException)
		{
			result = DefaultValue;
		}
		return result;
	}

	public DateTime ReadValue(string SectionName, string Key, DateTime DefaultValue)
	{
		DateTime result;
		if (DateTime.TryParse(this.ReadValue(SectionName, Key, DefaultValue.ToString(CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture, DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowInnerWhite | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.AssumeLocal, out result))
		{
			return result;
		}
		return DefaultValue;
	}

	public void WriteValue(string SectionName, string Key, bool Value)
	{
		this.WriteValue(SectionName, Key, Value ? "1" : "0");
	}

	public void WriteValue(string SectionName, string Key, int Value)
	{
		this.WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	public void WriteValue(string SectionName, string Key, long Value)
	{
		this.WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	public void WriteValue(string SectionName, string Key, double Value)
	{
		this.WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}

	public void WriteValue(string SectionName, string Key, byte[] Value)
	{
		this.WriteValue(SectionName, Key, this.EncodeByteArray(Value));
	}

	public void WriteValue(string SectionName, string Key, DateTime Value)
	{
		this.WriteValue(SectionName, Key, Value.ToString(CultureInfo.InvariantCulture));
	}
}
