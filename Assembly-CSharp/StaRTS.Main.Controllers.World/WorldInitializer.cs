using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Main.Views.World;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.World
{
	public class WorldInitializer : IEventObserver
	{
		private List<Entity> entitiesToLoad;

		private bool isPlanetAssetsLoaded;

		public PlanetView View
		{
			get;
			private set;
		}

		public bool AllowInvalidMapLoad
		{
			get;
			set;
		}

		public WorldInitializer()
		{
			Service.WorldInitializer = this;
			this.View = new PlanetView();
			this.AllowInvalidMapLoad = false;
		}

		public void PrepareWorld(Map map)
		{
			this.isPlanetAssetsLoaded = false;
			this.ProcessMapData(map);
			this.View.Prepare(map.Planet, new AssetsCompleteDelegate(this.OnWorldViewComplete), null);
		}

		public void ProcessMapData(Map map)
		{
			WorldController worldController = Service.WorldController;
			this.entitiesToLoad = new List<Entity>();
			Service.EventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.AfterDefault);
			Service.EventManager.RegisterObserver(this, EventId.BuildingViewFailed, EventPriority.AfterDefault);
			worldController.ResetWorld();
			Service.EventManager.SendEvent(EventId.MapDataProcessingStart, map);
			bool flag = Service.WorldTransitioner.IsCurrentWorldHome();
			if (map.Buildings != null)
			{
				List<Entity> list = null;
				for (int i = 0; i < map.Buildings.Count; i++)
				{
					try
					{
						BuildingTypeVO buildingTypeVO = Service.StaticDataController.Get<BuildingTypeVO>(map.Buildings[i].Uid);
						if (buildingTypeVO.Type != BuildingType.Clearable || flag)
						{
							bool flag2 = false;
							Entity item = worldController.ProcessWorldDataBuilding(map.Buildings[i], out flag2);
							if (!flag2 && flag)
							{
								if (list == null)
								{
									list = new List<Entity>();
								}
								list.Add(item);
							}
							this.entitiesToLoad.Add(item);
						}
					}
					catch (Exception)
					{
						Service.Logger.ErrorFormat("Error trying to load building {0}", new object[]
						{
							map.Buildings[i].Uid
						});
					}
				}
				if (list != null)
				{
					worldController.FindValidPositionsAndAddBuildings(list);
				}
			}
			EntityViewManager entityViewManager = Service.EntityViewManager;
			List<Entity> list2 = new List<Entity>(this.entitiesToLoad);
			for (int j = 0; j < list2.Count; j++)
			{
				entityViewManager.LoadEntityAsset(list2[j]);
			}
			Service.EventManager.SendEvent(EventId.MapDataProcessingEnd, map);
			this.CheckLoadComplete();
		}

		private void OnWorldViewComplete(object cookie)
		{
			this.isPlanetAssetsLoaded = true;
			this.CheckLoadComplete();
		}

		private void CheckLoadComplete()
		{
			if (this.IsWorldLoadComplete())
			{
				Service.EventManager.UnregisterObserver(this, EventId.BuildingViewFailed);
				Service.EventManager.UnregisterObserver(this, EventId.BuildingViewReady);
				Service.AssetManager.DonePreloading();
				Service.EventManager.SendEvent(EventId.WorldLoadComplete, null);
			}
		}

		private bool IsWorldLoadComplete()
		{
			return this.isPlanetAssetsLoaded && this.entitiesToLoad != null && this.entitiesToLoad.Count == 0;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.BuildingViewReady || id == EventId.BuildingViewFailed)
			{
				EntityViewParams entityViewParams = cookie as EntityViewParams;
				if (entityViewParams == null || entityViewParams.Entity == null)
				{
					Service.Logger.Error("WorldInitializer received a building event with a null cookie or building entity.");
				}
				else
				{
					this.entitiesToLoad.Remove(entityViewParams.Entity);
					this.CheckLoadComplete();
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
