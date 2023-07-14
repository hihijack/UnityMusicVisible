using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class S2_Control : MonoBehaviour
{
	public GameObject Cam;

	public Animator uiAnim;

	public Renderer sphere;

	public GameObject bgSphere;

	public Slider saturationS;

	public Slider hueS;

	public Slider color1;

	public Slider color2;

	public Slider glow;

	public Slider power;

	private Renderer bgSphereR;

	private soundFX SoundFX;

	private AudioSource source;

	private PostProcessVolume volume;

	private ColorGrading colorGradingLayer;

	private void Start()
	{
		this.SoundFX = this.Cam.GetComponent<soundFX>();
		this.source = this.Cam.GetComponent<AudioSource>();
		this.volume = this.Cam.GetComponent<PostProcessVolume>();
		this.bgSphereR = this.bgSphere.GetComponent<Renderer>();
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
		this.sphere.material.SetFloat("Vector1_666EF732", this.color1.value);
		this.sphere.material.SetFloat("Vector1_5E75F4E7", this.color2.value);
		this.bgSphereR.material.SetFloat("_ColorHue", this.color1.value);
	}

	private void updateScenes()
	{
		float num = this.SoundFX.sValue[0];
		float num2 = this.SoundFX.sValue[1];
		float num3 = this.SoundFX.sValue[2];
		float num4 = (num + num2 + num3) / 3f;
		float @float = this.sphere.material.GetFloat("Vector1_8347C8F8");
		this.sphere.material.SetFloat("Vector1_8347C8F8", @float + num * 0.02f);
		this.sphere.material.SetFloat("Vector1_32C4FE17", this.power.value * (num * 2f) * (num4 + 2f));
		this.sphere.material.SetFloat("Vector1_141EAB63", this.glow.value * (num2 * 2f + 1f));
		this.bgSphere.transform.Rotate(new Vector3(num * 0.5f, 0f, 0f));
		this.sphere.gameObject.transform.Rotate(new Vector3(-num, 0f, 0f));
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
