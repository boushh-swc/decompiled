using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class EntityRenderSystem : ViewSystemBase
	{
		private NodeList<EntityRenderNode> nodeList;

		private EntityRenderController entityRenderController;

		public override void AddToGame(Game game)
		{
			this.entityRenderController = Service.EntityRenderController;
			this.nodeList = Service.EntityController.GetNodeList<EntityRenderNode>();
		}

		public override void RemoveFromGame(Game game)
		{
			this.entityRenderController = null;
			this.nodeList = null;
		}

		protected override void Update(float dt)
		{
			Vector3 zero = Vector3.zero;
			for (EntityRenderNode entityRenderNode = this.nodeList.Head; entityRenderNode != null; entityRenderNode = entityRenderNode.Next)
			{
				Vector3 vector = zero;
				SmartEntity smartEntity = (SmartEntity)entityRenderNode.Entity;
				GameObjectViewComponent gameObjectViewComp = smartEntity.GameObjectViewComp;
				StateComponent stateComp = smartEntity.StateComp;
				EntityState rawState = stateComp.RawState;
				if (rawState == EntityState.Moving)
				{
					PathView pathView = smartEntity.PathingComp.PathView;
					if (pathView != null)
					{
						pathView.TimeOnPathSegment += dt;
						vector = this.entityRenderController.MoveGameObject(gameObjectViewComp, pathView, smartEntity.SizeComp.Width);
					}
				}
				if (rawState != EntityState.Dying)
				{
					this.entityRenderController.RotateGameObject(smartEntity, vector.x, vector.z, dt);
				}
				else if (smartEntity.TroopComp != null)
				{
					gameObjectViewComp.UpdateAllAttachments();
				}
				if (smartEntity.TroopComp != null || smartEntity.DroidComp != null)
				{
					bool isAbilityModeActive = smartEntity.TroopComp != null && smartEntity.TroopComp.IsAbilityModeActive;
					this.entityRenderController.UpdateAnimationState(gameObjectViewComp, stateComp, isAbilityModeActive);
				}
			}
		}
	}
}
