using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class PlayPlanetIntroStoryAction : AbstractStoryAction
	{
		private const string INTRO_VIEWED_STATE = "1";

		private string planetUID;

		private string reactionUID;

		public override string Reaction
		{
			get
			{
				return this.reactionUID;
			}
		}

		public PlayPlanetIntroStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
			this.reactionUID = string.Empty;
			this.planetUID = Service.CurrentPlayer.GetFirstPlanetUnlockedUID();
			if (!string.IsNullOrEmpty(vo.PrepareString))
			{
				this.planetUID = this.prepareArgs[0];
			}
			PlanetVO optional = Service.StaticDataController.GetOptional<PlanetVO>(this.planetUID);
			if (optional == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(optional.IntroStoryAction))
			{
				return;
			}
			string pref = Service.SharedPlayerPrefs.GetPref<string>(optional.Uid + "Viewed");
			if ("1".Equals(pref))
			{
				return;
			}
			this.reactionUID = optional.IntroStoryAction;
		}

		public override void Prepare()
		{
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			Service.SharedPlayerPrefs.SetPref(this.planetUID + "Viewed", "1");
			base.Execute();
			this.parent.ChildComplete(this);
		}
	}
}
