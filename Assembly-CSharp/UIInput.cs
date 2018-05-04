using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Input Field")]
public class UIInput : MonoBehaviour
{
	public enum InputType
	{
		Standard = 0,
		AutoCorrect = 1,
		Password = 2
	}

	public enum Validation
	{
		None = 0,
		Integer = 1,
		Float = 2,
		Alphanumeric = 3,
		Username = 4,
		Name = 5,
		Filename = 6
	}

	public enum KeyboardType
	{
		Default = 0,
		ASCIICapable = 1,
		NumbersAndPunctuation = 2,
		URL = 3,
		NumberPad = 4,
		PhonePad = 5,
		NamePhonePad = 6,
		EmailAddress = 7
	}

	public enum OnReturnKey
	{
		Default = 0,
		Submit = 1,
		NewLine = 2
	}

	public delegate char OnValidate(string text, int charIndex, char addedChar);

	public static UIInput current;

	public static UIInput selection;

	public UILabel label;

	public UIInput.InputType inputType;

	public UIInput.OnReturnKey onReturnKey;

	public UIInput.KeyboardType keyboardType;

	public bool hideInput;

	[NonSerialized]
	public bool selectAllTextOnFocus = true;

	public bool submitOnUnselect;

	public UIInput.Validation validation;

	public int characterLimit;

	public string savedAs;

	[HideInInspector, SerializeField]
	private GameObject selectOnTab;

	public Color activeTextColor = Color.white;

	public Color caretColor = new Color(1f, 1f, 1f, 0.8f);

	public Color selectionColor = new Color(1f, 0.8745098f, 0.5529412f, 0.5f);

	public List<EventDelegate> onSubmit = new List<EventDelegate>();

	public List<EventDelegate> onChange = new List<EventDelegate>();

	public UIInput.OnValidate onValidate;

	[HideInInspector, SerializeField]
	protected string mValue;

	[NonSerialized]
	protected string mDefaultText = string.Empty;

	[NonSerialized]
	protected Color mDefaultColor = Color.white;

	[NonSerialized]
	protected float mPosition;

	[NonSerialized]
	protected bool mDoInit = true;

	[NonSerialized]
	protected NGUIText.Alignment mAlignment = NGUIText.Alignment.Left;

	[NonSerialized]
	protected bool mLoadSavedValue = true;

	protected static int mDrawStart;

	protected static string mLastIME = string.Empty;

	protected static TouchScreenKeyboard mKeyboard;

	private static bool mWaitForKeyboard;

	[NonSerialized]
	protected int mSelectionStart;

	[NonSerialized]
	protected int mSelectionEnd;

	[NonSerialized]
	protected UITexture mHighlight;

	[NonSerialized]
	protected UITexture mCaret;

	[NonSerialized]
	protected Texture2D mBlankTex;

	[NonSerialized]
	protected float mNextBlink;

	[NonSerialized]
	protected float mLastAlpha;

	[NonSerialized]
	protected string mCached = string.Empty;

	[NonSerialized]
	protected int mSelectMe = -1;

	[NonSerialized]
	protected int mSelectTime = -1;

	[NonSerialized]
	protected bool mStarted;

	[NonSerialized]
	private UICamera mCam;

	[NonSerialized]
	private bool mEllipsis;

	private static int mIgnoreKey;

	[NonSerialized]
	public Action onUpArrow;

	[NonSerialized]
	public Action onDownArrow;

	public string defaultText
	{
		get
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			return this.mDefaultText;
		}
		set
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			this.mDefaultText = value;
			this.UpdateLabel();
		}
	}

	public Color defaultColor
	{
		get
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			return this.mDefaultColor;
		}
		set
		{
			this.mDefaultColor = value;
			if (!this.isSelected)
			{
				this.label.color = value;
			}
		}
	}

	public bool inputShouldBeHidden
	{
		get
		{
			return this.hideInput && this.label != null && !this.label.multiLine && this.inputType != UIInput.InputType.Password;
		}
	}

	[Obsolete("Use UIInput.value instead")]
	public string text
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	public string value
	{
		get
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			return this.mValue;
		}
		set
		{
			this.Set(value, true);
		}
	}

	[Obsolete("Use UIInput.isSelected instead")]
	public bool selected
	{
		get
		{
			return this.isSelected;
		}
		set
		{
			this.isSelected = value;
		}
	}

	public bool isSelected
	{
		get
		{
			return UIInput.selection == this;
		}
		set
		{
			if (!value)
			{
				if (this.isSelected)
				{
					UICamera.selectedObject = null;
				}
			}
			else
			{
				UICamera.selectedObject = base.gameObject;
			}
		}
	}

	public int cursorPosition
	{
		get
		{
			if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
			{
				return this.value.Length;
			}
			return (!this.isSelected) ? this.value.Length : this.mSelectionEnd;
		}
		set
		{
			if (this.isSelected)
			{
				if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
				{
					return;
				}
				this.mSelectionEnd = value;
				this.UpdateLabel();
			}
		}
	}

	public int selectionStart
	{
		get
		{
			if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
			{
				return 0;
			}
			return (!this.isSelected) ? this.value.Length : this.mSelectionStart;
		}
		set
		{
			if (this.isSelected)
			{
				if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
				{
					return;
				}
				this.mSelectionStart = value;
				this.UpdateLabel();
			}
		}
	}

	public int selectionEnd
	{
		get
		{
			if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
			{
				return this.value.Length;
			}
			return (!this.isSelected) ? this.value.Length : this.mSelectionEnd;
		}
		set
		{
			if (this.isSelected)
			{
				if (UIInput.mKeyboard != null && !this.inputShouldBeHidden)
				{
					return;
				}
				this.mSelectionEnd = value;
				this.UpdateLabel();
			}
		}
	}

	public UITexture caret
	{
		get
		{
			return this.mCaret;
		}
	}

	public void Set(string value, bool notify = true)
	{
		if (this.mDoInit)
		{
			this.Init();
		}
		if (value == this.value)
		{
			return;
		}
		UIInput.mDrawStart = 0;
		value = this.Validate(value);
		if (this.isSelected && UIInput.mKeyboard != null && this.mCached != value)
		{
			UIInput.mKeyboard.text = value;
			this.mCached = value;
		}
		if (this.mValue != value)
		{
			this.mValue = value;
			this.mLoadSavedValue = false;
			if (this.isSelected)
			{
				if (string.IsNullOrEmpty(value))
				{
					this.mSelectionStart = 0;
					this.mSelectionEnd = 0;
				}
				else
				{
					this.mSelectionStart = value.Length;
					this.mSelectionEnd = this.mSelectionStart;
				}
			}
			else if (this.mStarted)
			{
				this.SaveToPlayerPrefs(value);
			}
			this.UpdateLabel();
			if (notify)
			{
				this.ExecuteOnChange();
			}
		}
	}

	public string Validate(string val)
	{
		if (string.IsNullOrEmpty(val))
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(val.Length);
		for (int i = 0; i < val.Length; i++)
		{
			char c = val[i];
			if (this.onValidate != null)
			{
				c = this.onValidate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			else if (this.validation != UIInput.Validation.None)
			{
				c = this.Validate(stringBuilder.ToString(), stringBuilder.Length, c);
			}
			if (c != '\0')
			{
				stringBuilder.Append(c);
			}
		}
		if (this.characterLimit > 0 && stringBuilder.Length > this.characterLimit)
		{
			return stringBuilder.ToString(0, this.characterLimit);
		}
		return stringBuilder.ToString();
	}

	public void Start()
	{
		if (this.mStarted)
		{
			return;
		}
		if (this.selectOnTab != null)
		{
			UIKeyNavigation uIKeyNavigation = base.GetComponent<UIKeyNavigation>();
			if (uIKeyNavigation == null)
			{
				uIKeyNavigation = base.gameObject.AddComponent<UIKeyNavigation>();
				uIKeyNavigation.onDown = this.selectOnTab;
			}
			this.selectOnTab = null;
			NGUITools.SetDirty(this, "last change");
		}
		if (this.mLoadSavedValue && !string.IsNullOrEmpty(this.savedAs))
		{
			this.LoadValue();
		}
		else
		{
			this.value = this.mValue.Replace("\\n", "\n");
		}
		this.mStarted = true;
	}

	protected void Init()
	{
		if (this.mDoInit && this.label != null)
		{
			this.mDoInit = false;
			this.mDefaultText = this.label.text;
			this.mDefaultColor = this.label.color;
			this.mEllipsis = this.label.overflowEllipsis;
			if (this.label.alignment == NGUIText.Alignment.Justified)
			{
				this.label.alignment = NGUIText.Alignment.Left;
				Debug.LogWarning("Input fields using labels with justified alignment are not supported at this time", this);
			}
			this.mAlignment = this.label.alignment;
			this.mPosition = this.label.cachedTransform.localPosition.x;
			this.UpdateLabel();
		}
	}

	protected void SaveToPlayerPrefs(string val)
	{
		if (!string.IsNullOrEmpty(this.savedAs))
		{
			if (string.IsNullOrEmpty(val))
			{
				PlayerPrefs.DeleteKey(this.savedAs);
			}
			else
			{
				PlayerPrefs.SetString(this.savedAs, val);
			}
		}
	}

	protected virtual void OnSelect(bool isSelected)
	{
		if (isSelected)
		{
			if (this.label != null)
			{
				this.label.supportEncoding = false;
			}
			this.OnSelectEvent();
		}
		else
		{
			this.OnDeselectEvent();
		}
	}

	protected void OnSelectEvent()
	{
		this.mSelectTime = Time.frameCount;
		UIInput.selection = this;
		if (this.mDoInit)
		{
			this.Init();
		}
		if (this.label != null)
		{
			this.mEllipsis = this.label.overflowEllipsis;
			this.label.overflowEllipsis = false;
		}
		if (this.label != null && NGUITools.GetActive(this))
		{
			this.mSelectMe = Time.frameCount;
		}
	}

	protected void OnDeselectEvent()
	{
		if (this.mDoInit)
		{
			this.Init();
		}
		if (this.label != null)
		{
			this.label.overflowEllipsis = this.mEllipsis;
		}
		if (this.label != null && NGUITools.GetActive(this))
		{
			this.mValue = this.value;
			if (UIInput.mKeyboard != null)
			{
				UIInput.mWaitForKeyboard = false;
				UIInput.mKeyboard.active = false;
				UIInput.mKeyboard = null;
			}
			if (string.IsNullOrEmpty(this.mValue))
			{
				this.label.text = this.mDefaultText;
				this.label.color = this.mDefaultColor;
			}
			else
			{
				this.label.text = this.mValue;
			}
			Input.imeCompositionMode = IMECompositionMode.Auto;
			this.label.alignment = this.mAlignment;
		}
		UIInput.selection = null;
		this.UpdateLabel();
		if (this.submitOnUnselect)
		{
			this.Submit();
		}
	}

	protected virtual void Update()
	{
		if (!this.isSelected || this.mSelectTime == Time.frameCount)
		{
			return;
		}
		if (this.mDoInit)
		{
			this.Init();
		}
		if (UIInput.mWaitForKeyboard)
		{
			if (UIInput.mKeyboard != null && !UIInput.mKeyboard.active)
			{
				return;
			}
			UIInput.mWaitForKeyboard = false;
		}
		if (this.mSelectMe != -1 && this.mSelectMe != Time.frameCount)
		{
			this.mSelectMe = -1;
			this.mSelectionEnd = ((!string.IsNullOrEmpty(this.mValue)) ? this.mValue.Length : 0);
			UIInput.mDrawStart = 0;
			this.mSelectionStart = ((!this.selectAllTextOnFocus) ? this.mSelectionEnd : 0);
			this.label.color = this.activeTextColor;
			RuntimePlatform platform = Application.platform;
			if (platform == RuntimePlatform.IPhonePlayer || platform == RuntimePlatform.Android || platform == RuntimePlatform.MetroPlayerARM || platform == RuntimePlatform.MetroPlayerX64 || platform == RuntimePlatform.MetroPlayerX86)
			{
				TouchScreenKeyboardType touchScreenKeyboardType;
				string text;
				if (this.inputShouldBeHidden)
				{
					TouchScreenKeyboard.hideInput = true;
					touchScreenKeyboardType = (TouchScreenKeyboardType)this.keyboardType;
					text = "|";
				}
				else if (this.inputType == UIInput.InputType.Password)
				{
					TouchScreenKeyboard.hideInput = false;
					touchScreenKeyboardType = (TouchScreenKeyboardType)this.keyboardType;
					text = this.mValue;
					this.mSelectionStart = this.mSelectionEnd;
				}
				else
				{
					TouchScreenKeyboard.hideInput = false;
					touchScreenKeyboardType = (TouchScreenKeyboardType)this.keyboardType;
					text = this.mValue;
					this.mSelectionStart = this.mSelectionEnd;
				}
				UIInput.mWaitForKeyboard = true;
				UIInput.mKeyboard = ((this.inputType != UIInput.InputType.Password) ? TouchScreenKeyboard.Open(text, touchScreenKeyboardType, !this.inputShouldBeHidden && this.inputType == UIInput.InputType.AutoCorrect, this.label.multiLine && !this.hideInput, false, false, this.defaultText) : TouchScreenKeyboard.Open(text, touchScreenKeyboardType, false, false, true));
			}
			else
			{
				Vector2 compositionCursorPos = (!(UICamera.current != null) || !(UICamera.current.cachedCamera != null)) ? this.label.worldCorners[0] : UICamera.current.cachedCamera.WorldToScreenPoint(this.label.worldCorners[0]);
				compositionCursorPos.y = (float)Screen.height - compositionCursorPos.y;
				Input.imeCompositionMode = IMECompositionMode.On;
				Input.compositionCursorPos = compositionCursorPos;
			}
			this.UpdateLabel();
			if (string.IsNullOrEmpty(Input.inputString))
			{
				return;
			}
		}
		if (UIInput.mKeyboard != null)
		{
			string text2 = (!UIInput.mKeyboard.done && UIInput.mKeyboard.active) ? UIInput.mKeyboard.text : this.mCached;
			if (this.inputShouldBeHidden)
			{
				if (text2 != "|")
				{
					if (!string.IsNullOrEmpty(text2))
					{
						this.Insert(text2.Substring(1));
					}
					else if (!UIInput.mKeyboard.done && UIInput.mKeyboard.active)
					{
						this.DoBackspace();
					}
					UIInput.mKeyboard.text = "|";
				}
			}
			else if (this.mCached != text2)
			{
				this.mCached = text2;
				if (!UIInput.mKeyboard.done && UIInput.mKeyboard.active)
				{
					this.value = text2;
				}
			}
			if (UIInput.mKeyboard.done || !UIInput.mKeyboard.active)
			{
				if (!UIInput.mKeyboard.wasCanceled)
				{
					this.Submit();
				}
				UIInput.mKeyboard = null;
				this.isSelected = false;
				this.mCached = string.Empty;
			}
		}
		else
		{
			string compositionString = Input.compositionString;
			if (string.IsNullOrEmpty(compositionString) && !string.IsNullOrEmpty(Input.inputString))
			{
				string inputString = Input.inputString;
				for (int i = 0; i < inputString.Length; i++)
				{
					char c = inputString[i];
					if (c >= ' ')
					{
						if (c != '')
						{
							if (c != '')
							{
								if (c != '')
								{
									if (c != '')
									{
										if (c != '')
										{
											this.Insert(c.ToString());
										}
									}
								}
							}
						}
					}
				}
			}
			if (UIInput.mLastIME != compositionString)
			{
				this.mSelectionEnd = ((!string.IsNullOrEmpty(compositionString)) ? (this.mValue.Length + compositionString.Length) : this.mSelectionStart);
				UIInput.mLastIME = compositionString;
				this.UpdateLabel();
				this.ExecuteOnChange();
			}
		}
		if (this.mCaret != null && this.mNextBlink < RealTime.time)
		{
			this.mNextBlink = RealTime.time + 0.5f;
			this.mCaret.enabled = !this.mCaret.enabled;
		}
		if (this.isSelected && this.mLastAlpha != this.label.finalAlpha)
		{
			this.UpdateLabel();
		}
		if (this.mCam == null)
		{
			this.mCam = UICamera.FindCameraForLayer(base.gameObject.layer);
		}
		if (this.mCam != null)
		{
			bool flag = false;
			if (this.label.multiLine)
			{
				bool flag2 = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				if (this.onReturnKey == UIInput.OnReturnKey.Submit)
				{
					flag = flag2;
				}
				else
				{
					flag = !flag2;
				}
			}
			if (UICamera.GetKeyDown(this.mCam.submitKey0) || (this.mCam.submitKey0 == KeyCode.Return && UICamera.GetKeyDown(KeyCode.KeypadEnter)))
			{
				if (flag)
				{
					this.Insert("\n");
				}
				else
				{
					if (UICamera.controller.current != null)
					{
						UICamera.controller.clickNotification = UICamera.ClickNotification.None;
					}
					UICamera.currentKey = this.mCam.submitKey0;
					this.Submit();
				}
			}
			if (UICamera.GetKeyDown(this.mCam.submitKey1) || (this.mCam.submitKey1 == KeyCode.Return && UICamera.GetKeyDown(KeyCode.KeypadEnter)))
			{
				if (flag)
				{
					this.Insert("\n");
				}
				else
				{
					if (UICamera.controller.current != null)
					{
						UICamera.controller.clickNotification = UICamera.ClickNotification.None;
					}
					UICamera.currentKey = this.mCam.submitKey1;
					this.Submit();
				}
			}
			if (!this.mCam.useKeyboard && UICamera.GetKeyUp(KeyCode.Tab))
			{
				this.OnKey(KeyCode.Tab);
			}
		}
	}

	private void OnKey(KeyCode key)
	{
		int frameCount = Time.frameCount;
		if (UIInput.mIgnoreKey == frameCount)
		{
			return;
		}
		if (this.mCam != null && (key == this.mCam.cancelKey0 || key == this.mCam.cancelKey1))
		{
			UIInput.mIgnoreKey = frameCount;
			this.isSelected = false;
		}
		else if (key == KeyCode.Tab)
		{
			UIInput.mIgnoreKey = frameCount;
			this.isSelected = false;
			UIKeyNavigation component = base.GetComponent<UIKeyNavigation>();
			if (component != null)
			{
				component.OnKey(KeyCode.Tab);
			}
		}
	}

	protected void DoBackspace()
	{
		if (!string.IsNullOrEmpty(this.mValue))
		{
			if (this.mSelectionStart == this.mSelectionEnd)
			{
				if (this.mSelectionStart < 1)
				{
					return;
				}
				this.mSelectionEnd--;
			}
			this.Insert(string.Empty);
		}
	}

	protected virtual void Insert(string text)
	{
		string leftText = this.GetLeftText();
		string rightText = this.GetRightText();
		int length = rightText.Length;
		StringBuilder stringBuilder = new StringBuilder(leftText.Length + rightText.Length + text.Length);
		stringBuilder.Append(leftText);
		int i = 0;
		int length2 = text.Length;
		while (i < length2)
		{
			char c = text[i];
			if (c == '\b')
			{
				this.DoBackspace();
			}
			else
			{
				if (this.characterLimit > 0 && stringBuilder.Length + length >= this.characterLimit)
				{
					break;
				}
				if (this.onValidate != null)
				{
					c = this.onValidate(stringBuilder.ToString(), stringBuilder.Length, c);
				}
				else if (this.validation != UIInput.Validation.None)
				{
					c = this.Validate(stringBuilder.ToString(), stringBuilder.Length, c);
				}
				if (c != '\0')
				{
					stringBuilder.Append(c);
				}
			}
			i++;
		}
		this.mSelectionStart = stringBuilder.Length;
		this.mSelectionEnd = this.mSelectionStart;
		int j = 0;
		int length3 = rightText.Length;
		while (j < length3)
		{
			char c2 = rightText[j];
			if (this.onValidate != null)
			{
				c2 = this.onValidate(stringBuilder.ToString(), stringBuilder.Length, c2);
			}
			else if (this.validation != UIInput.Validation.None)
			{
				c2 = this.Validate(stringBuilder.ToString(), stringBuilder.Length, c2);
			}
			if (c2 != '\0')
			{
				stringBuilder.Append(c2);
			}
			j++;
		}
		this.mValue = stringBuilder.ToString();
		this.UpdateLabel();
		this.ExecuteOnChange();
	}

	protected string GetLeftText()
	{
		int num = Mathf.Min(new int[]
		{
			this.mSelectionStart,
			this.mSelectionEnd,
			this.mValue.Length
		});
		return (!string.IsNullOrEmpty(this.mValue) && num >= 0) ? this.mValue.Substring(0, num) : string.Empty;
	}

	protected string GetRightText()
	{
		int num = Mathf.Max(this.mSelectionStart, this.mSelectionEnd);
		return (!string.IsNullOrEmpty(this.mValue) && num < this.mValue.Length) ? this.mValue.Substring(num) : string.Empty;
	}

	protected string GetSelection()
	{
		if (string.IsNullOrEmpty(this.mValue) || this.mSelectionStart == this.mSelectionEnd)
		{
			return string.Empty;
		}
		int num = Mathf.Min(this.mSelectionStart, this.mSelectionEnd);
		int num2 = Mathf.Max(this.mSelectionStart, this.mSelectionEnd);
		return this.mValue.Substring(num, num2 - num);
	}

	protected int GetCharUnderMouse()
	{
		Vector3[] worldCorners = this.label.worldCorners;
		Ray currentRay = UICamera.currentRay;
		Plane plane = new Plane(worldCorners[0], worldCorners[1], worldCorners[2]);
		float distance;
		return (!plane.Raycast(currentRay, out distance)) ? 0 : (UIInput.mDrawStart + this.label.GetCharacterIndexAtPosition(currentRay.GetPoint(distance), false));
	}

	protected virtual void OnPress(bool isPressed)
	{
		if (isPressed && this.isSelected && this.label != null && (UICamera.currentScheme == UICamera.ControlScheme.Mouse || UICamera.currentScheme == UICamera.ControlScheme.Touch))
		{
			this.selectionEnd = this.GetCharUnderMouse();
			if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
			{
				this.selectionStart = this.mSelectionEnd;
			}
		}
	}

	protected virtual void OnDrag(Vector2 delta)
	{
		if (this.label != null && (UICamera.currentScheme == UICamera.ControlScheme.Mouse || UICamera.currentScheme == UICamera.ControlScheme.Touch))
		{
			this.selectionEnd = this.GetCharUnderMouse();
		}
	}

	private void OnDisable()
	{
		this.Cleanup();
	}

	protected virtual void Cleanup()
	{
		if (this.mHighlight)
		{
			this.mHighlight.enabled = false;
		}
		if (this.mCaret)
		{
			this.mCaret.enabled = false;
		}
		if (this.mBlankTex)
		{
			NGUITools.Destroy(this.mBlankTex);
			this.mBlankTex = null;
		}
	}

	public void Submit()
	{
		if (NGUITools.GetActive(this))
		{
			this.mValue = this.value;
			if (UIInput.current == null)
			{
				UIInput.current = this;
				EventDelegate.Execute(this.onSubmit);
				UIInput.current = null;
			}
			this.SaveToPlayerPrefs(this.mValue);
		}
	}

	public void UpdateLabel()
	{
		if (this.label != null)
		{
			if (this.mDoInit)
			{
				this.Init();
			}
			bool isSelected = this.isSelected;
			string value = this.value;
			bool flag = string.IsNullOrEmpty(value) && string.IsNullOrEmpty(Input.compositionString);
			this.label.color = ((!flag || isSelected) ? this.activeTextColor : this.mDefaultColor);
			string text;
			if (flag)
			{
				text = ((!isSelected) ? this.mDefaultText : string.Empty);
				this.label.alignment = this.mAlignment;
			}
			else
			{
				if (this.inputType == UIInput.InputType.Password)
				{
					text = string.Empty;
					string str = "*";
					if (this.label.bitmapFont != null && this.label.bitmapFont.bmFont != null && this.label.bitmapFont.bmFont.GetGlyph(42) == null)
					{
						str = "x";
					}
					int i = 0;
					int length = value.Length;
					while (i < length)
					{
						text += str;
						i++;
					}
				}
				else
				{
					text = value;
				}
				int num = (!isSelected) ? 0 : Mathf.Min(text.Length, this.cursorPosition);
				string str2 = text.Substring(0, num);
				if (isSelected)
				{
					str2 += Input.compositionString;
				}
				text = str2 + text.Substring(num, text.Length - num);
				if (isSelected && this.label.overflowMethod == UILabel.Overflow.ClampContent && this.label.maxLineCount == 1)
				{
					int num2 = this.label.CalculateOffsetToFit(text);
					if (num2 == 0)
					{
						UIInput.mDrawStart = 0;
						this.label.alignment = this.mAlignment;
					}
					else if (num < UIInput.mDrawStart)
					{
						UIInput.mDrawStart = num;
						this.label.alignment = NGUIText.Alignment.Left;
					}
					else if (num2 < UIInput.mDrawStart)
					{
						UIInput.mDrawStart = num2;
						this.label.alignment = NGUIText.Alignment.Left;
					}
					else
					{
						num2 = this.label.CalculateOffsetToFit(text.Substring(0, num));
						if (num2 > UIInput.mDrawStart)
						{
							UIInput.mDrawStart = num2;
							this.label.alignment = NGUIText.Alignment.Right;
						}
					}
					if (UIInput.mDrawStart != 0)
					{
						text = text.Substring(UIInput.mDrawStart, text.Length - UIInput.mDrawStart);
					}
				}
				else
				{
					UIInput.mDrawStart = 0;
					this.label.alignment = this.mAlignment;
				}
			}
			this.label.text = text;
			if (isSelected && (UIInput.mKeyboard == null || this.inputShouldBeHidden))
			{
				int num3 = this.mSelectionStart - UIInput.mDrawStart;
				int num4 = this.mSelectionEnd - UIInput.mDrawStart;
				if (this.mBlankTex == null)
				{
					this.mBlankTex = new Texture2D(2, 2, TextureFormat.ARGB32, false);
					for (int j = 0; j < 2; j++)
					{
						for (int k = 0; k < 2; k++)
						{
							this.mBlankTex.SetPixel(k, j, Color.white);
						}
					}
					this.mBlankTex.Apply();
				}
				if (num3 != num4)
				{
					if (this.mHighlight == null)
					{
						this.mHighlight = this.label.cachedGameObject.AddWidget(2147483647);
						this.mHighlight.name = "Input Highlight";
						this.mHighlight.mainTexture = this.mBlankTex;
						this.mHighlight.fillGeometry = false;
						this.mHighlight.pivot = this.label.pivot;
						this.mHighlight.SetAnchor(this.label.cachedTransform);
					}
					else
					{
						this.mHighlight.pivot = this.label.pivot;
						this.mHighlight.mainTexture = this.mBlankTex;
						this.mHighlight.MarkAsChanged();
						this.mHighlight.enabled = true;
					}
				}
				if (this.mCaret == null)
				{
					this.mCaret = this.label.cachedGameObject.AddWidget(2147483647);
					this.mCaret.name = "Input Caret";
					this.mCaret.mainTexture = this.mBlankTex;
					this.mCaret.fillGeometry = false;
					this.mCaret.pivot = this.label.pivot;
					this.mCaret.SetAnchor(this.label.cachedTransform);
				}
				else
				{
					this.mCaret.pivot = this.label.pivot;
					this.mCaret.mainTexture = this.mBlankTex;
					this.mCaret.MarkAsChanged();
					this.mCaret.enabled = true;
				}
				if (num3 != num4)
				{
					this.label.PrintOverlay(num3, num4, this.mCaret.geometry, this.mHighlight.geometry, this.caretColor, this.selectionColor);
					this.mHighlight.enabled = this.mHighlight.geometry.hasVertices;
				}
				else
				{
					this.label.PrintOverlay(num3, num4, this.mCaret.geometry, null, this.caretColor, this.selectionColor);
					if (this.mHighlight != null)
					{
						this.mHighlight.enabled = false;
					}
				}
				this.mNextBlink = RealTime.time + 0.5f;
				this.mLastAlpha = this.label.finalAlpha;
			}
			else
			{
				this.Cleanup();
			}
		}
	}

	protected char Validate(string text, int pos, char ch)
	{
		if (this.validation == UIInput.Validation.None || !base.enabled)
		{
			return ch;
		}
		if (this.validation == UIInput.Validation.Integer)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && pos == 0 && !text.Contains("-"))
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Float)
		{
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
			if (ch == '-' && pos == 0 && !text.Contains("-"))
			{
				return ch;
			}
			if (ch == '.' && !text.Contains("."))
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Alphanumeric)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return ch;
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Username)
		{
			if (ch >= 'A' && ch <= 'Z')
			{
				return ch - 'A' + 'a';
			}
			if (ch >= 'a' && ch <= 'z')
			{
				return ch;
			}
			if (ch >= '0' && ch <= '9')
			{
				return ch;
			}
		}
		else if (this.validation == UIInput.Validation.Filename)
		{
			if (ch == ':')
			{
				return '\0';
			}
			if (ch == '/')
			{
				return '\0';
			}
			if (ch == '\\')
			{
				return '\0';
			}
			if (ch == '<')
			{
				return '\0';
			}
			if (ch == '>')
			{
				return '\0';
			}
			if (ch == '|')
			{
				return '\0';
			}
			if (ch == '^')
			{
				return '\0';
			}
			if (ch == '*')
			{
				return '\0';
			}
			if (ch == ';')
			{
				return '\0';
			}
			if (ch == '"')
			{
				return '\0';
			}
			if (ch == '`')
			{
				return '\0';
			}
			if (ch == '\t')
			{
				return '\0';
			}
			if (ch == '\n')
			{
				return '\0';
			}
			return ch;
		}
		else if (this.validation == UIInput.Validation.Name)
		{
			char c = (text.Length <= 0) ? ' ' : text[Mathf.Clamp(pos, 0, text.Length - 1)];
			char c2 = (text.Length <= 0) ? '\n' : text[Mathf.Clamp(pos + 1, 0, text.Length - 1)];
			if (ch >= 'a' && ch <= 'z')
			{
				if (c == ' ')
				{
					return ch - 'a' + 'A';
				}
				return ch;
			}
			else if (ch >= 'A' && ch <= 'Z')
			{
				if (c != ' ' && c != '\'')
				{
					return ch - 'A' + 'a';
				}
				return ch;
			}
			else if (ch == '\'')
			{
				if (c != ' ' && c != '\'' && c2 != '\'' && !text.Contains("'"))
				{
					return ch;
				}
			}
			else if (ch == ' ' && c != ' ' && c != '\'' && c2 != ' ' && c2 != '\'')
			{
				return ch;
			}
		}
		return '\0';
	}

	protected void ExecuteOnChange()
	{
		if (UIInput.current == null && EventDelegate.IsValid(this.onChange))
		{
			UIInput.current = this;
			EventDelegate.Execute(this.onChange);
			UIInput.current = null;
		}
	}

	public void RemoveFocus()
	{
		this.isSelected = false;
	}

	public void SaveValue()
	{
		this.SaveToPlayerPrefs(this.mValue);
	}

	public void LoadValue()
	{
		if (!string.IsNullOrEmpty(this.savedAs))
		{
			string text = this.mValue.Replace("\\n", "\n");
			this.mValue = string.Empty;
			this.value = ((!PlayerPrefs.HasKey(this.savedAs)) ? text : PlayerPrefs.GetString(this.savedAs));
		}
	}
}
