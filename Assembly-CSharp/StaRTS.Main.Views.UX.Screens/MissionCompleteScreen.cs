using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class MissionCompleteScreen : ScreenBase
	{
		private const string TITLE_LABEL = "LabelTitle";

		private const string REWARD_TABLE = "RewardTable";

		private const string REWARD_ITEM_TEMPLATE = "RewardTemplate";

		private const string REWARD_ITEM_VALUE_LABEL = "RewardValueLabel";

		private const string REWARD_ICON_ROOT = "RewardIconType";

		private const string REWARD_ICONS_PREFIX = "RewardIcon";

		private const float ANIMATION_TIME = 3f;

		private Dictionary<string, string> rewardIconMap;

		private UXLabel titleLabel;

		private UXTable rewardTable;

		private CampaignMissionVO missionVO;

		public MissionCompleteScreen(CampaignMissionVO mission) : base("gui_mission_complete")
		{
			this.missionVO = mission;
		}

		protected override void OnScreenLoaded()
		{
			this.rewardIconMap = new Dictionary<string, string>();
			this.FillOutRewardIconMap();
			this.InitLabels();
			this.InitRewardGrid();
			Animator component = base.Root.GetComponent<Animator>();
			component.SetTrigger("Show");
			Service.ViewTimerManager.CreateViewTimer(3f, false, new TimerDelegate(this.OnAnimationFinishedTimer), null);
		}

		private void FillOutRewardIconMap()
		{
			this.rewardIconMap.Add("credits", "Credit");
			this.rewardIconMap.Add("materials", "Alloy");
			this.rewardIconMap.Add("crystals", "Crystal");
			this.rewardIconMap.Add("contraband", "Contraband");
		}

		private void InitLabels()
		{
			this.titleLabel = base.GetElement<UXLabel>("LabelTitle");
			this.titleLabel.Text = LangUtils.GetMissionTitle(this.missionVO);
		}

		private void InitRewardGrid()
		{
			RewardVO rewardVO = null;
			if (!string.IsNullOrEmpty(this.missionVO.Rewards))
			{
				rewardVO = Service.StaticDataController.Get<RewardVO>(this.missionVO.Rewards);
			}
			this.rewardTable = base.GetElement<UXTable>("RewardTable");
			if (rewardVO != null && rewardVO.CurrencyRewards != null)
			{
				this.rewardTable.Visible = true;
				this.rewardTable.SetTemplateItem("RewardTemplate");
				UXUtils.HideChildrenRecursively(base.GetElement<UXElement>("RewardIconType").Root, true);
				for (int i = 0; i < rewardVO.CurrencyRewards.Length; i++)
				{
					string[] array = rewardVO.CurrencyRewards[i].Split(new char[]
					{
						':'
					});
					string key = array[0];
					if (this.rewardIconMap.ContainsKey(key))
					{
						string text = "RewardIcon" + this.rewardIconMap[key];
						string text2 = this.lang.ThousandsSeparated(Convert.ToInt32(array[1]));
						string itemUid = text + i;
						UXElement item = this.rewardTable.CloneTemplateItem(itemUid);
						this.rewardTable.GetSubElement<UXSprite>(itemUid, text).Visible = true;
						this.rewardTable.GetSubElement<UXLabel>(itemUid, "RewardValueLabel").Text = text2;
						this.rewardTable.AddItem(item, i);
					}
				}
				this.rewardTable.RepositionItems();
			}
			else
			{
				this.rewardTable.Visible = false;
			}
		}

		private void OnAnimationFinishedTimer(uint id, object cookie)
		{
			this.Visible = false;
			this.Close(null);
			Service.EventManager.SendEvent(EventId.MissionCompleteScreenDisplayed, null);
		}

		public override void OnDestroyElement()
		{
			if (this.rewardTable != null)
			{
				this.rewardTable.Clear();
				this.rewardTable = null;
			}
			base.OnDestroyElement();
		}
	}
}
