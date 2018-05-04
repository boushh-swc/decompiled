using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class TransportSystem : ViewSystemBase
	{
		private const float FADE_FACTOR = 15f;

		private const float SCALE_FACTOR = 30f;

		private EntityController entityController;

		private NodeList<TransportNode> nodeList;

		public override void AddToGame(Game game)
		{
			this.entityController = Service.EntityController;
			this.nodeList = this.entityController.GetNodeList<TransportNode>();
		}

		public override void RemoveFromGame(Game game)
		{
			this.entityController = null;
			this.nodeList = null;
		}

		protected override void Update(float dt)
		{
			for (TransportNode transportNode = this.nodeList.Head; transportNode != null; transportNode = transportNode.Next)
			{
				GameObject gameObj = transportNode.Transport.GameObj;
				if (transportNode.State.CurState == EntityState.Moving && gameObj != null)
				{
					Vector3 position;
					Quaternion rotation;
					if (transportNode.Transport.Spline.Update(dt, out position, out rotation))
					{
						transportNode.State.CurState = EntityState.Idle;
						gameObj.SetActive(false);
						Service.EntityFactory.DestroyTransportEntity(transportNode.Entity);
					}
					else if (transportNode.Transport.ShadowMaterial != null)
					{
						Transform transform = gameObj.transform;
						transform.position = position;
						transform.rotation = rotation;
						GameObject shadowGameObject = transportNode.Transport.ShadowGameObject;
						Transform transform2 = shadowGameObject.transform;
						transform2.position = new Vector3(position.x, Mathf.Clamp(position.y, 0f, 1f), position.z);
						transform2.localScale = new Vector3(1f + position.y / 30f, 1f, 1f + position.y / 30f);
						transform2.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
						float a = Mathf.Clamp(1f - position.y / 15f, 0f, 1f);
						transportNode.Transport.ShadowMaterial.color = new Color(0f, 0f, 0f, a);
					}
				}
			}
		}
	}
}
