using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX
{
	public class ContextButtonTag
	{
		public string ContextId
		{
			get;
			set;
		}

		public string SpriteName
		{
			get;
			set;
		}

		public UXButton ContextButton
		{
			get;
			set;
		}

		public UXButton ContextDimButton
		{
			get;
			set;
		}

		public UXSprite ContextIconSprite
		{
			get;
			set;
		}

		public UXSprite ContextBackground
		{
			get;
			set;
		}

		public UXLabel ContextLabel
		{
			get;
			set;
		}

		public UXLabel HardCostLabel
		{
			get;
			set;
		}

		public UXLabel TimerLabel
		{
			get;
			set;
		}

		public GetTimerSecondsDelegate TimerSecondsDelegate
		{
			get;
			set;
		}
	}
}
