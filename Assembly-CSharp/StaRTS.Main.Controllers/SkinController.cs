using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class SkinController : IEventObserver
	{
		public SkinController()
		{
			Service.EventManager.RegisterObserver(this, EventId.ButtonCreated, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.TooltipCreated, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.TroopCreated, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.HologramCreated, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.TextureCreated, EventPriority.Default);
			Service.SkinController = this;
		}

		public virtual EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.ButtonCreated:
				this.ProcessSkin((GeometryTag)cookie);
				break;
			case EventId.TooltipCreated:
				this.ProcessTooltip((GeometryTag)cookie);
				break;
			case EventId.TroopCreated:
				this.ProcessSkin((SmartEntity)cookie);
				break;
			case EventId.HologramCreated:
				this.ProcessHologramSkin((GeometryTag)cookie);
				break;
			case EventId.TextureCreated:
				this.ProcessCardSkin((GeometryTag)cookie);
				break;
			}
			return EatResponse.NotEaten;
		}

		private void ProcessSkin(SmartEntity entity)
		{
			ActiveArmory activeArmory = (!(Service.GameStateMachine.CurrentState is HomeState)) ? null : Service.CurrentPlayer.ActiveArmory;
			CurrentBattle battle = (activeArmory != null) ? null : Service.BattleController.GetCurrentBattle();
			SkinTypeVO applicableSkin = this.GetApplicableSkin((TroopTypeVO)entity.TroopComp.TroopType, activeArmory, battle, entity.TeamComp.TeamType);
			if (applicableSkin != null)
			{
				entity.TroopComp.AssetName = applicableSkin.AssetName;
				entity.TroopComp.AudioVO = applicableSkin;
				if (entity.ShooterComp != null)
				{
					entity.ShooterComp.isSkinned = true;
				}
				if (applicableSkin.Override != null)
				{
					entity.TroopComp.TroopShooterVO = new SkinnedTroopShooterFacade(entity.TroopComp.TroopShooterVO, applicableSkin.Override);
					entity.TroopComp.OriginalTroopShooterVO = new SkinnedTroopShooterFacade(entity.TroopComp.OriginalTroopShooterVO, applicableSkin.Override);
					if (entity.ShooterComp != null)
					{
						entity.ShooterComp.SetVOData(new SkinnedShooterFacade(entity.ShooterComp.ShooterVO, applicableSkin.Override));
						entity.ShooterComp.OriginalShooterVO = new SkinnedShooterFacade(entity.ShooterComp.OriginalShooterVO, applicableSkin.Override);
					}
				}
				if (entity.WalkerComp != null)
				{
					entity.WalkerComp.SetVOData(applicableSkin);
				}
			}
		}

		private void ProcessHologramSkin(GeometryTag cookie)
		{
			if (cookie.geometry == null || !(cookie.geometry is TroopTypeVO))
			{
				return;
			}
			SkinTypeVO applicableSkin = this.GetApplicableSkin(cookie.geometry as TroopTypeVO, Service.CurrentPlayer.ActiveArmory, null, TeamType.Undefined);
			if (applicableSkin != null && applicableSkin.MobilizationHologram != null)
			{
				cookie.assetName = applicableSkin.MobilizationHologram.AssetName;
			}
		}

		private void ProcessCardSkin(GeometryTag cookie)
		{
			if (!(cookie.geometry is TroopTypeVO))
			{
				return;
			}
			SkinTypeVO applicableSkin = this.GetApplicableSkin(cookie.geometry as TroopTypeVO, Service.CurrentPlayer.ActiveArmory, null, TeamType.Undefined);
			if (applicableSkin != null && applicableSkin.CardTexture != null)
			{
				cookie.assetName = applicableSkin.CardTexture.AssetName;
			}
		}

		private void ProcessSkin(GeometryTag cookie)
		{
			if (!(cookie.geometry is TroopTypeVO))
			{
				return;
			}
			TroopTypeVO troop = cookie.geometry as TroopTypeVO;
			ProjectorConfig projector = cookie.projector;
			TeamType team = TeamType.Attacker;
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			if (currentBattle != null && !string.IsNullOrEmpty(currentBattle.MissionId))
			{
				CampaignMissionVO campaignMissionVO = Service.StaticDataController.Get<CampaignMissionVO>(currentBattle.MissionId);
				if (campaignMissionVO != null && campaignMissionVO.IsRaidDefense())
				{
					team = TeamType.Defender;
				}
			}
			SkinTypeVO applicableSkin = this.GetApplicableSkin(troop, cookie.armory, cookie.battle, team);
			if (applicableSkin != null)
			{
				ProjectorConfig config = ProjectorUtils.GenerateGeometryConfig(applicableSkin, projector.FrameSprite, projector.closeup);
				projector.MakeEquivalentTo(config);
			}
		}

		private void ProcessTooltip(GeometryTag cookie)
		{
			if (!(cookie.geometry is TroopTypeVO))
			{
				return;
			}
			TroopTypeVO troop = cookie.geometry as TroopTypeVO;
			SkinTypeVO applicableSkin = this.GetApplicableSkin(troop, cookie.armory, cookie.battle, TeamType.Attacker);
			if (applicableSkin != null)
			{
				cookie.tooltipText = LangUtils.GetSkinDisplayName(applicableSkin);
			}
		}

		private SkinTypeVO GetApplicableSkin(TroopTypeVO troop, ActiveArmory armory, BattleEntry battle, TeamType team)
		{
			if (armory != null)
			{
				return this.GetApplicableSkin(troop, armory.Equipment);
			}
			if (battle == null)
			{
				return null;
			}
			List<string> equipmentList = null;
			if (team == TeamType.Attacker)
			{
				equipmentList = battle.AttackerEquipment;
			}
			else if (team == TeamType.Defender)
			{
				equipmentList = battle.DefenderEquipment;
			}
			return this.GetApplicableSkin(troop, equipmentList);
		}

		public SkinTypeVO GetApplicableSkin(TroopTypeVO troop, List<string> equipmentList, out string equipmentUid)
		{
			equipmentUid = null;
			if (troop == null || equipmentList == null)
			{
				return null;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			int i = 0;
			int count = equipmentList.Count;
			while (i < count)
			{
				EquipmentVO equipmentVO = staticDataController.Get<EquipmentVO>(equipmentList[i]);
				if (equipmentVO != null)
				{
					string[] skins = equipmentVO.Skins;
					if (skins != null)
					{
						int j = 0;
						int num = skins.Length;
						while (j < num)
						{
							SkinTypeVO skinTypeVO = staticDataController.Get<SkinTypeVO>(skins[j]);
							if (troop.TroopID.Equals(skinTypeVO.UnitId))
							{
								equipmentUid = equipmentList[i];
								return skinTypeVO;
							}
							j++;
						}
					}
				}
				i++;
			}
			return null;
		}

		public SkinTypeVO GetApplicableSkin(TroopTypeVO troop, List<string> equipmentList)
		{
			string text = null;
			return this.GetApplicableSkin(troop, equipmentList, out text);
		}
	}
}
