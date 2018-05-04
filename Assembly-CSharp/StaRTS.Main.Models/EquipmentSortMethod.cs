using System;

namespace StaRTS.Main.Models
{
	public enum EquipmentSortMethod
	{
		UnlockedEquipment = 0,
		RequirementsMet = 1,
		CurrentPlanet = 2,
		CapacitySize = 3,
		Quality = 4,
		Alphabetical = 5,
		EmptyEquipment = 6,
		DecrementingIndex = 7,
		IncrementingEmptyIndex = 8
	}
}
