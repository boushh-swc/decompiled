using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class CircleButtonStoryAction : AbstractStoryAction, IEventObserver
	{
		private const int BUTTON_NAME_ARG = 0;

		private const int ARROW_POS_ARG = 1;

		private const int PERSIST_ARG = 2;

		private const string PERSIST = "persist";

		private UXButton highlightedButton;

		private UXCheckbox highlightedCheckbox;

		private UXButtonClickedDelegate originalButtonClicked;

		private UXCheckboxSelectedDelegate originalCheckboxSelected;

		private bool startedHidingHighlight;

		public CircleButtonStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
			this.startedHidingHighlight = false;
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(new int[]
			{
				1,
				2,
				3
			});
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			string elementName = this.prepareArgs[0];
			string arrowPosition = string.Empty;
			if (this.prepareArgs.Length > 1)
			{
				arrowPosition = this.prepareArgs[1].ToLower();
			}
			EventManager eventManager = Service.EventManager;
			UXElement uXElement = Service.ScreenController.FindElement<UXElement>(elementName);
			this.highlightedButton = (uXElement as UXButton);
			this.highlightedCheckbox = (uXElement as UXCheckbox);
			if (this.highlightedCheckbox != null && this.highlightedCheckbox.Selected && this.highlightedCheckbox.RadioGroup != 0)
			{
				this.parent.ChildComplete(this);
				return;
			}
			if (uXElement == null)
			{
				this.parent.ChildComplete(this);
				return;
			}
			StoreScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<StoreScreen>();
			if (highestLevelScreen != null && !highestLevelScreen.TransitionedIn)
			{
				eventManager.RegisterObserver(this, EventId.ScreenRefreshed, EventPriority.Default);
				eventManager.RegisterObserver(this, EventId.ScreenTransitionInComplete, EventPriority.Default);
				eventManager.RegisterObserver(this, EventId.StoreScreenReady, EventPriority.Default);
				return;
			}
			if (this.prepareArgs.Length == 3 && "persist".Equals(this.prepareArgs[2], StringComparison.InvariantCultureIgnoreCase))
			{
				this.parent.ChildComplete(this);
			}
			else if (this.highlightedCheckbox != null)
			{
				eventManager.RegisterObserver(this, EventId.ScreenRefreshed, EventPriority.Default);
				eventManager.RegisterObserver(this, EventId.ScreenTransitionInComplete, EventPriority.Default);
				Service.UserInputInhibitor.AllowOnly(this.highlightedCheckbox);
				if (this.highlightedCheckbox.OnSelected != new UXCheckboxSelectedDelegate(this.OnSelectedHighlighted))
				{
					this.originalCheckboxSelected = this.highlightedCheckbox.OnSelected;
					this.highlightedCheckbox.OnSelected = new UXCheckboxSelectedDelegate(this.OnSelectedHighlighted);
				}
			}
			else
			{
				eventManager.RegisterObserver(this, EventId.ScreenRefreshed, EventPriority.Default);
				eventManager.RegisterObserver(this, EventId.ScreenTransitionInComplete, EventPriority.Default);
				Service.UserInputInhibitor.AllowOnly(this.highlightedButton);
				if (this.highlightedButton.OnClicked != new UXButtonClickedDelegate(this.OnClickedHighlighted))
				{
					this.originalButtonClicked = this.highlightedButton.OnClicked;
					this.highlightedButton.OnClicked = new UXButtonClickedDelegate(this.OnClickedHighlighted);
				}
			}
			Service.UXController.MiscElementsManager.HighlightButton(uXElement, arrowPosition);
		}

		private void OnClickedHighlighted(UXButton button)
		{
			if (!this.startedHidingHighlight && button == this.highlightedButton)
			{
				this.AddHighlightListeners();
				this.startedHidingHighlight = true;
				Service.UserInputInhibitor.AllowAll();
				if (this.originalButtonClicked != null)
				{
					this.originalButtonClicked(this.highlightedButton);
				}
				Service.UXController.MiscElementsManager.HideHighlight();
			}
		}

		private void OnSelectedHighlighted(UXCheckbox checkbox, bool selected)
		{
			if (!this.startedHidingHighlight && checkbox == this.highlightedCheckbox && (selected || checkbox.RadioGroup == 0))
			{
				this.AddHighlightListeners();
				this.startedHidingHighlight = true;
				Service.UserInputInhibitor.AllowAll();
				if (this.originalCheckboxSelected != null)
				{
					this.originalCheckboxSelected(this.highlightedCheckbox, selected);
				}
				Service.UXController.MiscElementsManager.HideHighlight();
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.StoreScreenReady && id != EventId.ScreenRefreshed && id != EventId.ScreenTransitionInComplete)
			{
				if (id == EventId.ButtonHighlightHidden)
				{
					this.RemoveListeners();
					if (this.highlightedButton != null)
					{
						this.highlightedButton.OnClicked = this.originalButtonClicked;
					}
					else if (this.highlightedCheckbox != null)
					{
						this.highlightedCheckbox.OnSelected = this.originalCheckboxSelected;
					}
					this.parent.ChildComplete(this);
				}
			}
			else
			{
				this.Execute();
			}
			return EatResponse.NotEaten;
		}

		private void AddHighlightListeners()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ButtonHighlightHidden, EventPriority.Default);
		}

		private void RemoveListeners()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.ButtonHighlightHidden);
			eventManager.UnregisterObserver(this, EventId.ScreenRefreshed);
			eventManager.UnregisterObserver(this, EventId.ScreenTransitionInComplete);
			eventManager.UnregisterObserver(this, EventId.StoreScreenReady);
		}

		public override void Destroy()
		{
			Service.UXController.MiscElementsManager.HideHighlight();
			base.Destroy();
		}
	}
}
