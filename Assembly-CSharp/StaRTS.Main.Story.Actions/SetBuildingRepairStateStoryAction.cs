using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Entities.Systems;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Story.Actions
{
	public class SetBuildingRepairStateStoryAction : AbstractStoryAction
	{
		private const int ARG_REPAIR_PERCENT = 0;

		private const int ARG_BUILDING_TYPE = 1;

		private const int ARG_AREA_X = 2;

		private const int ARG_AREA_Z = 3;

		private const int ARG_AREA_W = 4;

		private const int ARG_AREA_H = 5;

		private float repairPercent;

		private Rect area;

		private BuildingType type;

		public SetBuildingRepairStateStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(new int[]
			{
				1,
				2,
				6
			});
			this.repairPercent = (float)Convert.ToInt32(this.prepareArgs[0]) / 100f;
			if (this.prepareArgs.Length >= 2)
			{
				this.type = StringUtils.ParseEnum<BuildingType>(this.prepareArgs[1]);
			}
			else
			{
				this.type = BuildingType.Any;
			}
			if (this.prepareArgs.Length >= 6)
			{
				this.area = new Rect((float)Convert.ToInt32(this.prepareArgs[2]), (float)Convert.ToInt32(this.prepareArgs[3]), (float)Convert.ToInt32(this.prepareArgs[4]), (float)Convert.ToInt32(this.prepareArgs[5]));
			}
			else
			{
				this.area = new Rect(-21f, -21f, 42f, 42f);
			}
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			List<SmartEntity> buildingListByType = Service.BuildingLookupController.GetBuildingListByType(this.type);
			PostBattleRepairController postBattleRepairController = Service.PostBattleRepairController;
			for (int i = 0; i < buildingListByType.Count; i++)
			{
				BuildingComponent buildingComp = buildingListByType[i].BuildingComp;
				if (this.area.Contains(new Vector2((float)buildingComp.BuildingTO.X, (float)buildingComp.BuildingTO.Z)))
				{
					postBattleRepairController.ForceRepairOnBuilding(buildingComp.Entity, this.repairPercent);
				}
			}
			Service.EntityController.GetViewSystem<HealthRenderSystem>().ForceUpdate();
			this.parent.ChildComplete(this);
		}
	}
}
