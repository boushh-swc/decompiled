using Disney.MobileConnector.Ads;
using StaRTS.Externals.MobileConnectorAds;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers.Startup
{
	public class MobileConnectorAdsStartupTask : StartupTask
	{
		private const string OBJ_NAME = "MCAdsManager";

		public MobileConnectorAdsStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			MobileConnectorAdsController mobileConnectorAdsController = new MobileConnectorAdsController();
			if (!mobileConnectorAdsController.CanInitialize())
			{
				base.Complete();
				return;
			}
			new GameObject
			{
				name = "MCAdsManager"
			}.AddComponent<MCAdsManager>();
			mobileConnectorAdsController.InitializeAdUnits();
			base.Complete();
		}
	}
}
