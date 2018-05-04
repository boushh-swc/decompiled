using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Utils
{
	public static class PlanetUtils
	{
		private const string ROTATION_SPEED = "_RotationSpeed";

		public static List<PlanetVO> GetAllPlayerFacingPlanets()
		{
			List<PlanetVO> list = new List<PlanetVO>();
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (PlanetVO current in staticDataController.GetAll<PlanetVO>())
			{
				if (current.PlayerFacing)
				{
					list.Add(current);
				}
			}
			return list;
		}

		public static Material StopPlanetSpinning(GameObject spinningPlanetGameObject)
		{
			Material planetMaterial = PlanetUtils.GetPlanetMaterial(spinningPlanetGameObject);
			if (planetMaterial != null)
			{
				planetMaterial.SetFloat("_RotationSpeed", 0f);
			}
			return planetMaterial;
		}

		public static Material GetPlanetMaterial(GameObject planetGameObject)
		{
			if (planetGameObject != null)
			{
				MeshRenderer componentInChildren = planetGameObject.GetComponentInChildren<MeshRenderer>();
				return UnityUtils.EnsureMaterialCopy(componentInChildren);
			}
			return null;
		}
	}
}
