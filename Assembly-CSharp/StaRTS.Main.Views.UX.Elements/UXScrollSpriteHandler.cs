using System;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXScrollSpriteHandler : IUXScrollSpriteHandler
	{
		private const string SCROLL_ARROW_PREFIX = "Sprite";

		private const string SCROLL_ARROW_UP_POSTFIX = "ScrollUp";

		private const string SCROLL_ARROW_DOWN_POSTFIX = "ScrollDown";

		private const string SCROLL_ARROW_RIGHT_POSTFIX = "ScrollRight";

		private const string SCROLL_ARROW_LEFT_POSTFIX = "ScrollLeft";

		private UXSprite scrollBackSprite;

		private UXSprite scrollForwardSprite;

		public void InitScrollSprites(UXFactory source, UIScrollView scrollView, float scrollPosition, bool isScrollable)
		{
			if (scrollView != null && source != null)
			{
				string text = "Sprite" + scrollView.name;
				string text2 = text;
				string text3 = text;
				UIScrollView.Movement movement = scrollView.movement;
				if (movement != UIScrollView.Movement.Horizontal)
				{
					if (movement == UIScrollView.Movement.Vertical)
					{
						text2 += "ScrollUp";
						text3 += "ScrollDown";
					}
				}
				else
				{
					text2 += "ScrollLeft";
					text3 += "ScrollRight";
				}
				this.scrollBackSprite = source.GetOptionalElement<UXSprite>(text2);
				this.scrollForwardSprite = source.GetOptionalElement<UXSprite>(text3);
				this.UpdateScrollSprites(scrollView, scrollPosition, isScrollable);
			}
		}

		public void HideScrollSprites()
		{
			if (this.scrollBackSprite != null && this.scrollForwardSprite != null)
			{
				this.scrollBackSprite.Visible = false;
				this.scrollForwardSprite.Visible = false;
			}
		}

		public void UpdateScrollSprites(UIScrollView scrollView, float scrollPosition, bool isScrollable)
		{
			if (scrollView != null && this.scrollBackSprite != null && this.scrollForwardSprite != null)
			{
				if (isScrollable)
				{
					if (scrollPosition == 0f)
					{
						this.scrollBackSprite.Visible = false;
						this.scrollForwardSprite.Visible = true;
					}
					else if (scrollPosition == 1f)
					{
						this.scrollBackSprite.Visible = true;
						this.scrollForwardSprite.Visible = false;
					}
					else
					{
						this.scrollBackSprite.Visible = true;
						this.scrollForwardSprite.Visible = true;
					}
				}
				else
				{
					this.scrollBackSprite.Visible = false;
					this.scrollForwardSprite.Visible = false;
				}
			}
		}
	}
}
