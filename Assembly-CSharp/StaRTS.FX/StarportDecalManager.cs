using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.FX
{
	public class StarportDecalManager : IEventObserver
	{
		public const string FX_STARPORT_DECAL_MESH_NAME = "numberMesh";

		public static readonly float[] FX_STARPORT_DECAL_OFFSET = new float[]
		{
			0f,
			0.666f,
			0.333f
		};

		private List<Material> decalMaterials = new List<Material>();

		public StarportDecalManager()
		{
			Service.EventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.WorldReset, EventPriority.Default);
		}

		private void SetStarportDecal(Entity entity)
		{
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			if (buildingComponent.BuildingType.Type == BuildingType.Starport)
			{
				int lvl = buildingComponent.BuildingType.Lvl;
				int num = (lvl - 1) % 3;
				float x = StarportDecalManager.FX_STARPORT_DECAL_OFFSET[num];
				Vector2 mainTextureOffset = new Vector2(x, 0f);
				GameObjectViewComponent gameObjectViewComponent = entity.Get<GameObjectViewComponent>();
				if (gameObjectViewComponent != null)
				{
					Transform[] componentsInChildren = gameObjectViewComponent.MainGameObject.GetComponentsInChildren<Transform>();
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						if (componentsInChildren[i].gameObject.name.Contains("numberMesh"))
						{
							GameObject gameObject = componentsInChildren[i].gameObject;
							Renderer component = gameObject.GetComponent<Renderer>();
							Material material = UnityUtils.EnsureMaterialCopy(component);
							material.mainTextureOffset = mainTextureOffset;
							this.decalMaterials.Add(material);
						}
					}
				}
			}
		}

		private void DestroyMaterials()
		{
			for (int i = 0; i < this.decalMaterials.Count; i++)
			{
				UnityUtils.DestroyMaterial(this.decalMaterials[i]);
			}
			this.decalMaterials.Clear();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.BuildingViewReady)
			{
				if (id == EventId.WorldReset)
				{
					this.DestroyMaterials();
				}
			}
			else
			{
				Entity entity = ((EntityViewParams)cookie).Entity;
				this.SetStarportDecal(entity);
			}
			return EatResponse.NotEaten;
		}
	}
}
