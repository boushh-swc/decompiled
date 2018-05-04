using StaRTS.Main.Controllers.TrapConditions;
using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TrapTypeVO : IValueObject
	{
		public TrapEventType EventType;

		public string TargetType;

		public string TriggerConditions;

		public string RevealAudio;

		public string DisarmConditions;

		public List<AddOnMapping> AddOns;

		public int RearmCreditsCost;

		public int RearmMaterialsCost;

		public int RearmContrabandCost;

		public List<TrapCondition> ParsedTrapConditions;

		public TurretTrapEventData TurretTED;

		public SpecialAttackTrapEventData ShipTED;

		private ITrapEventData eventData;

		public static int COLUMN_eventType
		{
			get;
			private set;
		}

		public static int COLUMN_eventData
		{
			get;
			private set;
		}

		public static int COLUMN_targetType
		{
			get;
			private set;
		}

		public static int COLUMN_triggerConditions
		{
			get;
			private set;
		}

		public static int COLUMN_revealAudio
		{
			get;
			private set;
		}

		public static int COLUMN_disarmConditions
		{
			get;
			private set;
		}

		public static int COLUMN_addOns
		{
			get;
			private set;
		}

		public static int COLUMN_rearmCreditsCost
		{
			get;
			private set;
		}

		public static int COLUMN_rearmMaterialsCost
		{
			get;
			private set;
		}

		public static int COLUMN_rearmContrabandCost
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.EventType = StringUtils.ParseEnum<TrapEventType>(row.TryGetString(TrapTypeVO.COLUMN_eventType));
			this.SetEventData(TrapUtils.ParseEventData(this.EventType, row.TryGetString(TrapTypeVO.COLUMN_eventData)));
			this.TargetType = row.TryGetString(TrapTypeVO.COLUMN_targetType);
			this.TriggerConditions = row.TryGetString(TrapTypeVO.COLUMN_triggerConditions);
			this.RevealAudio = row.TryGetString(TrapTypeVO.COLUMN_revealAudio);
			this.DisarmConditions = row.TryGetString(TrapTypeVO.COLUMN_disarmConditions);
			this.AddOns = TrapUtils.ParseAddons(row.TryGetString(TrapTypeVO.COLUMN_addOns));
			this.RearmCreditsCost = row.TryGetInt(TrapTypeVO.COLUMN_rearmCreditsCost);
			this.RearmMaterialsCost = row.TryGetInt(TrapTypeVO.COLUMN_rearmMaterialsCost);
			this.RearmContrabandCost = row.TryGetInt(TrapTypeVO.COLUMN_rearmContrabandCost);
			this.ParsedTrapConditions = TrapUtils.ParseConditions(this.TriggerConditions);
		}

		private void SetEventData(ITrapEventData ted)
		{
			this.eventData = ted;
			if (this.eventData is TurretTrapEventData)
			{
				this.TurretTED = (TurretTrapEventData)this.eventData;
				this.ShipTED = null;
			}
			else if (this.eventData is SpecialAttackTrapEventData)
			{
				this.TurretTED = null;
				this.ShipTED = (SpecialAttackTrapEventData)this.eventData;
			}
			else
			{
				this.TurretTED = null;
				this.ShipTED = null;
			}
		}
	}
}
