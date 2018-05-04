using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using System;

namespace StaRTS.Main.Controllers
{
	public interface ICurrencyController
	{
		bool TryCollectCurrencyOnSelection(Entity entity);

		void CollectCurrency(Entity buildingEntity);

		void HandleUnableToCollect(CurrencyType currencyType);

		void ForceCollectAccruedCurrencyForUpgrade(Entity buildingEntity);

		bool CanStoreCollectionAmountFromGenerator(Entity buildingEntity);

		bool IsGeneratorCollectable(Entity buildingEntity);

		bool IsGeneratorThresholdMet(Entity buildingEntity);

		int CalculateTimeUntilAllGeneratorsFull();

		int CalculateGeneratorFillTimeRemaining(Entity buildingEntity);

		void UpdateGeneratorAccruedCurrency(SmartEntity entity);

		float CurrencyPerSecond(BuildingTypeVO type);

		int CurrencyPerHour(BuildingTypeVO type);

		int CalculateAccruedCurrency(Building buildingTO, BuildingTypeVO type);

		void UpdateGeneratorAfterFinishedContract(BuildingTypeVO buildingVO, Building buildingTO, uint contractFinishTime, bool isConstructionContract);

		void UpdateAllStorageEffects();

		EatResponse OnEvent(EventId id, object cookie);
	}
}
