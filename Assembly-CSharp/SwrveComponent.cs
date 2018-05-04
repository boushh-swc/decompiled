using SwrveUnity;
using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SwrveComponent : MonoBehaviour
{
	public SwrveSDK SDK;

	public int AppId;

	public string ApiKey = "your_api_key_here";

	public SwrveConfig Config;

	public bool FlushEventsOnApplicationQuit = true;

	public bool InitialiseOnStart = true;

	protected static SwrveComponent instance;

	public static SwrveComponent Instance
	{
		get
		{
			if (!SwrveComponent.instance)
			{
				SwrveComponent[] array = UnityEngine.Object.FindObjectsOfType(typeof(SwrveComponent)) as SwrveComponent[];
				if (array != null && array.Length > 0)
				{
					SwrveComponent.instance = array[0];
				}
				else
				{
					SwrveLog.LogError("There needs to be one active SwrveComponent script on a GameObject in your scene.");
				}
			}
			return SwrveComponent.instance;
		}
	}

	public SwrveComponent()
	{
		this.Config = new SwrveConfig();
		this.SDK = new SwrveEmpty();
	}

	public void Init(int appId, string apiKey)
	{
		if (this.SDK == null || this.SDK is SwrveEmpty)
		{
			bool flag = SwrveSDK.IsSupportedAndroidVersion();
			if (flag)
			{
				this.SDK = new SwrveSDK();
			}
			else
			{
				this.SDK = new SwrveEmpty();
			}
		}
		this.SDK.Init(this, appId, apiKey, this.Config);
	}

	public void Start()
	{
		base.useGUILayout = false;
		if (this.InitialiseOnStart)
		{
			this.Init(this.AppId, this.ApiKey);
		}
	}

	public void OnGUI()
	{
		this.SDK.OnGUI();
	}

	public void Update()
	{
		if (this.SDK != null && this.SDK.Initialised)
		{
			this.SDK.Update();
		}
	}

	public virtual void OnDeviceRegistered(string registrationId)
	{
		if (this.SDK != null && this.SDK.Initialised)
		{
			this.SDK.RegistrationIdReceived(registrationId);
		}
	}

	public virtual void OnNotificationReceived(string notificationJson)
	{
		if (this.SDK != null && this.SDK.Initialised)
		{
			this.SDK.NotificationReceived(notificationJson);
		}
	}

	public virtual void OnOpenedFromPushNotification(string notificationJson)
	{
		if (this.SDK != null && this.SDK.Initialised)
		{
			this.SDK.OpenedFromPushNotification(notificationJson);
		}
	}

	public virtual void OnDeviceRegisteredADM(string registrationId)
	{
		if (this.SDK != null && this.SDK.Initialised)
		{
			this.SDK.RegistrationIdReceivedADM(registrationId);
		}
	}

	public virtual void OnNotificationReceivedADM(string notificationJson)
	{
		if (this.SDK != null && this.SDK.Initialised)
		{
			this.SDK.NotificationReceivedADM(notificationJson);
		}
	}

	public virtual void OnOpenedFromPushNotificationADM(string notificationJson)
	{
		if (this.SDK != null && this.SDK.Initialised)
		{
			this.SDK.OpenedFromPushNotificationADM(notificationJson);
		}
	}

	public virtual void OnNewAdvertisingId(string advertisingId)
	{
		if (this.SDK != null && this.SDK.Initialised)
		{
			this.SDK.SetGooglePlayAdvertisingId(advertisingId);
		}
	}

	public void OnDestroy()
	{
		if (this.SDK.Initialised)
		{
			this.SDK.OnSwrveDestroy();
		}
		base.StopAllCoroutines();
	}

	public void OnApplicationQuit()
	{
		if (this.SDK.Initialised && this.FlushEventsOnApplicationQuit)
		{
			this.SDK.OnSwrveDestroy();
		}
	}

	public void OnApplicationPause(bool pauseStatus)
	{
		if (this.SDK != null && this.SDK.Initialised && this.Config != null && this.Config.AutomaticSessionManagement)
		{
			if (pauseStatus)
			{
				this.SDK.OnSwrvePause();
			}
			else
			{
				this.SDK.OnSwrveResume();
			}
		}
	}

	public void SetLocationSegmentVersion(string locationSegmentVersion)
	{
		try
		{
			this.SDK.SetLocationSegmentVersion(int.Parse(locationSegmentVersion));
		}
		catch (Exception ex)
		{
			SwrveLog.LogError(ex.ToString(), "location");
		}
	}

	public void UserUpdate(string userUpdate)
	{
		try
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(userUpdate);
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			Dictionary<string, object>.Enumerator enumerator = dictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Dictionary<string, string> arg_49_0 = dictionary2;
				KeyValuePair<string, object> current = enumerator.Current;
				string arg_49_1 = current.Key;
				string arg_44_0 = "{0}";
				KeyValuePair<string, object> current2 = enumerator.Current;
				arg_49_0[arg_49_1] = string.Format(arg_44_0, current2.Value);
			}
			this.SDK.UserUpdate(dictionary2);
		}
		catch (Exception ex)
		{
			SwrveLog.LogError(ex.ToString(), "userUpdate");
		}
	}
}
