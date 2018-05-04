using SwrveUnity;
using SwrveUnity.Messaging;
using SwrveUnity.ResourceManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SwrveEmpty : SwrveSDK
{
	public override void Init(MonoBehaviour container, int appId, string apiKey, string userId)
	{
		SwrveConfig swrveConfig = new SwrveConfig();
		swrveConfig.UserId = userId;
		this.Init(container, 0, string.Empty, swrveConfig);
	}

	public override void Init(MonoBehaviour container, int appId, string apiKey, string userId, SwrveConfig config)
	{
		config.UserId = userId;
		this.Init(container, 0, string.Empty, config);
	}

	public override void Init(MonoBehaviour container, int appId, string apiKey, SwrveConfig config)
	{
		this.Container = container;
		this.ResourceManager = new SwrveResourceManager();
		this.prefabName = container.name;
		this.appId = appId;
		this.apiKey = apiKey;
		this.config = config;
		this.userId = config.UserId;
		this.Language = config.Language;
		this.Initialised = true;
	}

	public override bool SendQueuedEvents()
	{
		return true;
	}

	public override void GetUserResources(Action<Dictionary<string, Dictionary<string, string>>, string> onResult, Action<Exception> onError)
	{
	}

	public override void GetUserResourcesDiff(Action<Dictionary<string, Dictionary<string, string>>, Dictionary<string, Dictionary<string, string>>, string> onResult, Action<Exception> onError)
	{
	}

	public override void FlushToDisk(bool saveEventsBeingSent = false)
	{
	}

	[Obsolete("IsMessageDispaying is deprecated, please use IsMessageDisplaying instead.")]
	public override bool IsMessageDispaying()
	{
		return false;
	}

	public override bool IsMessageDisplaying()
	{
		return false;
	}

	public override SwrveMessage GetMessageForEvent(string eventName, IDictionary<string, string> payload)
	{
		return null;
	}

	public override SwrveConversation GetConversationForEvent(string eventName, IDictionary<string, string> payload = null)
	{
		return null;
	}

	[DebuggerHidden]
	public override IEnumerator ShowMessageForEvent(string eventName, SwrveMessage message, ISwrveInstallButtonListener installButtonListener = null, ISwrveCustomButtonListener customButtonListener = null, ISwrveMessageListener messageListener = null)
	{
		yield return null;
		yield break;
	}

	[DebuggerHidden]
	public override IEnumerator ShowConversationForEvent(string eventName, SwrveConversation conversation)
	{
		yield return null;
		yield break;
	}

	public override void DismissMessage()
	{
	}

	public override void RefreshUserResourcesAndCampaigns()
	{
	}

	public override void SessionStart()
	{
	}

	public override void NamedEvent(string name, Dictionary<string, string> payload = null)
	{
	}

	public override void UserUpdate(Dictionary<string, string> attributes)
	{
	}

	public override void UserUpdate(string name, DateTime date)
	{
	}

	public override void Purchase(string item, string currency, int cost, int quantity)
	{
	}

	public override void Iap(int quantity, string productId, double productPrice, string currency)
	{
	}

	public override void Iap(int quantity, string productId, double productPrice, string currency, IapRewards rewards)
	{
	}

	public override void CurrencyGiven(string givenCurrency, double amount)
	{
	}

	public override void LoadFromDisk()
	{
	}

	public override Dictionary<string, string> GetDeviceInfo()
	{
		return new Dictionary<string, string>();
	}

	public override void OnSwrvePause()
	{
	}

	public override void OnSwrveResume()
	{
	}

	public override void OnSwrveDestroy()
	{
	}

	public override List<SwrveBaseCampaign> GetCampaigns()
	{
		return new List<SwrveBaseCampaign>();
	}

	public override void ButtonWasPressedByUser(SwrveButton button)
	{
	}

	public override void MessageWasShownToUser(SwrveMessageFormat messageFormat)
	{
	}

	public override void ShowMessageCenterCampaign(SwrveBaseCampaign campaign)
	{
	}

	public override void ShowMessageCenterCampaign(SwrveBaseCampaign campaign, SwrveOrientation orientation)
	{
	}

	public override List<SwrveBaseCampaign> GetMessageCenterCampaigns()
	{
		return new List<SwrveBaseCampaign>();
	}

	public override List<SwrveBaseCampaign> GetMessageCenterCampaigns(SwrveOrientation orientation)
	{
		return new List<SwrveBaseCampaign>();
	}

	public override void RemoveMessageCenterCampaign(SwrveBaseCampaign campaign)
	{
	}

	public override SwrveMessage GetMessageForId(int messageId)
	{
		return null;
	}
}
