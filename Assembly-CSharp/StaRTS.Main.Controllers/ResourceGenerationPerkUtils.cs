using Net.RichardLord.Ash.Core;
using StaRTS.Externals.Manimal;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Perks;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public static class ResourceGenerationPerkUtils
	{
		[CompilerGenerated]
		private static Comparison<uint> <>f__mg$cache0;

		[CompilerGenerated]
		private static Comparison<CurrencyPerkEffectDataTO> <>f__mg$cache1;

		[CompilerGenerated]
		private static Comparison<CurrencyPerkEffectDataTO> <>f__mg$cache2;

		private static bool IsPerkActiveDuringTimeWindow(ActivatedPerkData perk, uint startTime, uint endTime)
		{
			return perk.StartTime <= endTime && perk.EndTime > startTime;
		}

		private static bool IsPerkEffectActiveDuringTimeWindow(CurrencyPerkEffectDataTO currencyPerkEffect, uint startTime, uint endTime)
		{
			return currencyPerkEffect.StartTime <= endTime && currencyPerkEffect.EndTime > startTime;
		}

		private static void AddEffectsOfMatchingCurrencyType(PerkVO perkData, CurrencyType currencyType, uint startTime, uint endTime, ref List<CurrencyPerkEffectDataTO> matchingEffects)
		{
			if (matchingEffects == null)
			{
				Service.Logger.Error("AddEffectsOfMatchingCurrencyType received null vo list");
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			string[] perkEffects = perkData.PerkEffects;
			int num = perkEffects.Length;
			for (int i = 0; i < num; i++)
			{
				PerkEffectVO perkEffectVO = staticDataController.Get<PerkEffectVO>(perkEffects[i]);
				if (perkEffectVO.Type == "generator" && perkEffectVO.Currency == currencyType)
				{
					matchingEffects.Add(new CurrencyPerkEffectDataTO(perkEffectVO.GenerationRate, startTime, endTime));
				}
			}
		}

		private static List<CurrencyPerkEffectDataTO> GetAllCurrencyPerksEffectsActiveDuringTimeWindow(CurrencyType type, uint startTime, uint endTime, List<ActivatedPerkData> allPerks)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			List<CurrencyPerkEffectDataTO> result = new List<CurrencyPerkEffectDataTO>();
			int count = allPerks.Count;
			for (int i = 0; i < count; i++)
			{
				ActivatedPerkData activatedPerkData = allPerks[i];
				if (ResourceGenerationPerkUtils.IsPerkActiveDuringTimeWindow(activatedPerkData, startTime, endTime))
				{
					PerkVO perkData = staticDataController.Get<PerkVO>(activatedPerkData.PerkId);
					ResourceGenerationPerkUtils.AddEffectsOfMatchingCurrencyType(perkData, type, activatedPerkData.StartTime, activatedPerkData.EndTime, ref result);
				}
			}
			return result;
		}

		private static void AddAllRateBonusesThatFallWithinTimeWindow(ref CurrencyPerkEffectDataTO timeWindow, List<CurrencyPerkEffectDataTO> perkEffects)
		{
			int count = perkEffects.Count;
			for (int i = 0; i < count; i++)
			{
				CurrencyPerkEffectDataTO currencyPerkEffectDataTO = perkEffects[i];
				if (ResourceGenerationPerkUtils.IsPerkEffectActiveDuringTimeWindow(currencyPerkEffectDataTO, timeWindow.StartTime, timeWindow.EndTime))
				{
					timeWindow.AdjustRateBonus(currencyPerkEffectDataTO.RateBonus);
				}
			}
		}

		private static List<CurrencyPerkEffectDataTO> CombineOverlappingPerkEffectsIntoFinalList(List<CurrencyPerkEffectDataTO> perkEffects, uint timeWindowStart, uint timeWindowEnd)
		{
			List<CurrencyPerkEffectDataTO> list = new List<CurrencyPerkEffectDataTO>();
			List<uint> list2 = new List<uint>();
			int count = perkEffects.Count;
			for (int i = 0; i < count; i++)
			{
				CurrencyPerkEffectDataTO currencyPerkEffectDataTO = perkEffects[i];
				list2.Add(currencyPerkEffectDataTO.StartTime);
				list2.Add(currencyPerkEffectDataTO.EndTime);
			}
			List<uint> arg_66_0 = list2;
			if (ResourceGenerationPerkUtils.<>f__mg$cache0 == null)
			{
				ResourceGenerationPerkUtils.<>f__mg$cache0 = new Comparison<uint>(ResourceGenerationPerkUtils.TimeSegmentCompare);
			}
			arg_66_0.Sort(ResourceGenerationPerkUtils.<>f__mg$cache0);
			count = list2.Count;
			for (int j = 0; j < count - 1; j++)
			{
				uint num = list2[j];
				uint num2 = list2[j + 1];
				if (num2 >= timeWindowStart && num <= timeWindowEnd)
				{
					if (num < timeWindowStart)
					{
						num = timeWindowStart;
					}
					if (num2 > timeWindowEnd)
					{
						num2 = timeWindowEnd;
					}
					CurrencyPerkEffectDataTO currencyPerkEffectDataTO2 = new CurrencyPerkEffectDataTO(0f, num, num2);
					ResourceGenerationPerkUtils.AddAllRateBonusesThatFallWithinTimeWindow(ref currencyPerkEffectDataTO2, perkEffects);
					if (currencyPerkEffectDataTO2.RateBonus != 0f)
					{
						list.Add(currencyPerkEffectDataTO2);
					}
				}
			}
			return list;
		}

		private static int TimeSegmentCompare(uint a, uint b)
		{
			if (a == b)
			{
				return 0;
			}
			if (b > a)
			{
				return -1;
			}
			return 1;
		}

		private static int StartTimeCompare(CurrencyPerkEffectDataTO a, CurrencyPerkEffectDataTO b)
		{
			if (a.StartTime == b.StartTime)
			{
				return 0;
			}
			if (b.StartTime > a.StartTime)
			{
				return 1;
			}
			return -1;
		}

		private static int EndTimeCompare(CurrencyPerkEffectDataTO a, CurrencyPerkEffectDataTO b)
		{
			if (a.EndTime == b.EndTime)
			{
				return 0;
			}
			if (b.EndTime < a.EndTime)
			{
				return 1;
			}
			return -1;
		}

		private static float BaseCurrencyRate(BuildingTypeVO type)
		{
			return (float)type.Produce / (float)type.CycleTime;
		}

		public static int GetPerkAdjustSecondsTillFull(BuildingTypeVO buildingVO, float remainingAmountTillFull, uint currentTime, List<ActivatedPerkData> allPerks)
		{
			int num = 0;
			float num2 = ResourceGenerationPerkUtils.BaseCurrencyRate(buildingVO);
			if (allPerks != null && allPerks.Count > 0)
			{
				List<CurrencyPerkEffectDataTO> allCurrencyPerksEffectsActiveDuringTimeWindow = ResourceGenerationPerkUtils.GetAllCurrencyPerksEffectsActiveDuringTimeWindow(buildingVO.Currency, currentTime, currentTime, allPerks);
				List<CurrencyPerkEffectDataTO> arg_48_0 = allCurrencyPerksEffectsActiveDuringTimeWindow;
				if (ResourceGenerationPerkUtils.<>f__mg$cache1 == null)
				{
					ResourceGenerationPerkUtils.<>f__mg$cache1 = new Comparison<CurrencyPerkEffectDataTO>(ResourceGenerationPerkUtils.EndTimeCompare);
				}
				arg_48_0.Sort(ResourceGenerationPerkUtils.<>f__mg$cache1);
				List<CurrencyPerkEffectDataTO> list = ResourceGenerationPerkUtils.CombineOverlappingPerkEffectsIntoFinalList(allCurrencyPerksEffectsActiveDuringTimeWindow, currentTime, 4294967295u);
				int count = list.Count;
				int num3 = 0;
				while (num3 < count && remainingAmountTillFull > 0f)
				{
					CurrencyPerkEffectDataTO currencyPerkEffectDataTO = list[num3];
					int num4 = currencyPerkEffectDataTO.Duration;
					float num5 = (1f + currencyPerkEffectDataTO.RateBonus) * num2;
					float num6 = num5 * (float)num4;
					if (num6 > remainingAmountTillFull)
					{
						num6 = remainingAmountTillFull;
						num4 = Mathf.FloorToInt(num6 / num5);
					}
					remainingAmountTillFull -= num6;
					num += num4;
					num3++;
				}
			}
			if (remainingAmountTillFull > 0f)
			{
				num += Mathf.FloorToInt(remainingAmountTillFull / num2);
			}
			return num;
		}

		public static int GetPerkAdjustedAccruedCurrency(BuildingTypeVO buildingVO, uint startTime, uint endTime, List<ActivatedPerkData> allPerks)
		{
			int num = 0;
			int num2 = (int)(endTime - startTime);
			float num3 = ResourceGenerationPerkUtils.BaseCurrencyRate(buildingVO);
			if (allPerks != null && allPerks.Count > 0)
			{
				List<CurrencyPerkEffectDataTO> allCurrencyPerksEffectsActiveDuringTimeWindow = ResourceGenerationPerkUtils.GetAllCurrencyPerksEffectsActiveDuringTimeWindow(buildingVO.Currency, startTime, endTime, allPerks);
				List<CurrencyPerkEffectDataTO> arg_4C_0 = allCurrencyPerksEffectsActiveDuringTimeWindow;
				if (ResourceGenerationPerkUtils.<>f__mg$cache2 == null)
				{
					ResourceGenerationPerkUtils.<>f__mg$cache2 = new Comparison<CurrencyPerkEffectDataTO>(ResourceGenerationPerkUtils.StartTimeCompare);
				}
				arg_4C_0.Sort(ResourceGenerationPerkUtils.<>f__mg$cache2);
				List<CurrencyPerkEffectDataTO> list = ResourceGenerationPerkUtils.CombineOverlappingPerkEffectsIntoFinalList(allCurrencyPerksEffectsActiveDuringTimeWindow, startTime, endTime);
				int count = list.Count;
				int num4 = 0;
				while (num4 < count && num2 > 0)
				{
					CurrencyPerkEffectDataTO currencyPerkEffectDataTO = list[num4];
					int num5 = currencyPerkEffectDataTO.Duration;
					if (num5 > num2)
					{
						num5 = num2;
					}
					float num6 = (1f + currencyPerkEffectDataTO.RateBonus) * num3;
					num += Mathf.FloorToInt(num6 * (float)num5);
					num2 -= num5;
					num4++;
				}
			}
			if (num2 > 0)
			{
				num += Mathf.FloorToInt(num3 * (float)num2);
			}
			return num;
		}

		public static void ProcessResouceGenPerkEffectsIntoStorage(List<ActivatedPerkData> allPerks)
		{
			ISupportController iSupportController = Service.ISupportController;
			NodeList<GeneratorViewNode> nodeList = Service.EntityController.GetNodeList<GeneratorViewNode>();
			for (GeneratorViewNode generatorViewNode = nodeList.Head; generatorViewNode != null; generatorViewNode = generatorViewNode.Next)
			{
				BuildingComponent buildingComp = generatorViewNode.BuildingComp;
				Building buildingTO = buildingComp.BuildingTO;
				BuildingTypeVO buildingType = buildingComp.BuildingType;
				Contract contract = iSupportController.FindCurrentContract(buildingComp.BuildingTO.Key);
				if (buildingType.Type == BuildingType.Resource)
				{
					if (contract == null)
					{
						uint time = ServerTime.Time;
						uint lastCollectTime = buildingTO.LastCollectTime;
						buildingTO.LastCollectTime = time;
						int perkAdjustedAccruedCurrency = ResourceGenerationPerkUtils.GetPerkAdjustedAccruedCurrency(buildingType, lastCollectTime, time, allPerks);
						buildingTO.CurrentStorage += perkAdjustedAccruedCurrency;
						if (buildingTO.CurrentStorage > buildingType.Storage)
						{
							buildingTO.CurrentStorage = buildingType.Storage;
						}
						buildingTO.AccruedCurrency = buildingTO.CurrentStorage;
					}
				}
			}
		}

		public static int GetCurrentCurrencyGenerationRate(BuildingTypeVO buildingVO, List<ActivatedPerkData> allPerks)
		{
			float num = ResourceGenerationPerkUtils.BaseCurrencyRate(buildingVO);
			float num2 = num;
			if (allPerks != null && allPerks.Count > 0)
			{
				List<CurrencyPerkEffectDataTO> allCurrencyPerksEffectsActiveDuringTimeWindow = ResourceGenerationPerkUtils.GetAllCurrencyPerksEffectsActiveDuringTimeWindow(buildingVO.Currency, DateUtils.GetNowSeconds(), DateUtils.GetNowSeconds(), allPerks);
				int i = 0;
				int count = allCurrencyPerksEffectsActiveDuringTimeWindow.Count;
				while (i < count)
				{
					CurrencyPerkEffectDataTO currencyPerkEffectDataTO = allCurrencyPerksEffectsActiveDuringTimeWindow[i];
					num2 += currencyPerkEffectDataTO.RateBonus * num;
					i++;
				}
				return Mathf.RoundToInt(num2 * 3600f);
			}
			return Mathf.RoundToInt(num * 3600f);
		}
	}
}
