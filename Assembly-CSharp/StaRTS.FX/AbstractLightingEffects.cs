using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using System;
using UnityEngine;

namespace StaRTS.FX
{
	public class AbstractLightingEffects
	{
		protected const string PL_COLOR_BUILDINGS_LIGHT = "_PL_Buildings_Light";

		protected const string PL_COLOR_BUILDINGS_DARK = "_PL_Buildings_Dark";

		protected const string PL_COLOR_TERRAIN_LIGHT = "_PL_Terrain_Light";

		protected const string PL_COLOR_TERRAIN_DARK = "_PL_Terrain_Dark";

		protected const string PL_COLOR_SHADOW = "_PL_Shadow";

		protected const string PL_COLOR_GRID_WALL = "_PL_Grid_Wall";

		protected const string PL_COLOR_GRID = "_PL_Grid";

		protected const string PL_COLOR_GRID_BUILDINGS = "_PL_Grid_Buildings";

		protected const string PL_COLOR_UNITS = "_PL_Units";

		protected const string PLANETARY_OUTER_SHADOW_MATERIAL = "_outerShadow";

		protected PlanetVO planetVO;

		protected string planetLightingUid;

		protected Color defaultColor = new Color(0.5f, 0.5f, 0.5f, 1f);

		public Color PLColorBuildingLight
		{
			get;
			set;
		}

		public Color PLColorBuildingDark
		{
			get;
			set;
		}

		public Color PLColorTerrainLight
		{
			get;
			set;
		}

		public Color PLColorTerrainDark
		{
			get;
			set;
		}

		public Color PLColorShadow
		{
			get;
			set;
		}

		public Color PLColorUnits
		{
			get;
			set;
		}

		public Color PLColorWall
		{
			get;
			set;
		}

		public Color PLColorGrid
		{
			get;
			set;
		}

		public Color PLColorGridBuildings
		{
			get;
			set;
		}

		public AbstractLightingEffects()
		{
			this.SetDefaultColors();
			this.RefreshShaderColors();
		}

		public virtual void SetDefaultColors()
		{
		}

		public virtual void RefreshShaderColors()
		{
		}

		public virtual Color GetCurrentLightingColor(LightingColorType type)
		{
			return this.defaultColor;
		}

		public virtual void UpdateEnvironmentLighting(float dt)
		{
		}

		public virtual void ApplyDelayedLightingDataOverride(EventId triggerEvent, string dataAssetName)
		{
		}

		public virtual void RemoveLightingDataOverride()
		{
		}

		public virtual void SetupDelayedLightingOverrideRemoval(EventId triggerEvent)
		{
		}
	}
}
