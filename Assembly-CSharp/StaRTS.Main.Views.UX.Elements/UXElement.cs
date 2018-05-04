using NGUIExtensions;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXElement
	{
		public const string ON_ANIMATION_EVENT_FUNCTION = "OnAnimationEvent";

		private const string ANIMATOR_MISSING = "Animator missing for : '{0}'";

		private const string ANIMATOR_NOT_INITIALIZED = "Animator not set. Call InitAnimator(). On: '{0}'";

		private const string COLLIDER_MISSING = "Collider missing for : '{0}'";

		protected UXCamera uxCamera;

		protected GameObject root;

		protected UIButton enabler;

		protected UXButton uxButton;

		protected UXTween uxTween;

		private UIWidget NGUIWidget;

		private UIPanel rootPanel;

		protected Animator animator;

		public bool SendDestroyEvent;

		public object Tag
		{
			get;
			set;
		}

		public bool OrigVisible
		{
			get;
			private set;
		}

		public GameObject Root
		{
			get
			{
				return this.root;
			}
		}

		public UIWidget GetUIWidget
		{
			get
			{
				return this.NGUIWidget;
			}
		}

		public UXCamera UXCamera
		{
			get
			{
				return this.uxCamera;
			}
			set
			{
				this.uxCamera = value;
			}
		}

		public virtual bool Visible
		{
			get
			{
				return this.root != null && this.root.activeSelf;
			}
			set
			{
				if (this.root != null)
				{
					if (value && !this.root.activeSelf)
					{
						this.root.SetActive(true);
					}
					else if (!value && this.root.activeSelf)
					{
						this.root.SetActive(false);
					}
				}
			}
		}

		public bool Enabled
		{
			get
			{
				return this.enabler == null || this.enabler.isEnabled;
			}
			set
			{
				if (this.enabler != null)
				{
					this.enabler.isEnabled = value;
				}
				if (this.root != null)
				{
					ButtonTap component = this.root.GetComponent<ButtonTap>();
					if (component != null)
					{
						component.enabled = value;
					}
				}
				if (this.uxButton != null)
				{
					this.uxButton.Enabled = value;
				}
			}
		}

		public UXButtonClickedDelegate OnElementClicked
		{
			get
			{
				return this.uxButton.OnClicked;
			}
			set
			{
				if (this.uxButton != null)
				{
					this.uxButton.OnClicked = value;
				}
			}
		}

		public UXElement Parent
		{
			set
			{
				if (this.root != null)
				{
					this.root.transform.parent = ((value != null) ? value.Root.transform : null);
				}
			}
		}

		public Vector3 WorldPosition
		{
			get
			{
				if (this.root != null)
				{
					return this.root.transform.position;
				}
				return Vector3.zero;
			}
		}

		public Vector3 Position
		{
			get
			{
				if (this.root != null)
				{
					Vector3 a = this.uxCamera.Camera.WorldToScreenPoint(this.root.transform.position);
					return a * this.uxCamera.Scale;
				}
				return Vector3.zero;
			}
			set
			{
				if (this.root != null)
				{
					Vector3 position = value;
					position.x = Mathf.Round(position.x / this.uxCamera.Scale);
					position.y = Mathf.Round(position.y / this.uxCamera.Scale);
					position.z = Mathf.Round(position.z / this.uxCamera.Scale);
					this.root.transform.position = this.uxCamera.Camera.ScreenToWorldPoint(position);
				}
			}
		}

		public Vector3 LocalPosition
		{
			get
			{
				return (!(this.root == null)) ? (this.root.transform.localPosition * this.uxCamera.Scale) : Vector3.zero;
			}
			set
			{
				if (this.root != null)
				{
					Vector3 localPosition = value;
					localPosition.x = Mathf.Round(localPosition.x / this.uxCamera.Scale);
					localPosition.y = Mathf.Round(localPosition.y / this.uxCamera.Scale);
					localPosition.z = Mathf.Round(localPosition.z / this.uxCamera.Scale);
					this.root.transform.localPosition = localPosition;
				}
			}
		}

		public Vector3 LocalScale
		{
			get
			{
				return (!(this.root == null)) ? (this.root.transform.localScale * this.uxCamera.Scale) : Vector3.one;
			}
			set
			{
				if (this.root != null)
				{
					this.root.transform.localScale = value / this.uxCamera.Scale;
				}
			}
		}

		public float Width
		{
			get
			{
				return (!(this.NGUIWidget == null)) ? ((float)this.NGUIWidget.width * this.uxCamera.Scale) : 0f;
			}
			set
			{
				if (this.NGUIWidget != null)
				{
					this.NGUIWidget.width = (int)Mathf.Round(value / this.uxCamera.Scale);
				}
			}
		}

		public float Height
		{
			get
			{
				return (!(this.NGUIWidget == null)) ? ((float)this.NGUIWidget.height * this.uxCamera.Scale) : 0f;
			}
			set
			{
				if (this.NGUIWidget != null)
				{
					this.NGUIWidget.height = (int)Mathf.Round(value / this.uxCamera.Scale);
				}
			}
		}

		public float ColliderXUnscaled
		{
			get
			{
				if (this.HasCollider())
				{
					return this.root.GetComponent<BoxCollider>().center.x;
				}
				Service.Logger.WarnFormat("Collider missing for : '{0}'", new object[]
				{
					(!(this.root == null)) ? this.root.name : "null root"
				});
				return 1f;
			}
		}

		public float ColliderWidthUnscaled
		{
			get
			{
				if (this.HasCollider())
				{
					return this.root.GetComponent<BoxCollider>().size.x;
				}
				Service.Logger.WarnFormat("Collider missing for : '{0}'", new object[]
				{
					(!(this.root == null)) ? this.root.name : "null root"
				});
				return 1f;
			}
		}

		public float ColliderWidth
		{
			get
			{
				if (this.root != null && this.root.GetComponent<Collider>() != null)
				{
					return this.uxCamera.ScaleColliderHorizontally(this.root.GetComponent<Collider>().bounds.size.x);
				}
				return 0f;
			}
		}

		public float ColliderHeight
		{
			get
			{
				if (this.root != null && this.root.GetComponent<Collider>() != null)
				{
					return this.uxCamera.ScaleColliderVertically(this.root.GetComponent<Collider>().bounds.size.y);
				}
				return 0f;
			}
		}

		public int WidgetDepth
		{
			get
			{
				int result = 0;
				if (this.root != null)
				{
					UXElement.GetRootDepth(this.root, ref result);
				}
				return result;
			}
			set
			{
				if (this.root != null)
				{
					int rootDepth = 0;
					if (UXElement.GetRootDepth(this.root, ref rootDepth))
					{
						UXElement.SetHierarchyDepth(this.root, value, rootDepth);
					}
				}
			}
		}

		public UXElement(UXCamera uxCamera, GameObject root, UIButton enabler)
		{
			this.uxCamera = uxCamera;
			this.enabler = enabler;
			this.InternalSetRoot(root);
		}

		public void InternalSetRoot(GameObject root)
		{
			this.root = root;
			this.NGUIWidget = ((!(root == null)) ? root.GetComponent<UIWidget>() : null);
			if (root != null)
			{
				this.OrigVisible = this.Visible;
			}
		}

		public void EnablePlayTween()
		{
			if (this.uxTween != null)
			{
				this.uxTween.Enable = true;
			}
		}

		public void DisablePlayTween()
		{
			if (this.uxTween != null)
			{
				this.uxTween.Enable = false;
			}
		}

		public bool IsPlayTweenEnabled()
		{
			return this.uxTween != null && this.uxTween.Enable;
		}

		public void ResetPlayTweenTarget()
		{
			if (this.uxTween != null)
			{
				this.uxTween.ResetUIPlayTweenTargetToBegining();
			}
		}

		public void InitTweenComponent()
		{
			if (this.Root != null)
			{
				if (this.uxTween == null)
				{
					this.uxTween = new UXTween();
				}
				this.uxTween.Init(this.Root);
			}
		}

		public void PlayTween(bool play)
		{
			UIPlayTween component = this.root.GetComponent<UIPlayTween>();
			if (component == null)
			{
				return;
			}
			component.Play(play);
		}

		public void AddUXButton(UXButton btn)
		{
			this.uxButton = btn;
		}

		public bool HasCollider()
		{
			return this.root != null && this.root.GetComponent<Collider>() != null;
		}

		public void SetRootName(string name)
		{
			if (this.root != null)
			{
				this.root.name = name;
			}
		}

		private GameObject GetPanelUnifiedAnchorTarget(UIPanel panel)
		{
			GameObject result = null;
			if (panel != null)
			{
				if (panel.topAnchor.target != null)
				{
					result = panel.topAnchor.target.gameObject;
				}
				else if (panel.leftAnchor.target != null)
				{
					result = panel.leftAnchor.target.gameObject;
				}
				else if (panel.bottomAnchor.target != null)
				{
					result = panel.bottomAnchor.target.gameObject;
				}
				else if (panel.rightAnchor.target != null)
				{
					result = panel.rightAnchor.target.gameObject;
				}
			}
			return result;
		}

		private UIPanel GetRootPanel()
		{
			if (this.rootPanel == null && this.Root != null)
			{
				this.rootPanel = this.Root.GetComponent<UIPanel>();
			}
			return this.rootPanel;
		}

		public void RefreshPanel()
		{
			UIPanel uIPanel = this.GetRootPanel();
			if (uIPanel != null)
			{
				uIPanel.Refresh();
			}
		}

		public int GetPanelAnchorOffset(UXAnchorSection anchorSection)
		{
			int result = 0;
			UIPanel uIPanel = this.GetRootPanel();
			if (uIPanel != null)
			{
				switch (anchorSection)
				{
				case UXAnchorSection.Top:
					result = uIPanel.topAnchor.absolute;
					break;
				case UXAnchorSection.Left:
					result = uIPanel.leftAnchor.absolute;
					break;
				case UXAnchorSection.Bottom:
					result = uIPanel.bottomAnchor.absolute;
					break;
				case UXAnchorSection.Right:
					result = uIPanel.rightAnchor.absolute;
					break;
				}
			}
			return result;
		}

		public void SetAnchorWidget(UXElement target)
		{
			UIWidget component = this.Root.GetComponent<UIWidget>();
			component.SetAnchor(target.Root.transform);
			component.ResetAndUpdateAnchors();
		}

		public void SetPanelUnifiedAnchorOffsets(int left, int bottom, int right, int top)
		{
			UIPanel uIPanel = this.GetRootPanel();
			if (uIPanel != null)
			{
				GameObject panelUnifiedAnchorTarget = this.GetPanelUnifiedAnchorTarget(uIPanel);
				uIPanel.SetAnchor(panelUnifiedAnchorTarget, left, bottom, right, top);
			}
		}

		public void SetPanelUnifiedAnchorBottomOffset(int bottom)
		{
			UIPanel uIPanel = this.GetRootPanel();
			if (uIPanel != null)
			{
				int absolute = uIPanel.leftAnchor.absolute;
				int absolute2 = uIPanel.rightAnchor.absolute;
				int absolute3 = uIPanel.topAnchor.absolute;
				this.SetPanelUnifiedAnchorOffsets(absolute, bottom, absolute2, absolute3);
			}
		}

		public void SetPanelUnifiedAnchorTopOffset(int top)
		{
			UIPanel uIPanel = this.GetRootPanel();
			if (uIPanel != null)
			{
				int absolute = uIPanel.leftAnchor.absolute;
				int absolute2 = uIPanel.rightAnchor.absolute;
				int absolute3 = uIPanel.bottomAnchor.absolute;
				this.SetPanelUnifiedAnchorOffsets(absolute, absolute3, absolute2, top);
			}
		}

		public void SetPanelUnifiedAnchorLeftOffset(int left)
		{
			UIPanel uIPanel = this.GetRootPanel();
			if (uIPanel != null)
			{
				int absolute = uIPanel.rightAnchor.absolute;
				int absolute2 = uIPanel.topAnchor.absolute;
				int absolute3 = uIPanel.bottomAnchor.absolute;
				this.SetPanelUnifiedAnchorOffsets(left, absolute3, absolute, absolute2);
			}
		}

		public void SetPanelUnifiedAnchorRightOffset(int right)
		{
			UIPanel uIPanel = this.GetRootPanel();
			if (uIPanel != null)
			{
				int absolute = uIPanel.leftAnchor.absolute;
				int absolute2 = uIPanel.topAnchor.absolute;
				int absolute3 = uIPanel.bottomAnchor.absolute;
				this.SetPanelUnifiedAnchorOffsets(absolute, absolute3, right, absolute2);
			}
		}

		private static bool GetRootDepth(GameObject gameObject, ref int depth)
		{
			UIPanel component = gameObject.GetComponent<UIPanel>();
			if (component != null)
			{
				depth = component.depth;
				return true;
			}
			UIWidget component2 = gameObject.GetComponent<UIWidget>();
			if (component2 != null)
			{
				depth = component2.depth;
				return true;
			}
			bool flag = false;
			Transform transform = gameObject.transform;
			int i = 0;
			int childCount = transform.childCount;
			while (i < childCount)
			{
				int num = 0;
				if (UXElement.GetRootDepth(transform.GetChild(i).gameObject, ref num) && (!flag || num < depth))
				{
					depth = num;
					flag = true;
				}
				i++;
			}
			return flag;
		}

		private static void SetHierarchyDepth(GameObject gameObject, int depth, int rootDepth)
		{
			UIWidget component = gameObject.GetComponent<UIWidget>();
			if (component != null)
			{
				int num = depth + component.depth - rootDepth;
				Vector3 localPosition = gameObject.transform.localPosition;
				localPosition.z = (float)(-(float)num) / 10000f;
				gameObject.transform.localPosition = localPosition;
				component.depth = num;
			}
			UIPanel component2 = gameObject.GetComponent<UIPanel>();
			if (component2 != null)
			{
				component2.depth = depth + component2.depth - rootDepth;
			}
			Transform transform = gameObject.transform;
			int i = 0;
			int childCount = transform.childCount;
			while (i < childCount)
			{
				UXElement.SetHierarchyDepth(transform.GetChild(i).gameObject, depth, rootDepth);
				i++;
			}
		}

		public GameObject CloneRoot(string name, GameObject parent)
		{
			if (this.root == null)
			{
				return null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.root);
			gameObject.layer = parent.layer;
			UXUtils.AppendNameRecursively(gameObject, name, true);
			gameObject.SetActive(true);
			gameObject.transform.parent = parent.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
			return gameObject;
		}

		public virtual void InternalDestroyComponent()
		{
		}

		public virtual void OnDestroyElement()
		{
			if (this.SendDestroyEvent)
			{
				Service.EventManager.SendEvent(EventId.ElementDestroyed, this);
			}
			this.enabler = null;
		}

		protected void SendClickEvent()
		{
			if (this.root != null)
			{
				Service.EventManager.SendEvent(EventId.ButtonClicked, this.root.name);
			}
		}

		public void InitAnimator()
		{
			if (this.Root != null)
			{
				this.animator = this.Root.GetComponent<Animator>();
				if (this.animator == null)
				{
					Service.Logger.WarnFormat("Animator missing for : '{0}'", new object[]
					{
						this.root.name
					});
				}
			}
		}

		private bool IsAnimatorSet()
		{
			if (this.animator == null)
			{
				if (this.root != null)
				{
					Service.Logger.WarnFormat("Animator not set. Call InitAnimator(). On: '{0}'", new object[]
					{
						this.root.name
					});
				}
				return false;
			}
			return true;
		}

		public void SetTrigger(string triggerName)
		{
			if (this.IsAnimatorSet())
			{
				this.animator.SetTrigger(triggerName);
			}
		}

		public void ResetTrigger(string triggerName)
		{
			if (this.IsAnimatorSet())
			{
				this.animator.ResetTrigger(triggerName);
			}
		}

		public bool IsCurrentAnimatorState(string stateName)
		{
			return this.IsAnimatorSet() && this.animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
		}

		public bool IsAnimatorTransitioning()
		{
			return this.IsAnimatorSet() && this.animator.IsInTransition(0);
		}

		public Vector3[] GetWorldCorners()
		{
			if (this.NGUIWidget != null)
			{
				return this.NGUIWidget.worldCorners;
			}
			return null;
		}

		public void SkipBoundsCalculations(bool skip)
		{
			Transform transform = this.root.transform;
			int i = 0;
			int childCount = transform.childCount;
			while (i < childCount)
			{
				UIWidget component = transform.GetChild(i).GetComponent<UIWidget>();
				if (component != null)
				{
					component.skipBoundsCalculations = skip;
				}
				i++;
			}
		}

		public void SetPanelAnimationEventCallback(UIPanel.OnAnimationEventCallback callback)
		{
			this.GetRootPanel().animationEventCallback = callback;
		}
	}
}
