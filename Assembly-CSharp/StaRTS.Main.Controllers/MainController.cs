using StaRTS.Externals.GameServices;
using StaRTS.FX;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Performance;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.World;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class MainController
	{
		public MainController()
		{
			Service.MainController = this;
			GameStateMachine gameStateMachine = new GameStateMachine();
			gameStateMachine.SetState(new ApplicationLoadState());
			new PerformanceSampler();
		}

		public static void StaticInit()
		{
			CollisionFilters.StaticInit();
		}

		public static void StaticReset()
		{
			Camera[] allCameras = Camera.allCameras;
			int i = 0;
			int num = allCameras.Length;
			while (i < num)
			{
				allCameras[i].enabled = false;
				i++;
			}
			UnityUtils.StaticReset();
			GameServicesManager.StaticReset();
			MultipleEmittersPool.StaticReset();
			if (Service.AudioManager != null)
			{
				Service.AudioManager.CleanUp();
			}
			if (Service.WWWManager != null)
			{
				Service.WWWManager.CancelAll();
			}
			if (Service.AssetManager != null)
			{
				Service.AssetManager.ReleaseAll();
			}
			if (Service.EntityController != null)
			{
				Service.EntityController.StaticReset();
			}
			if (Service.StaticDataController != null)
			{
				Service.StaticDataController.Exterminate();
			}
			if (Service.ISocialDataController != null)
			{
				Service.ISocialDataController.StaticReset();
			}
			JsonParser.StaticReset();
			CollisionFilters.StaticReset();
			ProcessingScreen.StaticReset();
			YesNoScreen.StaticReset();
			DynamicRadiusView.StaticReset();
		}

		public static void CleanupReferences()
		{
			if (Service.SquadController != null)
			{
				Service.SquadController.Destroy();
			}
			if (Service.ProjectileViewManager != null)
			{
				Service.ProjectileViewManager.Destroy();
			}
			if (Service.BILoggingController != null)
			{
				Service.BILoggingController.Destroy();
			}
			if (Service.DMOAnalyticsController != null)
			{
				Service.DMOAnalyticsController.Destroy();
			}
			if (Service.MobileConnectorAdsController != null)
			{
				Service.MobileConnectorAdsController.Destroy();
			}
		}
	}
}
