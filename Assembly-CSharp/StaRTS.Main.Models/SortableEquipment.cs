using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Models
{
	public class SortableEquipment
	{
		private const int EFFECTIVE_MIN_INT = -900000;

		public CurrentPlayer Player
		{
			get;
			set;
		}

		public EquipmentVO Equipment
		{
			get;
			private set;
		}

		public int IncrementingIndex
		{
			get;
			set;
		}

		public int EmptyIndex
		{
			get;
			set;
		}

		public SortableEquipment(EquipmentVO equipment)
		{
			this.Equipment = equipment;
			if (this.Equipment == null)
			{
				this.EmptyIndex = -900000;
			}
			else
			{
				this.IncrementingIndex = -900000;
			}
		}

		public SortableEquipment(CurrentPlayer player, EquipmentVO equipment)
		{
			this.Player = player;
			this.Equipment = equipment;
			this.EmptyIndex = -900000;
		}

		public SortableEquipment(EquipmentVO equipment, int index)
		{
			this.Equipment = equipment;
			this.IncrementingIndex = index;
			this.EmptyIndex = -900000;
		}

		public bool HasEquipment()
		{
			return this.Equipment != null;
		}
	}
}
