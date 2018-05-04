using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Story.Actions
{
	public class ShowBuildingTooltipByTypeStoryAction : AbstractStoryAction
	{
		private const int ARG_BUILDING_TYPE = 0;

		private const int ARG_AREA_X = 1;

		private const int ARG_AREA_Z = 2;

		private const int ARG_AREA_W = 3;

		private const int ARG_AREA_H = 4;

		private bool show;

		private Rect area;

		private BuildingType type;

		public ShowBuildingTooltipByTypeStoryAction(StoryActionVO vo, IStoryReactor parent, bool show) : base(vo, parent)
		{
			this.show = show;
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(new int[]
			{
				1,
				5
			});
			this.type = StringUtils.ParseEnum<BuildingType>(this.prepareArgs[0]);
			if (this.prepareArgs.Length >= 5)
			{
				this.area = new Rect((float)Convert.ToInt32(this.prepareArgs[1]), (float)Convert.ToInt32(this.prepareArgs[2]), (float)Convert.ToInt32(this.prepareArgs[3]), (float)Convert.ToInt32(this.prepareArgs[4]));
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
			for (int i = 0; i < buildingListByType.Count; i++)
			{
				SmartEntity smartEntity = buildingListByType[i];
				BuildingComponent buildingComp = smartEntity.BuildingComp;
				if (this.area.Contains(new Vector2((float)buildingComp.BuildingTO.X, (float)buildingComp.BuildingTO.Z)))
				{
					if (!ContractUtils.IsBuildingConstructing(smartEntity))
					{
						if (smartEntity.HealthViewComp != null)
						{
							smartEntity.HealthViewComp.SetEnabled(this.show);
						}
						if (smartEntity.SupportViewComp != null)
						{
							smartEntity.SupportViewComp.SetEnabled(this.show);
						}
						if (smartEntity.GeneratorViewComp != null)
						{
							GeneratorViewComponent generatorViewComp = smartEntity.GeneratorViewComp;
							if (this.show)
							{
								generatorViewComp.SetEnabled(this.show);
								NodeList<GeneratorViewNode> nodeList = Service.EntityController.GetNodeList<GeneratorViewNode>();
								for (GeneratorViewNode generatorViewNode = nodeList.Head; generatorViewNode != null; generatorViewNode = generatorViewNode.Next)
								{
									if (generatorViewNode.Entity == smartEntity)
									{
										Service.ICurrencyController.UpdateGeneratorAccruedCurrency(smartEntity);
									}
								}
							}
							else
							{
								generatorViewComp.ShowCollectButton(false);
								generatorViewComp.SetEnabled(this.show);
							}
						}
						if (this.show)
						{
							Service.BuildingTooltipController.EnsureBuildingTooltip(smartEntity);
						}
					}
				}
			}
			this.parent.ChildComplete(this);
		}
	}
}
