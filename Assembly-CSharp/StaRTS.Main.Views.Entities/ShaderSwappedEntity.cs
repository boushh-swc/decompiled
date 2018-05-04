using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers.CombineMesh;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Entities
{
	public class ShaderSwappedEntity : ShaderSwappedAsset
	{
		private Entity entity;

		private CombineMeshManager combineMeshManager;

		public Entity Entity
		{
			get
			{
				return this.entity;
			}
		}

		public ShaderSwappedEntity(Entity entity, string shaderName)
		{
			this.entity = entity;
			this.combineMeshManager = Service.CombineMeshManager;
			GameObjectViewComponent gameObjectViewComponent = entity.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent != null)
			{
				base.Init(gameObjectViewComponent.MainGameObject, shaderName);
			}
		}

		protected override void RestoreMaterialForRenderer(Renderer renderer)
		{
			if (this.combineMeshManager.IsRendererCombined((SmartEntity)this.entity, renderer))
			{
				renderer.enabled = false;
			}
			base.RestoreMaterialForRenderer(renderer);
		}

		protected override void EnsureMaterialForRenderer(Renderer renderer, Shader shader)
		{
			if (this.combineMeshManager.IsRendererCombined((SmartEntity)this.entity, renderer))
			{
				renderer.enabled = true;
			}
			base.EnsureMaterialForRenderer(renderer, shader);
		}
	}
}
