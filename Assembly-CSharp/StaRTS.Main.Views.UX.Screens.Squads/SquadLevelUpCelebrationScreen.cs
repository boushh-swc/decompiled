using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadLevelUpCelebrationScreen : ClosableScreen
	{
		private const string SHOW_ANIM = "Show";

		private const string UNLOCKED_SLOT_ID_PREFIX = "UnlockedSlot";

		private const string TITLE_LABEL = "LabelTitle";

		private const string SQUAD_LVL_LABEL = "LabelSquadLevel";

		private const string PRIMARY_BTN = "ButtonPrimaryAction";

		private const string PRIMARY_BTN_LABEL = "LabelPrimaryAction";

		private const string UNLOCKED_ITEMS_LABEL = "LabelUnlockItems";

		private const string UNLOCKED_ITEMS_GRID = "GridUnlockItems";

		private const string UNLOCKED_PERK_TEMPLATE = "TemplateCard";

		private const string ULOCKED_PERK_LVL_LABEL = "LabelPerkLvlUpCard";

		private const string UNLOCKED_PERK_TITLE_LABEL = "LabelPerkTitleUpCard";

		private const string UNLOCKED_PERK_IMAGE = "TexturePerkArtCard";

		private const string UNLOCKED_SLOT_TEMPLATE = "SlotCard";

		private const string UNLOCKED_SLOT_LABEL = "LabelSlotTitleUpCard";

		private const string SQUAD_LEVEL_UP_CELEBRATION_TITLE = "SQUAD_LEVEL_UP_CELEBRATION_TITLE";

		private const string SQUAD_LEVEL_UP_CELEBRATION_DESC = "SQUAD_LEVEL_UP_CELEBRATION_DESC";

		private const string SQUAD_LEVEL_UP_CELEBRATION_SLOT = "SQUAD_LEVEL_UP_CELEBRATION_SLOT";

		private const string CONTINUE = "CONTINUE";

		private UXLabel titleLabel;

		private UXLabel squadLevelLabel;

		private UXButton primaryBtn;

		private UXLabel primaryBtnLabel;

		private UXLabel unlockedItemsDescLabel;

		private UXGrid unlockedItemsGrid;

		private int squadLevel;

		private List<PerkVO> unlockedPerks;

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public SquadLevelUpCelebrationScreen(int squadLevel, List<PerkVO> perksUnlockedForInvest) : base("gui_celeb_squadlevelup")
		{
			this.squadLevel = squadLevel;
			this.unlockedPerks = perksUnlockedForInvest;
			base.IsAlwaysOnTop = true;
		}

		protected override void OnScreenLoaded()
		{
			this.titleLabel = base.GetElement<UXLabel>("LabelTitle");
			this.titleLabel.Text = this.lang.Get("SQUAD_LEVEL_UP_CELEBRATION_TITLE", new object[0]);
			this.squadLevelLabel = base.GetElement<UXLabel>("LabelSquadLevel");
			this.squadLevelLabel.Text = this.squadLevel.ToString();
			this.primaryBtn = base.GetElement<UXButton>("ButtonPrimaryAction");
			this.primaryBtn.OnClicked = new UXButtonClickedDelegate(this.OnContinueClicked);
			this.primaryBtnLabel = base.GetElement<UXLabel>("LabelPrimaryAction");
			this.primaryBtnLabel.Text = this.lang.Get("CONTINUE", new object[0]);
			this.unlockedItemsDescLabel = base.GetElement<UXLabel>("LabelUnlockItems");
			this.unlockedItemsDescLabel.Text = this.lang.Get("SQUAD_LEVEL_UP_CELEBRATION_DESC", new object[0]);
			this.unlockedItemsGrid = base.GetElement<UXGrid>("GridUnlockItems");
			this.AddUnlockedSlotsToGrid();
			this.AddUnlockedPerksToGrid();
			Animator component = base.Root.GetComponent<Animator>();
			component.SetTrigger("Show");
			Service.EventManager.SendEvent(EventId.SquadLeveledUpCelebration, null);
		}

		private void AddUnlockedPerksToGrid()
		{
			this.unlockedItemsGrid.SetTemplateItem("TemplateCard");
			if (this.unlockedPerks == null)
			{
				return;
			}
			PerkViewController perkViewController = Service.PerkViewController;
			int count = this.unlockedPerks.Count;
			for (int i = 0; i < count; i++)
			{
				PerkVO perkVO = this.unlockedPerks[i];
				string uid = perkVO.Uid;
				UXElement item = this.unlockedItemsGrid.CloneTemplateItem(uid);
				UXLabel subElement = this.unlockedItemsGrid.GetSubElement<UXLabel>(uid, "LabelPerkLvlUpCard");
				subElement.Text = StringUtils.GetRomanNumeral(perkVO.PerkTier);
				UXLabel subElement2 = this.unlockedItemsGrid.GetSubElement<UXLabel>(uid, "LabelPerkTitleUpCard");
				subElement2.Text = perkViewController.GetPerkNameForGroup(perkVO.PerkGroup);
				UXTexture subElement3 = this.unlockedItemsGrid.GetSubElement<UXTexture>(uid, "TexturePerkArtCard");
				perkViewController.SetPerkImage(subElement3, perkVO);
				this.unlockedItemsGrid.AddItem(item, this.unlockedItemsGrid.Count);
			}
		}

		private void AddUnlockedSlotsToGrid()
		{
			this.unlockedItemsGrid.SetTemplateItem("SlotCard");
			PerkManager perkManager = Service.PerkManager;
			string squadLevelUIDFromLevel = GameUtils.GetSquadLevelUIDFromLevel(this.squadLevel - 1);
			string squadLevelUIDFromLevel2 = GameUtils.GetSquadLevelUIDFromLevel(this.squadLevel);
			int availableSlotsCount = perkManager.GetAvailableSlotsCount(squadLevelUIDFromLevel);
			int availableSlotsCount2 = perkManager.GetAvailableSlotsCount(squadLevelUIDFromLevel2);
			int num = availableSlotsCount2 - availableSlotsCount;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					string itemUid = "UnlockedSlot" + i;
					UXElement item = this.unlockedItemsGrid.CloneTemplateItem(itemUid);
					UXLabel subElement = this.unlockedItemsGrid.GetSubElement<UXLabel>(itemUid, "LabelSlotTitleUpCard");
					subElement.Text = this.lang.Get("SQUAD_LEVEL_UP_CELEBRATION_SLOT", new object[0]);
					this.unlockedItemsGrid.AddItem(item, this.unlockedItemsGrid.Count);
				}
			}
		}

		private void OnContinueClicked(UXButton btn)
		{
			this.Close(null);
		}

		public override void OnDestroyElement()
		{
			base.OnDestroyElement();
		}
	}
}
