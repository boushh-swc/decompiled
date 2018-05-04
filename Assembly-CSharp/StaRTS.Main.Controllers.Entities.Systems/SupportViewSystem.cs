using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class SupportViewSystem : ViewSystemBase
	{
		private EntityController entityController;

		private ISupportController supportController;

		private NodeList<SupportViewNode> nodeList;

		private float accumulatedDt;

		private bool requireViewRefresh;

		public override void AddToGame(Game game)
		{
			this.entityController = Service.EntityController;
			this.supportController = Service.ISupportController;
			this.accumulatedDt = 0f;
			this.requireViewRefresh = true;
			this.nodeList = this.entityController.GetNodeList<SupportViewNode>();
		}

		public override void RemoveFromGame(Game game)
		{
			this.nodeList = this.entityController.GetNodeList<SupportViewNode>();
			for (SupportViewNode supportViewNode = this.nodeList.Head; supportViewNode != null; supportViewNode = supportViewNode.Next)
			{
				if (supportViewNode.SupportView != null)
				{
					supportViewNode.SupportView.TeardownElements();
				}
			}
			this.entityController = null;
			this.supportController = null;
			this.nodeList = null;
			this.accumulatedDt = 0f;
		}

		protected override void Update(float dt)
		{
			if (this.nodeList == null)
			{
				return;
			}
			bool flag = false;
			this.accumulatedDt += dt;
			if (this.accumulatedDt >= 0.1f)
			{
				flag = true;
				this.accumulatedDt = 0f;
			}
			for (SupportViewNode supportViewNode = this.nodeList.Head; supportViewNode != null; supportViewNode = supportViewNode.Next)
			{
				if (flag)
				{
					Contract contract = this.supportController.FindCurrentContract(supportViewNode.BuildingComp.BuildingTO.Key);
					if (contract != null)
					{
						int remainingTimeForView = contract.GetRemainingTimeForView();
						if (remainingTimeForView <= 0)
						{
							supportViewNode.SupportView.Refresh();
						}
						else
						{
							if (this.requireViewRefresh)
							{
								supportViewNode.SupportView.Refresh();
							}
							supportViewNode.SupportView.UpdateTime(remainingTimeForView, contract.TotalTime, this.requireViewRefresh);
						}
					}
				}
				supportViewNode.SupportView.UpdateLocation();
			}
			if (this.requireViewRefresh && flag)
			{
				this.requireViewRefresh = false;
			}
		}
	}
}
