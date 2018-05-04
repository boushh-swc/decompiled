using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ConfigureControlsStoryAction : AbstractStoryAction
	{
		private const int CONTROLS_ARG = 0;

		private const string NONE = "none";

		public ConfigureControlsStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(1);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			string[] list;
			if (string.Compare("none", this.prepareArgs[0], StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				list = new string[0];
			}
			else
			{
				list = this.prepareArgs[0].Split(new char[]
				{
					','
				});
			}
			HudConfig config = new HudConfig(list);
			Service.UXController.HUD.ConfigureControls(config);
			this.parent.ChildComplete(this);
		}
	}
}
