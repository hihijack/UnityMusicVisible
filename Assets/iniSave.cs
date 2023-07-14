using System;
using UnityEngine;
using UnityEngine.UI;

public class iniSave : MonoBehaviour
{
	public soundFX soundfx;

	public GameObject[] scenes;

	public Slider[] s1_sliders;

	public Slider[] s2_sliders;

	public GameObject[] canvas;

	[HideInInspector]
	public bool useMicrophone;

	[HideInInspector]
	public string deviceName;

	[HideInInspector]
	public int sceneIndex;

	[HideInInspector]
	public float s1_saturation;

	[HideInInspector]
	public float s1_hue;

	[HideInInspector]
	public float s1_power;

	[HideInInspector]
	public float s2_saturation;

	[HideInInspector]
	public float s2_hue;

	[HideInInspector]
	public float s2_color1_hue;

	[HideInInspector]
	public float s2_color2_hue;

	[HideInInspector]
	public float s2_glow;

	[HideInInspector]
	public float s2_power;

	private void Start()
	{
		this.soundfx = base.GetComponent<soundFX>();
		this.loadInI();
		this.setValue();
		bool flag = false;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			Debug.Log(string.Concat(new object[]
			{
				"ARG ",
				i,
				": ",
				commandLineArgs[i]
			}));
			if (commandLineArgs[i] == "-showCanvas")
			{
				flag = true;
			}
		}
		if (!flag)
		{
			GameObject[] array = this.canvas;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].SetActive(false);
			}
		}
	}

	private void OnApplicationQuit()
	{
		this.getValue();
		this.saveINI();
	}

	public void saveINI()
	{
		INIParser expr_05 = new INIParser();
		expr_05.Open(Application.dataPath + "\\save.ini");
		expr_05.WriteValue("General", "useMicrophone", this.useMicrophone);
		expr_05.WriteValue("General", "deviceName", this.deviceName);
		expr_05.WriteValue("General", "sceneIndex", this.sceneIndex);
		expr_05.WriteValue("Scene1", "s1_saturation", (double)this.s1_saturation);
		expr_05.WriteValue("Scene1", "s1_hue", (double)this.s1_hue);
		expr_05.WriteValue("Scene1", "s1_power", (double)this.s1_power);
		expr_05.WriteValue("Scene2", "s2_saturation", (double)this.s2_saturation);
		expr_05.WriteValue("Scene2", "s2_hue", (double)this.s2_hue);
		expr_05.WriteValue("Scene2", "s2_color1_hue", (double)this.s2_color1_hue);
		expr_05.WriteValue("Scene2", "s2_color2_hue", (double)this.s2_color2_hue);
		expr_05.WriteValue("Scene2", "s2_glow", (double)this.s2_glow);
		expr_05.WriteValue("Scene2", "s2_power", (double)this.s2_power);
		expr_05.Close();
	}

	public void loadInI()
	{
		INIParser iNIParser = new INIParser();
		iNIParser.Open(Application.dataPath + "\\save.ini");
		this.useMicrophone = iNIParser.ReadValue("General", "useMicrophone", false);
		this.deviceName = iNIParser.ReadValue("General", "deviceName", "");
		this.sceneIndex = iNIParser.ReadValue("General", "sceneIndex", 0);
		this.s1_saturation = (float)iNIParser.ReadValue("Scene1", "s1_saturation", 20.0);
		this.s1_hue = (float)iNIParser.ReadValue("Scene1", "s1_hue", 0.0);
		this.s1_power = (float)iNIParser.ReadValue("Scene1", "s1_power", 0.40000000596046448);
		this.s2_saturation = (float)iNIParser.ReadValue("Scene2", "s2_saturation", 20.0);
		this.s2_hue = (float)iNIParser.ReadValue("Scene2", "s2_hue", 0.0);
		this.s2_color1_hue = (float)iNIParser.ReadValue("Scene2", "s2_color1_hue", 0.0);
		this.s2_color2_hue = (float)iNIParser.ReadValue("Scene2", "s2_color2_hue", 0.0);
		this.s2_glow = (float)iNIParser.ReadValue("Scene2", "s2_glow", 0.5);
		this.s2_power = (float)iNIParser.ReadValue("Scene2", "s2_power", 0.10000000149011612);
		iNIParser.Close();
	}

	public void getValue()
	{
		this.useMicrophone = this.soundfx.useMicroPhone;
		this.deviceName = this.soundfx.selectDevice;
		for (int i = 0; i < this.scenes.Length; i++)
		{
			if (this.scenes[i].gameObject.activeSelf)
			{
				this.sceneIndex = i;
			}
		}
		this.s1_saturation = this.s1_sliders[0].value;
		this.s1_hue = this.s1_sliders[1].value;
		this.s1_power = this.s1_sliders[2].value;
		this.s2_saturation = this.s2_sliders[0].value;
		this.s2_hue = this.s2_sliders[1].value;
		this.s2_color1_hue = this.s2_sliders[2].value;
		this.s2_color2_hue = this.s2_sliders[3].value;
		this.s2_glow = this.s2_sliders[4].value;
		this.s2_power = this.s2_sliders[5].value;
	}

	public void setValue()
	{
		this.soundfx.useMicroPhone = this.useMicrophone;
		this.soundfx.selectDevice = this.deviceName;
		for (int i = 0; i < this.scenes.Length; i++)
		{
			if (this.sceneIndex == i)
			{
				this.scenes[i].SetActive(true);
			}
			else
			{
				this.scenes[i].SetActive(false);
			}
		}

		this.s1_sliders[1].value = (this.s1_hue);
		this.s1_sliders[2].value = (this.s1_power);
		this.s2_sliders[0].value = (this.s2_saturation);
		this.s1_sliders[0].value = (this.s1_saturation);
		this.s2_sliders[1].value = (this.s2_hue);
		this.s2_sliders[2].value = (this.s2_color1_hue);
		this.s2_sliders[3].value = (this.s2_color2_hue);
		this.s2_sliders[4].value = (this.s2_glow);
		this.s2_sliders[5].value = (this.s2_power);
	}
}
