using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Utils
{
	public class FactionIconUpgradeController : IEventObserver
	{
		private const string FACTION_SPRITE_EMPIRE_PREFIX = "FactionUpEmp";

		private const string FACTION_SPRITE_REBEL_PREFIX = "FactionUpReb";

		private const string FACTION_EMPIRE = "Empire";

		private const string FACTION_REBEL = "Rebel";

		private const int FACTION_COL_LENGTH = 5;

		private const string POSTFIX_DIGETS = "D2";

		private List<IconUpgradeVO> iconUpgradeList;

		public FactionIconUpgradeController()
		{
			Service.FactionIconUpgradeController = this;
			Service.EventManager.RegisterObserver(this, EventId.WorldInTransitionComplete, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.HomeStateTransitionComplete, EventPriority.Default);
		}

		public string GetIcon(string faction, int rating)
		{
			FactionType factionType = FactionType.Rebel;
			if (faction.ToLower() == "Empire".ToLower())
			{
				factionType = FactionType.Empire;
			}
			return this.GetIcon(factionType, rating);
		}

		public string GetIcon(FactionType factionType, int rating)
		{
			string result = string.Empty;
			if (this.UseUpgradeImage(rating))
			{
				string iconPostfixFromRating = this.GetIconPostfixFromRating(rating);
				if (factionType == FactionType.Rebel)
				{
					result = "FactionUpReb" + iconPostfixFromRating;
				}
				else
				{
					result = "FactionUpEmp" + iconPostfixFromRating;
				}
			}
			else
			{
				result = UXUtils.GetIconNameFromFactionType(factionType);
			}
			return result;
		}

		public bool UseUpgradeImage(int rating)
		{
			return GameConstants.ENABLE_FACTION_ICON_UPGRADES && this.RatingToLevel(rating) > 0;
		}

		private string GetIconPostfixFromRating(int rating)
		{
			int num = this.RatingToLevel(rating);
			string result = string.Empty;
			if (num > 0)
			{
				int num2 = (num - 1) / 5 + 1;
				int num3 = (num - 1) % 5 + 1;
				result = num2.ToString("D2") + num3.ToString("D2");
			}
			return result;
		}

		public int GetCurrentPlayerVictoryRating()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			return GameUtils.CalculatePlayerVictoryRating(currentPlayer);
		}

		public int GetTotalVictoryRatingToCurrentLevel()
		{
			List<IconUpgradeVO> sortedFactionIconData = this.GetSortedFactionIconData();
			int count = sortedFactionIconData.Count;
			int result = 0;
			int num = this.RatingToLevel(this.GetCurrentPlayerVictoryRating());
			if (count > 0 && num > 0)
			{
				IconUpgradeVO iconUpgradeVO = sortedFactionIconData[num];
				result = iconUpgradeVO.Rating;
			}
			return result;
		}

		public int GetTotalVictoryRatingToNextLevel()
		{
			List<IconUpgradeVO> sortedFactionIconData = this.GetSortedFactionIconData();
			int count = sortedFactionIconData.Count;
			int result = 0;
			int num = this.RatingToLevel(this.GetCurrentPlayerVictoryRating());
			num++;
			if (num >= count)
			{
				num = count - 1;
			}
			if (count > 0)
			{
				IconUpgradeVO iconUpgradeVO = sortedFactionIconData[num];
				result = iconUpgradeVO.Rating;
			}
			return result;
		}

		private int SortAscending(IconUpgradeVO a, IconUpgradeVO b)
		{
			return a.Level.CompareTo(b.Level);
		}

		private List<IconUpgradeVO> GetSortedFactionIconData()
		{
			if (this.iconUpgradeList == null)
			{
				this.iconUpgradeList = new List<IconUpgradeVO>();
				Dictionary<string, IconUpgradeVO>.ValueCollection all = Service.StaticDataController.GetAll<IconUpgradeVO>();
				foreach (IconUpgradeVO current in all)
				{
					this.iconUpgradeList.Add(current);
				}
				this.iconUpgradeList.Sort(new Comparison<IconUpgradeVO>(this.SortAscending));
			}
			return this.iconUpgradeList;
		}

		public int RatingToLevel(int rating)
		{
			List<IconUpgradeVO> sortedFactionIconData = this.GetSortedFactionIconData();
			int count = sortedFactionIconData.Count;
			int result = 0;
			if (count > 0)
			{
				int i = 0;
				IconUpgradeVO iconUpgradeVO = sortedFactionIconData[i];
				result = iconUpgradeVO.Level;
				for (i = 1; i < count; i++)
				{
					iconUpgradeVO = sortedFactionIconData[i];
					if (rating < iconUpgradeVO.Rating)
					{
						break;
					}
					result = iconUpgradeVO.Level;
				}
				if (i == count)
				{
					result = sortedFactionIconData[count - 1].Level;
				}
			}
			return result;
		}

		public int RatingToDisplayLevel(int rating)
		{
			return this.RatingToLevel(rating) + 1;
		}

		public int GetCurrentPlayerLevel()
		{
			return this.RatingToLevel(this.GetCurrentPlayerVictoryRating());
		}

		public int GetCurrentPlayerDisplayLevel()
		{
			return this.RatingToDisplayLevel(this.GetCurrentPlayerVictoryRating());
		}

		public int GetCurrentPlayerDisplayNextLevel()
		{
			return this.GetCurrentPlayerDisplayLevel() + 1;
		}

		public bool ShouldShowIconUpgradeCelebration()
		{
			int pref = Service.SharedPlayerPrefs.GetPref<int>("fi_bl");
			return GameConstants.ENABLE_FACTION_ICON_UPGRADES && pref < this.GetCurrentPlayerLevel();
		}

		public void ShowIconUpgradeCelebrationScreen()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			string icon = this.GetIcon(currentPlayer.Faction, this.GetCurrentPlayerVictoryRating());
			Service.ScreenController.AddScreen(new FactionIconCelebScreen(icon), false, QueueScreenBehavior.QueueAndDeferTillClosed);
			this.SaveIconProgress();
		}

		private void SaveIconProgress()
		{
			Service.SharedPlayerPrefs.SetPref("fi_bl", this.GetCurrentPlayerLevel().ToString());
			Service.EventManager.SendEvent(EventId.FactionIconUpgraded, null);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if ((id == EventId.HomeStateTransitionComplete || id == EventId.WorldInTransitionComplete) && Service.GameStateMachine.CurrentState is HomeState && this.ShouldShowIconUpgradeCelebration())
			{
				this.ShowIconUpgradeCelebrationScreen();
			}
			return EatResponse.NotEaten;
		}
	}
}
