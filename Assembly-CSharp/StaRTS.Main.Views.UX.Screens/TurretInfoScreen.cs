using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TurretInfoScreen : BuildingInfoScreen
	{
		private const string TARGET_PREF_PREFIX = "target_pref_";

		private const string LABEL_LEFT_1 = "LabelLeftColumn1";

		private const string LABEL_RIGHT_1 = "LabelRightColumn1";

		private const string LABEL_LEFT_2 = "LabelLeftColumn2";

		private const string LABEL_RIGHT_2 = "LabelRightColumn2";

		private const string LABEL_LEFT_3 = "LabelLeftColumn3";

		private const string LABEL_RIGHT_3 = "LabelRightColumn3";

		private const string LABEL_LEFT_4 = "LabelLeftColumn4";

		private const string LABEL_RIGHT_4 = "LabelRightColumn4";

		protected const int TURRET_SLIDER_HITPOINTS = 0;

		protected const int TURRET_SLIDER_DPS = 1;

		protected const int TURRET_SLIDER_COUNT = 2;

		private TurretTypeVO turretInfo;

		public TurretInfoScreen(SmartEntity turretBuilding) : base(turretBuilding)
		{
			this.useTurretGroup = true;
		}

		protected override void SetSelectedBuilding(SmartEntity newSelectedBuilding)
		{
			base.SetSelectedBuilding(newSelectedBuilding);
			this.turretInfo = Service.StaticDataController.Get<TurretTypeVO>(this.buildingInfo.TurretUid);
		}

		protected override void InitGroups()
		{
			base.InitGroups();
			base.GetElement<UXElement>("BuildingInfo").Visible = true;
			base.GetElement<UXElement>("InfoTurret").Visible = true;
			base.GetElement<UXElement>("Info").Visible = false;
			base.GetElement<UXElement>("BuildingInfoStorage").Visible = false;
			base.GetElement<UXElement>("SwapGroup").Visible = false;
			base.GetElement<UXElement>("UnlockItems").Visible = false;
			base.GetElement<UXElement>("UpgradeTime").Visible = false;
		}

		protected override void OnLoaded()
		{
			base.InitControls(2);
			this.InitHitpoints(0);
			this.UpdateDps(1);
			this.SetupDetailsTable();
		}

		protected void UpdateDps(int sliderIndex)
		{
			BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
			StaticDataController staticDataController = Service.StaticDataController;
			BuildingTypeVO maxLevel = buildingUpgradeCatalog.GetMaxLevel(this.buildingInfo.UpgradeGroup);
			TurretTypeVO turretTypeVO = staticDataController.Get<TurretTypeVO>(maxLevel.TurretUid);
			if (turretTypeVO.ProjectileType.IsBeam)
			{
				int beamDamage = turretTypeVO.ProjectileType.BeamDamage;
				int beamDamage2 = this.turretInfo.ProjectileType.BeamDamage;
				this.sliders[1].DescLabel.Text = this.lang.Get("BEAM_DAMAGE", new object[]
				{
					beamDamage2
				});
				this.sliders[sliderIndex].CurrentLabel.Text = string.Empty;
				this.sliders[sliderIndex].CurrentSlider.Value = ((beamDamage != 0) ? ((float)beamDamage2 / (float)beamDamage) : 0f);
				if (this.useUpgradeGroup && this.nextBuildingInfo != null)
				{
					BuildingTypeVO nextLevel = buildingUpgradeCatalog.GetNextLevel(this.buildingInfo);
					int beamDamage3 = staticDataController.Get<TurretTypeVO>(nextLevel.TurretUid).ProjectileType.BeamDamage;
					this.sliders[sliderIndex].NextLabel.Text = this.lang.Get("PLUS", new object[]
					{
						this.lang.ThousandsSeparated(beamDamage3 - beamDamage2)
					});
					this.sliders[sliderIndex].NextSlider.Value = ((beamDamage != 0) ? ((float)beamDamage3 / (float)beamDamage) : 0f);
				}
			}
			else
			{
				int dPS = this.turretInfo.DPS;
				int dPS2 = turretTypeVO.DPS;
				this.sliders[1].DescLabel.Text = this.lang.Get("DAMAGE_DPS", new object[]
				{
					string.Empty
				});
				this.sliders[sliderIndex].CurrentLabel.Text = this.lang.ThousandsSeparated(dPS);
				this.sliders[sliderIndex].CurrentSlider.Value = ((dPS2 != 0) ? ((float)dPS / (float)dPS2) : 0f);
				if (this.useUpgradeGroup && this.nextBuildingInfo != null)
				{
					BuildingTypeVO nextLevel2 = buildingUpgradeCatalog.GetNextLevel(this.buildingInfo);
					int dPS3 = staticDataController.Get<TurretTypeVO>(nextLevel2.TurretUid).DPS;
					this.sliders[sliderIndex].NextLabel.Text = this.lang.Get("PLUS", new object[]
					{
						this.lang.ThousandsSeparated(dPS3 - dPS)
					});
					this.sliders[sliderIndex].NextSlider.Value = ((dPS2 != 0) ? ((float)dPS3 / (float)dPS2) : 0f);
				}
			}
		}

		protected void SetupDetailsTable()
		{
			uint minAttackRange = this.turretInfo.MinAttackRange;
			uint maxAttackRange = this.turretInfo.MaxAttackRange;
			base.GetElement<UXLabel>("LabelLeftColumn1").Text = this.lang.Get("RANGE", new object[0]);
			if (minAttackRange == 0u)
			{
				base.GetElement<UXLabel>("LabelRightColumn1").Text = this.lang.Get("TILE_COUNT", new object[]
				{
					maxAttackRange
				});
			}
			else
			{
				base.GetElement<UXLabel>("LabelRightColumn1").Text = this.lang.Get("TILE_RANGE", new object[]
				{
					minAttackRange,
					maxAttackRange
				});
			}
			base.GetElement<UXLabel>("LabelLeftColumn2").Text = this.lang.Get("DAMAGE_TYPE", new object[0]);
			base.GetElement<UXLabel>("LabelRightColumn2").Text = this.lang.Get((!this.turretInfo.ProjectileType.SeeksTarget) ? "AREA_OF_EFFECT" : "SINGLE_TARGET", new object[0]);
			base.GetElement<UXLabel>("LabelLeftColumn3").Text = this.lang.Get("TARGET", new object[0]);
			base.GetElement<UXLabel>("LabelRightColumn3").Text = this.lang.Get((!this.turretInfo.ProjectileType.SeeksTarget) ? "ATTACK_TARGETS_GROUND" : "ATTACK_TARGETS_ENEMY", new object[0]);
			base.GetElement<UXLabel>("LabelLeftColumn4").Text = this.lang.Get("FAVORITE_TARGET", new object[0]);
			base.GetElement<UXLabel>("LabelRightColumn4").Text = this.lang.Get("target_pref_" + this.turretInfo.FavoriteTargetType, new object[0]);
		}
	}
}
