using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class HQUpgradeScreen : BuildingInfoScreen
	{
		private const int CENTER_THRESHOLD = 7;

		private const string ITEM_TURRETS = "TURRETS";

		private const string LABEL_TURRETS = "LabelTurrets";

		private const string ICON_INFO_BUTTON = "SpriteUnlockItemInfo";

		private UXGrid itemGrid;

		public HQUpgradeScreen(Entity selectedBuilding) : base(selectedBuilding)
		{
			this.useUpgradeGroup = true;
		}

		public override void OnDestroyElement()
		{
			if (this.itemGrid != null)
			{
				this.itemGrid.Clear();
				this.itemGrid = null;
			}
			base.OnDestroyElement();
		}

		protected override void InitGroups()
		{
			base.InitGroups();
			base.GetElement<UXElement>("UnlockItems").Visible = true;
		}

		protected override void InitLabels()
		{
			base.InitLabels();
			if (this.nextBuildingInfo != null)
			{
				this.labelUnlockDesc.Visible = true;
				this.labelUnlockDesc.Text = this.lang.Get("UPGRADE_HQ_LOOT_NOTE", new object[]
				{
					this.nextBuildingInfo.Lvl
				});
				this.labelUnlock = base.GetElement<UXLabel>("LabelUnlock");
				this.labelUnlock.Text = this.lang.Get("UNLOCKS_BUILDINGS", new object[0]);
			}
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();
			this.SetupItemGrid();
		}

		private void SetupItemGrid()
		{
			this.itemGrid = base.GetElement<UXGrid>("BuildingUnlockGrid");
			this.itemGrid.SetTemplateItem("BuildingUnlockTemplate");
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			Dictionary<BuildingTypeVO, int> buildingsUnlockedBy = buildingLookupController.GetBuildingsUnlockedBy(this.nextBuildingInfo);
			int num = 0;
			foreach (KeyValuePair<BuildingTypeVO, int> current in buildingsUnlockedBy)
			{
				BuildingTypeVO key = current.Key;
				int value = current.Value;
				if (key.Type == BuildingType.Turret && key.BuildingRequirement != this.nextBuildingInfo.Uid)
				{
					if (num == 0)
					{
						num = value;
					}
				}
				else
				{
					string uid = key.Uid;
					UXElement item = this.itemGrid.CloneTemplateItem(uid);
					UXSprite subElement = this.itemGrid.GetSubElement<UXSprite>(uid, "SpriteItemImage");
					ProjectorConfig projectorConfig = ProjectorUtils.GenerateBuildingConfig(key, subElement);
					projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
					ProjectorUtils.GenerateProjector(projectorConfig);
					UXLabel subElement2 = this.itemGrid.GetSubElement<UXLabel>(uid, "LabelUnlockCount");
					if (key.Type == BuildingType.Turret)
					{
						subElement2.Visible = false;
					}
					else
					{
						subElement2.Text = this.lang.Get("TROOP_MULTIPLIER", new object[]
						{
							value
						});
					}
					this.itemGrid.GetSubElement<UXSprite>(uid, "SpriteUnlockItemInfo").Visible = false;
					this.itemGrid.AddItem(item, key.Order);
				}
			}
			UXElement item2 = this.itemGrid.CloneTemplateItem("TURRETS");
			UXSprite subElement3 = this.itemGrid.GetSubElement<UXSprite>("TURRETS", "SpriteItemImage");
			subElement3.Visible = false;
			UXLabel subElement4 = this.itemGrid.GetSubElement<UXLabel>("TURRETS", "LabelUnlockCount");
			subElement4.Visible = false;
			UXLabel subElement5 = this.itemGrid.GetSubElement<UXLabel>("TURRETS", "LabelTurrets");
			subElement5.Visible = true;
			subElement5.Text = this.lang.Get("HQ_UPGRADE_TURRETS_UNLOCKED", new object[]
			{
				num
			});
			this.itemGrid.GetSubElement<UXSprite>("TURRETS", "SpriteUnlockItemInfo").Visible = false;
			this.itemGrid.AddItem(item2, 99999999);
			this.itemGrid.RepositionItems();
			this.itemGrid.Scroll((this.itemGrid.Count <= 7) ? 0.5f : 0f);
		}
	}
}
