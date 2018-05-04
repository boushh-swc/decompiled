using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Objectives;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Controllers.Objectives
{
	public class ObjectiveController : IViewClockTimeObserver, IEventObserver
	{
		public delegate void OnUpdate(int timeRemaining);

		private const string OBJECTIVE_COLLECTED = "OBJECTIVE_COLLECTED";

		private const string OBJECTIVE_ACTIVE_UNLOCKED = "OBJECTIVE_ACTIVE_UNLOCKED";

		private const string OBJECTIVE_GRACE_UNLOCKED = "OBJECTIVE_GRACE_UNLOCKED";

		private const string OBJECTIVE_PROGRESS = "OBJECTIVE_PROGRESS";

		private const string OBJECTIVE_PROGRESS_EXPIRED = "OBJECTIVE_EXPIRED";

		private const string FRACTION = "FRACTION";

		private const string EXPIRES_IN = "expires_in";

		private const string GRACE_PERIOD = "grace_period";

		private const string RED_X_SPRITE_NAME = "icoCancelRed";

		private const string GREEN_CHECK_SPRITE_NAME = "IcoCheck";

		private const string BI_LOG_DETAILS = "details";

		private const string BI_LOG_PLAY = "play";

		private const float FLY_BATTLE_PROGRESS_DELAY = 1.5f;

		private const float FLY_BATTLE_PROGRESS_DURATION = 0.5f;

		private const float OBJECTIVE_COLLECTED_DIM = 0.6f;

		private const float ICON_ALPHA_FULL = 1f;

		private const float ICON_ALPHA_DIM = 0.5f;

		public static readonly Color TEXT_WHITE_COLOR = Color.white;

		public static readonly Color TEXT_GREY_COLOR = new Color(0.549019635f, 0.549019635f, 0.549019635f);

		public static readonly Color FULL_BG_COLOR = new Color(0f, 0.192156866f, 0.2627451f);

		public static readonly Color DIM_BG_COLOR = new Color(0f, 0.113725491f, 0.149019614f);

		public static readonly Color TEXT_GREEN_COLOR = new Color(0.7647059f, 0.9647059f, 0.215686277f);

		public static readonly Color TEXT_RED_COLOR = new Color(0.905882359f, 0f, 0f);

		public static readonly Color TEXT_BLUE_COLOR = new Color(0f, 0.745098054f, 1f);

		public static readonly Color TEXT_RED_DIM_COLOR = new Color(0.5137255f, 0.113725491f, 0.145098045f);

		public static readonly Color TEXT_GREEN_DIM_COLOR = new Color(0.443137258f, 0.5686275f, 0.180392161f);

		public static readonly Color TEXT_YELLOW_COLOR = new Color(0.992156863f, 0.7921569f, 0f);

		private ObjectiveController.OnUpdate onUpdate;

		private ObjectiveManager objectiveManager;

		private CurrentPlayer player;

		private List<ObjectiveProgress> completedObjectiveProgress;

		private bool currentlyClaiming;

		public ObjectiveController()
		{
			Service.ObjectiveController = this;
			this.objectiveManager = Service.ObjectiveManager;
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			this.completedObjectiveProgress = new List<ObjectiveProgress>();
			this.player = Service.CurrentPlayer;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.UpdateObjectiveToastData);
			eventManager.RegisterObserver(this, EventId.ObjectivesUpdated);
			eventManager.RegisterObserver(this, EventId.ClaimObjectiveFailed);
			eventManager.RegisterObserver(this, EventId.ScreenClosing);
			eventManager.RegisterObserver(this, EventId.GameStateChanged);
		}

		public void OnViewClockTime(float dt)
		{
			this.objectiveManager.Update();
		}

		public bool IsGracePeriod(int now, ObjectiveGroup group)
		{
			return now > group.GraceTimestamp && now < group.EndTimestamp;
		}

		public int GetRemainingTime(int now, ObjectiveGroup group)
		{
			if (this.IsGracePeriod(now, group))
			{
				return group.EndTimestamp - now;
			}
			return group.GraceTimestamp - now;
		}

		public void UpdateObjectiveEntry(ObjectiveViewData objectiveData, bool isGrace)
		{
			ObjectiveProgress objective = objectiveData.Objective;
			UXSprite spriteCheckmark = objectiveData.SpriteCheckmark;
			UXSprite spriteSupplyCrate = objectiveData.SpriteSupplyCrate;
			UXSprite spriteObjectiveIcon = objectiveData.SpriteObjectiveIcon;
			UXSprite spritePreview = objectiveData.SpritePreview;
			UXLabel statusLabel = objectiveData.StatusLabel;
			UXLabel titleLabel = objectiveData.TitleLabel;
			UXSlider progressSlider = objectiveData.ProgressSlider;
			UXSprite radialProgress = objectiveData.RadialProgress;
			UXLabel expiredLabel = objectiveData.ExpiredLabel;
			UXSprite spriteObjectiveExpired = objectiveData.SpriteObjectiveExpired;
			UXElement objectiveBgComplete = objectiveData.ObjectiveBgComplete;
			UXElement objectiveBgCollected = objectiveData.ObjectiveBgCollected;
			UXElement objectiveBgExpired = objectiveData.ObjectiveBgExpired;
			UXElement objectiveBgActive = objectiveData.ObjectiveBgActive;
			UXElement objectiveContainer = objectiveData.ObjectiveContainer;
			UXElement objectiveContainerLEI = objectiveData.ObjectiveContainerLEI;
			bool flag = objective.State == ObjectiveState.Complete || (objective.State == ObjectiveState.Active && !isGrace);
			Lang lang = Service.Lang;
			ObjectiveVO vO = objectiveData.Objective.VO;
			string crateRewardUid = vO.CrateRewardUid;
			CrateVO optional = Service.StaticDataController.GetOptional<CrateVO>(crateRewardUid);
			if (optional == null)
			{
				Service.Logger.ErrorFormat("The objective {0} specifies an invalid crateRewardUid {1}", new object[]
				{
					vO.Uid,
					crateRewardUid
				});
				return;
			}
			string value = null;
			if (objectiveContainerLEI != null)
			{
				FactionType faction = Service.CurrentPlayer.Faction;
				if (faction != FactionType.Rebel)
				{
					if (faction == FactionType.Empire)
					{
						value = optional.EmpireLEIUid;
					}
				}
				else
				{
					value = optional.RebelLEIUid;
				}
			}
			if (objectiveBgCollected == null || objectiveContainer == null || objectiveContainerLEI == null || objectiveBgComplete == null || objectiveBgExpired == null)
			{
				Service.Logger.Error("ObjectiveViewData is initialized incompletely");
			}
			else
			{
				ObjectiveState state = objective.State;
				if (state != ObjectiveState.Active)
				{
					if (state != ObjectiveState.Complete)
					{
						if (state == ObjectiveState.Rewarded)
						{
							objectiveBgCollected.Visible = true;
							objectiveBgExpired.Visible = false;
							objectiveContainer.Visible = false;
							objectiveContainerLEI.Visible = false;
							objectiveBgComplete.Visible = false;
							objectiveBgActive.Visible = false;
						}
					}
					else
					{
						objectiveBgComplete.Visible = true;
						objectiveBgExpired.Visible = false;
						objectiveContainer.Visible = false;
						objectiveContainerLEI.Visible = false;
						objectiveBgCollected.Visible = false;
						objectiveBgActive.Visible = false;
					}
				}
				else if (isGrace)
				{
					objectiveBgExpired.Visible = true;
					objectiveBgCollected.Visible = false;
					objectiveContainer.Visible = false;
					objectiveContainerLEI.Visible = false;
					objectiveBgComplete.Visible = false;
					objectiveBgActive.Visible = false;
				}
				else
				{
					objectiveContainerLEI.Visible = !string.IsNullOrEmpty(value);
					objectiveContainer.Visible = string.IsNullOrEmpty(value);
					objectiveBgActive.Visible = true;
					objectiveBgExpired.Visible = false;
					objectiveBgCollected.Visible = false;
					objectiveBgComplete.Visible = false;
				}
			}
			if (progressSlider != null)
			{
				if (objective.State == ObjectiveState.Complete || (objective.State == ObjectiveState.Active && !isGrace))
				{
					progressSlider.Visible = true;
					progressSlider.Value = (float)objective.Count / (float)objective.Target;
				}
				else
				{
					progressSlider.Visible = false;
				}
			}
			if (radialProgress != null)
			{
				if (objective.State == ObjectiveState.Complete || (objective.State == ObjectiveState.Active && !isGrace))
				{
					radialProgress.Visible = true;
					radialProgress.FillAmount = (float)objective.Count / (float)objective.Target;
				}
				else
				{
					radialProgress.Visible = false;
				}
			}
			if (expiredLabel != null)
			{
				expiredLabel.Visible = (isGrace && objective.State == ObjectiveState.Active);
			}
			if (spriteObjectiveExpired != null)
			{
				spriteObjectiveExpired.Visible = (isGrace && objective.State == ObjectiveState.Active);
			}
			if (statusLabel != null)
			{
				ObjectiveState state2 = objective.State;
				if (state2 != ObjectiveState.Active)
				{
					if (state2 != ObjectiveState.Complete)
					{
						if (state2 == ObjectiveState.Rewarded)
						{
							statusLabel.TextColor = ((!flag) ? ObjectiveController.TEXT_GREEN_DIM_COLOR : ObjectiveController.TEXT_GREEN_COLOR);
							statusLabel.Text = lang.Get("OBJECTIVE_COLLECTED", new object[]
							{
								lang.ThousandsSeparated(objective.Count),
								lang.ThousandsSeparated(objective.Target)
							});
						}
					}
					else
					{
						statusLabel.TextColor = ((!flag) ? ObjectiveController.TEXT_GREEN_DIM_COLOR : ObjectiveController.TEXT_GREEN_COLOR);
						statusLabel.Text = lang.Get("OBJECTIVE_ACTIVE_UNLOCKED", new object[]
						{
							lang.ThousandsSeparated(objective.Count),
							lang.ThousandsSeparated(objective.Target)
						});
					}
				}
				else if (isGrace)
				{
					statusLabel.TextColor = ObjectiveController.TEXT_RED_DIM_COLOR;
					statusLabel.Text = lang.Get("OBJECTIVE_EXPIRED", new object[0]);
				}
				else
				{
					statusLabel.TextColor = ((!flag) ? ObjectiveController.TEXT_GREEN_DIM_COLOR : ObjectiveController.TEXT_GREEN_COLOR);
					statusLabel.Text = lang.Get("OBJECTIVE_PROGRESS", new object[]
					{
						lang.ThousandsSeparated(objective.Count),
						lang.ThousandsSeparated(objective.Target)
					});
				}
			}
			if (titleLabel != null)
			{
				titleLabel.Text = lang.Get(objective.VO.ObjString, new object[]
				{
					lang.ThousandsSeparated(objective.Target)
				});
				titleLabel.TextColor = ((!flag) ? ObjectiveController.TEXT_GREY_COLOR : ObjectiveController.TEXT_WHITE_COLOR);
			}
			UXSprite uXSprite = null;
			UXSprite uXSprite2 = null;
			if (spriteSupplyCrate != null)
			{
				spriteSupplyCrate.Tag = objectiveData;
				uXSprite2 = spriteSupplyCrate;
				ObjectiveState state3 = objective.State;
				if (state3 != ObjectiveState.Active)
				{
					if (state3 != ObjectiveState.Complete)
					{
						if (state3 == ObjectiveState.Rewarded)
						{
							spriteCheckmark.Visible = true;
							spriteCheckmark.SpriteName = "IcoCheck";
							spriteSupplyCrate.Alpha = 0.6f;
							spriteSupplyCrate.Visible = true;
						}
					}
					else
					{
						spriteCheckmark.Visible = false;
						spriteSupplyCrate.Alpha = 1f;
						spriteSupplyCrate.Visible = true;
					}
				}
				else
				{
					spriteCheckmark.Visible = false;
					spriteSupplyCrate.Visible = !isGrace;
				}
			}
			if (spritePreview != null)
			{
				spritePreview.Tag = objectiveData;
				ObjectiveState state4 = objective.State;
				if (state4 != ObjectiveState.Active)
				{
					if (state4 != ObjectiveState.Complete)
					{
						if (state4 == ObjectiveState.Rewarded)
						{
							uXSprite2 = spritePreview;
							uXSprite = null;
							spriteCheckmark.Visible = true;
							spriteCheckmark.SpriteName = "IcoCheck";
						}
					}
					else
					{
						uXSprite2 = spritePreview;
						uXSprite = null;
						spriteCheckmark.Visible = false;
					}
				}
				else
				{
					uXSprite = spritePreview;
					uXSprite2 = null;
					spriteCheckmark.Visible = isGrace;
					spriteCheckmark.SpriteName = "icoCancelRed";
				}
			}
			if (spriteObjectiveIcon != null)
			{
				uXSprite = spriteObjectiveIcon;
			}
			if (objectiveData.GeoControlCrate != null)
			{
				objectiveData.GeoControlCrate.Destroy();
			}
			if (objectiveData.GeoControlIcon != null)
			{
				objectiveData.GeoControlIcon.Destroy();
			}
			if (uXSprite2 != null)
			{
				ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(optional, uXSprite2);
				projectorConfig.AnimState = this.GetAnimStateFromObjectiveState(objectiveData.Objective.State);
				projectorConfig.AnimPreference = AnimationPreference.AnimationAlways;
				projectorConfig.Outline = (objective.State == ObjectiveState.Complete);
				projectorConfig.OutlineInner = GameConstants.CRATE_OUTLINE_INNER;
				projectorConfig.OutlineOuter = GameConstants.CRATE_OUTLINE_OUTER;
				objectiveData.GeoControlCrate = ProjectorUtils.GenerateProjector(projectorConfig);
				uXSprite2.Alpha = ((!flag) ? 0.5f : 1f);
			}
			if (uXSprite != null)
			{
				IGeometryVO iconVOFromObjective = GameUtils.GetIconVOFromObjective(objectiveData.Objective.VO, objectiveData.Objective.HQ);
				ProjectorConfig projectorConfig2 = ProjectorUtils.GenerateGeometryConfig(iconVOFromObjective, uXSprite);
				projectorConfig2.AnimPreference = AnimationPreference.AnimationPreferred;
				objectiveData.GeoControlIcon = ProjectorUtils.GenerateProjector(projectorConfig2);
				uXSprite.Alpha = ((!flag) ? 0.5f : 1f);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.ObjectivesUpdated:
			{
				this.DisplayCombatProgress(cookie as ObjectiveProgress);
				PlanetDetailsScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<PlanetDetailsScreen>();
				if (highestLevelScreen != null)
				{
					highestLevelScreen.objectivesView.RefreshScreenForPlanetChange();
					highestLevelScreen.largeObjectivesView.RefreshScreenForPlanetChange();
				}
				return EatResponse.NotEaten;
			}
			case EventId.UpdateObjectiveToastData:
				this.completedObjectiveProgress.Add(cookie as ObjectiveProgress);
				Service.EventManager.SendEvent(EventId.ShowObjectiveToast, null);
				return EatResponse.NotEaten;
			case EventId.ShowObjectiveToast:
			{
				IL_1C:
				if (id == EventId.GameStateChanged)
				{
					IState currentState = Service.GameStateMachine.CurrentState;
					if (currentState is BattleStartState || currentState is BattlePlayState || currentState is BattleEndState || currentState is BattlePlaybackState || currentState is BattleEndPlaybackState)
					{
						Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
					}
					else
					{
						Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
					}
					return EatResponse.NotEaten;
				}
				if (id != EventId.ScreenClosing)
				{
					return EatResponse.NotEaten;
				}
				ScreenBase screenBase = cookie as ScreenBase;
				if (screenBase is InventoryCrateCollectionScreen)
				{
					this.currentlyClaiming = false;
				}
				return EatResponse.NotEaten;
			}
			case EventId.ClaimObjectiveFailed:
				this.currentlyClaiming = false;
				return EatResponse.NotEaten;
			}
			goto IL_1C;
		}

		public ObjectiveProgress GetNextCompletedObjective()
		{
			if (this.completedObjectiveProgress != null && this.completedObjectiveProgress.Count > 0)
			{
				ObjectiveProgress result = this.completedObjectiveProgress[0];
				this.completedObjectiveProgress.RemoveAt(0);
				return result;
			}
			return null;
		}

		private AnimState GetAnimStateFromObjectiveState(ObjectiveState state)
		{
			if (state == ObjectiveState.Active)
			{
				return AnimState.Closed;
			}
			if (state == ObjectiveState.Complete)
			{
				return AnimState.Unlocked;
			}
			if (state != ObjectiveState.Rewarded)
			{
				return AnimState.Closed;
			}
			return AnimState.Idle;
		}

		public void addTestDataForToast(int count)
		{
			for (int i = 0; i < count; i++)
			{
				this.completedObjectiveProgress.Add(new ObjectiveProgress("planet1"));
			}
			Service.EventManager.SendEvent(EventId.ShowObjectiveToast, null);
		}

		public bool ShouldShowObjectives()
		{
			int num = this.player.Map.FindHighestHqLevel();
			return num >= GameConstants.OBJECTIVES_UNLOCKED;
		}

		public void GetTimeData(Lang lang, ObjectiveGroup group, ref bool isGrace, ref string timeRemainingString)
		{
			int serverTime = (int)Service.ServerAPI.ServerTime;
			int totalSeconds = Math.Max(0, this.GetRemainingTime(serverTime, group));
			isGrace = this.IsGracePeriod(serverTime, group);
			timeRemainingString = ((!isGrace) ? lang.Get("expires_in", new object[]
			{
				GameUtils.GetTimeLabelFromSeconds(totalSeconds)
			}) : lang.Get("grace_period", new object[]
			{
				GameUtils.GetTimeLabelFromSeconds(totalSeconds)
			}));
		}

		public void OnCrateClickedFromDetail(UXButton button)
		{
			ObjectiveViewData objectiveViewData = button.Tag as ObjectiveViewData;
			ObjectiveProgress objective = objectiveViewData.Objective;
			if (objective.State == ObjectiveState.Rewarded)
			{
				return;
			}
			this.HandleCrateClicked(objective, button);
			if (objective.State == ObjectiveState.Active)
			{
				string bILoggingMessageForCrates = this.GetBILoggingMessageForCrates(objective, "details");
				Service.EventManager.SendEvent(EventId.ObjectiveLockedCrateClicked, bILoggingMessageForCrates);
			}
		}

		public void HandleCrateClicked(ObjectiveProgress progress, UXButton btn)
		{
			if (this.currentlyClaiming)
			{
				return;
			}
			if (progress.State == ObjectiveState.Active)
			{
				ObjectiveVO vO = progress.VO;
				if (vO == null)
				{
					Service.Logger.ErrorFormat("ObjectiveVO does not exist, objectivData:", new object[]
					{
						progress.ObjectiveUid
					});
					return;
				}
				string crateRewardUid = vO.CrateRewardUid;
				CrateInfoModalScreen crateInfoModalScreen = CrateInfoModalScreen.CreateForObjectiveProgressInfo(crateRewardUid, progress);
				crateInfoModalScreen.IsAlwaysOnTop = true;
				Service.ScreenController.AddScreen(crateInfoModalScreen, true, false);
			}
			else if (progress.State == ObjectiveState.Complete && !progress.ClaimAttempt)
			{
				this.currentlyClaiming = true;
				this.objectiveManager.Claim(progress);
			}
		}

		private string GetBILoggingMessageForCrates(ObjectiveProgress progress, string location)
		{
			ObjectiveVO objectiveVO = Service.StaticDataController.Get<ObjectiveVO>(progress.ObjectiveUid);
			string value = string.Empty;
			PlanetDetailsScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<PlanetDetailsScreen>();
			string value2 = string.Empty;
			if (highestLevelScreen != null)
			{
				ObjectiveGroup objectiveGroup = this.player.Objectives[highestLevelScreen.viewingPlanetVO.Uid];
				value = objectiveGroup.GroupId;
				value2 = highestLevelScreen.viewingPlanetVO.PlanetBIName;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value).Append("|");
			stringBuilder.Append(objectiveVO.Uid).Append("|");
			stringBuilder.Append(objectiveVO.CrateRewardUid).Append("|");
			stringBuilder.Append(location).Append("|");
			stringBuilder.Append(value2);
			return stringBuilder.ToString();
		}

		private void DisplayCombatProgress(ObjectiveProgress progress)
		{
			if (progress == null)
			{
				return;
			}
			if (this.objectiveManager.GetGoalType(progress.VO) != GoalType.Loot)
			{
				Lang lang = Service.Lang;
				string text = lang.Get("FRACTION", new object[]
				{
					progress.Count,
					progress.Target
				});
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(progress.VO.Uid, lang.Get(progress.VO.ObjString, new object[]
				{
					text
				}), 1.5f, 0.5f);
			}
		}
	}
}
