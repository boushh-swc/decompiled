using System;

namespace StaRTS.Main.Views.UX.Elements
{
	public interface IUXScrollSpriteHandler
	{
		void InitScrollSprites(UXFactory source, UIScrollView scrollView, float scrollPosition, bool isScrollable);

		void HideScrollSprites();

		void UpdateScrollSprites(UIScrollView scrollView, float scrollPosition, bool isScrollable);
	}
}
