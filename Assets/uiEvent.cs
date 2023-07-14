using NAudio.Wave;
using SFB;
using System;
using UnityEngine;
using UnityEngine.UI;

public class uiEvent : MonoBehaviour
{
	private string path;

	public AudioClip audioC;

	public AudioSource audioS;

	public soundFX soundfx;

	public Animator uiAnim;

	public GameObject[] scenes;

	private IWavePlayer waveOutDevice;

	private AudioFileReader audioFileReader;

	public Color selectBtnColor;

	public Button WASAPI_Btn;

	public Button Microphone_Btn;

	private void Start()
	{
		this.audioS = base.GetComponent<AudioSource>();
		this.soundfx = base.GetComponent<soundFX>();
		if (this.soundfx.useMicroPhone)
		{
			this.changeBtnColor(this.Microphone_Btn, this.selectBtnColor);
			return;
		}
		this.changeBtnColor(this.WASAPI_Btn, this.selectBtnColor);
	}

	private void Update()
	{
		if (Input.GetKey("escape"))
		{
			Application.Quit();
		}
	}

	public void pausePlay()
	{
		if (!this.audioS.isPlaying)
		{
			this.audioS.Play();
			return;
		}
		this.audioS.Pause();
	}

	public void OpenMusic()
	{
		this.path = StandaloneFileBrowser.OpenFilePanel("Open File", "", "mp3", false)[0];
		if (this.path != null)
		{
			WWW wWW = new WWW("file:///" + this.path);
			this.audioS.clip = (NAudioPlayer.FromMp3Data(wWW.bytes));
			this.audioS.Play();
		}
	}

	public void useMicroPhoneDevice(bool yesOrNo)
	{
		this.soundfx.useMicroPhone = yesOrNo;
		if (!yesOrNo)
		{
			this.changeBtnColor(this.WASAPI_Btn, this.selectBtnColor);
			this.changeBtnColor(this.Microphone_Btn, new Color(255f, 255f, 255f));
			return;
		}
		this.changeBtnColor(this.Microphone_Btn, this.selectBtnColor);
		this.changeBtnColor(this.WASAPI_Btn, new Color(255f, 255f, 255f));
	}

	public void panelSwitch(Text text)
	{
		if (text.text == "<")
		{
			text.text = (">");
			this.uiAnim.CrossFade("moveIn", 0.1f);
			return;
		}
		text.text = ("<");
		this.uiAnim.CrossFade("moveOut", 0.1f);
	}

	public void changeBtnColor(Button btn, Color color)
	{
		ColorBlock colors = btn.colors;
		colors.normalColor = (color);
		colors.highlightedColor = (color);
		colors.pressedColor = (color);
		colors.selectedColor = (color);
		btn.colors = (colors);
	}

	public void switchScenes(GameObject show)
	{
		for (int i = 0; i < this.scenes.Length; i++)
		{
			this.scenes[i].gameObject.SetActive(false);
		}
		show.SetActive(true);
	}

	public void aboutLink()
	{
		Application.OpenURL("http://jom.wang/");
	}
}
