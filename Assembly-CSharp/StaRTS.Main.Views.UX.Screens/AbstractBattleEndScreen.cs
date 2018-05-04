using StaRTS.Assets;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public abstract class AbstractBattleEndScreen : ScreenBase
	{
		private const string BUTTON_REPLAY_BATTLE = "ButtonReplayBattle";

		private const string PRIMARY_ACTION_BUTTON_LABEL = "LabelPrimaryAction";

		public const string PRIMARY_ACTION_BUTTON = "ButtonPrimaryAction";

		private const string STAR_ANIM_DEFAULT = "StarAnim{0}";

		private const string STAR_ANIM_EMPIRE = "StarAnim{0}_Empire";

		private const string STAR_ANIM_REBEL = "StarAnim{0}_Rebel";

		protected const float STAR_ANIM_DELAY = 1f;

		protected const float STAR_ANIM_DURATION = 0.35f;

		protected const string ANIM_TRIGGER = "Show";

		protected const int MAX_STARS = 3;

		protected const int STAR_DAMAGE = 50;

		protected const string LANG_CONTINUE = "CONTINUE";

		protected const string LANG_EXPENDED = "EXPENDED";

		protected const string LANG_PERCENTAGE = "PERCENTAGE";

		protected UXButton primaryActionButton;

		protected UXButton replayBattleButton;

		protected UXGrid troopGrid;

		protected List<uint> viewTimers;

		protected List<Animator> viewAnimators;

		protected bool isNonAttackerReplayView;

		protected bool isReplay;

		private FactionType faction;

		private CameraShake cameraShake;

		private bool setup;

		private bool destroyed;

		private TroopTooltipHelper troopTooltipHelper;

		private AssetHandle starsHandle;

		private GameObject starsObject;

		protected int numAssetsPendingLoad;

		protected override bool AllowGarbageCollection
		{
			get
			{
				return false;
			}
		}

		protected override bool WantTransitions
		{
			get
			{
				return !base.IsClosing;
			}
		}

		protected abstract string StarsPlaceHolderName
		{
			get;
		}

		protected abstract string ReplayPrefix
		{
			get;
		}

		protected abstract string TroopCardName
		{
			get;
		}

		protected abstract string TroopCardDefaultName
		{
			get;
		}

		protected abstract string TroopCardQualityName
		{
			get;
		}

		protected abstract string TroopCardIconName
		{
			get;
		}

		protected abstract string TroopCardAmountName
		{
			get;
		}

		protected abstract string TroopCardLevelName
		{
			get;
		}

		protected abstract string TroopHeroDecalName
		{
			get;
		}

		public AbstractBattleEndScreen(string assetName, bool isReplay) : base(assetName)
		{
			this.isReplay = isReplay;
			this.isNonAttackerReplayView = (isReplay && Service.CurrentPlayer.PlayerId != Service.BattleController.GetCurrentBattle().Attacker.PlayerId);
			this.setup = false;
			this.destroyed = false;
			this.viewTimers = new List<uint>();
			this.viewAnimators = new List<Animator>();
			this.faction = Service.CurrentPlayer.Faction;
			this.cameraShake = new CameraShake(null);
			this.troopTooltipHelper = new TroopTooltipHelper();
			this.numAssetsPendingLoad++;
			this.starsHandle = AssetHandle.Invalid;
			Service.AssetManager.Load(ref this.starsHandle, "gui_battle_stars", new AssetSuccessDelegate(this.OnStarsLoadSuccess), new AssetFailureDelegate(this.OnAssetLoadFail), null);
		}

		public override void OnDestroyElement()
		{
			int i = 0;
			int count = this.viewAnimators.Count;
			while (i < count)
			{
				Animator animator = this.viewAnimators[i];
				animator.enabled = false;
				i++;
			}
			this.viewAnimators.Clear();
			int j = 0;
			int count2 = this.viewTimers.Count;
			while (j < count2)
			{
				Service.ViewTimerManager.KillViewTimer(this.viewTimers[j]);
				j++;
			}
			this.viewTimers.Clear();
			this.troopTooltipHelper.Destroy();
			this.troopTooltipHelper = null;
			if (this.starsHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.starsHandle);
				this.starsHandle = AssetHandle.Invalid;
			}
			if (this.starsObject != null)
			{
				UnityEngine.Object.Destroy(this.starsObject);
				this.starsObject = null;
			}
			if (this.troopGrid != null)
			{
				this.troopGrid.Clear();
				this.troopGrid = null;
			}
			this.numAssetsPendingLoad = 0;
			this.primaryActionButton.Enabled = true;
			this.setup = false;
			this.destroyed = true;
			this.cameraShake = null;
			base.OnDestroyElement();
		}

		protected override void OnScreenLoaded()
		{
			this.TryInit();
		}

		public override void SetupRootCollider()
		{
		}

		public override void RefreshView()
		{
			this.TryInit();
		}

		protected abstract void InitElements();

		protected abstract void SetupView();

		private void TryInit()
		{
			if (!base.IsLoaded() || this.destroyed || this.numAssetsPendingLoad > 0)
			{
				if (this.root != null && this.root.activeSelf)
				{
					this.root.SetActive(false);
				}
				return;
			}
			if (!this.setup)
			{
				this.setup = true;
				this.Init();
			}
		}

		private void Init()
		{
			this.root.SetActive(true);
			this.InitElements();
			this.InitButtons();
			this.InitLabels();
			this.SetupView();
			base.SetupRootCollider();
		}

		private void OnStarsLoadSuccess(object asset, object cookie)
		{
			this.OnAssetLoadSuccess(asset, this.StarsPlaceHolderName, null, ref this.starsObject);
		}

		protected void OnAssetLoadSuccess(object asset, string placeholderName, string overrideGameObjectName, ref GameObject gameObject)
		{
			gameObject = Service.AssetManager.CloneGameObject(asset as GameObject);
			if (overrideGameObjectName != null)
			{
				gameObject.name = overrideGameObjectName;
			}
			Transform transform = gameObject.transform;
			transform.parent = base.GetElement<UXElement>(placeholderName).Root.transform;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			base.CreateElements(gameObject);
			this.numAssetsPendingLoad--;
			this.TryInit();
		}

		protected void OnAssetLoadFail(object cookie)
		{
			this.numAssetsPendingLoad--;
			this.TryInit();
		}

		protected void InitStars()
		{
			bool flag = this.faction == FactionType.Empire;
			bool flag2 = this.faction == FactionType.Rebel;
			bool visible = !flag && !flag2;
			string format = (!this.isNonAttackerReplayView) ? "StarAnim{0}" : (this.ReplayPrefix + "StarAnim{0}");
			string format2 = (!this.isNonAttackerReplayView) ? "StarAnim{0}_Empire" : (this.ReplayPrefix + "StarAnim{0}_Empire");
			string format3 = (!this.isNonAttackerReplayView) ? "StarAnim{0}_Rebel" : (this.ReplayPrefix + "StarAnim{0}_Rebel");
			for (int i = 1; i <= 3; i++)
			{
				base.GetElement<UXElement>(string.Format(format, i)).Visible = visible;
				base.GetElement<UXElement>(string.Format(format2, i)).Visible = flag;
				base.GetElement<UXElement>(string.Format(format3, i)).Visible = flag2;
			}
		}

		protected virtual void InitButtons()
		{
			this.primaryActionButton = base.GetElement<UXButton>("ButtonPrimaryAction");
			this.primaryActionButton.OnClicked = new UXButtonClickedDelegate(this.OnPrimaryActionButtonClicked);
			base.CurrentBackButton = this.primaryActionButton;
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnPrimaryActionButtonClicked);
			base.AllowFUEBackButton = true;
			this.replayBattleButton = base.GetElement<UXButton>("ButtonReplayBattle");
			this.replayBattleButton.OnClicked = new UXButtonClickedDelegate(this.OnReplayBattleButtonClicked);
		}

		protected virtual void InitLabels()
		{
			base.GetElement<UXLabel>("LabelPrimaryAction").Text = this.lang.Get("CONTINUE", new object[0]);
		}

		protected void InitTroopGrid(string troopGridName, string troopTemplateName, BattleEntry battleEntry)
		{
			this.troopGrid = base.GetElement<UXGrid>(troopGridName);
			this.troopGrid.SetTemplateItem(troopTemplateName);
			if (this.isNonAttackerReplayView)
			{
				battleEntry.SetupExpendedTroops();
			}
			BattleDeploymentData attackerDeployedData = battleEntry.AttackerDeployedData;
			Dictionary<string, int> dictionary = null;
			if (attackerDeployedData.SquadData != null)
			{
				dictionary = new Dictionary<string, int>(attackerDeployedData.SquadData);
			}
			this.PopulateExpendedTroops(attackerDeployedData.TroopData, dictionary, false, battleEntry);
			this.PopulateExpendedTroops(dictionary, null, false, battleEntry);
			this.PopulateExpendedTroops(attackerDeployedData.SpecialAttackData, null, true, battleEntry);
			this.PopulateExpendedTroops(attackerDeployedData.HeroData, null, false, battleEntry);
			this.PopulateExpendedTroops(attackerDeployedData.ChampionData, null, false, battleEntry);
			this.troopGrid.RepositionItems();
			this.troopGrid.Scroll(0.5f);
		}

		private void PopulateExpendedTroops(Dictionary<string, int> troops, Dictionary<string, int> squadTroops, bool isSpecialAttack, BattleEntry battle)
		{
			if (troops == null)
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (KeyValuePair<string, int> current in troops)
			{
				if (current.Value >= 1)
				{
					int num = 0;
					if (squadTroops != null && squadTroops.ContainsKey(current.Key))
					{
						num = squadTroops[current.Key];
						squadTroops.Remove(current.Key);
					}
					IDeployableVO deployableVO = (!isSpecialAttack) ? staticDataController.Get<TroopTypeVO>(current.Key) : staticDataController.Get<SpecialAttackTypeVO>(current.Key);
					UXElement item = this.CreateDeployableUXElement(this.troopGrid, current.Key, deployableVO.AssetName, current.Value + num, deployableVO, battle);
					this.troopGrid.AddItem(item, deployableVO.Order);
				}
			}
		}

		private UXElement CreateDeployableUXElement(UXGrid grid, string uid, string assetName, int amount, IDeployableVO troop, BattleEntry battle)
		{
			UXElement result = grid.CloneTemplateItem(uid);
			UXSprite subElement = grid.GetSubElement<UXSprite>(uid, this.TroopCardIconName);
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(troop, subElement);
			Service.EventManager.SendEvent(EventId.ButtonCreated, new GeometryTag(troop, projectorConfig, battle));
			projectorConfig.AnimPreference = AnimationPreference.NoAnimation;
			ProjectorUtils.GenerateProjector(projectorConfig);
			UXLabel subElement2 = grid.GetSubElement<UXLabel>(uid, this.TroopCardAmountName);
			subElement2.Text = LangUtils.GetMultiplierText(amount);
			UXLabel subElement3 = grid.GetSubElement<UXLabel>(uid, this.TroopCardLevelName);
			subElement3.Text = LangUtils.GetLevelText(troop.Lvl);
			FactionDecal.UpdateDeployableDecal(uid, grid, troop, this.TroopHeroDecalName);
			string text = null;
			if (troop is TroopTypeVO)
			{
				TroopTypeVO troop2 = troop as TroopTypeVO;
				Service.SkinController.GetApplicableSkin(troop2, battle.AttackerEquipment, out text);
			}
			int qualityInt;
			if (!string.IsNullOrEmpty(text))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				EquipmentVO equipmentVO = staticDataController.Get<EquipmentVO>(text);
				qualityInt = (int)equipmentVO.Quality;
			}
			else
			{
				qualityInt = Service.DeployableShardUnlockController.GetUpgradeQualityForDeployable(troop);
			}
			UXUtils.SetCardQuality(this, this.troopGrid, uid, qualityInt, this.TroopCardQualityName, this.TroopCardDefaultName);
			UXButton subElement4 = grid.GetSubElement<UXButton>(uid, this.TroopCardName);
			this.troopTooltipHelper.RegisterButtonTooltip(subElement4, troop, battle);
			return result;
		}

		protected void AnimateStars(int stars)
		{
			for (int i = 1; i <= stars; i++)
			{
				FactionType factionType = this.faction;
				string format;
				if (factionType != FactionType.Empire)
				{
					if (factionType != FactionType.Rebel)
					{
						format = ((!this.isNonAttackerReplayView) ? "StarAnim{0}" : (this.ReplayPrefix + "StarAnim{0}"));
					}
					else
					{
						format = ((!this.isNonAttackerReplayView) ? "StarAnim{0}_Rebel" : (this.ReplayPrefix + "StarAnim{0}_Rebel"));
					}
				}
				else
				{
					format = ((!this.isNonAttackerReplayView) ? "StarAnim{0}_Empire" : (this.ReplayPrefix + "StarAnim{0}_Empire"));
				}
				UXElement element = base.GetElement<UXElement>(string.Format(format, i));
				Animator component = element.Root.GetComponent<Animator>();
				if (component == null)
				{
					Service.Logger.WarnFormat("Unable to play end star anim #{0}", new object[]
					{
						i
					});
				}
				else
				{
					this.viewAnimators.Add(component);
					KeyValuePair<int, Animator> keyValuePair = new KeyValuePair<int, Animator>(i, component);
					uint item = Service.ViewTimerManager.CreateViewTimer((float)(i - 1) * 1f, false, new TimerDelegate(this.OnAnimateStarTimer), keyValuePair);
					this.viewTimers.Add(item);
				}
			}
			uint item2 = Service.ViewTimerManager.CreateViewTimer((float)(stars - 1) * 1f + 0.35f, false, new TimerDelegate(this.OnAllStarsComplete), null);
			this.viewTimers.Add(item2);
		}

		private void OnAnimateStarTimer(uint id, object cookie)
		{
			KeyValuePair<int, Animator> keyValuePair = (KeyValuePair<int, Animator>)cookie;
			Animator value = keyValuePair.Value;
			value.enabled = true;
			value.SetTrigger("Show");
			uint item = Service.ViewTimerManager.CreateViewTimer(0.35f, false, new TimerDelegate(this.OnStarAnimationComplete), null);
			this.viewTimers.Add(item);
			int key = keyValuePair.Key;
			Service.EventManager.SendEvent(EventId.BattleEndVictoryStarDisplayed, key);
		}

		private void OnStarAnimationComplete(uint id, object cookie)
		{
			if (this.cameraShake != null)
			{
				this.cameraShake.Shake(0.5f, 0.75f);
			}
		}

		protected virtual void OnAllStarsComplete(uint id, object cookie)
		{
		}

		private void OnPrimaryActionButtonClicked(UXButton button)
		{
			button.Enabled = false;
			this.replayBattleButton.Enabled = false;
			BattleType type = Service.BattleController.GetCurrentBattle().Type;
			if (type == BattleType.PveDefend)
			{
				HomeState.GoToHomeStateAndReloadMap();
				this.Close(null);
			}
			else
			{
				HomeState.GoToHomeState(null, false);
			}
			Service.BattlePlaybackController.DiscardLastReplay();
		}

		private void OnReplayBattleButtonClicked(UXButton button)
		{
			button.Enabled = false;
			this.primaryActionButton.Enabled = false;
			Service.EventManager.SendEvent(EventId.BattleReplayRequested, null);
		}
	}
}
