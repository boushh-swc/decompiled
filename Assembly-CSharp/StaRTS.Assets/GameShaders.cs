using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Assets
{
	public class GameShaders
	{
		public const string SIMPLE_SOLID_COLOR = "SimpleSolidColor";

		public const string UNLIT_TEXTURE_FADE = "UnlitTexture_Fade";

		public const string WIPE_LINEAR = "Wipe_Linear";

		public const string WIPE_ELLIPTICAL = "Wipe_Elliptical";

		public const string OUTLINE_UNLIT = "Outline_Unlit";

		public const string SCROLL_HORIZONTAL = "Scroll_Horizontal";

		public const string SPAWN_PROTECTION_PL = "Grid_Protection_PL";

		public const string FOOT_PRINT_PL = "Grid_Building_Color_PL";

		public const string TRANSPORT_SHADOW = "TransportShadow";

		public const string PLANETARY_LIGHTING_1COLOR = "Unlit_Texture_Planet_1Color";

		public const string PLANETARY_LIGHTING_2COLOR = "Unlit_Texture_Planet_2Color";

		public const string PLANETARY_LIGHTING_SHADOW = "ShadowMultiplyPlanetColor_Index-1";

		public const string PLANETARY_LIGHTING_INNER_TERRAIN = "GroundPlanetaryVertexColor_Index-10";

		public const string PLANETARY_LIGHTING_OUTER_TERRAIN = "GroundPlanetaryVertexColor_Index0";

		public const string PLANETARY_LIGHTING_OUTER_SHADOW = "ShadowMultiplyPlanetColor_Index0";

		public const string UNLIT_TEXTURE_BOOST = "UnlitTexture_Color_Boosted";

		public const string SCROLL_XY_ALPHA = "Scroll_XY_Alpha";

		public const string UNLIT_PREMULTIPLIED_COLORED = "Unlit/Premultiplied Colored";

		public const string UNLIT_TRANSPARENT_COLORED = "Unlit/Transparent Colored";

		public const string WARBOARD_BUILDING_HOLO_FADE = "PL_2Color_Mask_HoloBldg";

		public const string WARBOARD_BUILDING_OUTLINE = "PL_2Color_Mask_HoloBldg_Outline";

		public const string BUILDING_PERK_EFFECT_HIGHLIGHT = "PL_2Color_Mask_SA";

		public const string HOLOGRAM_EFFECT = "HologramScanlines";

		public const string PLANETARY_EQUIPMENT_PREFIX = "PL_2";

		public const string SIMPLE_SOLID_COLOR_ATTRIBUTE = "_Pigment";

		private Dictionary<string, Shader> shaders;

		public GameShaders(AssetsCompleteDelegate onCompleteCallback, object onCompleteCookie)
		{
			this.shaders = new Dictionary<string, Shader>();
			List<string> list = new List<string>();
			StaticDataController staticDataController = Service.StaticDataController;
			Dictionary<string, ShaderTypeVO>.ValueCollection all = staticDataController.GetAll<ShaderTypeVO>();
			foreach (ShaderTypeVO current in all)
			{
				list.Add(current.AssetName);
			}
			staticDataController.Unload<ShaderTypeVO>();
			List<object> list2 = new List<object>();
			List<AssetHandle> list3 = new List<AssetHandle>();
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				list2.Add(list[i]);
				list3.Add(AssetHandle.Invalid);
				i++;
			}
			Service.AssetManager.MultiLoad(list3, list, new AssetSuccessDelegate(this.LoadSuccess), null, list2, onCompleteCallback, onCompleteCookie);
		}

		private void LoadSuccess(object asset, object cookie)
		{
			Shader value = asset as Shader;
			string key = cookie as string;
			this.shaders.Add(key, value);
		}

		public Shader GetShader(string shaderName)
		{
			return (string.IsNullOrEmpty(shaderName) || !this.shaders.ContainsKey(shaderName)) ? null : this.shaders[shaderName];
		}
	}
}
