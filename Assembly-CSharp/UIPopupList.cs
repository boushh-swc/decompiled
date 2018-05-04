using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Popup List"), ExecuteInEditMode]
public class UIPopupList : UIWidgetContainer
{
	public enum Position
	{
		Auto = 0,
		Above = 1,
		Below = 2
	}

	public enum Selection
	{
		OnPress = 0,
		OnClick = 1
	}

	public enum OpenOn
	{
		ClickOrTap = 0,
		RightClick = 1,
		DoubleClick = 2,
		Manual = 3
	}

	public delegate void LegacyEvent(string val);

	public static UIPopupList current;

	protected static GameObject mChild;

	protected static float mFadeOutComplete;

	private const float animSpeed = 0.15f;

	public UIAtlas atlas;

	public UIFont bitmapFont;

	public Font trueTypeFont;

	public int fontSize = 16;

	public FontStyle fontStyle;

	public string backgroundSprite;

	public string highlightSprite;

	public Sprite background2DSprite;

	public Sprite highlight2DSprite;

	public UIPopupList.Position position;

	public UIPopupList.Selection selection;

	public NGUIText.Alignment alignment = NGUIText.Alignment.Left;

	public List<string> items = new List<string>();

	public List<object> itemData = new List<object>();

	public List<Action> itemCallbacks = new List<Action>();

	public Vector2 padding = new Vector3(4f, 4f);

	public Color textColor = Color.white;

	public Color backgroundColor = Color.white;

	public Color highlightColor = new Color(0.882352948f, 0.784313738f, 0.5882353f, 1f);

	public bool isAnimated = true;

	public bool isLocalized;

	public UILabel.Modifier textModifier;

	public bool separatePanel = true;

	public int overlap;

	public UIPopupList.OpenOn openOn;

	public List<EventDelegate> onChange = new List<EventDelegate>();

	[HideInInspector, SerializeField]
	protected string mSelectedItem;

	[HideInInspector, SerializeField]
	protected UIPanel mPanel;

	[HideInInspector, SerializeField]
	protected UIBasicSprite mBackground;

	[HideInInspector, SerializeField]
	protected UIBasicSprite mHighlight;

	[HideInInspector, SerializeField]
	protected UILabel mHighlightedLabel;

	[HideInInspector, SerializeField]
	protected List<UILabel> mLabelList = new List<UILabel>();

	[HideInInspector, SerializeField]
	protected float mBgBorder;

	[Tooltip("Whether the selection will be persistent even after the popup list is closed. By default the selection is cleared when the popup is closed so that the same selection can be chosen again the next time the popup list is opened. If enabled, the selection will persist, but selecting the same choice in succession will not result in the onChange notification being triggered more than once.")]
	public bool keepValue;

	[NonSerialized]
	protected GameObject mSelection;

	[NonSerialized]
	protected int mOpenFrame;

	[HideInInspector, SerializeField]
	private GameObject eventReceiver;

	[HideInInspector, SerializeField]
	private string functionName = "OnSelectionChange";

	[HideInInspector, SerializeField]
	private float textScale;

	[HideInInspector, SerializeField]
	private UIFont font;

	[HideInInspector, SerializeField]
	private UILabel textLabel;

	[NonSerialized]
	public Vector3 startingPosition;

	private UIPopupList.LegacyEvent mLegacyEvent;

	[NonSerialized]
	protected bool mExecuting;

	protected bool mUseDynamicFont;

	[NonSerialized]
	protected bool mStarted;

	protected bool mTweening;

	public GameObject source;

	public UnityEngine.Object ambigiousFont
	{
		get
		{
			if (this.trueTypeFont != null)
			{
				return this.trueTypeFont;
			}
			if (this.bitmapFont != null)
			{
				return this.bitmapFont;
			}
			return this.font;
		}
		set
		{
			if (value is Font)
			{
				this.trueTypeFont = (value as Font);
				this.bitmapFont = null;
				this.font = null;
			}
			else if (value is UIFont)
			{
				this.bitmapFont = (value as UIFont);
				this.trueTypeFont = null;
				this.font = null;
			}
		}
	}

	[Obsolete("Use EventDelegate.Add(popup.onChange, YourCallback) instead, and UIPopupList.current.value to determine the state")]
	public UIPopupList.LegacyEvent onSelectionChange
	{
		get
		{
			return this.mLegacyEvent;
		}
		set
		{
			this.mLegacyEvent = value;
		}
	}

	public static bool isOpen
	{
		get
		{
			return UIPopupList.current != null && (UIPopupList.mChild != null || UIPopupList.mFadeOutComplete > Time.unscaledTime);
		}
	}

	public virtual string value
	{
		get
		{
			return this.mSelectedItem;
		}
		set
		{
			this.Set(value, true);
		}
	}

	public virtual object data
	{
		get
		{
			int num = this.items.IndexOf(this.mSelectedItem);
			return (num <= -1 || num >= this.itemData.Count) ? null : this.itemData[num];
		}
	}

	public Action callback
	{
		get
		{
			int num = this.items.IndexOf(this.mSelectedItem);
			return (num <= -1 || num >= this.itemCallbacks.Count) ? null : this.itemCallbacks[num];
		}
	}

	public bool isColliderEnabled
	{
		get
		{
			Collider component = base.GetComponent<Collider>();
			if (component != null)
			{
				return component.enabled;
			}
			Collider2D component2 = base.GetComponent<Collider2D>();
			return component2 != null && component2.enabled;
		}
	}

	protected bool isValid
	{
		get
		{
			return this.bitmapFont != null || this.trueTypeFont != null;
		}
	}

	protected int activeFontSize
	{
		get
		{
			return (!(this.trueTypeFont != null) && !(this.bitmapFont == null)) ? this.bitmapFont.defaultSize : this.fontSize;
		}
	}

	protected float activeFontScale
	{
		get
		{
			return (!(this.trueTypeFont != null) && !(this.bitmapFont == null)) ? ((float)this.fontSize / (float)this.bitmapFont.defaultSize) : 1f;
		}
	}

	protected float fitScale
	{
		get
		{
			if (this.separatePanel)
			{
				float num = (float)this.items.Count * ((float)this.fontSize + this.padding.y) + this.padding.y;
				float y = NGUITools.screenSize.y;
				if (num > y)
				{
					return y / num;
				}
			}
			else if (this.mPanel != null && this.mPanel.anchorCamera != null && this.mPanel.anchorCamera.orthographic)
			{
				float num2 = (float)this.items.Count * ((float)this.fontSize + this.padding.y) + this.padding.y;
				float height = this.mPanel.height;
				if (num2 > height)
				{
					return height / num2;
				}
			}
			return 1f;
		}
	}

	public void Set(string value, bool notify = true)
	{
		if (this.mSelectedItem != value)
		{
			this.mSelectedItem = value;
			if (this.mSelectedItem == null)
			{
				return;
			}
			if (notify && this.mSelectedItem != null)
			{
				this.TriggerCallbacks();
			}
			if (!this.keepValue)
			{
				this.mSelectedItem = null;
			}
		}
	}

	public virtual void Clear()
	{
		this.items.Clear();
		this.itemData.Clear();
		this.itemCallbacks.Clear();
	}

	public virtual void AddItem(string text)
	{
		this.items.Add(text);
		this.itemData.Add(text);
		this.itemCallbacks.Add(null);
	}

	public virtual void AddItem(string text, Action del)
	{
		this.items.Add(text);
		this.itemCallbacks.Add(del);
	}

	public virtual void AddItem(string text, object data, Action del = null)
	{
		this.items.Add(text);
		this.itemData.Add(data);
		this.itemCallbacks.Add(del);
	}

	public virtual void RemoveItem(string text)
	{
		int num = this.items.IndexOf(text);
		if (num != -1)
		{
			this.items.RemoveAt(num);
			this.itemData.RemoveAt(num);
			if (num < this.itemCallbacks.Count)
			{
				this.itemCallbacks.RemoveAt(num);
			}
		}
	}

	public virtual void RemoveItemByData(object data)
	{
		int num = this.itemData.IndexOf(data);
		if (num != -1)
		{
			this.items.RemoveAt(num);
			this.itemData.RemoveAt(num);
			if (num < this.itemCallbacks.Count)
			{
				this.itemCallbacks.RemoveAt(num);
			}
		}
	}

	protected void TriggerCallbacks()
	{
		if (!this.mExecuting)
		{
			this.mExecuting = true;
			UIPopupList uIPopupList = UIPopupList.current;
			UIPopupList.current = this;
			if (this.mLegacyEvent != null)
			{
				this.mLegacyEvent(this.mSelectedItem);
			}
			if (EventDelegate.IsValid(this.onChange))
			{
				EventDelegate.Execute(this.onChange);
			}
			else if (this.eventReceiver != null && !string.IsNullOrEmpty(this.functionName))
			{
				this.eventReceiver.SendMessage(this.functionName, this.mSelectedItem, SendMessageOptions.DontRequireReceiver);
			}
			Action callback = this.callback;
			if (callback != null)
			{
				callback();
			}
			UIPopupList.current = uIPopupList;
			this.mExecuting = false;
		}
	}

	protected virtual void OnEnable()
	{
		if (EventDelegate.IsValid(this.onChange))
		{
			this.eventReceiver = null;
			this.functionName = null;
		}
		if (this.font != null)
		{
			if (this.font.isDynamic)
			{
				this.trueTypeFont = this.font.dynamicFont;
				this.fontStyle = this.font.dynamicFontStyle;
				this.mUseDynamicFont = true;
			}
			else if (this.bitmapFont == null)
			{
				this.bitmapFont = this.font;
				this.mUseDynamicFont = false;
			}
			this.font = null;
		}
		if (this.textScale != 0f)
		{
			this.fontSize = ((!(this.bitmapFont != null)) ? 16 : Mathf.RoundToInt((float)this.bitmapFont.defaultSize * this.textScale));
			this.textScale = 0f;
		}
		if (this.trueTypeFont == null && this.bitmapFont != null && this.bitmapFont.isDynamic && this.bitmapFont.replacement == null)
		{
			this.trueTypeFont = this.bitmapFont.dynamicFont;
			this.bitmapFont = null;
		}
	}

	protected virtual void OnValidate()
	{
		Font x = this.trueTypeFont;
		UIFont uIFont = this.bitmapFont;
		this.bitmapFont = null;
		this.trueTypeFont = null;
		if (x != null && (uIFont == null || !this.mUseDynamicFont))
		{
			this.bitmapFont = null;
			this.trueTypeFont = x;
			this.mUseDynamicFont = true;
		}
		else if (uIFont != null)
		{
			if (uIFont.replacement == null)
			{
				if (uIFont.isDynamic)
				{
					this.trueTypeFont = uIFont.dynamicFont;
					this.fontStyle = uIFont.dynamicFontStyle;
					this.fontSize = uIFont.defaultSize;
					this.mUseDynamicFont = true;
				}
				else
				{
					this.bitmapFont = uIFont;
					this.mUseDynamicFont = false;
				}
			}
		}
		else
		{
			this.trueTypeFont = x;
			this.mUseDynamicFont = true;
		}
	}

	public virtual void Start()
	{
		if (this.mStarted)
		{
			return;
		}
		this.mStarted = true;
		if (this.keepValue)
		{
			string value = this.mSelectedItem;
			this.mSelectedItem = null;
			this.value = value;
		}
		else
		{
			this.mSelectedItem = null;
		}
		if (this.textLabel != null)
		{
			EventDelegate.Add(this.onChange, new EventDelegate.Callback(this.textLabel.SetCurrentSelection));
			this.textLabel = null;
		}
	}

	protected virtual void OnLocalize()
	{
		if (this.isLocalized)
		{
			this.TriggerCallbacks();
		}
	}

	protected virtual void Highlight(UILabel lbl, bool instant)
	{
		if (this.mHighlight != null)
		{
			this.mHighlightedLabel = lbl;
			Vector3 highlightPosition = this.GetHighlightPosition();
			if (!instant && this.isAnimated)
			{
				TweenPosition.Begin(this.mHighlight.gameObject, 0.1f, highlightPosition).method = UITweener.Method.EaseOut;
				if (!this.mTweening)
				{
					this.mTweening = true;
					base.StartCoroutine("UpdateTweenPosition");
				}
			}
			else
			{
				this.mHighlight.cachedTransform.localPosition = highlightPosition;
			}
		}
	}

	protected virtual Vector3 GetHighlightPosition()
	{
		if (this.mHighlightedLabel == null || this.mHighlight == null)
		{
			return Vector3.zero;
		}
		Vector4 border = this.mHighlight.border;
		float num = (!(this.atlas != null)) ? 1f : this.atlas.pixelSize;
		float num2 = border.x * num;
		float y = border.w * num;
		return this.mHighlightedLabel.cachedTransform.localPosition + new Vector3(-num2, y, 1f);
	}

	[DebuggerHidden]
	protected virtual IEnumerator UpdateTweenPosition()
	{
		if (this.mHighlight != null && this.mHighlightedLabel != null)
		{
			TweenPosition component = this.mHighlight.GetComponent<TweenPosition>();
			while (component != null && component.enabled)
			{
				component.to = this.GetHighlightPosition();
				yield return null;
			}
		}
		this.mTweening = false;
		yield break;
	}

	protected virtual void OnItemHover(GameObject go, bool isOver)
	{
		if (isOver)
		{
			UILabel component = go.GetComponent<UILabel>();
			this.Highlight(component, false);
		}
	}

	protected virtual void OnItemPress(GameObject go, bool isPressed)
	{
		if (isPressed && this.selection == UIPopupList.Selection.OnPress)
		{
			this.OnItemClick(go);
		}
	}

	protected virtual void OnItemClick(GameObject go)
	{
		this.Select(go.GetComponent<UILabel>(), true);
		UIEventListener component = go.GetComponent<UIEventListener>();
		this.value = (component.parameter as string);
		UIPlaySound[] components = base.GetComponents<UIPlaySound>();
		int i = 0;
		int num = components.Length;
		while (i < num)
		{
			UIPlaySound uIPlaySound = components[i];
			if (uIPlaySound.trigger == UIPlaySound.Trigger.OnClick)
			{
				NGUITools.PlaySound(uIPlaySound.audioClip, uIPlaySound.volume, 1f);
			}
			i++;
		}
		this.CloseSelf();
	}

	private void Select(UILabel lbl, bool instant)
	{
		this.Highlight(lbl, instant);
	}

	protected virtual void OnNavigate(KeyCode key)
	{
		if (base.enabled && UIPopupList.current == this)
		{
			int num = this.mLabelList.IndexOf(this.mHighlightedLabel);
			if (num == -1)
			{
				num = 0;
			}
			if (key == KeyCode.UpArrow)
			{
				if (num > 0)
				{
					this.Select(this.mLabelList[num - 1], false);
				}
			}
			else if (key == KeyCode.DownArrow && num + 1 < this.mLabelList.Count)
			{
				this.Select(this.mLabelList[num + 1], false);
			}
		}
	}

	protected virtual void OnKey(KeyCode key)
	{
		if (base.enabled && UIPopupList.current == this && (key == UICamera.current.cancelKey0 || key == UICamera.current.cancelKey1))
		{
			this.OnSelect(false);
		}
	}

	protected virtual void OnDisable()
	{
		this.CloseSelf();
	}

	protected virtual void OnSelect(bool isSelected)
	{
		if (!isSelected)
		{
			GameObject selectedObject = UICamera.selectedObject;
			if (selectedObject == null || (!(selectedObject == UIPopupList.mChild) && (!(UIPopupList.mChild != null) || !(selectedObject != null) || !NGUITools.IsChild(UIPopupList.mChild.transform, selectedObject.transform))))
			{
				this.CloseSelf();
			}
		}
	}

	public static void Close()
	{
		if (UIPopupList.current != null)
		{
			UIPopupList.current.CloseSelf();
			UIPopupList.current = null;
		}
	}

	public virtual void CloseSelf()
	{
		if (UIPopupList.mChild != null && UIPopupList.current == this)
		{
			base.StopCoroutine("CloseIfUnselected");
			this.mSelection = null;
			this.mLabelList.Clear();
			if (this.isAnimated)
			{
				UIWidget[] componentsInChildren = UIPopupList.mChild.GetComponentsInChildren<UIWidget>();
				int i = 0;
				int num = componentsInChildren.Length;
				while (i < num)
				{
					UIWidget uIWidget = componentsInChildren[i];
					Color color = uIWidget.color;
					color.a = 0f;
					TweenColor.Begin(uIWidget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
					i++;
				}
				Collider[] componentsInChildren2 = UIPopupList.mChild.GetComponentsInChildren<Collider>();
				int j = 0;
				int num2 = componentsInChildren2.Length;
				while (j < num2)
				{
					componentsInChildren2[j].enabled = false;
					j++;
				}
				UnityEngine.Object.Destroy(UIPopupList.mChild, 0.15f);
				UIPopupList.mFadeOutComplete = Time.unscaledTime + Mathf.Max(0.1f, 0.15f);
			}
			else
			{
				UnityEngine.Object.Destroy(UIPopupList.mChild);
				UIPopupList.mFadeOutComplete = Time.unscaledTime + 0.1f;
			}
			this.mBackground = null;
			this.mHighlight = null;
			UIPopupList.mChild = null;
			UIPopupList.current = null;
		}
	}

	protected virtual void AnimateColor(UIWidget widget)
	{
		Color color = widget.color;
		widget.color = new Color(color.r, color.g, color.b, 0f);
		TweenColor.Begin(widget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
	}

	protected virtual void AnimatePosition(UIWidget widget, bool placeAbove, float bottom)
	{
		Vector3 localPosition = widget.cachedTransform.localPosition;
		Vector3 localPosition2 = (!placeAbove) ? new Vector3(localPosition.x, 0f, localPosition.z) : new Vector3(localPosition.x, bottom, localPosition.z);
		widget.cachedTransform.localPosition = localPosition2;
		GameObject gameObject = widget.gameObject;
		TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
	}

	protected virtual void AnimateScale(UIWidget widget, bool placeAbove, float bottom)
	{
		GameObject gameObject = widget.gameObject;
		Transform cachedTransform = widget.cachedTransform;
		float fitScale = this.fitScale;
		float num = (float)this.activeFontSize * this.activeFontScale + this.mBgBorder * 2f;
		cachedTransform.localScale = new Vector3(fitScale, fitScale * num / (float)widget.height, fitScale);
		TweenScale.Begin(gameObject, 0.15f, Vector3.one).method = UITweener.Method.EaseOut;
		if (placeAbove)
		{
			Vector3 localPosition = cachedTransform.localPosition;
			cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y - fitScale * (float)widget.height + fitScale * num, localPosition.z);
			TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
		}
	}

	protected void Animate(UIWidget widget, bool placeAbove, float bottom)
	{
		this.AnimateColor(widget);
		this.AnimatePosition(widget, placeAbove, bottom);
	}

	protected virtual void OnClick()
	{
		if (this.mOpenFrame == Time.frameCount)
		{
			return;
		}
		if (UIPopupList.mChild == null)
		{
			if (this.openOn == UIPopupList.OpenOn.DoubleClick || this.openOn == UIPopupList.OpenOn.Manual)
			{
				return;
			}
			if (this.openOn == UIPopupList.OpenOn.RightClick && UICamera.currentTouchID != -2)
			{
				return;
			}
			this.Show();
		}
		else if (this.mHighlightedLabel != null)
		{
			this.OnItemPress(this.mHighlightedLabel.gameObject, true);
		}
	}

	protected virtual void OnDoubleClick()
	{
		if (this.openOn == UIPopupList.OpenOn.DoubleClick)
		{
			this.Show();
		}
	}

	[DebuggerHidden]
	private IEnumerator CloseIfUnselected()
	{
		GameObject selectedObject;
		do
		{
			yield return null;
			selectedObject = UICamera.selectedObject;
		}
		while (!(selectedObject != this.mSelection) || (!(selectedObject == null) && (selectedObject == UIPopupList.mChild || NGUITools.IsChild(UIPopupList.mChild.transform, selectedObject.transform))));
		this.CloseSelf();
		yield break;
	}

	public virtual void Show()
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && UIPopupList.mChild == null && this.isValid && this.items.Count > 0)
		{
			this.mLabelList.Clear();
			base.StopCoroutine("CloseIfUnselected");
			UICamera.selectedObject = (UICamera.hoveredObject ?? base.gameObject);
			this.mSelection = UICamera.selectedObject;
			this.source = this.mSelection;
			if (this.source == null)
			{
				UnityEngine.Debug.LogError("Popup list needs a source object...");
				return;
			}
			this.mOpenFrame = Time.frameCount;
			if (this.mPanel == null)
			{
				this.mPanel = UIPanel.Find(base.transform);
				if (this.mPanel == null)
				{
					return;
				}
			}
			UIPopupList.mChild = new GameObject("Drop-down List");
			UIPopupList.mChild.layer = base.gameObject.layer;
			if (this.separatePanel)
			{
				if (base.GetComponent<Collider>() != null)
				{
					Rigidbody rigidbody = UIPopupList.mChild.AddComponent<Rigidbody>();
					rigidbody.isKinematic = true;
				}
				else if (base.GetComponent<Collider2D>() != null)
				{
					Rigidbody2D rigidbody2D = UIPopupList.mChild.AddComponent<Rigidbody2D>();
					rigidbody2D.isKinematic = true;
				}
				UIPanel uIPanel = UIPopupList.mChild.AddComponent<UIPanel>();
				uIPanel.depth = 1000000;
				uIPanel.sortingOrder = this.mPanel.sortingOrder;
			}
			UIPopupList.current = this;
			Transform cachedTransform = this.mPanel.cachedTransform;
			Transform transform = UIPopupList.mChild.transform;
			transform.parent = cachedTransform;
			Transform parent = cachedTransform;
			if (this.separatePanel)
			{
				UIRoot uIRoot = this.mPanel.GetComponentInParent<UIRoot>();
				if (uIRoot == null && UIRoot.list.Count != 0)
				{
					uIRoot = UIRoot.list[0];
				}
				if (uIRoot != null)
				{
					parent = uIRoot.transform;
				}
			}
			Vector3 vector;
			Vector3 vector2;
			if (this.openOn == UIPopupList.OpenOn.Manual && this.mSelection != base.gameObject)
			{
				this.startingPosition = UICamera.lastEventPosition;
				vector = cachedTransform.InverseTransformPoint(this.mPanel.anchorCamera.ScreenToWorldPoint(this.startingPosition));
				vector2 = vector;
				transform.localPosition = vector;
				this.startingPosition = transform.position;
			}
			else
			{
				Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(cachedTransform, base.transform, false, false);
				vector = bounds.min;
				vector2 = bounds.max;
				transform.localPosition = vector;
				this.startingPosition = transform.position;
			}
			base.StartCoroutine("CloseIfUnselected");
			float fitScale = this.fitScale;
			transform.localRotation = Quaternion.identity;
			transform.localScale = new Vector3(fitScale, fitScale, fitScale);
			int num = (!this.separatePanel) ? NGUITools.CalculateNextDepth(this.mPanel.gameObject) : 0;
			if (this.background2DSprite != null)
			{
				UI2DSprite uI2DSprite = UIPopupList.mChild.AddWidget(num);
				uI2DSprite.sprite2D = this.background2DSprite;
				this.mBackground = uI2DSprite;
			}
			else
			{
				if (!(this.atlas != null))
				{
					return;
				}
				this.mBackground = UIPopupList.mChild.AddSprite(this.atlas, this.backgroundSprite, num);
			}
			bool flag = this.position == UIPopupList.Position.Above;
			if (this.position == UIPopupList.Position.Auto)
			{
				UICamera uICamera = UICamera.FindCameraForLayer(this.mSelection.layer);
				if (uICamera != null)
				{
					flag = (uICamera.cachedCamera.WorldToViewportPoint(this.startingPosition).y < 0.5f);
				}
			}
			this.mBackground.pivot = UIWidget.Pivot.TopLeft;
			this.mBackground.color = this.backgroundColor;
			Vector4 border = this.mBackground.border;
			this.mBgBorder = border.y;
			this.mBackground.cachedTransform.localPosition = new Vector3(0f, (!flag) ? ((float)this.overlap) : (border.y * 2f - (float)this.overlap), 0f);
			if (this.highlight2DSprite != null)
			{
				UI2DSprite uI2DSprite2 = UIPopupList.mChild.AddWidget(num + 1);
				uI2DSprite2.sprite2D = this.highlight2DSprite;
				this.mHighlight = uI2DSprite2;
			}
			else
			{
				if (!(this.atlas != null))
				{
					return;
				}
				this.mHighlight = UIPopupList.mChild.AddSprite(this.atlas, this.highlightSprite, num + 1);
			}
			float num2 = 0f;
			float num3 = 0f;
			if (this.mHighlight.hasBorder)
			{
				num2 = this.mHighlight.border.w;
				num3 = this.mHighlight.border.x;
			}
			this.mHighlight.pivot = UIWidget.Pivot.TopLeft;
			this.mHighlight.color = this.highlightColor;
			float num4 = (float)this.activeFontSize * this.activeFontScale;
			float num5 = num4 + this.padding.y;
			float num6 = 0f;
			float num7 = (!flag) ? (-this.padding.y - border.y + (float)this.overlap) : (border.y - this.padding.y - (float)this.overlap);
			float num8 = border.y * 2f + this.padding.y;
			List<UILabel> list = new List<UILabel>();
			if (!this.items.Contains(this.mSelectedItem))
			{
				this.mSelectedItem = null;
			}
			int i = 0;
			int count = this.items.Count;
			while (i < count)
			{
				string text = this.items[i];
				UILabel uILabel = UIPopupList.mChild.AddWidget(this.mBackground.depth + 2);
				uILabel.name = i.ToString();
				uILabel.pivot = UIWidget.Pivot.TopLeft;
				uILabel.bitmapFont = this.bitmapFont;
				uILabel.trueTypeFont = this.trueTypeFont;
				uILabel.fontSize = this.fontSize;
				uILabel.fontStyle = this.fontStyle;
				uILabel.text = ((!this.isLocalized) ? text : Localization.Get(text, true));
				uILabel.modifier = this.textModifier;
				uILabel.color = this.textColor;
				uILabel.cachedTransform.localPosition = new Vector3(border.x + this.padding.x - uILabel.pivotOffset.x, num7, -1f);
				uILabel.overflowMethod = UILabel.Overflow.ResizeFreely;
				uILabel.alignment = this.alignment;
				uILabel.symbolStyle = NGUIText.SymbolStyle.Colored;
				list.Add(uILabel);
				num8 += num5;
				num7 -= num5;
				num6 = Mathf.Max(num6, uILabel.printedSize.x);
				UIEventListener uIEventListener = UIEventListener.Get(uILabel.gameObject);
				uIEventListener.onHover = new UIEventListener.BoolDelegate(this.OnItemHover);
				uIEventListener.onPress = new UIEventListener.BoolDelegate(this.OnItemPress);
				uIEventListener.onClick = new UIEventListener.VoidDelegate(this.OnItemClick);
				uIEventListener.parameter = text;
				if (this.mSelectedItem == text || (i == 0 && string.IsNullOrEmpty(this.mSelectedItem)))
				{
					this.Highlight(uILabel, true);
				}
				this.mLabelList.Add(uILabel);
				i++;
			}
			num6 = Mathf.Max(num6, vector2.x - vector.x - (border.x + this.padding.x) * 2f);
			float num9 = num6;
			Vector3 vector3 = new Vector3(num9 * 0.5f, -num4 * 0.5f, 0f);
			Vector3 vector4 = new Vector3(num9, num4 + this.padding.y, 1f);
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				UILabel uILabel2 = list[j];
				NGUITools.AddWidgetCollider(uILabel2.gameObject);
				uILabel2.autoResizeBoxCollider = false;
				BoxCollider component = uILabel2.GetComponent<BoxCollider>();
				if (component != null)
				{
					vector3.z = component.center.z;
					component.center = vector3;
					component.size = vector4;
				}
				else
				{
					BoxCollider2D component2 = uILabel2.GetComponent<BoxCollider2D>();
					component2.offset = vector3;
					component2.size = vector4;
				}
				j++;
			}
			int width = Mathf.RoundToInt(num6);
			num6 += (border.x + this.padding.x) * 2f;
			num7 -= border.y;
			this.mBackground.width = Mathf.RoundToInt(num6);
			this.mBackground.height = Mathf.RoundToInt(num8);
			int k = 0;
			int count3 = list.Count;
			while (k < count3)
			{
				UILabel uILabel3 = list[k];
				uILabel3.overflowMethod = UILabel.Overflow.ShrinkContent;
				uILabel3.width = width;
				k++;
			}
			float num10 = (!(this.atlas != null)) ? 2f : (2f * this.atlas.pixelSize);
			float f = num6 - (border.x + this.padding.x) * 2f + num3 * num10;
			float f2 = num4 + num2 * num10;
			this.mHighlight.width = Mathf.RoundToInt(f);
			this.mHighlight.height = Mathf.RoundToInt(f2);
			if (this.isAnimated)
			{
				this.AnimateColor(this.mBackground);
				if (Time.timeScale == 0f || Time.timeScale >= 0.1f)
				{
					float bottom = num7 + num4;
					this.Animate(this.mHighlight, flag, bottom);
					int l = 0;
					int count4 = list.Count;
					while (l < count4)
					{
						this.Animate(list[l], flag, bottom);
						l++;
					}
					this.AnimateScale(this.mBackground, flag, bottom);
				}
			}
			if (flag)
			{
				float num11 = border.y * fitScale;
				vector.y = vector2.y - border.y * fitScale;
				vector2.y = vector.y + ((float)this.mBackground.height - border.y * 2f) * fitScale;
				vector2.x = vector.x + (float)this.mBackground.width * fitScale;
				transform.localPosition = new Vector3(vector.x, vector2.y - num11, vector.z);
			}
			else
			{
				vector2.y = vector.y + border.y * fitScale;
				vector.y = vector2.y - (float)this.mBackground.height * fitScale;
				vector2.x = vector.x + (float)this.mBackground.width * fitScale;
			}
			UIPanel uIPanel2 = this.mPanel;
			while (true)
			{
				UIRect parent2 = uIPanel2.parent;
				if (parent2 == null)
				{
					break;
				}
				UIPanel componentInParent = parent2.GetComponentInParent<UIPanel>();
				if (componentInParent == null)
				{
					break;
				}
				uIPanel2 = componentInParent;
			}
			if (cachedTransform != null)
			{
				vector = cachedTransform.TransformPoint(vector);
				vector2 = cachedTransform.TransformPoint(vector2);
				vector = uIPanel2.cachedTransform.InverseTransformPoint(vector);
				vector2 = uIPanel2.cachedTransform.InverseTransformPoint(vector2);
				float pixelSizeAdjustment = UIRoot.GetPixelSizeAdjustment(base.gameObject);
				vector /= pixelSizeAdjustment;
				vector2 /= pixelSizeAdjustment;
			}
			Vector3 b = uIPanel2.CalculateConstrainOffset(vector, vector2);
			Vector3 localPosition = transform.localPosition + b;
			localPosition.x = Mathf.Round(localPosition.x);
			localPosition.y = Mathf.Round(localPosition.y);
			transform.localPosition = localPosition;
			transform.parent = parent;
		}
		else
		{
			this.OnSelect(false);
		}
	}
}
