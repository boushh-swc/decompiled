using StaRTS.FX;
using StaRTS.Main.Configs;
using StaRTS.Main.Controllers.Holonet;
using StaRTS.Main.Controllers.Notifications;
using StaRTS.Main.Controllers.Objectives;
using StaRTS.Main.Controllers.ShardShop;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Controllers.VictoryConditions;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Models;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Animation;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class GeneralStartupTask : StartupTask
	{
		private bool holonetPrepared;

		private bool shardShopPrepared;

		public GeneralStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			Service.EventManager.SendEvent(EventId.InitializeGeneralSystemsStart, null);
			new WorldTransitioner();
			new VictoryConditionController();
			new DefensiveBattleController();
			new RaidDefenseController();
			new CombatTriggerManager();
			new TrapViewController();
			new TrapController();
			new StunController();
			new BattleController();
			new BattleRecordController();
			new BattlePlaybackController();
			new PostBattleRepairController();
			new ShooterController();
			new TargetingController();
			new TurretAttackController();
			new TroopAttackController();
			new BuffController();
			new TroopAbilityController();
			new ArmoryController();
			new DeployableShardUnlockController();
			new FXManager();
			new AnimationEventManager();
			new StarportDecalManager();
			if (!HardwareProfile.IsLowEndDevice())
			{
				new TerrainBlendController();
			}
			new BuildingTooltipController();
			new CurrencyEffects();
			new CurrencyController();
			new StorageEffects();
			new AnimController();
			new UnlockController();
			new RewardManager();
			new CampaignController();
			new TournamentController();
			new PvpManager();
			new NeighborVisitManager();
			new TransportController();
			new ShuttleController();
			new ShieldEffects();
			new ShieldController();
			new MobilizationEffectsManager();
			new SocialPushNotificationController();
			new FactionIconUpgradeController();
			new TroopDonationTrackController();
			new LimitedEditionItemController();
			new StickerController();
			Service.NotificationController.Init();
			Service.EventManager.SendEvent(EventId.InitializeGeneralSystemsEnd, null);
			new TargetedBundleController();
			ShardShopController shardShopController = new ShardShopController();
			shardShopController.GetOffering(new Action(this.OnShardShopOfferingReceived), false);
			HolonetController holonetController = new HolonetController();
			holonetController.PrepareContent(new HolonetController.HolonetPreparedDelegate(this.OnHolonetPrepared));
			new InventoryCrateRewardController();
			new ObjectiveManager();
			new ObjectiveController();
			if (GameConstants.ALLOW_SUMMON)
			{
				new SummonController();
			}
		}

		private void OnShardShopOfferingReceived()
		{
			this.shardShopPrepared = true;
			this.AttemptComplete();
		}

		private void OnHolonetPrepared()
		{
			this.holonetPrepared = true;
			this.AttemptComplete();
		}

		private void AttemptComplete()
		{
			if (!this.shardShopPrepared)
			{
				return;
			}
			if (!this.holonetPrepared)
			{
				return;
			}
			base.Complete();
		}
	}
}
