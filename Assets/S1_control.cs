using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class S1_control : MonoBehaviour
{
	public Renderer groundM1;

	public Renderer star;

	public Renderer sun;

	public Renderer gridBG;

	public GameObject Cam;

	public Animator uiAnim;

	public Slider saturationS;

	public Slider hueS;

	public Slider powerS;

	private soundFX SoundFX;

	private AudioSource source;

	private PostProcessVolume volume;

	private ColorGrading colorGradingLayer;

	private void Start()
	{
		this.SoundFX = this.Cam.GetComponent<soundFX>();
		this.source = this.Cam.GetComponent<AudioSource>();
		this.volume = this.Cam.GetComponent<PostProcessVolume>();
		this.volume.profile.TryGetSettings<ColorGrading>(out this.colorGradingLayer);
	}

	private void Update()
	{
		this.updateScenes();
	}

	private void LateUpdate()
	{
		this.updateValue();
	}

	private void updateValue()
	{
		this.colorGradingLayer.saturation.value = this.saturationS.value;
		this.colorGradingLayer.hueShift.value = this.hueS.value;
	}

	private void updateScenes()
	{
		float num = this.SoundFX.sValue[0];
		float num2 = this.SoundFX.sValue[1];
		float num3 = this.SoundFX.sValue[2];
		float arg_87_0 = (num + num2 + num3) / 3f;
		float num4 = this.groundM1.material.GetVector("Vector2_B9934CDA")[1];
		this.groundM1.material.SetVector("Vector2_B9934CDA", new Vector2(0f, num4 - num * 0.01f));
		if (arg_87_0 > 0f)
		{
			this.groundM1.material.SetFloat("Vector1_EB8E7931", this.powerS.value);
			this.star.material.SetVector("Vector2_B9934CDA", new Vector2(0f, Time.time* 0.03f));
			this.gridBG.material.SetVector("Vector2_B9934CDA", new Vector2(0f, Time.time* 0.01f));
		}
		float num5 = num3 * 0.05f;
		this.star.material.SetFloat("Vector1_EB8E7931", this.powerS.value * 0.2f - num5);
		this.sun.material.SetFloat("Vector1_E19F2264", num2 * (this.powerS.value* 0.5f) + 0.15f);
	}

	public void panelSwitch(Text text)
	{
		if (text.text == ">")
		{
			text.text = "<";
			this.uiAnim.CrossFade("Cp_In", 0.1f);
			return;
		}
		text.text = ">";
		this.uiAnim.CrossFade("Cp_Out", 0.1f);
	}
}
