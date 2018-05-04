using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Perks;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ActivePerksInfoScreen : ClosableScreen, IViewClockTimeObserver
	{
		private const string TITLE_LABEL = "LabelTitleModalActivePerks";

		private const string PERKS_GRID = "GridModalActivePerks";

		private const string PERK_GRID_TEMPLATE = "TemplateModalActivePerks";

		private const string PERK_TITLE_LABEL = "LabelPerkTitleModalActivePerks";

		private const string PERK_IMAGE = "TexturePerkArtModalActivePerks";

		private const string PERK_LEVEL = "LabelPerkLvlModalActivePerks";

		private const string PERK_EFFECT_DESC_GRID = "GridSubModalActivePerks";

		private const string PERK_EFFECT_DESC_LABEL = "ItemLabelModalActivePerks";

		private const string PERK_TIMER_LABEL = "LabelTimerModalActivePerks";

		private const string TITLE = "PERK_BUILDING_INFO_SUMMARY_TITLE";

		private UXGrid perksGrid;

		private bool needsPerkStateRefresh;

		private BuildingTypeVO buildingVO;

		public ActivePerksInfoScreen(BuildingTypeVO vo) : base("gui_modal_activeperks")
		{
			this.needsPerkStateRefresh = false;
			this.buildingVO = vo;
			base.IsAlwaysOnTop = true;
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			this.InitLabels();
			this.InitElements();
		}

		private void InitElements()
		{
			this.perksGrid = base.GetElement<UXGrid>("GridModalActivePerks");
			this.perksGrid.SetTemplateItem("TemplateModalActivePerks");
			this.perksGrid.Clear();
			this.InitializePerksGrid();
		}

		public void OnViewClockTime(float dt)
		{
			int i = 0;
			int count = this.perksGrid.Count;
			while (i < count)
			{
				this.UpdatePerkTimer(this.perksGrid.GetItem(i));
				i++;
			}
			if (this.needsPerkStateRefresh)
			{
				this.RefreshPerkStates();
			}
		}

		private void RefreshPerkStates()
		{
			this.needsPerkStateRefresh = false;
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			this.InitializePerksGrid();
		}

		private void InitializePerksGrid()
		{
			this.perksGrid.Clear();
			PerkManager perkManager = Service.PerkManager;
			List<ActivatedPerkData> perksAppliedToBuilding = perkManager.GetPerksAppliedToBuilding(this.buildingVO);
			int i = 0;
			int count = perksAppliedToBuilding.Count;
			while (i < count)
			{
				this.AddPerkToGrid(perksAppliedToBuilding[i]);
				i++;
			}
			if (perksAppliedToBuilding.Count > 0)
			{
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			}
			else
			{
				this.Close(null);
				Service.EventManager.SendEvent(EventId.ActivePerksUpdated, null);
			}
		}

		private void AddPerkToGrid(ActivatedPerkData perkData)
		{
			string perkId = perkData.PerkId;
			PerkViewController perkViewController = Service.PerkViewController;
			PerkVO perkVO = Service.StaticDataController.Get<PerkVO>(perkId);
			UXElement uXElement = this.perksGrid.CloneTemplateItem(perkId);
			uXElement.Tag = perkData;
			UXLabel subElement = this.perksGrid.GetSubElement<UXLabel>(perkId, "LabelPerkTitleModalActivePerks");
			subElement.Text = perkViewController.GetPerkNameForGroup(perkVO.PerkGroup);
			UXTexture subElement2 = this.perksGrid.GetSubElement<UXTexture>(perkId, "TexturePerkArtModalActivePerks");
			perkViewController.SetPerkImage(subElement2, perkVO);
			UXLabel subElement3 = this.perksGrid.GetSubElement<UXLabel>(perkId, "LabelPerkLvlModalActivePerks");
			subElement3.Text = StringUtils.GetRomanNumeral(perkVO.PerkTier);
			UXLabel subElement4 = this.perksGrid.GetSubElement<UXLabel>(perkId, "ItemLabelModalActivePerks");
			subElement4.Visible = false;
			UXGrid subElement5 = this.perksGrid.GetSubElement<UXGrid>(perkId, "GridSubModalActivePerks");
			subElement5.SetTemplateItem("ItemLabelModalActivePerks");
			subElement5.Clear();
			PerkManager perkManager = Service.PerkManager;
			int i = 0;
			int num = perkVO.PerkEffects.Length;
			while (i < num)
			{
				string text = perkVO.PerkEffects[i];
				PerkEffectVO perkEffectVO = Service.StaticDataController.Get<PerkEffectVO>(text);
				UXLabel uXLabel = (UXLabel)subElement5.CloneTemplateItem(text);
				uXLabel.Text = this.lang.Get(perkEffectVO.StatStringId, new object[0]) + Service.PerkViewController.GetFormattedValueBasedOnEffectType(perkEffectVO, null);
				if (!perkManager.IsPerkEffectAppliedToBuilding(perkEffectVO, this.buildingVO))
				{
					uXLabel.TextColor = UXUtils.COLOR_PERK_EFFECT_NOT_APPLIED;
				}
				subElement5.AddItem(uXLabel, i);
				i++;
			}
			this.UpdatePerkTimer(uXElement);
			this.perksGrid.AddItem(uXElement, 1);
		}

		private void UpdatePerkTimer(UXElement item)
		{
			ActivatedPerkData activatedPerkData = (ActivatedPerkData)item.Tag;
			UXLabel subElement = this.perksGrid.GetSubElement<UXLabel>(activatedPerkData.PerkId, "LabelTimerModalActivePerks");
			int num = (int)(activatedPerkData.EndTime - ServerTime.Time);
			if (num < 1)
			{
				this.needsPerkStateRefresh = true;
			}
			string text = LangUtils.FormatTime((long)num);
			subElement.Text = text;
		}

		private void InitLabels()
		{
			base.GetElement<UXLabel>("LabelTitleModalActivePerks").Text = this.lang.Get("PERK_BUILDING_INFO_SUMMARY_TITLE", new object[0]);
		}

		public override void OnDestroyElement()
		{
			base.OnDestroyElement();
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			this.perksGrid.Clear();
			this.perksGrid = null;
		}
	}
}
