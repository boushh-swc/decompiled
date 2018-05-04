using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story
{
	public class PlanetIntroStoryUtil
	{
		public const string VIEW_POST_FIX = "Viewed";

		public const string INTRO_VIEWED_STATE = "1";

		public static bool ShouldPlanetIntroStoryBePlayed(string planetUID)
		{
			PlanetVO optional = Service.StaticDataController.GetOptional<PlanetVO>(planetUID);
			if (optional == null)
			{
				return false;
			}
			if (string.IsNullOrEmpty(optional.IntroStoryAction))
			{
				return false;
			}
			if (!Service.CurrentPlayer.IsPlanetUnlocked(planetUID))
			{
				return false;
			}
			string prefName = planetUID + "Viewed";
			string pref = Service.SharedPlayerPrefs.GetPref<string>(prefName);
			return !"1".Equals(pref);
		}

		public static void PlayPlanetIntroStoryChain(string planetUID)
		{
			PlanetVO optional = Service.StaticDataController.GetOptional<PlanetVO>(planetUID);
			if (optional == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(optional.IntroStoryAction))
			{
				return;
			}
			if (Service.StaticDataController.GetOptional<StoryActionVO>(optional.IntroStoryAction) == null)
			{
				return;
			}
			Service.SharedPlayerPrefs.SetPref(planetUID + "Viewed", "1");
			new ActionChain(optional.IntroStoryAction);
		}
	}
}
