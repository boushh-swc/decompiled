using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using UnityEngine;

namespace StaRTS.FX
{
	public class DefaultLightingEffects : AbstractLightingEffects, IEventObserver
	{
		private bool IsCEEState;

		public DefaultLightingEffects()
		{
			Service.EventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.BeforeDefault);
			Service.EventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.BeforeDefault);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.GameStateChanged)
			{
				if (id == EventId.BuildingViewReady)
				{
					this.UpdateEnvironmentLighting(0f);
				}
			}
			else
			{
				this.UpdateEnvironmentLighting(0f);
			}
			return EatResponse.NotEaten;
		}

		public override void SetDefaultColors()
		{
			base.SetDefaultColors();
			base.PLColorBuildingLight = this.defaultColor;
			base.PLColorBuildingDark = this.defaultColor;
			base.PLColorTerrainLight = this.defaultColor;
			base.PLColorTerrainDark = this.defaultColor;
			base.PLColorShadow = this.defaultColor;
			base.PLColorUnits = this.defaultColor;
			base.PLColorWall = this.defaultColor;
			base.PLColorGrid = this.defaultColor;
			base.PLColorGridBuildings = this.defaultColor;
		}

		public override void RefreshShaderColors()
		{
			base.RefreshShaderColors();
			Shader.SetGlobalColor("_PL_Buildings_Light", base.PLColorBuildingLight);
			Shader.SetGlobalColor("_PL_Buildings_Dark", base.PLColorBuildingDark);
			Shader.SetGlobalColor("_PL_Terrain_Light", base.PLColorTerrainLight);
			Shader.SetGlobalColor("_PL_Terrain_Dark", base.PLColorTerrainDark);
			Shader.SetGlobalColor("_PL_Shadow", base.PLColorShadow);
			Shader.SetGlobalColor("_PL_Units", base.PLColorUnits);
		}

		public override void UpdateEnvironmentLighting(float dt)
		{
			base.UpdateEnvironmentLighting(dt);
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is BattleStartState || currentState is HomeState || currentState is NeighborVisitState)
			{
				this.UpdateEnvironmentForPlanetaryLighting();
			}
		}

		private void UpdateEnvironmentForPlanetaryLighting()
		{
			IMapDataLoader mapDataLoader = Service.WorldTransitioner.GetMapDataLoader();
			if (mapDataLoader == null)
			{
				return;
			}
			this.planetVO = mapDataLoader.GetPlanetData();
			if (this.planetVO == null)
			{
				this.planetVO = Service.StaticDataController.Get<PlanetVO>("planet1");
			}
			this.planetLightingUid = this.planetVO.PlanetaryLighting;
			if (this.planetLightingUid != null)
			{
				this.SetShaderColorsFromPlanetLightingVO(this.planetLightingUid);
			}
		}

		public void ResetPlanetColors()
		{
			if (this.planetLightingUid != null)
			{
				this.SetShaderColorsFromPlanetLightingVO(this.planetLightingUid);
			}
			else
			{
				this.SetDefaultColors();
			}
		}

		public override Color GetCurrentLightingColor(LightingColorType type)
		{
			if (this.planetVO == null)
			{
				return this.defaultColor;
			}
			DefaultLightingVO defaultLightingVO = Service.StaticDataController.Get<DefaultLightingVO>(this.planetVO.PlanetaryLighting);
			string hexColor = string.Empty;
			switch (type)
			{
			case LightingColorType.BuildingColorDark:
				hexColor = defaultLightingVO.LightingColorDark;
				break;
			case LightingColorType.BuildingColorLight:
				hexColor = defaultLightingVO.LightingColorLight;
				break;
			case LightingColorType.UnitColor:
				hexColor = defaultLightingVO.LightingColorMedian;
				break;
			case LightingColorType.ShadowColor:
				hexColor = defaultLightingVO.LightingColorShadow;
				break;
			case LightingColorType.GroundColor:
				hexColor = defaultLightingVO.LightingColorGround;
				break;
			case LightingColorType.GroundColorLight:
				hexColor = defaultLightingVO.LightingColorGroundLight;
				break;
			case LightingColorType.GridColor:
				hexColor = defaultLightingVO.LightingColorGrid;
				break;
			case LightingColorType.WallGridColor:
				hexColor = defaultLightingVO.LightingColorWallGrid;
				break;
			}
			return FXUtils.ConvertHexStringToColorObject(hexColor);
		}

		private void SetShaderColorsFromPlanetLightingVO(string planetLightUid)
		{
			DefaultLightingVO defaultLightingVO = Service.StaticDataController.Get<DefaultLightingVO>(planetLightUid);
			string lightingColorDark = defaultLightingVO.LightingColorDark;
			base.PLColorBuildingDark = FXUtils.ConvertHexStringToColorObject(lightingColorDark);
			Shader.SetGlobalColor("_PL_Buildings_Dark", base.PLColorBuildingDark);
			string lightingColorLight = defaultLightingVO.LightingColorLight;
			base.PLColorBuildingLight = FXUtils.ConvertHexStringToColorObject(lightingColorLight);
			Shader.SetGlobalColor("_PL_Buildings_Light", base.PLColorBuildingLight);
			string lightingColorMedian = defaultLightingVO.LightingColorMedian;
			base.PLColorUnits = FXUtils.ConvertHexStringToColorObject(lightingColorMedian);
			Shader.SetGlobalColor("_PL_Units", base.PLColorUnits);
			string lightingColorGround = defaultLightingVO.LightingColorGround;
			base.PLColorTerrainDark = FXUtils.ConvertHexStringToColorObject(lightingColorGround);
			Shader.SetGlobalColor("_PL_Terrain_Dark", base.PLColorTerrainDark);
			string lightingColorGroundLight = defaultLightingVO.LightingColorGroundLight;
			base.PLColorTerrainLight = FXUtils.ConvertHexStringToColorObject(lightingColorGroundLight);
			Shader.SetGlobalColor("_PL_Terrain_Light", base.PLColorTerrainLight);
			string lightingColorShadow = defaultLightingVO.LightingColorShadow;
			base.PLColorShadow = FXUtils.ConvertHexStringToColorObject(lightingColorShadow);
			Shader.SetGlobalColor("_PL_Shadow", base.PLColorShadow);
		}
	}
}
