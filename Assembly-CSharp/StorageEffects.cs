using Net.RichardLord.Ash.Core;
using StaRTS.FX;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using UnityEngine;

public class StorageEffects : IEventObserver
{
	private const float FILL_STATE_THRESHOLD_1 = 0.05f;

	private const float FILL_STATE_THRESHOLD_2 = 0.5f;

	private const float FILL_STATE_THRESHOLD_3 = 0.99f;

	private const string FILL_STATE_MESH_1 = "fillStateMesh1";

	private const string FILL_STATE_MESH_2 = "fillStateMesh2";

	private const string FILL_STATE_MESH_3 = "fillStateMesh3";

	public StorageEffects()
	{
		Service.StorageEffects = this;
		Service.EventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.Default);
	}

	public void UpdateFillState(Entity entity, BuildingTypeVO buildingVO)
	{
		SmartEntity smartEntity = (SmartEntity)entity;
		if (buildingVO.Storage <= 0 || string.IsNullOrEmpty(buildingVO.FillStateAssetName) || string.IsNullOrEmpty(buildingVO.FillStateBundleName))
		{
			return;
		}
		if (!entity.Has<GameObjectViewComponent>())
		{
			return;
		}
		int num = 0;
		IResourceFillable resourceFillable = null;
		if (buildingVO.Type == BuildingType.Resource)
		{
			GeneratorComponent generatorComponent = entity.Get<GeneratorComponent>();
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			num = buildingComponent.BuildingTO.AccruedCurrency;
			resourceFillable = generatorComponent;
		}
		else if (buildingVO.Type == BuildingType.Storage)
		{
			num = StorageSpreadUtils.CalculateAssumedCurrencyInStorage(buildingVO.Currency, entity);
			resourceFillable = entity.Get<StorageComponent>();
		}
		if (resourceFillable == null)
		{
			return;
		}
		float num2 = (float)num / (float)buildingVO.Storage;
		float currentFullnessPercentage = resourceFillable.CurrentFullnessPercentage;
		resourceFillable.CurrentFullnessPercentage = num2;
		resourceFillable.PreviousFullnessPercentage = currentFullnessPercentage;
		this.UpdateFillStateFX(entity, buildingVO, num2, currentFullnessPercentage);
		if (smartEntity.BuildingComp.BuildingType.Type == BuildingType.Storage && smartEntity.BuildingComp.BuildingType.Currency == CurrencyType.Credits)
		{
			Service.EventManager.SendEvent(EventId.StorageDoorEvent, smartEntity);
		}
	}

	public void UpdateFillStateFX(Entity entity, BuildingTypeVO buildingVO, float currentFullnessPercent, float previousFullnessPercent)
	{
		float collectNotificationPercentage = this.GetCollectNotificationPercentage(buildingVO);
		if (Service.PostBattleRepairController.IsEntityInRepair(entity))
		{
			if (previousFullnessPercent >= collectNotificationPercentage)
			{
				Service.FXManager.RemoveAttachedFXFromEntity(entity, "fillState");
			}
			return;
		}
		if (currentFullnessPercent == previousFullnessPercent)
		{
			return;
		}
		if (currentFullnessPercent >= collectNotificationPercentage && previousFullnessPercent < collectNotificationPercentage)
		{
			Service.FXManager.CreateAndAttachFXToEntity(entity, buildingVO.FillStateAssetName, "fillState", new FXManager.AttachedFXLoadedCallback(this.OnFillStateLoaded), currentFullnessPercent, false, Vector3.zero);
		}
		else if (currentFullnessPercent >= collectNotificationPercentage)
		{
			GameObject attachedGameObject = entity.Get<GameObjectViewComponent>().GetAttachedGameObject("fillState");
			if (attachedGameObject != null)
			{
				this.UpdateFillStateMeshes(attachedGameObject, entity, currentFullnessPercent);
			}
		}
		else if (currentFullnessPercent < collectNotificationPercentage && previousFullnessPercent >= collectNotificationPercentage)
		{
			Service.FXManager.RemoveAttachedFXFromEntity(entity, "fillState");
		}
	}

	private void OnFillStateLoaded(GameObject fillStateInstance, Entity parentEntity, float currentFillPercent)
	{
		this.UpdateFillStateMeshes(fillStateInstance, parentEntity, currentFillPercent);
	}

	private void UpdateFillStateMeshes(GameObject fillStateInstance, Entity parentEntity, float currentFullnessPercentage)
	{
		BuildingComponent buildingComponent = parentEntity.Get<BuildingComponent>();
		BuildingTypeVO buildingType = buildingComponent.BuildingType;
		float collectNotificationPercentage = this.GetCollectNotificationPercentage(buildingType);
		this.UpdateFillStateMesh(fillStateInstance, collectNotificationPercentage, currentFullnessPercentage, "fillStateMesh1");
		this.UpdateFillStateMesh(fillStateInstance, 0.5f, currentFullnessPercentage, "fillStateMesh2");
		this.UpdateFillStateMesh(fillStateInstance, 0.99f, currentFullnessPercentage, "fillStateMesh3");
	}

	private void UpdateFillStateMesh(GameObject fillInstance, float threshold, float currentFullnessPercentage, string meshName)
	{
		GameObject gameObject = UnityUtils.FindGameObject(fillInstance, meshName);
		gameObject.SetActive(currentFullnessPercentage >= threshold);
	}

	private float GetCollectNotificationPercentage(BuildingTypeVO buildingVO)
	{
		if (buildingVO.Type == BuildingType.Storage)
		{
			return 0.05f;
		}
		float result;
		if (buildingVO.Storage > 0)
		{
			result = (float)buildingVO.CollectNotify / (float)buildingVO.Storage;
		}
		else
		{
			result = 0.05f;
		}
		return result;
	}

	public EatResponse OnEvent(EventId id, object cookie)
	{
		if (id == EventId.BuildingViewReady)
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			if (!(currentState is BattleStartState) && !(currentState is BattlePlaybackState) && !(currentState is FueBattleStartState))
			{
				Entity entity = ((EntityViewParams)cookie).Entity;
				BuildingTypeVO buildingType = entity.Get<BuildingComponent>().BuildingType;
				this.UpdateFillState(entity, buildingType);
			}
		}
		return EatResponse.NotEaten;
	}
}
