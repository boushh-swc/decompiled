using System;
using UnityEngine;

public class Splash : MonoBehaviour
{
	private const string PREF_SFX_VOLUME = "prefSfxVolume";

	private const string MAIN_SCENE = "MainScene";

	private const string STARTUP_SOUND = "stingerStartup";

	private const string UX_ROOT = "UX Root";

	private const string UX_CAMERA_NAME = "UX Camera";

	private const float FADE_DURATION = 0.4f;

	private float[] durations;

	private GameObject uxRoot;

	private int state;

	private float startTime;

	private AudioSource source;

	private float fadePerc;

	private bool transitioningOut;

	private Camera uxCamera;

	public static float CalculateScale()
	{
		float num;
		if (Screen.height < 640)
		{
			num = 0.75f;
		}
		else if (Screen.height >= 640 && Screen.height <= 768)
		{
			num = 1f;
		}
		else if (Screen.height == 1080)
		{
			num = 1.5f;
		}
		else if (Screen.height == 1440)
		{
			num = 2f;
		}
		else
		{
			num = (float)Screen.height / 768f;
			num = Mathf.Floor(num * 4f) * 0.25f;
		}
		return num;
	}

	private void SetOrientation()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.starts.PluginActivity");
		if (androidJavaClass != null)
		{
			AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			if (androidJavaObject != null && androidJavaObject.Call<bool>("IsAutoRotationEnabled", new object[0]))
			{
				Screen.orientation = ScreenOrientation.AutoRotation;
			}
			else if (Input.deviceOrientation == DeviceOrientation.LandscapeRight)
			{
				Screen.orientation = ScreenOrientation.LandscapeRight;
			}
			else
			{
				Screen.orientation = ScreenOrientation.LandscapeLeft;
			}
		}
	}

	private void Start()
	{
		this.SetOrientation();
		float num = Splash.CalculateScale();
		this.uxCamera = GameObject.Find("UX Camera").GetComponent<Camera>();
		this.uxCamera.transform.localScale = Vector3.one * num;
		NGUIText.fontResolutionMultiplier = num;
		this.uxRoot = GameObject.Find("UX Root");
		this.uxRoot.GetComponent<UIPanel>().alpha = 0f;
		this.durations = new float[]
		{
			0f,
			0.75f,
			1f,
			0.5f,
			0.5f
		};
		base.gameObject.AddComponent<AudioListener>();
		AudioClip clip = Resources.Load("stingerStartup") as AudioClip;
		this.source = base.gameObject.AddComponent<AudioSource>();
		this.source.clip = clip;
		this.source.volume = this.GetSfxVolume();
	}

	private float GetSfxVolume()
	{
		return (!PlayerPrefs.HasKey("prefSfxVolume")) ? 1f : PlayerPrefs.GetFloat("prefSfxVolume");
	}

	private void Update()
	{
		if (Time.time >= this.startTime)
		{
			this.state++;
			switch (this.state)
			{
			case 1:
				this.FadeIn();
				break;
			case 3:
				this.FadeOut();
				break;
			case 4:
				this.transitioningOut = true;
				break;
			case 5:
				this.Finish();
				return;
			}
			this.startTime += this.durations[this.state];
		}
		if (this.transitioningOut)
		{
			if (this.fadePerc < 1f)
			{
				this.fadePerc += Time.deltaTime / 0.4f;
			}
			else
			{
				this.transitioningOut = false;
			}
		}
	}

	private void FadeIn()
	{
		TweenAlpha.Begin(this.uxRoot, this.durations[this.state], 1f);
		this.source.Play();
	}

	private void FadeOut()
	{
		TweenAlpha.Begin(this.uxRoot, this.durations[this.state], 0f);
	}

	private void Finish()
	{
		Application.LoadLevel("MainScene");
	}
}
