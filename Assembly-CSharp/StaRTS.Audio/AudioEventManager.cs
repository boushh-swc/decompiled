using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.World;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Audio
{
	public class AudioEventManager : IEventObserver
	{
		private const float FIXED_MUSIC_LOOP_DELAY = 7f;

		private AudioManager audioManager;

		private EventManager eventManager;

		private uint timerId;

		private List<StrIntPair> droidClips;

		private float galaxyUIPlanetFocusThrottle;

		public AudioEventManager(AudioManager audioManager)
		{
			this.audioManager = audioManager;
			this.eventManager = Service.EventManager;
			this.droidClips = new List<StrIntPair>();
			this.droidClips.Add(new StrIntPair("sfx_ui_droid_1", 25));
			this.droidClips.Add(new StrIntPair("sfx_ui_droid_2", 25));
			this.droidClips.Add(new StrIntPair("sfx_ui_droid_3", 25));
			this.droidClips.Add(new StrIntPair("sfx_ui_droid_4", 25));
			this.eventManager.RegisterObserver(this, EventId.BattleLogScreenTabSelected);
			this.eventManager.RegisterObserver(this, EventId.BattleLogScreenRevengeButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.BattleLogScreenReplayButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.BattleLogScreenShareButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.ShooterWarmingUp);
			this.eventManager.RegisterObserver(this, EventId.EntityAttackedTarget);
			this.eventManager.RegisterObserver(this, EventId.ProjectileViewImpacted);
			this.eventManager.RegisterObserver(this, EventId.EntityKilled);
			this.eventManager.RegisterObserver(this, EventId.TroopPlacedOnBoard);
			this.eventManager.RegisterObserver(this, EventId.ButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.ContextButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.GameStateChanged);
			this.eventManager.RegisterObserver(this, EventId.MusicUnmuted);
			this.eventManager.RegisterObserver(this, EventId.TroopNotPlacedInvalidArea);
			this.eventManager.RegisterObserver(this, EventId.TroopNotPlacedInvalidTroop);
			this.eventManager.RegisterObserver(this, EventId.HeroNotActivated);
			this.eventManager.RegisterObserver(this, EventId.TroopAbilityActivate);
			this.eventManager.RegisterObserver(this, EventId.ProjectileViewDeflected);
			this.eventManager.RegisterObserver(this, EventId.ScreenClosing);
			this.eventManager.RegisterObserver(this, EventId.ScreenOverlayClosing);
			this.eventManager.RegisterObserver(this, EventId.ScreenLoaded);
			this.eventManager.RegisterObserver(this, EventId.UserLiftedBuildingAudio);
			this.eventManager.RegisterObserver(this, EventId.UserGridMovedBuildingAudio);
			this.eventManager.RegisterObserver(this, EventId.UserLoweredBuildingAudio);
			this.eventManager.RegisterObserver(this, EventId.BuildingPurchaseCanceled);
			this.eventManager.RegisterObserver(this, EventId.BuildingPurchaseConfirmed);
			this.eventManager.RegisterObserver(this, EventId.BuildingSelectedFromStore);
			this.eventManager.RegisterObserver(this, EventId.BuildingSelected);
			this.eventManager.RegisterObserver(this, EventId.StoreCategorySelected);
			this.eventManager.RegisterObserver(this, EventId.BackButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.HoloEvent);
			this.eventManager.RegisterObserver(this, EventId.PlayHoloGreet);
			this.eventManager.RegisterObserver(this, EventId.StoryTranscriptDisplayed);
			this.eventManager.RegisterObserver(this, EventId.SpecialAttackDeployed);
			this.eventManager.RegisterObserver(this, EventId.SpecialAttackDropshipFlyingAway);
			this.eventManager.RegisterObserver(this, EventId.SpecialAttackFired);
			this.eventManager.RegisterObserver(this, EventId.StoryAttackButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.StoryNextButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.StorySkipButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.HolonetChangeTabs);
			this.eventManager.RegisterObserver(this, EventId.HolonetNextPrevTransmision);
			this.eventManager.RegisterObserver(this, EventId.StarEarned);
			this.eventManager.RegisterObserver(this, EventId.BattleEndVictoryStarDisplayed);
			this.eventManager.RegisterObserver(this, EventId.IntroStarted);
			this.eventManager.RegisterObserver(this, EventId.CurrencyCollected);
			this.eventManager.RegisterObserver(this, EventId.AudibleCurrencySpent);
			this.eventManager.RegisterObserver(this, EventId.BuildingLevelUpgraded);
			this.eventManager.RegisterObserver(this, EventId.StarshipLevelUpgraded);
			this.eventManager.RegisterObserver(this, EventId.TroopLevelUpgraded);
			this.eventManager.RegisterObserver(this, EventId.EquipmentUpgraded);
			this.eventManager.RegisterObserver(this, EventId.TroopRecruited);
			this.eventManager.RegisterObserver(this, EventId.StarshipMobilized);
			this.eventManager.RegisterObserver(this, EventId.StarshipMobilizedFromPrize);
			this.eventManager.RegisterObserver(this, EventId.HeroMobilized);
			this.eventManager.RegisterObserver(this, EventId.HeroMobilizedFromPrize);
			this.eventManager.RegisterObserver(this, EventId.TransportArrived);
			this.eventManager.RegisterObserver(this, EventId.TransportDeparted);
			this.eventManager.RegisterObserver(this, EventId.InitiatedBuyout);
			this.eventManager.RegisterObserver(this, EventId.ContractAdded);
			this.eventManager.RegisterObserver(this, EventId.BuildingConstructed);
			this.eventManager.RegisterObserver(this, EventId.ShuttleAnimStateChanged);
			this.eventManager.RegisterObserver(this, EventId.ShieldBorderDestroyed);
			this.eventManager.RegisterObserver(this, EventId.ShieldStarted);
			this.eventManager.RegisterObserver(this, EventId.ChampionShieldActivated);
			this.eventManager.RegisterObserver(this, EventId.ChampionShieldDeactivated);
			this.eventManager.RegisterObserver(this, EventId.ChampionShieldDestroyed);
			this.eventManager.RegisterObserver(this, EventId.SquadEdited);
			this.eventManager.RegisterObserver(this, EventId.SquadSelect);
			this.eventManager.RegisterObserver(this, EventId.SquadSend);
			this.eventManager.RegisterObserver(this, EventId.SquadNext);
			this.eventManager.RegisterObserver(this, EventId.SquadMore);
			this.eventManager.RegisterObserver(this, EventId.SquadFB);
			this.eventManager.RegisterObserver(this, EventId.SquadCredits);
			this.eventManager.RegisterObserver(this, EventId.InfoButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.MissionActionButtonClicked);
			this.eventManager.RegisterObserver(this, EventId.InventoryResourceUpdated);
			this.eventManager.RegisterObserver(this, EventId.LongPressStarted);
			this.eventManager.RegisterObserver(this, EventId.ApplicationPauseToggled);
			this.eventManager.RegisterObserver(this, EventId.DeviceMusicPlayerStateChanged);
			this.eventManager.RegisterObserver(this, EventId.WorldInTransitionComplete);
			this.eventManager.RegisterObserver(this, EventId.MuteEvent);
			this.eventManager.RegisterObserver(this, EventId.UnmuteEvent);
			this.eventManager.RegisterObserver(this, EventId.SimulateAudioEvent);
			this.eventManager.RegisterObserver(this, EventId.HQCelebrationPlayed);
			this.eventManager.RegisterObserver(this, EventId.TrapTriggered);
			this.eventManager.RegisterObserver(this, EventId.TrapDestroyed);
			this.eventManager.RegisterObserver(this, EventId.DroidPurchaseCancelled);
			this.eventManager.RegisterObserver(this, EventId.TroopLoadingIntoStarport);
			this.eventManager.RegisterObserver(this, EventId.TroopPlacedInsideShieldError);
			this.eventManager.RegisterObserver(this, EventId.TroopPlacedInsideBuildingError);
			this.eventManager.RegisterObserver(this, EventId.TextCrawlStarted);
			this.eventManager.RegisterObserver(this, EventId.TextCrawlComplete);
			this.eventManager.RegisterObserver(this, EventId.PlanetRelocateStarted);
			this.eventManager.RegisterObserver(this, EventId.GalaxyGoToGalaxyView);
			this.eventManager.RegisterObserver(this, EventId.GalaxyGoToPlanetView);
			this.eventManager.RegisterObserver(this, EventId.GalaxyPlanetTapped);
			this.eventManager.RegisterObserver(this, EventId.PlanetConfirmRelocate);
			this.eventManager.RegisterObserver(this, EventId.GalaxyTransitionToNextPlanet);
			this.eventManager.RegisterObserver(this, EventId.GalaxyTransitionToPreviousPlanet);
			this.eventManager.RegisterObserver(this, EventId.GalaxyNotEnoughRelocateStarsClose);
			this.eventManager.RegisterObserver(this, EventId.GalaxyUIPlanetFocus);
			this.eventManager.RegisterObserver(this, EventId.ObjectiveDetailsClicked);
			this.eventManager.RegisterObserver(this, EventId.ObjectiveCrateInfoScreenOpened);
			this.eventManager.RegisterObserver(this, EventId.ObjectiveCompleted);
			this.eventManager.RegisterObserver(this, EventId.ObjectiveRewardDataCardRevealed);
			this.eventManager.RegisterObserver(this, EventId.UIFilterSelected);
			this.eventManager.RegisterObserver(this, EventId.PerkSelected);
			this.eventManager.RegisterObserver(this, EventId.TroopDonationTrackRewardReceived);
			this.eventManager.RegisterObserver(this, EventId.SquadLeveledUpCelebration);
			this.eventManager.RegisterObserver(this, EventId.PerkActivated);
			this.eventManager.RegisterObserver(this, EventId.PerkInvested);
			this.eventManager.RegisterObserver(this, EventId.PerkCelebStarted);
			this.eventManager.RegisterObserver(this, EventId.DeployableUnlockCelebrationPlayed);
			this.eventManager.RegisterObserver(this, EventId.EquipmentUnlockCelebrationPlayed);
			this.eventManager.RegisterObserver(this, EventId.EquipmentActivated);
			this.eventManager.RegisterObserver(this, EventId.EquipmentDataFragmentEarned);
			this.eventManager.RegisterObserver(this, EventId.SupplyCrateProgressBar);
			this.eventManager.RegisterObserver(this, EventId.InventoryCrateAnimationStateChange);
			this.eventManager.RegisterObserver(this, EventId.InventoryCrateAnimVFXTriggered);
			this.eventManager.RegisterObserver(this, EventId.CrateRewardIdleHop);
		}

		private void PlayInventoryCrateAnimSFXBasedOnState(InventoryCrateAnimationState animState)
		{
			switch (animState)
			{
			case InventoryCrateAnimationState.Landed:
				this.audioManager.PlayAudio("sfx_rewards_crate_land");
				break;
			case InventoryCrateAnimationState.Open:
				this.audioManager.PlayAudio("sfx_rewards_crate_open");
				break;
			case InventoryCrateAnimationState.ShowPBar:
				this.audioManager.PlayAudio("sfx_rewards_progressbar");
				break;
			case InventoryCrateAnimationState.Hop:
				this.audioManager.PlayAudio("sfx_rewards_crate_hop");
				break;
			}
		}

		private void PlayInventoryCrateAnimVfxSfx(string sfxName)
		{
			this.audioManager.PlayAudio(sfxName);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.ProjectileViewImpacted:
			{
				SmartEntity smartEntity = (SmartEntity)cookie;
				this.PlayBattleSound(smartEntity, AudioCollectionType.Impact);
				return EatResponse.NotEaten;
			}
			case EventId.ProjectileViewPathComplete:
			case EventId.DamagePercentUpdated:
			case EventId.StarportMeterUpdated:
			case EventId.SquadLeaderboardUpdated:
			case EventId.HolonetLeaderBoardUpdated:
			case EventId.ClearableCleared:
			case EventId.ClearableStarted:
			case EventId.StartupTasksCompleted:
			case EventId.TroopDeployed:
			case EventId.ChampionStartedRepairing:
			case EventId.ChampionRepaired:
			case EventId.TroopDonationTrackProgressUpdated:
				IL_85:
				switch (id)
				{
				case EventId.SquadEdited:
					this.audioManager.PlayAudio("sfx_button_squad_edit");
					return EatResponse.NotEaten;
				case EventId.SquadSelect:
					this.audioManager.PlayAudio("sfx_button_squad_select");
					return EatResponse.NotEaten;
				case EventId.SquadSend:
					this.audioManager.PlayAudio("sfx_button_squad_chat");
					return EatResponse.NotEaten;
				case EventId.SquadNext:
					this.audioManager.PlayAudio("sfx_button_next");
					return EatResponse.NotEaten;
				case EventId.SquadMore:
					this.audioManager.PlayAudio("sfx_button_more_info");
					return EatResponse.NotEaten;
				case EventId.SquadFB:
					this.audioManager.PlayAudio("sfx_button_facebook");
					return EatResponse.NotEaten;
				case EventId.SquadCredits:
					this.audioManager.PlayAudio("sfx_button_usecredits");
					return EatResponse.NotEaten;
				case EventId.VisitPlayer:
				case EventId.TroopViewReady:
				case EventId.BuildingSwapped:
				case EventId.BuildingReplaced:
				case EventId.SpecialAttackSpawned:
				case EventId.HudComplete:
				case EventId.IntroComplete:
				case EventId.InventoryCapacityChanged:
				case EventId.MapDataProcessingStart:
				case EventId.MapDataProcessingEnd:
				case EventId.WorldLoadComplete:
					IL_FB:
					switch (id)
					{
					case EventId.GalaxyPlanetTapped:
						this.audioManager.PlayAudio("sfx_swipe_planet");
						return EatResponse.NotEaten;
					case EventId.GalaxyPlanetInfoButton:
					case EventId.PlanetScoutingStart:
					case EventId.PlanetUnlocked:
					case EventId.PlanetsLoadingComplete:
					case EventId.FacebookLoggedIn:
					case EventId.SettingsAboutButtonClicked:
					case EventId.SettingsFacebookLoggedIn:
					case EventId.SettingsHelpButtonClicked:
					case EventId.SettingsMusicCheckboxSelected:
					case EventId.SettingsFanForumsButtonClicked:
					case EventId.SettingsSfxCheckboxSelected:
						IL_160:
						switch (id)
						{
						case EventId.BuildingPurchaseCanceled:
							this.audioManager.PlayAudio("sfx_ui_placement_stop");
							return EatResponse.NotEaten;
						case EventId.BuildingPurchaseConfirmed:
							this.audioManager.PlayAudio("sfx_ui_placement_confirm");
							return EatResponse.NotEaten;
						case EventId.BuildingPurchaseSuccess:
						case EventId.BuildingPurchaseModeStarted:
						case EventId.BuildingPurchaseModeEnded:
						case EventId.BuildingStartedUpgrading:
						case EventId.BuildingSelected:
						case EventId.BuildingDeselected:
						case EventId.BuildingQuickStashed:
						case EventId.ShardItemSelectedFromStore:
						case EventId.DroidPurchaseAnimationComplete:
						case EventId.DroidPurchaseCompleted:
							IL_1BA:
							switch (id)
							{
							case EventId.GameStateChanged:
								goto IL_6C8;
							case EventId.GameStateAboutToChange:
							case EventId.ShooterClipUsed:
							case EventId.ShooterStoppedAttacking:
							case EventId.TroopAcquiredFirstTarget:
							case EventId.TroopReachedPathEnd:
							case EventId.UserWantedEditBaseState:
							case EventId.UserLiftedBuilding:
							case EventId.UserMovedLiftedBuilding:
							case EventId.UserLoweredBuilding:
							case EventId.UserStashedBuilding:
								IL_217:
								switch (id)
								{
								case EventId.SimulateAudioEvent:
								{
									AudioEventData audioEventData = (AudioEventData)cookie;
									this.OnEvent(audioEventData.EventId, audioEventData.EventCookie);
									return EatResponse.NotEaten;
								}
								case EventId.MuteEvent:
									this.eventManager.UnregisterObserver(this, (EventId)((int)cookie));
									return EatResponse.NotEaten;
								case EventId.UnmuteEvent:
									this.eventManager.RegisterObserver(this, (EventId)((int)cookie), EventPriority.Default);
									return EatResponse.NotEaten;
								case EventId.MusicUnmuted:
									goto IL_6C8;
								case EventId.IAPProductIDsReady:
								case EventId.UIIAPDisclaimerClosed:
								case EventId.UIIAPDisclaimerViewed:
								case EventId.UIFactionFlipAction:
								case EventId.UIFactionFlipConfirmAction:
								case EventId.UIFactionFlipOpened:
								case EventId.TrapDisarmed:
								case EventId.PlanetRelocateButtonPressed:
									IL_264:
									switch (id)
									{
									case EventId.HoloEvent:
										this.audioManager.PlayAudio(cookie as string);
										return EatResponse.NotEaten;
									case EventId.StoryTranscriptDisplayed:
										this.audioManager.PlayAudio(cookie as string);
										return EatResponse.NotEaten;
									case EventId.HolocommScreenLoadComplete:
									case EventId.HoloCommScreenDestroyed:
									case EventId.StoryChainCompleted:
									case EventId.LogStoryActionExecuted:
									case EventId.HeroDeployed:
										IL_2A5:
										switch (id)
										{
										case EventId.PerkSelected:
											goto IL_88E;
										case EventId.SquadLeveledUpCelebration:
											this.audioManager.PlayAudio("sfx_stinger_perk_squad_level_up");
											return EatResponse.NotEaten;
										case EventId.PerkActivated:
											this.audioManager.PlayAudio("sfx_button_perk_activate");
											return EatResponse.NotEaten;
										case EventId.PerkInvested:
											this.audioManager.PlayAudio("sfx_button_perk_rep_invest");
											return EatResponse.NotEaten;
										case EventId.PerkCelebStarted:
											this.audioManager.PlayAudio("sfx_stinger_perk_upgrade");
											return EatResponse.NotEaten;
										case EventId.SquadAdvancementTabSelected:
										case EventId.TargetedBundleChampionRedeemed:
										case EventId.TargetedBundleRewardRedeemed:
										case EventId.TargetedBundleReserve:
										case EventId.ShardsEarned:
										case EventId.EquipmentUnlocked:
											IL_2E6:
											switch (id)
											{
											case EventId.ShuttleAnimStateChanged:
												switch (((ShuttleAnim)cookie).State)
												{
												case ShuttleState.Landing:
													this.audioManager.PlayAudio("sfx_ui_shuttle_arrive");
													break;
												case ShuttleState.LiftOff:
													this.audioManager.PlayAudio("sfx_ui_shuttle_full");
													break;
												}
												return EatResponse.NotEaten;
											case EventId.ShieldBorderDestroyed:
												this.audioManager.PlayAudio("sfx_shields_power_down");
												return EatResponse.NotEaten;
											case EventId.ShieldStarted:
												this.audioManager.PlayAudio("sfx_shields_power_up");
												return EatResponse.NotEaten;
											case EventId.ShieldDisabled:
												IL_30F:
												switch (id)
												{
												case EventId.EquipmentUnlockCelebrationPlayed:
													goto IL_4C4;
												case EventId.EquipmentBuffShaderRemove:
												case EventId.EquipmentBuffShaderApply:
													IL_330:
													switch (id)
													{
													case EventId.ScreenClosing:
													case EventId.ScreenOverlayClosing:
														this.audioManager.PlayAudio("sfx_button_close");
														return EatResponse.NotEaten;
													case EventId.ScreenClosed:
														IL_34D:
														switch (id)
														{
														case EventId.ObjectiveDetailsClicked:
														case EventId.ObjectiveCrateInfoScreenOpened:
															this.audioManager.PlayAudio("sfx_button_context");
															return EatResponse.NotEaten;
														case EventId.ObjectiveCompleted:
															this.audioManager.PlayAudio("sfx_ui_collectcredits_1");
															return EatResponse.NotEaten;
														case EventId.ObjectiveRewardDataCardRevealed:
															goto IL_4C4;
														default:
															switch (id)
															{
															case EventId.CrateRewardIdleHop:
																this.audioManager.PlayAudio("sfx_rewards_crate_hop");
																return EatResponse.NotEaten;
															case EventId.TroopUpgradeScreenOpened:
																IL_387:
																switch (id)
																{
																case EventId.EntityAttackedTarget:
																{
																	SmartEntity smartEntity = cookie as SmartEntity;
																	if (smartEntity != null && smartEntity.TroopComp != null && smartEntity.TroopComp.IsAbilityModeActive)
																	{
																		TroopAbilityVO abilityVO = smartEntity.TroopComp.AbilityVO;
																		string randomClip = this.audioManager.GetRandomClip(abilityVO.AudioAbilityAttack);
																		this.audioManager.PlayAudio(randomClip);
																	}
																	else
																	{
																		this.PlayBattleSound(smartEntity, AudioCollectionType.Attack);
																	}
																	return EatResponse.NotEaten;
																}
																case EventId.PreEntityKilled:
																	IL_39D:
																	if (id == EventId.InitiatedBuyout)
																	{
																		this.audioManager.PlayAudio("sfx_button_finishnow");
																		return EatResponse.NotEaten;
																	}
																	if (id == EventId.InventoryResourceUpdated)
																	{
																		this.PlayInventoryUpdatedSound((string)cookie);
																		return EatResponse.NotEaten;
																	}
																	if (id == EventId.StarEarned || id == EventId.BattleEndVictoryStarDisplayed)
																	{
																		this.PlayStarSound((int)cookie);
																		return EatResponse.NotEaten;
																	}
																	if (id == EventId.ShardUnitUpgraded)
																	{
																		goto IL_A0A;
																	}
																	if (id == EventId.DeployableUnlockCelebrationPlayed)
																	{
																		goto IL_4C4;
																	}
																	if (id == EventId.ApplicationPauseToggled)
																	{
																		this.HandleApplicationPause((bool)cookie);
																		return EatResponse.NotEaten;
																	}
																	if (id == EventId.UIFilterSelected)
																	{
																		goto IL_8A4;
																	}
																	if (id == EventId.ProjectileViewDeflected)
																	{
																		BuffTypeVO buffTypeVO = (BuffTypeVO)cookie;
																		this.audioManager.PlayAudio(this.audioManager.GetRandomClip(buffTypeVO.AudioAbilityEvent));
																		return EatResponse.NotEaten;
																	}
																	if (id == EventId.PlayHoloGreet)
																	{
																		this.audioManager.PlayAudio(cookie as string);
																		return EatResponse.NotEaten;
																	}
																	if (id == EventId.MissionActionButtonClicked)
																	{
																		this.PlayMissionActionSound(cookie as CampaignMissionVO);
																		return EatResponse.NotEaten;
																	}
																	if (id == EventId.HQCelebrationPlayed)
																	{
																		goto IL_4C4;
																	}
																	if (id != EventId.HolonetDevNotes)
																	{
																		return EatResponse.NotEaten;
																	}
																	goto IL_942;
																case EventId.EntityKilled:
																{
																	SmartEntity smartEntity = cookie as SmartEntity;
																	if (smartEntity.TroopComp == null || smartEntity.TroopComp.TroopType.Type == TroopType.Vehicle || LangUtils.ShouldPlayVOClips())
																	{
																		this.PlayBattleSound(smartEntity, AudioCollectionType.Death);
																	}
																	return EatResponse.NotEaten;
																}
																}
																goto IL_39D;
															case EventId.InventoryCrateAnimationStateChange:
															{
																InventoryCrateAnimationState animState = (InventoryCrateAnimationState)((int)cookie);
																this.PlayInventoryCrateAnimSFXBasedOnState(animState);
																return EatResponse.NotEaten;
															}
															case EventId.InventoryCrateAnimVFXTriggered:
															{
																string sfxName = Convert.ToString(cookie);
																this.PlayInventoryCrateAnimVfxSfx(sfxName);
																return EatResponse.NotEaten;
															}
															}
															goto IL_387;
														}
														break;
													case EventId.ScreenLoaded:
														this.PlayScreenLoadedSound(cookie as ScreenBase);
														return EatResponse.NotEaten;
													}
													goto IL_34D;
												case EventId.EquipmentDataFragmentEarned:
													goto IL_9F4;
												case EventId.SupplyCrateProgressBar:
													this.audioManager.PlayAudio("sfx_button_editmode");
													return EatResponse.NotEaten;
												}
												goto IL_330;
												IL_4C4:
												this.audioManager.PlayAudio("sfx_ui_hq_celebration");
												return EatResponse.NotEaten;
											case EventId.ChampionShieldDeactivated:
												this.audioManager.PlayAudio("sfx_champion_shield_deactivate");
												return EatResponse.NotEaten;
											case EventId.ChampionShieldActivated:
												this.audioManager.PlayAudio("sfx_champion_shield_activate");
												return EatResponse.NotEaten;
											case EventId.ChampionShieldDestroyed:
												this.audioManager.PlayAudio("sfx_champion_shield_destroyed");
												return EatResponse.NotEaten;
											}
											goto IL_30F;
										case EventId.EquipmentUpgraded:
											goto IL_A0A;
										case EventId.EquipmentActivated:
											this.audioManager.PlayAudio("sfx_button_trainunit");
											return EatResponse.NotEaten;
										}
										goto IL_2E6;
									case EventId.StoryNextButtonClicked:
									case EventId.StoryAttackButtonClicked:
									case EventId.StorySkipButtonClicked:
										goto IL_942;
									case EventId.TextCrawlStarted:
										goto IL_915;
									case EventId.TextCrawlComplete:
										this.PlayStateMusicOrEffect();
										return EatResponse.NotEaten;
									case EventId.TroopAbilityActivate:
									{
										SmartEntity smartEntity2 = (SmartEntity)cookie;
										TroopAbilityVO abilityVO2 = smartEntity2.TroopComp.AbilityVO;
										this.audioManager.PlayAudio(this.audioManager.GetRandomClip(abilityVO2.AudioAbilityActivate));
										new AudioEffectLoop(abilityVO2.Duration * 0.001f, abilityVO2.AudioAbilityLoop);
										return EatResponse.NotEaten;
									}
									}
									goto IL_2A5;
								case EventId.TrapTriggered:
									this.PlayTrapSound((TrapComponent)cookie);
									return EatResponse.NotEaten;
								case EventId.TrapDestroyed:
									this.PlayTrapDestroySound((TrapComponent)cookie);
									return EatResponse.NotEaten;
								case EventId.PlanetConfirmRelocate:
									this.audioManager.Stop(AudioCategory.Ambience, AudioCategory.Music);
									this.audioManager.PlayAudio("sfx_trans_planet_to_hyperspace");
									return EatResponse.NotEaten;
								case EventId.PlanetRelocateStarted:
									this.audioManager.PlayAudio("sfx_trans_hyperspace");
									return EatResponse.NotEaten;
								}
								goto IL_264;
							case EventId.ShooterWarmingUp:
								this.PlayBattleSound((SmartEntity)cookie, AudioCollectionType.Charge);
								return EatResponse.NotEaten;
							case EventId.ButtonClicked:
								this.PlayButtonEffect(cookie.ToString());
								return EatResponse.NotEaten;
							case EventId.ContextButtonClicked:
							{
								string a = (string)cookie;
								string id2 = (!(a == "Move")) ? "sfx_button_context" : "sfx_button_editmode";
								this.audioManager.PlayAudio(id2);
								return EatResponse.NotEaten;
							}
							case EventId.InfoButtonClicked:
								this.audioManager.PlayAudio("sfx_button_more_info");
								return EatResponse.NotEaten;
							case EventId.UserLiftedBuildingAudio:
								this.audioManager.PlayAudio("sfx_ui_placement_start");
								return EatResponse.NotEaten;
							case EventId.UserGridMovedBuildingAudio:
								this.audioManager.PlayAudio("sfx_ui_placement_move");
								return EatResponse.NotEaten;
							case EventId.UserLoweredBuildingAudio:
								this.audioManager.PlayAudio("sfx_ui_placement_drop");
								return EatResponse.NotEaten;
							case EventId.LongPressStarted:
								this.audioManager.PlayAudio("sfx_button_editfill");
								return EatResponse.NotEaten;
							case EventId.ContractAdded:
							{
								ContractEventData contractEventData = (ContractEventData)cookie;
								this.PlayContractSound(contractEventData.Contract);
								return EatResponse.NotEaten;
							}
							}
							goto IL_217;
							IL_6C8:
							this.PlayStateMusicOrEffect();
							return EatResponse.NotEaten;
						case EventId.BuildingSelectedFromStore:
							goto IL_88E;
						case EventId.BuildingSelectedSound:
							this.audioManager.PlayAudio("sfx_button_selectbuilding");
							return EatResponse.NotEaten;
						case EventId.StoreCategorySelected:
							goto IL_8A4;
						case EventId.BackButtonClicked:
							this.audioManager.PlayAudio("sfx_button_back");
							return EatResponse.NotEaten;
						case EventId.DroidPurchaseCancelled:
							this.audioManager.PlayAudio("sfx_ui_droid_purchase_cancel");
							return EatResponse.NotEaten;
						case EventId.DeviceMusicPlayerStateChanged:
							this.HandleDeviceMusicPlayerStateChanged((bool)cookie);
							return EatResponse.NotEaten;
						case EventId.CurrencyCollected:
							this.PlayCurrencyCollectionEffect(((CurrencyCollectionTag)cookie).Type);
							return EatResponse.NotEaten;
						case EventId.AudibleCurrencySpent:
							switch ((int)cookie)
							{
							case 1:
								this.audioManager.PlayAudio("sfx_button_usecredits");
								break;
							case 2:
								this.audioManager.PlayAudio("sfx_button_usematerials");
								break;
							case 3:
								this.audioManager.PlayAudio("sfx_button_usematerials");
								break;
							}
							return EatResponse.NotEaten;
						}
						goto IL_1BA;
						IL_88E:
						this.audioManager.PlayAudio("sfx_button_store_selectbuilding");
						return EatResponse.NotEaten;
						IL_8A4:
						this.audioManager.PlayAudio("sfx_button_store_selectcategory");
						return EatResponse.NotEaten;
					case EventId.GalaxyGoToPlanetView:
						this.audioManager.Stop(AudioCategory.Ambience, AudioCategory.Music);
						this.audioManager.PlayAudio("sfx_trans_base_to_playscreen");
						this.audioManager.PlayAudio("music_galaxy_map_01");
						return EatResponse.NotEaten;
					case EventId.GalaxyGoToGalaxyView:
						this.audioManager.Stop(AudioCategory.Ambience, AudioCategory.Music);
						this.audioManager.PlayAudio("sfx_trans_base_to_galaxy");
						this.audioManager.PlayAudio("music_galaxy_map_01");
						return EatResponse.NotEaten;
					case EventId.GalaxyTransitionToNextPlanet:
						this.audioManager.PlayAudio("sfx_swipe_planet");
						return EatResponse.NotEaten;
					case EventId.GalaxyTransitionToPreviousPlanet:
						this.audioManager.PlayAudio("sfx_swipe_planet");
						return EatResponse.NotEaten;
					case EventId.GalaxyNotEnoughRelocateStarsClose:
						this.audioManager.PlayAudio("sfx_button_no_relocate");
						return EatResponse.NotEaten;
					case EventId.GalaxyUIPlanetFocus:
						if (Time.time - this.galaxyUIPlanetFocusThrottle > GameConstants.GALAXY_UI_PLANET_FOCUS_THROTTLE)
						{
							this.galaxyUIPlanetFocusThrottle = Time.time;
							this.audioManager.PlayAudio("sfx_ui_planet_focus");
						}
						return EatResponse.NotEaten;
					case EventId.BattleLogScreenTabSelected:
						this.audioManager.PlayAudio("sfx_button_squad_select");
						return EatResponse.NotEaten;
					case EventId.BattleLogScreenShareButtonClicked:
						this.audioManager.PlayAudio("sfx_button_more_info");
						return EatResponse.NotEaten;
					case EventId.BattleLogScreenRevengeButtonClicked:
					case EventId.BattleLogScreenReplayButtonClicked:
						this.audioManager.PlayAudio("sfx_button_startbattle");
						return EatResponse.NotEaten;
					}
					goto IL_160;
				case EventId.TroopLevelUpgraded:
				case EventId.StarshipLevelUpgraded:
					goto IL_A0A;
				case EventId.BuildingLevelUpgraded:
					this.PlayBuildingUpgradedSound(cookie as ContractEventData);
					return EatResponse.NotEaten;
				case EventId.BuildingConstructed:
					this.PlayBuildingUpgradedSound(cookie as ContractEventData);
					return EatResponse.NotEaten;
				case EventId.SpecialAttackDeployed:
				{
					SpecialAttack specialAttack = (SpecialAttack)cookie;
					this.PlayBattleSound(specialAttack.VO, AudioCollectionType.Movement);
					return EatResponse.NotEaten;
				}
				case EventId.SpecialAttackDropshipFlyingAway:
				{
					SpecialAttack specialAttack2 = (SpecialAttack)cookie;
					this.PlayBattleSound(specialAttack2.VO, AudioCollectionType.MovementAway);
					return EatResponse.NotEaten;
				}
				case EventId.SpecialAttackFired:
					this.PlayBattleSound((IAudioVO)cookie, AudioCollectionType.Attack);
					return EatResponse.NotEaten;
				case EventId.IntroStarted:
					goto IL_915;
				case EventId.WorldInTransitionComplete:
					this.PlayStateMusicOrEffect();
					return EatResponse.NotEaten;
				}
				goto IL_FB;
				IL_915:
				this.audioManager.Stop(AudioCategory.Ambience);
				this.audioManager.PlayAudio("music_intro");
				return EatResponse.NotEaten;
				IL_A0A:
				this.audioManager.PlayAudio("sfx_button_readystarship");
				return EatResponse.NotEaten;
			case EventId.HolonetChangeTabs:
			case EventId.HolonetNextPrevTransmision:
				goto IL_942;
			case EventId.TransportArrived:
				this.audioManager.PlayAudio("sfx_ui_vehiclepickup");
				return EatResponse.NotEaten;
			case EventId.TransportDeparted:
				this.audioManager.PlayAudio("sfx_ui_vehicletransport");
				return EatResponse.NotEaten;
			case EventId.TroopPlacedOnBoard:
			{
				SmartEntity smartEntity = cookie as SmartEntity;
				TroopComponent troopComp = smartEntity.TroopComp;
				if (smartEntity.ShooterComp != null && smartEntity.ShooterComp.isSkinned)
				{
					this.audioManager.PlayAudio("sfx_placement_skinned_notify_01");
				}
				if (troopComp.TroopType.Type == TroopType.Vehicle || LangUtils.ShouldPlayVOClips())
				{
					this.PlayBattleSound(troopComp.AudioVO, AudioCollectionType.Placement);
				}
				return EatResponse.NotEaten;
			}
			case EventId.TroopRecruited:
			{
				ContractEventData contractEventData2 = (ContractEventData)cookie;
				if (!contractEventData2.Silent)
				{
					BuildingType type = contractEventData2.BuildingVO.Type;
					if (type == BuildingType.Factory)
					{
						this.audioManager.PlayAudio("sfx_ui_vehiclecompleted_1");
					}
					else if (type == BuildingType.Barracks || type == BuildingType.Cantina)
					{
						if (LangUtils.ShouldPlayVOClips())
						{
							TroopTypeVO troopTypeVO = Service.StaticDataController.Get<TroopTypeVO>(contractEventData2.Contract.ProductUid);
							IAudioVO audioType = troopTypeVO;
							ActiveArmory activeArmory = Service.CurrentPlayer.ActiveArmory;
							if (activeArmory != null)
							{
								SkinController skinController = Service.SkinController;
								SkinTypeVO applicableSkin = skinController.GetApplicableSkin(troopTypeVO, activeArmory.Equipment);
								if (applicableSkin != null && applicableSkin.AudioTrain != null && applicableSkin.AudioTrain.Count > 0)
								{
									audioType = applicableSkin;
								}
							}
							this.PlayRandomClip(audioType, AudioCollectionType.Train);
						}
					}
				}
				return EatResponse.NotEaten;
			}
			case EventId.TroopLoadingIntoStarport:
				this.audioManager.PlayAudio("sfx_ui_troopload_1");
				return EatResponse.NotEaten;
			case EventId.StarshipMobilized:
			case EventId.HeroMobilized:
			case EventId.StarshipMobilizedFromPrize:
			case EventId.HeroMobilizedFromPrize:
				goto IL_9F4;
			case EventId.TroopNotPlacedInvalidArea:
			case EventId.TroopNotPlacedInvalidTroop:
			case EventId.TroopPlacedInsideShieldError:
			case EventId.TroopPlacedInsideBuildingError:
			case EventId.HeroNotActivated:
				this.audioManager.PlayAudio("sfx_ui_placement_error");
				return EatResponse.NotEaten;
			case EventId.TroopDonationTrackRewardReceived:
				this.audioManager.PlayAudio("sfx_stinger_perk_donation_rep_reward");
				return EatResponse.NotEaten;
			}
			goto IL_85;
			IL_942:
			this.audioManager.Stop(AudioCategory.Dialogue);
			this.audioManager.PlayAudio("sfx_button_next");
			return EatResponse.NotEaten;
			IL_9F4:
			this.audioManager.PlayAudio("sfx_button_readyhero");
			return EatResponse.NotEaten;
		}

		private void HandleApplicationPause(bool paused)
		{
			if (paused)
			{
				return;
			}
			if (this.audioManager.IsThirdPartyNativePluginActive())
			{
				return;
			}
			Service.EnvironmentController.GainAudioFocus();
			this.audioManager.RefreshMusic();
		}

		private void HandleDeviceMusicPlayerStateChanged(bool isDeviceMusicPlaying)
		{
			this.audioManager.RefreshMusic();
		}

		private void PlayBattleSound(Entity entity, AudioCollectionType clipType)
		{
			if (this.audioManager.GetBattleAudioFlag(clipType))
			{
				return;
			}
			this.PlayRandomClip(entity, clipType);
			this.audioManager.SetBattleAudioFlag(clipType);
		}

		private void PlayBattleSound(IAudioVO audioType, AudioCollectionType clipType)
		{
			if (this.audioManager.GetBattleAudioFlag(clipType))
			{
				return;
			}
			this.PlayRandomClip(audioType, clipType);
			this.audioManager.SetBattleAudioFlag(clipType);
		}

		private void PlayStarSound(int starCount)
		{
			if (Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				return;
			}
			switch (starCount)
			{
			case 1:
				this.audioManager.PlayAudio("sfx_stinger_victory_onestar");
				break;
			case 2:
				this.audioManager.PlayAudio("sfx_stinger_victory_twostar");
				break;
			case 3:
				this.audioManager.PlayAudio("sfx_stinger_victory_threestar");
				break;
			}
		}

		private void PlayScreenLoadedSound(ScreenBase screen)
		{
			if (screen is AlertScreen && !(screen is ConfirmRelocateScreen))
			{
				this.audioManager.PlayAudio("sfx_ui_alert");
			}
		}

		private void PlayInventoryUpdatedSound(string resourceType)
		{
			string text = (!(resourceType == "droids")) ? string.Empty : "sfx_ui_droid_purchase";
			if (text != string.Empty)
			{
				this.audioManager.PlayAudio(text);
			}
		}

		private void PlayContractSound(Contract contract)
		{
			string id = string.Empty;
			switch (contract.DeliveryType)
			{
			case DeliveryType.Infantry:
			case DeliveryType.Vehicle:
			case DeliveryType.Starship:
			case DeliveryType.Hero:
			case DeliveryType.Champion:
			case DeliveryType.Mercenary:
				id = "sfx_button_trainunit";
				break;
			case DeliveryType.Building:
			case DeliveryType.UpgradeBuilding:
			case DeliveryType.SwapBuilding:
				id = this.audioManager.GetRandomClip(this.droidClips);
				break;
			}
			this.audioManager.PlayAudio(id);
		}

		private void PlayBuildingUpgradedSound(ContractEventData contractData)
		{
			if (contractData.Silent)
			{
				return;
			}
			FactionType faction = Service.CurrentPlayer.Faction;
			string text;
			if (contractData.BuildingVO.Type == BuildingType.HQ)
			{
				FactionType factionType = faction;
				if (factionType != FactionType.Empire)
				{
					if (factionType != FactionType.Rebel)
					{
						text = "sfx_stinger_upgradehq";
					}
					else
					{
						text = "sfx_stinger_rebel_upgradehq";
					}
				}
				else
				{
					text = "sfx_stinger_empire_upgradehq";
				}
			}
			else
			{
				FactionType factionType = faction;
				if (factionType != FactionType.Empire)
				{
					if (factionType != FactionType.Rebel)
					{
						text = "sfx_stinger_upgradebuilding";
					}
					else
					{
						text = "sfx_stinger_rebel_upgradebuilding";
					}
				}
				else
				{
					text = "sfx_stinger_empire_upgradebuilding";
				}
			}
			if (text != null)
			{
				this.audioManager.PlayAudio(text);
			}
		}

		private void PlayMissionActionSound(CampaignMissionVO vo)
		{
			string id = string.Empty;
			switch (vo.MissionType)
			{
			case MissionType.Attack:
			case MissionType.Defend:
			case MissionType.RaidDefend:
				id = "sfx_button_startbattle";
				break;
			case MissionType.Collect:
				id = "sfx_ui_collectreward_1";
				break;
			}
			this.audioManager.PlayAudio(id);
		}

		private void PlayRandomClip(Entity entity, AudioCollectionType clipType)
		{
			if (entity == null)
			{
				return;
			}
			IAudioVO audioTypeFromEntity = this.GetAudioTypeFromEntity(entity);
			this.PlayRandomClip(audioTypeFromEntity, clipType);
		}

		private void PlayRandomClip(IAudioVO audioType, AudioCollectionType clipType)
		{
			if (audioType == null)
			{
				return;
			}
			List<StrIntPair> list = null;
			switch (clipType)
			{
			case AudioCollectionType.Charge:
				list = audioType.AudioCharge;
				break;
			case AudioCollectionType.Attack:
				list = audioType.AudioAttack;
				break;
			case AudioCollectionType.Death:
				list = audioType.AudioDeath;
				break;
			case AudioCollectionType.Placement:
				list = audioType.AudioPlacement;
				break;
			case AudioCollectionType.Movement:
				list = audioType.AudioMovement;
				break;
			case AudioCollectionType.MovementAway:
				list = audioType.AudioMovementAway;
				break;
			case AudioCollectionType.Impact:
				list = audioType.AudioImpact;
				break;
			case AudioCollectionType.Train:
				list = audioType.AudioTrain;
				break;
			}
			if (list == null)
			{
				return;
			}
			string randomClip = this.audioManager.GetRandomClip(list);
			this.audioManager.PlayAudio(randomClip);
		}

		private IAudioVO GetAudioTypeFromEntity(Entity entity)
		{
			IAudioVO result = null;
			if (entity.Has<BuildingComponent>())
			{
				result = entity.Get<BuildingComponent>().BuildingType;
			}
			else if (entity.Has<TroopComponent>())
			{
				result = entity.Get<TroopComponent>().AudioVO;
			}
			return result;
		}

		private void PlayStateMusicOrEffect()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			FactionType faction = currentPlayer.Faction;
			PlanetVO planet = currentPlayer.Planet;
			string id = string.Empty;
			string text = null;
			if (currentState is EditBaseState)
			{
				this.audioManager.PlayAudio("sfx_button_editmode");
			}
			else if (currentState is HomeState || currentState is FueBattleStartState)
			{
				this.PlayPlanetBaseMusic(planet, faction);
			}
			else if (currentState is NeighborVisitState)
			{
				PlanetVO planet2 = Service.NeighborVisitManager.NeighborPlayer.Map.Planet;
				this.PlayPlanetBaseMusic(planet2, faction);
			}
			else if (currentState is WarBoardState)
			{
				this.PlayPlanetBaseMusic(planet, faction);
			}
			else if (currentState is BattleStartState)
			{
				if (!Service.WorldTransitioner.IsTransitioning())
				{
					this.audioManager.Stop(AudioCategory.Ambience);
					this.PlayPreBattleMusic();
				}
				else
				{
					this.audioManager.Stop(AudioCategory.Ambience, AudioCategory.Music);
				}
			}
			else if (currentState is BattlePlaybackState || currentState is BattlePlayState)
			{
				this.audioManager.Stop(AudioCategory.Ambience);
				if (Service.WorldTransitioner.IsTransitioning())
				{
					this.PlayPreBattleMusic();
					return;
				}
				string text2 = null;
				FactionType factionType = faction;
				if (factionType != FactionType.Empire)
				{
					if (factionType == FactionType.Rebel)
					{
						text2 = "sfx_stinger_rebel_battle";
					}
				}
				else
				{
					text2 = "sfx_stinger_empire_battle";
				}
				CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
				string battleMusic = currentBattle.BattleMusic;
				if (!string.IsNullOrEmpty(battleMusic))
				{
					text = battleMusic;
				}
				if (string.IsNullOrEmpty(text))
				{
					PlanetVO planetVO = Service.StaticDataController.Get<PlanetVO>(currentBattle.PlanetId);
					text = planetVO.BattleMusic;
				}
				if (string.IsNullOrEmpty(text))
				{
					text = planet.BattleMusic;
				}
				if (text2 != null)
				{
					this.audioManager.PlayAudio(text2);
					float delay = 7f;
					this.timerId = Service.ViewTimerManager.CreateViewTimer(delay, false, new TimerDelegate(this.StartMusicOnTimer), text);
				}
				else
				{
					this.audioManager.PlayAudio(text);
				}
			}
			else if (currentState is BattleEndPlaybackState || currentState is BattleEndState)
			{
				this.audioManager.Stop(AudioCategory.Music);
				Service.ViewTimerManager.KillViewTimer(this.timerId);
				string ambientMusic = Service.BattleController.GetCurrentBattle().AmbientMusic;
				if (!string.IsNullOrEmpty(ambientMusic))
				{
					text = ambientMusic;
				}
				else
				{
					text = planet.AmbientMusic;
				}
				this.audioManager.PlayAudio(text);
				bool isReplay = Service.BattleController.GetCurrentBattle().IsReplay;
				BattleEntry battleEntry;
				if (isReplay)
				{
					battleEntry = Service.BattlePlaybackController.CurrentBattleEntry;
				}
				else
				{
					battleEntry = Service.BattleController.GetCurrentBattle();
				}
				bool won = battleEntry.Won;
				FactionType factionType = faction;
				if (factionType != FactionType.Empire)
				{
					id = ((!won) ? "sfx_stinger_rebel_defeat" : "sfx_stinger_rebel_victory");
				}
				else
				{
					id = ((!won) ? "sfx_stinger_empire_defeat" : "sfx_stinger_empire_victory");
				}
				this.audioManager.PlayAudio(id);
			}
		}

		private void PlayPlanetBaseMusic(PlanetVO planet, FactionType faction)
		{
			string id = string.Empty;
			if (!Service.WorldTransitioner.IsTransitioning())
			{
				string text = planet.AmbientMusic;
				if (string.IsNullOrEmpty(text))
				{
					text = "sfx_ambient_tatooine";
				}
				if (!this.audioManager.IsPlaying(AudioCategory.Ambience, text))
				{
					this.audioManager.Stop(AudioCategory.Ambience);
					this.audioManager.PlayAudio(text);
				}
			}
			else
			{
				this.audioManager.Stop(AudioCategory.Ambience);
			}
			switch (faction)
			{
			case FactionType.Empire:
				id = planet.EmpireMusic;
				break;
			case FactionType.Rebel:
				id = planet.RebelMusic;
				break;
			case FactionType.Smuggler:
				id = "music_rebel_base_1";
				break;
			}
			if (!this.audioManager.IsPlaying(AudioCategory.Music, id))
			{
				this.audioManager.Stop(AudioCategory.Music);
				this.audioManager.PlayAudio(id);
			}
		}

		private void PlayPreBattleMusic()
		{
			this.audioManager.Stop(AudioCategory.Music);
			string id = string.Empty;
			string ambientMusic = Service.BattleController.GetCurrentBattle().AmbientMusic;
			if (!string.IsNullOrEmpty(ambientMusic))
			{
				id = ambientMusic;
			}
			else
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				PlanetVO planet = currentPlayer.Planet;
				id = planet.AmbientMusic;
			}
			this.audioManager.PlayAudio(id);
		}

		private void StartMusicOnTimer(uint timerId, object cookie)
		{
			this.audioManager.PlayAudio(cookie as string);
		}

		private void PlayTrapSound(TrapComponent trapComp)
		{
			this.audioManager.PlayAudio(trapComp.Type.RevealAudio);
		}

		private void PlayTrapDestroySound(TrapComponent trapComp)
		{
			this.audioManager.PlayAudio(trapComp.Type.RevealAudio);
		}

		private void PlayCurrencyCollectionEffect(CurrencyType currencyType)
		{
			string id = string.Empty;
			switch (currencyType)
			{
			case CurrencyType.Credits:
				id = "sfx_ui_collectcredits_1";
				goto IL_56;
			case CurrencyType.Materials:
				id = "sfx_ui_collectmaterials_1";
				goto IL_56;
			case CurrencyType.Contraband:
				id = "sfx_ui_collectmaterials_1";
				goto IL_56;
			case CurrencyType.Crystals:
				id = "sfx_ui_collecthardcurrency_1";
				goto IL_56;
			}
			return;
			IL_56:
			this.audioManager.PlayAudio(id);
		}

		private void PlayButtonEffect(string elementName)
		{
			string id = string.Empty;
			switch (elementName)
			{
			case "ButtonNextBattle":
				id = "sfx_button_startbattle";
				goto IL_310;
			case "ButtonBattle":
			case "ButtonWar":
				id = "sfx_button_mission";
				goto IL_310;
			case "BtnMusic":
			case "BtnSoundEffects":
				id = "sfx_button_next";
				goto IL_310;
			case "BtnAbout":
			case "BtnLanguage":
			case "BtnHelp":
			case "BtnPrivacyPolicy":
			case "BtnTOS":
			case "BtnFacebook":
				id = "sfx_button_facebook";
				goto IL_310;
			case "ButtonEndBattle":
				id = "sfx_button_endbattle";
				goto IL_310;
			case "ButtonHome":
			case "ButtonExitEdit":
				id = "sfx_button_home";
				goto IL_310;
			case "Crystals":
			case "Shield":
			case "ButtonStore":
				id = "sfx_button_store";
				goto IL_310;
			case "ButtonSettings":
			case "Medals":
			case "BaseRating":
			case "ButtonLog":
				id = "sfx_button_more_info";
				goto IL_310;
			case "BtnSwap":
				id = "sfx_button_more_info";
				goto IL_310;
			case "BtnCancel":
				id = "sfx_button_back";
				goto IL_310;
			case "ButtonClans":
				id = "sfx_button_squad";
				goto IL_310;
			case "ButtonLeaderboard":
				id = "sfx_button_squad";
				goto IL_310;
			case "ButtonPrimaryAction":
				id = "sfx_button_restartbattle";
				goto IL_310;
			case "Newspaper":
				id = "sfx_button_more_info";
				goto IL_310;
			case "BtnOption1Top":
			case "BtnOption2Top":
			case "BtnOption1Bottom":
			case "BtnOption2Bottom":
				id = "sfx_button_next";
				goto IL_310;
			case "TabActivatePerks":
			case "TabUpgradePerks":
				id = "sfx_button_squad_select";
				goto IL_310;
			}
			if (elementName.StartsWith("CheckboxTroop"))
			{
				id = "sfx_button_selecttroop";
			}
			else if (elementName.StartsWith("ButtonCampaignCard") || elementName.StartsWith("ButtonObjectiveCard"))
			{
				id = "sfx_button_campaign";
			}
			IL_310:
			this.audioManager.PlayAudio(id);
		}
	}
}
