using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Models
{
	public class ContractEventData
	{
		public Contract Contract
		{
			get;
			private set;
		}

		public bool Silent
		{
			get;
			private set;
		}

		public bool SendBILog
		{
			get;
			private set;
		}

		public SmartEntity Entity
		{
			get;
			private set;
		}

		public BuildingTypeVO BuildingVO
		{
			get;
			private set;
		}

		public string BuildingKey
		{
			get;
			set;
		}

		public ContractEventData(Contract contract, SmartEntity entity, bool silent, bool sendBILog)
		{
			this.Contract = contract;
			this.Entity = entity;
			BuildingComponent buildingComp = entity.BuildingComp;
			this.BuildingVO = buildingComp.BuildingType;
			this.BuildingKey = buildingComp.BuildingTO.Key;
			this.Silent = silent;
			this.SendBILog = sendBILog;
		}

		public ContractEventData(Contract contract, SmartEntity entity, bool silent)
		{
			this.Contract = contract;
			this.Entity = entity;
			BuildingComponent buildingComp = entity.BuildingComp;
			this.BuildingVO = buildingComp.BuildingType;
			this.BuildingKey = buildingComp.BuildingTO.Key;
			this.Silent = silent;
			this.SendBILog = true;
		}
	}
}
