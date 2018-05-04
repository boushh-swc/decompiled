using System;

namespace StaRTS.Main.Views.UX.Elements
{
	public class NoOpUXScrollSpriteHandler : IUXScrollSpriteHandler
	{
		public void InitScrollSprites(UXFactory source, UIScrollView scrollView, float scrollPosition, bool isScrollable)
		{
		}

		public void HideScrollSprites()
		{
		}

		public void UpdateScrollSprites(UIScrollView scrollView, float scrollPosition, bool isScrollable)
		{
		}
	}
}
