using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class AbstractUXListComponent : MonoBehaviour, IEventObserver, IViewFrameTimeObserver
	{
		private IUXScrollSpriteHandler scrollSpriteHandler;

		protected Action reposCallback;

		public UIPanel NGUIPanel
		{
			get;
			set;
		}

		public UIScrollView NGUIScrollView
		{
			get;
			set;
		}

		public UICenterOnChild NGUICenterOnChild
		{
			get;
			set;
		}

		public Vector4 ClipRegion
		{
			get
			{
				return (!(this.NGUIPanel == null)) ? this.NGUIPanel.baseClipRegion : Vector4.zero;
			}
			set
			{
				if (this.NGUIPanel != null)
				{
					this.NGUIPanel.baseClipRegion = value;
				}
			}
		}

		public Action RepositionCallback
		{
			get
			{
				return this.reposCallback;
			}
			set
			{
				this.reposCallback = value;
			}
		}

		public virtual void Init()
		{
			this.scrollSpriteHandler = new UXScrollSpriteHandler();
			Service.EventManager.RegisterObserver(this, EventId.AllUXElementsCreated, EventPriority.Default);
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public virtual void InternalDestroyComponent()
		{
			Service.EventManager.UnregisterObserver(this, EventId.AllUXElementsCreated);
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		public virtual Vector3 GetItemDimension()
		{
			return Vector3.zero;
		}

		public virtual void Scroll(float location)
		{
		}

		public virtual void RepositionItems(bool delayedReposition)
		{
		}

		protected virtual void OnDrag()
		{
		}

		public virtual float GetCurrentScrollPosition(bool softClip)
		{
			return 0f;
		}

		protected void OnReposition()
		{
			if (this.reposCallback != null)
			{
				this.reposCallback();
			}
		}

		public bool IsScrollable()
		{
			if (this.NGUIScrollView != null)
			{
				if (!this.NGUIScrollView.disableDragIfFits)
				{
					return true;
				}
				UIPanel panel = this.NGUIScrollView.panel;
				if (panel != null)
				{
					Vector4 finalClipRegion = panel.finalClipRegion;
					Bounds bounds = this.NGUIScrollView.bounds;
					float num = (finalClipRegion.z != 0f) ? (finalClipRegion.z * 0.5f) : ((float)Screen.width);
					float num2 = (finalClipRegion.w != 0f) ? (finalClipRegion.w * 0.5f) : ((float)Screen.height);
					if (this.NGUIScrollView.canMoveHorizontally)
					{
						if (bounds.min.x < finalClipRegion.x - num)
						{
							return true;
						}
						if (bounds.max.x > finalClipRegion.x + num)
						{
							return true;
						}
					}
					if (this.NGUIScrollView.canMoveVertically)
					{
						if (bounds.min.y < finalClipRegion.y - num2)
						{
							return true;
						}
						if (bounds.max.y > finalClipRegion.y + num2)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public void InitScrollArrows(UXFactory source)
		{
			this.scrollSpriteHandler.InitScrollSprites(source, this.NGUIScrollView, this.GetCurrentScrollPosition(false), this.IsScrollable());
			Service.EventManager.UnregisterObserver(this, EventId.AllUXElementsCreated);
		}

		public void HideScrollArrows()
		{
			this.scrollSpriteHandler.HideScrollSprites();
		}

		public virtual void UpdateScrollArrows()
		{
			this.scrollSpriteHandler.UpdateScrollSprites(this.NGUIScrollView, this.GetCurrentScrollPosition(false), this.IsScrollable());
		}

		public virtual void OnViewFrameTime(float dt)
		{
			if (this.NGUIScrollView != null && this.NGUIScrollView.isDragging)
			{
				this.OnDrag();
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.AllUXElementsCreated)
			{
				this.InitScrollArrows((UXFactory)cookie);
			}
			return EatResponse.NotEaten;
		}

		public void ResetScrollViewPosition()
		{
			if (this.NGUIScrollView != null)
			{
				this.NGUIScrollView.ResetPosition();
			}
		}
	}
}
