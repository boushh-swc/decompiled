using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Controllers
{
	public class RewardManager
	{
		public delegate void SuccessCallback(object cookie);

		private const string CRYSTAL_REWARD = "specialoffers";

		private const string LVL_DISPLAY_STRING_ID = "lcfly_level";

		private const string AMOUNT_AND_NAME = "AMOUNT_AND_NAME";

		private Dictionary<int, RewardTag> rewardTags;

		private CurrentPlayer cp;

		private StaticDataController sdc;

		private Lang lang;

		private int callbackCounter;

		public RewardManager()
		{
			Service.RewardManager = this;
			this.rewardTags = new Dictionary<int, RewardTag>();
			this.cp = Service.CurrentPlayer;
			this.sdc = Service.StaticDataController;
			this.lang = Service.Lang;
		}

		public bool IsRewardOnlySoftCurrency(string rewardUid)
		{
			if (string.IsNullOrEmpty(rewardUid))
			{
				return true;
			}
			RewardVO rewardVO = this.sdc.Get<RewardVO>(rewardUid);
			return rewardVO.BuildingUnlocks == null && rewardVO.HeroRewards == null && rewardVO.HeroUnlocks == null && rewardVO.SpecialAttackRewards == null && rewardVO.SpecialAttackUnlocks == null && rewardVO.TroopRewards == null && rewardVO.TroopUnlocks == null;
		}

		public void TryAndGrantReward(string rewardUid, RewardManager.SuccessCallback onSuccess, object cookie)
		{
			this.TryAndGrantReward(rewardUid, onSuccess, cookie, false);
		}

		public void TryAndGrantReward(string rewardUid, RewardManager.SuccessCallback onSuccess, object cookie, bool checkCurrencyCapacity)
		{
			if (string.IsNullOrEmpty(rewardUid))
			{
				return;
			}
			RewardVO vo = this.sdc.Get<RewardVO>(rewardUid);
			this.TryAndGrantReward(vo, onSuccess, cookie, checkCurrencyCapacity);
		}

		public void TryAndGrantReward(RewardVO vo, RewardManager.SuccessCallback onSuccess, object cookie, bool checkCurrencyCapacity)
		{
			RewardabilityResult rewardabilityResult = RewardUtils.CanPlayerHandleReward(this.cp, vo, checkCurrencyCapacity);
			int num = ++this.callbackCounter;
			RewardTag rewardTag = new RewardTag();
			rewardTag.Vo = vo;
			rewardTag.GlobalSuccess = onSuccess;
			rewardTag.Cookie = cookie;
			this.rewardTags.Add(num, rewardTag);
			if (rewardabilityResult.CanAward)
			{
				this.GrantReward(num);
			}
			else
			{
				string message = Service.Lang.Get(rewardabilityResult.Reason, new object[0]);
				string title = Service.Lang.Get("INVENTORY_NO_ROOM_TITLE", new object[0]);
				if (rewardabilityResult.Reason == "INVENTORY_NO_ROOM")
				{
					YesNoScreen.ShowModal(title, message, false, new OnScreenModalResult(this.ForceCurrencyRewardUsage), rewardTag);
				}
				else
				{
					AlertScreen.ShowModal(false, null, message, null, null);
				}
			}
		}

		private void ForceCurrencyRewardUsage(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			RewardTag rewardTag = cookie as RewardTag;
			this.TryAndGrantReward(rewardTag.Vo.Uid, rewardTag.GlobalSuccess, rewardTag.Cookie);
		}

		private void GrantReward(int identifier)
		{
			RewardTag rewardTag = this.rewardTags[identifier];
			RewardUtils.GrantReward(this.cp, rewardTag.Vo);
			this.rewardTags.Remove(identifier);
			if (rewardTag.GlobalSuccess != null)
			{
				rewardTag.GlobalSuccess(rewardTag.Cookie);
			}
		}

		public void GetFirstRewardAssetName(string rewardUid, ref RewardType type, out IGeometryVO config)
		{
			if (string.IsNullOrEmpty(rewardUid))
			{
				config = null;
				return;
			}
			RewardVO rewardVO = this.sdc.Get<RewardVO>(rewardUid);
			if (rewardVO.BuildingUnlocks != null && rewardVO.BuildingUnlocks.Length > 0)
			{
				BuildingTypeVO buildingTypeVO = this.sdc.Get<BuildingTypeVO>(rewardVO.BuildingUnlocks[0]);
				type = RewardType.Building;
				config = buildingTypeVO;
				return;
			}
			if (rewardVO.TroopUnlocks != null && rewardVO.TroopUnlocks.Length > 0)
			{
				TroopTypeVO troopTypeVO = this.sdc.Get<TroopTypeVO>(rewardVO.TroopUnlocks[0]);
				type = RewardType.Troop;
				config = troopTypeVO;
				return;
			}
			if (rewardVO.TroopRewards != null && rewardVO.TroopRewards.Length > 0)
			{
				string[] array = rewardVO.TroopRewards[0].Split(new char[]
				{
					':'
				});
				if (array != null && array.Length > 0)
				{
					TroopTypeVO troopTypeVO = this.sdc.Get<TroopTypeVO>(array[0]);
					type = RewardType.Troop;
					config = troopTypeVO;
					return;
				}
			}
			if (rewardVO.HeroUnlocks != null && rewardVO.HeroUnlocks.Length > 0)
			{
				TroopTypeVO troopTypeVO2 = this.sdc.Get<TroopTypeVO>(rewardVO.HeroUnlocks[0]);
				type = RewardType.Troop;
				config = troopTypeVO2;
				return;
			}
			if (rewardVO.HeroRewards != null && rewardVO.HeroRewards.Length > 0)
			{
				string[] array2 = rewardVO.HeroRewards[0].Split(new char[]
				{
					':'
				});
				if (array2 != null && array2.Length > 0)
				{
					TroopTypeVO troopTypeVO2 = this.sdc.Get<TroopTypeVO>(array2[0]);
					type = RewardType.Troop;
					config = troopTypeVO2;
					return;
				}
			}
			if (rewardVO.SpecialAttackRewards != null && rewardVO.SpecialAttackRewards.Length > 0)
			{
				string[] array3 = rewardVO.SpecialAttackRewards[0].Split(new char[]
				{
					':'
				});
				if (array3 != null && array3.Length > 0)
				{
					SpecialAttackTypeVO specialAttackTypeVO = this.sdc.Get<SpecialAttackTypeVO>(array3[0]);
					type = RewardType.Troop;
					config = specialAttackTypeVO;
					return;
				}
			}
			if (rewardVO.SpecialAttackUnlocks != null && rewardVO.SpecialAttackUnlocks.Length > 0)
			{
				SpecialAttackTypeVO specialAttackTypeVO = this.sdc.Get<SpecialAttackTypeVO>(rewardVO.SpecialAttackUnlocks[0]);
				type = RewardType.Troop;
				config = specialAttackTypeVO;
				return;
			}
			if (rewardVO.CurrencyRewards != null && rewardVO.CurrencyRewards.Length > 0)
			{
				type = RewardType.Currency;
				config = null;
				string[] array4 = rewardVO.CurrencyRewards[0].Split(new char[]
				{
					':'
				});
				if (array4 != null && array4.Length > 0)
				{
					string text = array4[0];
					if (!string.IsNullOrEmpty(text))
					{
						config = UXUtils.GetDefaultCurrencyIconVO(text);
					}
					return;
				}
			}
			type = RewardType.Invalid;
			config = null;
		}

		public string GetRewardString(string rewardUid)
		{
			if (string.IsNullOrEmpty(rewardUid))
			{
				return string.Empty;
			}
			RewardVO vo = this.sdc.Get<RewardVO>(rewardUid);
			return this.GetRewardString(vo, Service.Lang.Get("SupplyRewardFormat", new object[0]));
		}

		public string GetRewardString(RewardVO vo, string formatString)
		{
			return this.GetRewardStringInternal(vo, formatString, false);
		}

		public string GetRewardStringWithLevel(RewardVO vo, string formatString)
		{
			return this.GetRewardStringInternal(vo, formatString, true);
		}

		private string GetRewardStringInternal(RewardVO vo, string formatString, bool withLevel)
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			if (vo.CurrencyRewards != null)
			{
				int i = 0;
				int num = vo.CurrencyRewards.Length;
				while (i < num)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					string[] array = vo.CurrencyRewards[i].Split(new char[]
					{
						':'
					});
					stringBuilder.AppendFormat(formatString, this.lang.Get(array[0].ToUpper(), new object[0]), this.lang.ThousandsSeparated(Convert.ToInt32(array[1])));
					flag = false;
					i++;
				}
			}
			if (vo.ShardRewards != null)
			{
				int i = 0;
				int num = vo.ShardRewards.Length;
				while (i < num)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					string[] array2 = vo.ShardRewards[i].Split(new char[]
					{
						':'
					});
					this.AppendRewardString(stringBuilder, formatString, LangUtils.GetEquipmentDisplayNameById(array2[0]), array2[1]);
					flag = false;
					i++;
				}
			}
			if (vo.BuildingUnlocks != null)
			{
				int i = 0;
				int num = vo.BuildingUnlocks.Length;
				while (i < num)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					BuildingTypeVO buildingTypeVO = this.sdc.Get<BuildingTypeVO>(vo.BuildingUnlocks[i]);
					bool unlocked = Service.CurrentPlayer.UnlockedLevels.Buildings.Has(buildingTypeVO);
					this.AppendUnlockString(stringBuilder, LangUtils.GetBuildingDisplayName(buildingTypeVO), unlocked, flag);
					flag = false;
					i++;
				}
			}
			if (vo.TroopUnlocks != null)
			{
				int i = 0;
				int num = vo.TroopUnlocks.Length;
				while (i < num)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					TroopTypeVO troopTypeVO = this.sdc.Get<TroopTypeVO>(vo.TroopUnlocks[i]);
					bool unlocked = Service.CurrentPlayer.UnlockedLevels.Troops.Has(troopTypeVO);
					this.AppendUnlockString(stringBuilder, LangUtils.GetTroopDisplayName(troopTypeVO), unlocked, flag);
					flag = false;
					i++;
				}
			}
			if (vo.TroopRewards != null)
			{
				int i = 0;
				int num = vo.TroopRewards.Length;
				while (i < num)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					string[] array3 = vo.TroopRewards[i].Split(new char[]
					{
						':'
					});
					TroopTypeVO troopTypeVO = this.sdc.Get<TroopTypeVO>(array3[0]);
					string text = LangUtils.GetTroopDisplayName(troopTypeVO);
					if (withLevel)
					{
						text = this.lang.Get("AMOUNT_AND_NAME", new object[]
						{
							text,
							this.lang.Get("lcfly_level", new object[]
							{
								troopTypeVO.Lvl
							})
						});
					}
					this.AppendRewardString(stringBuilder, formatString, text, array3[1]);
					flag = false;
					i++;
				}
			}
			if (vo.HeroUnlocks != null)
			{
				int i = 0;
				int num = vo.HeroUnlocks.Length;
				while (i < num)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					TroopTypeVO troopTypeVO2 = this.sdc.Get<TroopTypeVO>(vo.HeroUnlocks[i]);
					bool unlocked = Service.CurrentPlayer.UnlockedLevels.Troops.Has(troopTypeVO2);
					this.AppendUnlockString(stringBuilder, LangUtils.GetTroopDisplayName(troopTypeVO2), unlocked, flag);
					flag = false;
					i++;
				}
			}
			if (vo.HeroRewards != null)
			{
				int i = 0;
				int num = vo.HeroRewards.Length;
				while (i < num)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					string[] array4 = vo.HeroRewards[i].Split(new char[]
					{
						':'
					});
					TroopTypeVO troopTypeVO2 = this.sdc.Get<TroopTypeVO>(array4[0]);
					string text2 = LangUtils.GetTroopDisplayName(troopTypeVO2);
					if (withLevel)
					{
						text2 = this.lang.Get("AMOUNT_AND_NAME", new object[]
						{
							text2,
							this.lang.Get("lcfly_level", new object[]
							{
								troopTypeVO2.Lvl
							})
						});
					}
					this.AppendRewardString(stringBuilder, formatString, text2, array4[1]);
					flag = false;
					i++;
				}
			}
			if (vo.SpecialAttackUnlocks != null)
			{
				int i = 0;
				int num = vo.SpecialAttackUnlocks.Length;
				while (i < num)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					SpecialAttackTypeVO specialAttackTypeVO = this.sdc.Get<SpecialAttackTypeVO>(vo.SpecialAttackUnlocks[i]);
					bool unlocked = Service.CurrentPlayer.UnlockedLevels.Starships.Has(specialAttackTypeVO);
					this.AppendUnlockString(stringBuilder, LangUtils.GetStarshipDisplayName(specialAttackTypeVO), unlocked, flag);
					flag = false;
					i++;
				}
			}
			if (vo.SpecialAttackRewards != null)
			{
				int i = 0;
				int num = vo.SpecialAttackRewards.Length;
				while (i < num)
				{
					if (!flag)
					{
						stringBuilder.Append(", ");
					}
					string[] array5 = vo.SpecialAttackRewards[i].Split(new char[]
					{
						':'
					});
					SpecialAttackTypeVO specialAttackTypeVO = this.sdc.Get<SpecialAttackTypeVO>(array5[0]);
					string text3 = LangUtils.GetStarshipDisplayName(specialAttackTypeVO);
					if (withLevel)
					{
						text3 = this.lang.Get("AMOUNT_AND_NAME", new object[]
						{
							text3,
							this.lang.Get("lcfly_level", new object[]
							{
								specialAttackTypeVO.Lvl
							})
						});
					}
					this.AppendRewardString(stringBuilder, formatString, text3, array5[1]);
					flag = false;
					i++;
				}
			}
			return stringBuilder.ToString();
		}

		private void AppendUnlockString(StringBuilder sb, string displayName, bool unlocked, bool first)
		{
			if (unlocked)
			{
				sb.AppendFormat(this.lang.Get("UPGRADE_UNIT", new object[0]), displayName);
			}
			else
			{
				sb.AppendFormat(this.lang.Get("LABEL_CAMPAIGN_UNLOCKS", new object[0]), displayName);
			}
		}

		private void AppendRewardString(StringBuilder sb, string format, string displayName, string amount)
		{
			sb.AppendFormat(format, displayName, this.lang.ThousandsSeparated(Convert.ToInt32(amount)));
		}
	}
}
