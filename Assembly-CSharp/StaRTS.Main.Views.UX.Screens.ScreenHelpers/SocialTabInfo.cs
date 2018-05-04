using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class SocialTabInfo
	{
		public UXCheckbox TabButton
		{
			get;
			set;
		}

		public UXLabel TabLabel
		{
			get;
			set;
		}

		public Action LoadAction
		{
			get;
			private set;
		}

		public GridLoadHelper TabGridLoadHelper
		{
			get;
			set;
		}

		public EventId TabEventId
		{
			get;
			private set;
		}

		public string EventActionId
		{
			get;
			private set;
		}

		public PlayerListType ListType
		{
			get;
			private set;
		}

		public SocialTabInfo(Action loadAction, EventId eventId, string eventActionId, PlayerListType listType)
		{
			this.LoadAction = loadAction;
			this.TabEventId = eventId;
			this.EventActionId = eventActionId;
			this.ListType = listType;
		}
	}
}
