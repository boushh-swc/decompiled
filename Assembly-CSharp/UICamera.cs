using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("NGUI/UI/NGUI Event System (UICamera)"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour
{
	public enum ControlScheme
	{
		Mouse = 0,
		Touch = 1,
		Controller = 2
	}

	public enum ClickNotification
	{
		None = 0,
		Always = 1,
		BasedOnDelta = 2
	}

	public class MouseOrTouch
	{
		public KeyCode key;

		public Vector2 pos;

		public Vector2 lastPos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject last;

		public GameObject current;

		public GameObject pressed;

		public GameObject dragged;

		public float pressTime;

		public float clickTime;

		public UICamera.ClickNotification clickNotification = UICamera.ClickNotification.Always;

		public bool touchBegan = true;

		public bool pressStarted;

		public bool dragStarted;

		public int ignoreDelta;

		public float deltaTime
		{
			get
			{
				return RealTime.time - this.pressTime;
			}
		}

		public bool isOverUI
		{
			get
			{
				return this.current != null && this.current != UICamera.fallThrough && NGUITools.FindInParents<UIRoot>(this.current) != null;
			}
		}
	}

	public enum EventType
	{
		World_3D = 0,
		UI_3D = 1,
		World_2D = 2,
		UI_2D = 3
	}

	public delegate bool GetKeyStateFunc(KeyCode key);

	public delegate float GetAxisFunc(string name);

	public delegate bool GetAnyKeyFunc();

	public delegate UICamera.MouseOrTouch GetMouseDelegate(int button);

	public delegate UICamera.MouseOrTouch GetTouchDelegate(int id, bool createIfMissing);

	public delegate void RemoveTouchDelegate(int id);

	public delegate void OnScreenResize();

	public enum ProcessEventsIn
	{
		Update = 0,
		LateUpdate = 1
	}

	public delegate void OnCustomInput();

	public delegate void OnSchemeChange();

	public delegate void MoveDelegate(Vector2 delta);

	public delegate void VoidDelegate(GameObject go);

	public delegate void BoolDelegate(GameObject go, bool state);

	public delegate void FloatDelegate(GameObject go, float delta);

	public delegate void VectorDelegate(GameObject go, Vector2 delta);

	public delegate void ObjectDelegate(GameObject go, GameObject obj);

	public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

	private struct DepthEntry
	{
		public int depth;

		public RaycastHit hit;

		public Vector3 point;

		public GameObject go;
	}

	public class Touch
	{
		public int fingerId;

		public TouchPhase phase;

		public Vector2 position;

		public int tapCount;
	}

	public delegate int GetTouchCountCallback();

	public delegate UICamera.Touch GetTouchCallback(int index);

	public static BetterList<UICamera> list = new BetterList<UICamera>();

	public static UICamera.GetKeyStateFunc GetKeyDown = (KeyCode key) => (key < KeyCode.JoystickButton0 || !UICamera.ignoreControllerInput) && Input.GetKeyDown(key);

	public static UICamera.GetKeyStateFunc GetKeyUp = (KeyCode key) => (key < KeyCode.JoystickButton0 || !UICamera.ignoreControllerInput) && Input.GetKeyUp(key);

	public static UICamera.GetKeyStateFunc GetKey = (KeyCode key) => (key < KeyCode.JoystickButton0 || !UICamera.ignoreControllerInput) && Input.GetKey(key);

	public static UICamera.GetAxisFunc GetAxis = delegate(string axis)
	{
		if (UICamera.ignoreControllerInput)
		{
			return 0f;
		}
		return Input.GetAxis(axis);
	};

	public static UICamera.GetAnyKeyFunc GetAnyKeyDown;

	public static UICamera.GetMouseDelegate GetMouse = (int button) => UICamera.mMouse[button];

	public static UICamera.GetTouchDelegate GetTouch = delegate(int id, bool createIfMissing)
	{
		if (id < 0)
		{
			return UICamera.GetMouse(-id - 1);
		}
		int i = 0;
		int count = UICamera.mTouchIDs.Count;
		while (i < count)
		{
			if (UICamera.mTouchIDs[i] == id)
			{
				return UICamera.activeTouches[i];
			}
			i++;
		}
		if (createIfMissing)
		{
			UICamera.MouseOrTouch mouseOrTouch = new UICamera.MouseOrTouch();
			mouseOrTouch.pressTime = RealTime.time;
			mouseOrTouch.touchBegan = true;
			UICamera.activeTouches.Add(mouseOrTouch);
			UICamera.mTouchIDs.Add(id);
			return mouseOrTouch;
		}
		return null;
	};

	public static UICamera.RemoveTouchDelegate RemoveTouch = delegate(int id)
	{
		int i = 0;
		int count = UICamera.mTouchIDs.Count;
		while (i < count)
		{
			if (UICamera.mTouchIDs[i] == id)
			{
				UICamera.mTouchIDs.RemoveAt(i);
				UICamera.activeTouches.RemoveAt(i);
				return;
			}
			i++;
		}
	};

	public static UICamera.OnScreenResize onScreenResize;

	public UICamera.EventType eventType = UICamera.EventType.UI_3D;

	public bool eventsGoToColliders;

	public LayerMask eventReceiverMask = -1;

	public UICamera.ProcessEventsIn processEventsIn;

	public bool debug;

	public bool useMouse = true;

	public bool useTouch = true;

	public bool allowMultiTouch = true;

	public bool useKeyboard = true;

	public bool useController = true;

	public bool stickyTooltip = true;

	public float tooltipDelay = 1f;

	public bool longPressTooltip;

	public float mouseDragThreshold = 4f;

	public float mouseClickThreshold = 10f;

	public float touchDragThreshold = 40f;

	public float touchClickThreshold = 40f;

	public float rangeDistance = -1f;

	public string horizontalAxisName = "Horizontal";

	public string verticalAxisName = "Vertical";

	public string horizontalPanAxisName;

	public string verticalPanAxisName;

	public string scrollAxisName = "Mouse ScrollWheel";

	[Tooltip("If enabled, command-click will result in a right-click event on OSX")]
	public bool commandClick = true;

	public KeyCode submitKey0 = KeyCode.Return;

	public KeyCode submitKey1 = KeyCode.JoystickButton0;

	public KeyCode cancelKey0 = KeyCode.Escape;

	public KeyCode cancelKey1 = KeyCode.JoystickButton1;

	public bool autoHideCursor = true;

	public static UICamera.OnCustomInput onCustomInput;

	public static bool showTooltips = true;

	public static bool ignoreAllEvents = false;

	public static bool ignoreControllerInput = false;

	private static bool mDisableController = false;

	private static Vector2 mLastPos = Vector2.zero;

	public static Vector3 lastWorldPosition = Vector3.zero;

	public static Ray lastWorldRay = default(Ray);

	public static RaycastHit lastHit;

	public static UICamera current = null;

	public static Camera currentCamera = null;

	public static UICamera.OnSchemeChange onSchemeChange;

	private static UICamera.ControlScheme mLastScheme = UICamera.ControlScheme.Mouse;

	public static int currentTouchID = -100;

	private static KeyCode mCurrentKey = KeyCode.Alpha0;

	public static UICamera.MouseOrTouch currentTouch = null;

	private static bool mInputFocus = false;

	private static GameObject mGenericHandler;

	public static GameObject fallThrough;

	public static UICamera.VoidDelegate onClick;

	public static UICamera.VoidDelegate onDoubleClick;

	public static UICamera.BoolDelegate onHover;

	public static UICamera.BoolDelegate onPress;

	public static UICamera.BoolDelegate onSelect;

	public static UICamera.FloatDelegate onScroll;

	public static UICamera.VectorDelegate onDrag;

	public static UICamera.VoidDelegate onDragStart;

	public static UICamera.ObjectDelegate onDragOver;

	public static UICamera.ObjectDelegate onDragOut;

	public static UICamera.VoidDelegate onDragEnd;

	public static UICamera.ObjectDelegate onDrop;

	public static UICamera.KeyCodeDelegate onKey;

	public static UICamera.KeyCodeDelegate onNavigate;

	public static UICamera.VectorDelegate onPan;

	public static UICamera.BoolDelegate onTooltip;

	public static UICamera.MoveDelegate onMouseMove;

	private static UICamera.MouseOrTouch[] mMouse = new UICamera.MouseOrTouch[]
	{
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch(),
		new UICamera.MouseOrTouch()
	};

	public static UICamera.MouseOrTouch controller = new UICamera.MouseOrTouch();

	public static List<UICamera.MouseOrTouch> activeTouches = new List<UICamera.MouseOrTouch>();

	private static List<int> mTouchIDs = new List<int>();

	private static int mWidth = 0;

	private static int mHeight = 0;

	private static GameObject mTooltip = null;

	private Camera mCam;

	private static float mTooltipTime = 0f;

	private float mNextRaycast;

	public static bool isDragging = false;

	private static int mLastInteractionCheck = -1;

	private static bool mLastInteractionResult = false;

	private static int mLastFocusCheck = -1;

	private static bool mLastFocusResult = false;

	private static int mLastOverCheck = -1;

	private static bool mLastOverResult = false;

	private static GameObject mRayHitObject;

	private static GameObject mHover;

	private static GameObject mSelected;

	private static UICamera.DepthEntry mHit = default(UICamera.DepthEntry);

	private static BetterList<UICamera.DepthEntry> mHits = new BetterList<UICamera.DepthEntry>();

	private static RaycastHit[] mRayHits;

	private static Collider2D[] mOverlap;

	private static Plane m2DPlane = new Plane(Vector3.back, 0f);

	private static float mNextEvent = 0f;

	private static int mNotifying = 0;

	private static bool mUsingTouchEvents = true;

	public static UICamera.GetTouchCountCallback GetInputTouchCount;

	public static UICamera.GetTouchCallback GetInputTouch;

	[CompilerGenerated]
	private static BetterList<UICamera>.CompareFunc <>f__mg$cache0;

	[CompilerGenerated]
	private static BetterList<UICamera>.CompareFunc <>f__mg$cache1;

	[Obsolete("Use new OnDragStart / OnDragOver / OnDragOut / OnDragEnd events instead")]
	public bool stickyPress
	{
		get
		{
			return true;
		}
	}

	public static bool disableController
	{
		get
		{
			return UICamera.mDisableController && !UIPopupList.isOpen;
		}
		set
		{
			UICamera.mDisableController = value;
		}
	}

	[Obsolete("Use lastEventPosition instead. It handles controller input properly.")]
	public static Vector2 lastTouchPosition
	{
		get
		{
			return UICamera.mLastPos;
		}
		set
		{
			UICamera.mLastPos = value;
		}
	}

	public static Vector2 lastEventPosition
	{
		get
		{
			UICamera.ControlScheme currentScheme = UICamera.currentScheme;
			if (currentScheme == UICamera.ControlScheme.Controller)
			{
				GameObject hoveredObject = UICamera.hoveredObject;
				if (hoveredObject != null)
				{
					Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(hoveredObject.transform);
					Camera camera = NGUITools.FindCameraForLayer(hoveredObject.layer);
					return camera.WorldToScreenPoint(bounds.center);
				}
			}
			return UICamera.mLastPos;
		}
		set
		{
			UICamera.mLastPos = value;
		}
	}

	public static UICamera first
	{
		get
		{
			if (UICamera.list == null || UICamera.list.size == 0)
			{
				return null;
			}
			return UICamera.list[0];
		}
	}

	public static UICamera.ControlScheme currentScheme
	{
		get
		{
			if (UICamera.mCurrentKey == KeyCode.None)
			{
				return UICamera.ControlScheme.Touch;
			}
			if (UICamera.mCurrentKey >= KeyCode.JoystickButton0)
			{
				return UICamera.ControlScheme.Controller;
			}
			if (!(UICamera.current != null))
			{
				return UICamera.ControlScheme.Mouse;
			}
			if (UICamera.mLastScheme == UICamera.ControlScheme.Controller && (UICamera.mCurrentKey == UICamera.current.submitKey0 || UICamera.mCurrentKey == UICamera.current.submitKey1))
			{
				return UICamera.ControlScheme.Controller;
			}
			if (UICamera.current.useMouse)
			{
				return UICamera.ControlScheme.Mouse;
			}
			if (UICamera.current.useTouch)
			{
				return UICamera.ControlScheme.Touch;
			}
			return UICamera.ControlScheme.Controller;
		}
		set
		{
			if (UICamera.mLastScheme != value)
			{
				if (value == UICamera.ControlScheme.Mouse)
				{
					UICamera.currentKey = KeyCode.Mouse0;
				}
				else if (value == UICamera.ControlScheme.Controller)
				{
					UICamera.currentKey = KeyCode.JoystickButton0;
				}
				else if (value == UICamera.ControlScheme.Touch)
				{
					UICamera.currentKey = KeyCode.None;
				}
				else
				{
					UICamera.currentKey = KeyCode.Alpha0;
				}
				UICamera.mLastScheme = value;
			}
		}
	}

	public static KeyCode currentKey
	{
		get
		{
			return UICamera.mCurrentKey;
		}
		set
		{
			if (UICamera.mCurrentKey != value)
			{
				UICamera.ControlScheme controlScheme = UICamera.mLastScheme;
				UICamera.mCurrentKey = value;
				UICamera.mLastScheme = UICamera.currentScheme;
				if (controlScheme != UICamera.mLastScheme)
				{
					UICamera.HideTooltip();
					if (UICamera.mLastScheme == UICamera.ControlScheme.Mouse)
					{
						Cursor.lockState = CursorLockMode.None;
						Cursor.visible = true;
					}
					else if (UICamera.current != null && UICamera.current.autoHideCursor)
					{
						Cursor.visible = false;
						Cursor.lockState = CursorLockMode.Locked;
						UICamera.mMouse[0].ignoreDelta = 2;
					}
					if (UICamera.onSchemeChange != null)
					{
						UICamera.onSchemeChange();
					}
				}
			}
		}
	}

	public static Ray currentRay
	{
		get
		{
			return (!(UICamera.currentCamera != null) || UICamera.currentTouch == null) ? default(Ray) : UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
		}
	}

	public static bool inputHasFocus
	{
		get
		{
			return UICamera.mInputFocus && UICamera.mSelected && UICamera.mSelected.activeInHierarchy;
		}
	}

	[Obsolete("Use delegates instead such as UICamera.onClick, UICamera.onHover, etc.")]
	public static GameObject genericEventHandler
	{
		get
		{
			return UICamera.mGenericHandler;
		}
		set
		{
			UICamera.mGenericHandler = value;
		}
	}

	public static UICamera.MouseOrTouch mouse0
	{
		get
		{
			return UICamera.mMouse[0];
		}
	}

	public static UICamera.MouseOrTouch mouse1
	{
		get
		{
			return UICamera.mMouse[1];
		}
	}

	public static UICamera.MouseOrTouch mouse2
	{
		get
		{
			return UICamera.mMouse[2];
		}
	}

	private bool handlesEvents
	{
		get
		{
			return UICamera.eventHandler == this;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if (this.mCam == null)
			{
				this.mCam = base.GetComponent<Camera>();
			}
			return this.mCam;
		}
	}

	public static GameObject tooltipObject
	{
		get
		{
			return UICamera.mTooltip;
		}
		set
		{
			UICamera.ShowTooltip(value);
		}
	}

	public static bool isOverUI
	{
		get
		{
			int frameCount = Time.frameCount;
			if (UICamera.mLastOverCheck != frameCount)
			{
				UICamera.mLastOverCheck = frameCount;
				if (UICamera.currentTouch != null)
				{
					if (UICamera.currentTouch.pressed != null)
					{
						UICamera.mLastOverResult = UICamera.IsPartOfUI(UICamera.currentTouch.pressed);
						return UICamera.mLastOverResult;
					}
					UICamera.mLastOverResult = UICamera.IsPartOfUI(UICamera.currentTouch.current);
					return UICamera.mLastOverResult;
				}
				else
				{
					int i = 0;
					int count = UICamera.activeTouches.Count;
					while (i < count)
					{
						UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
						if (UICamera.IsPartOfUI(mouseOrTouch.pressed))
						{
							UICamera.mLastOverResult = true;
							return UICamera.mLastOverResult;
						}
						i++;
					}
					for (int j = 0; j < 3; j++)
					{
						UICamera.MouseOrTouch mouseOrTouch2 = UICamera.mMouse[j];
						if (UICamera.IsPartOfUI((!(mouseOrTouch2.pressed != null)) ? mouseOrTouch2.current : mouseOrTouch2.pressed))
						{
							UICamera.mLastOverResult = true;
							return UICamera.mLastOverResult;
						}
					}
					UICamera.mLastOverResult = UICamera.IsPartOfUI(UICamera.controller.pressed);
				}
			}
			return UICamera.mLastOverResult;
		}
	}

	public static bool uiHasFocus
	{
		get
		{
			int frameCount = Time.frameCount;
			if (UICamera.mLastFocusCheck != frameCount)
			{
				UICamera.mLastFocusCheck = frameCount;
				if (UICamera.inputHasFocus)
				{
					UICamera.mLastFocusResult = true;
					return UICamera.mLastFocusResult;
				}
				if (UICamera.currentTouch != null)
				{
					UICamera.mLastFocusResult = UICamera.currentTouch.isOverUI;
					return UICamera.mLastFocusResult;
				}
				int i = 0;
				int count = UICamera.activeTouches.Count;
				while (i < count)
				{
					UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
					if (UICamera.IsPartOfUI(mouseOrTouch.pressed))
					{
						UICamera.mLastFocusResult = true;
						return UICamera.mLastFocusResult;
					}
					i++;
				}
				for (int j = 0; j < 3; j++)
				{
					UICamera.MouseOrTouch mouseOrTouch2 = UICamera.mMouse[j];
					if (UICamera.IsPartOfUI(mouseOrTouch2.pressed) || UICamera.IsPartOfUI(mouseOrTouch2.current))
					{
						UICamera.mLastFocusResult = true;
						return UICamera.mLastFocusResult;
					}
				}
				UICamera.mLastFocusResult = UICamera.IsPartOfUI(UICamera.controller.pressed);
			}
			return UICamera.mLastFocusResult;
		}
	}

	public static bool interactingWithUI
	{
		get
		{
			int frameCount = Time.frameCount;
			if (UICamera.mLastInteractionCheck != frameCount)
			{
				UICamera.mLastInteractionCheck = frameCount;
				if (UICamera.inputHasFocus)
				{
					UICamera.mLastInteractionResult = true;
					return UICamera.mLastInteractionResult;
				}
				int i = 0;
				int count = UICamera.activeTouches.Count;
				while (i < count)
				{
					UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
					if (UICamera.IsPartOfUI(mouseOrTouch.pressed))
					{
						UICamera.mLastInteractionResult = true;
						return UICamera.mLastInteractionResult;
					}
					i++;
				}
				for (int j = 0; j < 3; j++)
				{
					UICamera.MouseOrTouch mouseOrTouch2 = UICamera.mMouse[j];
					if (UICamera.IsPartOfUI(mouseOrTouch2.pressed))
					{
						UICamera.mLastInteractionResult = true;
						return UICamera.mLastInteractionResult;
					}
				}
				UICamera.mLastInteractionResult = UICamera.IsPartOfUI(UICamera.controller.pressed);
			}
			return UICamera.mLastInteractionResult;
		}
	}

	public static GameObject hoveredObject
	{
		get
		{
			if (UICamera.currentTouch != null && (UICamera.currentScheme != UICamera.ControlScheme.Mouse || UICamera.currentTouch.dragStarted))
			{
				return UICamera.currentTouch.current;
			}
			if (UICamera.mHover && UICamera.mHover.activeInHierarchy)
			{
				return UICamera.mHover;
			}
			UICamera.mHover = null;
			return null;
		}
		set
		{
			if (UICamera.mHover == value)
			{
				return;
			}
			bool flag = false;
			UICamera uICamera = UICamera.current;
			if (UICamera.currentTouch == null)
			{
				flag = true;
				UICamera.currentTouchID = -100;
				UICamera.currentTouch = UICamera.controller;
			}
			UICamera.ShowTooltip(null);
			if (UICamera.mSelected && UICamera.currentScheme == UICamera.ControlScheme.Controller)
			{
				UICamera.Notify(UICamera.mSelected, "OnSelect", false);
				if (UICamera.onSelect != null)
				{
					UICamera.onSelect(UICamera.mSelected, false);
				}
				UICamera.mSelected = null;
			}
			if (UICamera.mHover)
			{
				UICamera.Notify(UICamera.mHover, "OnHover", false);
				if (UICamera.onHover != null)
				{
					UICamera.onHover(UICamera.mHover, false);
				}
			}
			UICamera.mHover = value;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
			if (UICamera.mHover)
			{
				if (UICamera.mHover != UICamera.controller.current && UICamera.mHover.GetComponent<UIKeyNavigation>() != null)
				{
					UICamera.controller.current = UICamera.mHover;
				}
				if (flag)
				{
					UICamera uICamera2 = (!(UICamera.mHover != null)) ? UICamera.list[0] : UICamera.FindCameraForLayer(UICamera.mHover.layer);
					if (uICamera2 != null)
					{
						UICamera.current = uICamera2;
						UICamera.currentCamera = uICamera2.cachedCamera;
					}
				}
				if (UICamera.onHover != null)
				{
					UICamera.onHover(UICamera.mHover, true);
				}
				UICamera.Notify(UICamera.mHover, "OnHover", true);
			}
			if (flag)
			{
				UICamera.current = uICamera;
				UICamera.currentCamera = ((!(uICamera != null)) ? null : uICamera.cachedCamera);
				UICamera.currentTouch = null;
				UICamera.currentTouchID = -100;
			}
		}
	}

	public static GameObject controllerNavigationObject
	{
		get
		{
			if (UICamera.controller.current && UICamera.controller.current.activeInHierarchy)
			{
				return UICamera.controller.current;
			}
			if (UICamera.currentScheme == UICamera.ControlScheme.Controller && UICamera.current != null && UICamera.current.useController && !UICamera.ignoreControllerInput && UIKeyNavigation.list.size > 0)
			{
				for (int i = 0; i < UIKeyNavigation.list.size; i++)
				{
					UIKeyNavigation uIKeyNavigation = UIKeyNavigation.list[i];
					if (uIKeyNavigation && uIKeyNavigation.constraint != UIKeyNavigation.Constraint.Explicit && uIKeyNavigation.startsSelected)
					{
						UICamera.hoveredObject = uIKeyNavigation.gameObject;
						UICamera.controller.current = UICamera.mHover;
						return UICamera.mHover;
					}
				}
				if (UICamera.mHover == null)
				{
					for (int j = 0; j < UIKeyNavigation.list.size; j++)
					{
						UIKeyNavigation uIKeyNavigation2 = UIKeyNavigation.list[j];
						if (uIKeyNavigation2 && uIKeyNavigation2.constraint != UIKeyNavigation.Constraint.Explicit)
						{
							UICamera.hoveredObject = uIKeyNavigation2.gameObject;
							UICamera.controller.current = UICamera.mHover;
							return UICamera.mHover;
						}
					}
				}
			}
			UICamera.controller.current = null;
			return null;
		}
		set
		{
			if (UICamera.controller.current != value && UICamera.controller.current)
			{
				UICamera.Notify(UICamera.controller.current, "OnHover", false);
				if (UICamera.onHover != null)
				{
					UICamera.onHover(UICamera.controller.current, false);
				}
				UICamera.controller.current = null;
			}
			UICamera.hoveredObject = value;
		}
	}

	public static GameObject selectedObject
	{
		get
		{
			if (UICamera.mSelected && UICamera.mSelected.activeInHierarchy)
			{
				return UICamera.mSelected;
			}
			UICamera.mSelected = null;
			return null;
		}
		set
		{
			if (UICamera.mSelected == value)
			{
				UICamera.hoveredObject = value;
				UICamera.controller.current = value;
				return;
			}
			UICamera.ShowTooltip(null);
			bool flag = false;
			UICamera uICamera = UICamera.current;
			if (UICamera.currentTouch == null)
			{
				flag = true;
				UICamera.currentTouchID = -100;
				UICamera.currentTouch = UICamera.controller;
			}
			UICamera.mInputFocus = false;
			if (UICamera.mSelected)
			{
				UICamera.Notify(UICamera.mSelected, "OnSelect", false);
				if (UICamera.onSelect != null)
				{
					UICamera.onSelect(UICamera.mSelected, false);
				}
			}
			UICamera.mSelected = value;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
			if (value != null)
			{
				UIKeyNavigation component = value.GetComponent<UIKeyNavigation>();
				if (component != null)
				{
					UICamera.controller.current = value;
				}
			}
			if (UICamera.mSelected && flag)
			{
				UICamera uICamera2 = (!(UICamera.mSelected != null)) ? UICamera.list[0] : UICamera.FindCameraForLayer(UICamera.mSelected.layer);
				if (uICamera2 != null)
				{
					UICamera.current = uICamera2;
					UICamera.currentCamera = uICamera2.cachedCamera;
				}
			}
			if (UICamera.mSelected)
			{
				UICamera.mInputFocus = (UICamera.mSelected.activeInHierarchy && UICamera.mSelected.GetComponent<UIInput>() != null);
				if (UICamera.onSelect != null)
				{
					UICamera.onSelect(UICamera.mSelected, true);
				}
				UICamera.Notify(UICamera.mSelected, "OnSelect", true);
			}
			if (flag)
			{
				UICamera.current = uICamera;
				UICamera.currentCamera = ((!(uICamera != null)) ? null : uICamera.cachedCamera);
				UICamera.currentTouch = null;
				UICamera.currentTouchID = -100;
			}
		}
	}

	[Obsolete("Use either 'CountInputSources()' or 'activeTouches.Count'")]
	public static int touchCount
	{
		get
		{
			return UICamera.CountInputSources();
		}
	}

	public static int dragCount
	{
		get
		{
			int num = 0;
			int i = 0;
			int count = UICamera.activeTouches.Count;
			while (i < count)
			{
				UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
				if (mouseOrTouch.dragged != null)
				{
					num++;
				}
				i++;
			}
			for (int j = 0; j < UICamera.mMouse.Length; j++)
			{
				if (UICamera.mMouse[j].dragged != null)
				{
					num++;
				}
			}
			if (UICamera.controller.dragged != null)
			{
				num++;
			}
			return num;
		}
	}

	public static Camera mainCamera
	{
		get
		{
			UICamera eventHandler = UICamera.eventHandler;
			return (!(eventHandler != null)) ? null : eventHandler.cachedCamera;
		}
	}

	public static UICamera eventHandler
	{
		get
		{
			for (int i = 0; i < UICamera.list.size; i++)
			{
				UICamera uICamera = UICamera.list.buffer[i];
				if (!(uICamera == null) && uICamera.enabled && NGUITools.GetActive(uICamera.gameObject))
				{
					return uICamera;
				}
			}
			return null;
		}
	}

	public static bool IsPartOfUI(GameObject go)
	{
		return !(go == null) && !(go == UICamera.fallThrough) && NGUITools.FindInParents<UIRoot>(go) != null;
	}

	public static bool IsPressed(GameObject go)
	{
		for (int i = 0; i < 3; i++)
		{
			if (UICamera.mMouse[i].pressed == go)
			{
				return true;
			}
		}
		int j = 0;
		int count = UICamera.activeTouches.Count;
		while (j < count)
		{
			UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[j];
			if (mouseOrTouch.pressed == go)
			{
				return true;
			}
			j++;
		}
		return UICamera.controller.pressed == go;
	}

	public static int CountInputSources()
	{
		int num = 0;
		int i = 0;
		int count = UICamera.activeTouches.Count;
		while (i < count)
		{
			UICamera.MouseOrTouch mouseOrTouch = UICamera.activeTouches[i];
			if (mouseOrTouch.pressed != null)
			{
				num++;
			}
			i++;
		}
		for (int j = 0; j < UICamera.mMouse.Length; j++)
		{
			if (UICamera.mMouse[j].pressed != null)
			{
				num++;
			}
		}
		if (UICamera.controller.pressed != null)
		{
			num++;
		}
		return num;
	}

	private static int CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	private static Rigidbody FindRootRigidbody(Transform trans)
	{
		while (trans != null)
		{
			if (trans.GetComponent<UIPanel>() != null)
			{
				break;
			}
			Rigidbody component = trans.GetComponent<Rigidbody>();
			if (component != null)
			{
				return component;
			}
			trans = trans.parent;
		}
		return null;
	}

	private static Rigidbody2D FindRootRigidbody2D(Transform trans)
	{
		while (trans != null)
		{
			if (trans.GetComponent<UIPanel>() != null)
			{
				break;
			}
			Rigidbody2D component = trans.GetComponent<Rigidbody2D>();
			if (component != null)
			{
				return component;
			}
			trans = trans.parent;
		}
		return null;
	}

	public static void Raycast(UICamera.MouseOrTouch touch)
	{
		if (!UICamera.Raycast(touch.pos))
		{
			UICamera.mRayHitObject = UICamera.fallThrough;
		}
		if (UICamera.mRayHitObject == null)
		{
			UICamera.mRayHitObject = UICamera.mGenericHandler;
		}
		touch.last = touch.current;
		touch.current = UICamera.mRayHitObject;
		UICamera.mLastPos = touch.pos;
	}

	public static bool Raycast(Vector3 inPos)
	{
		for (int i = 0; i < UICamera.list.size; i++)
		{
			UICamera uICamera = UICamera.list.buffer[i];
			if (uICamera.enabled && NGUITools.GetActive(uICamera.gameObject))
			{
				UICamera.currentCamera = uICamera.cachedCamera;
				if (UICamera.currentCamera.targetDisplay == 0)
				{
					Vector3 vector = UICamera.currentCamera.ScreenToViewportPoint(inPos);
					if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y))
					{
						if (vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f)
						{
							Ray ray = UICamera.currentCamera.ScreenPointToRay(inPos);
							int layerMask = UICamera.currentCamera.cullingMask & uICamera.eventReceiverMask;
							float num = (uICamera.rangeDistance <= 0f) ? (UICamera.currentCamera.farClipPlane - UICamera.currentCamera.nearClipPlane) : uICamera.rangeDistance;
							if (uICamera.eventType == UICamera.EventType.World_3D)
							{
								UICamera.lastWorldRay = ray;
								if (Physics.Raycast(ray, out UICamera.lastHit, num, layerMask, QueryTriggerInteraction.Ignore))
								{
									UICamera.lastWorldPosition = UICamera.lastHit.point;
									UICamera.mRayHitObject = UICamera.lastHit.collider.gameObject;
									if (!uICamera.eventsGoToColliders)
									{
										Rigidbody componentInParent = UICamera.mRayHitObject.gameObject.GetComponentInParent<Rigidbody>();
										if (componentInParent != null)
										{
											UICamera.mRayHitObject = componentInParent.gameObject;
										}
									}
									return true;
								}
							}
							else if (uICamera.eventType == UICamera.EventType.UI_3D)
							{
								if (UICamera.mRayHits == null)
								{
									UICamera.mRayHits = new RaycastHit[50];
								}
								int num2 = Physics.RaycastNonAlloc(ray, UICamera.mRayHits, num, layerMask, QueryTriggerInteraction.Collide);
								if (num2 > 1)
								{
									int j = 0;
									while (j < num2)
									{
										GameObject gameObject = UICamera.mRayHits[j].collider.gameObject;
										UIWidget component = gameObject.GetComponent<UIWidget>();
										if (component != null)
										{
											if (component.isVisible)
											{
												if (component.hitCheck == null || component.hitCheck(UICamera.mRayHits[j].point))
												{
													goto IL_291;
												}
											}
										}
										else
										{
											UIRect uIRect = NGUITools.FindInParents<UIRect>(gameObject);
											if (!(uIRect != null) || uIRect.finalAlpha >= 0.001f)
											{
												goto IL_291;
											}
										}
										IL_31B:
										j++;
										continue;
										IL_291:
										UICamera.mHit.depth = NGUITools.CalculateRaycastDepth(gameObject);
										if (UICamera.mHit.depth != 2147483647)
										{
											UICamera.mHit.hit = UICamera.mRayHits[j];
											UICamera.mHit.point = UICamera.mRayHits[j].point;
											UICamera.mHit.go = UICamera.mRayHits[j].collider.gameObject;
											UICamera.mHits.Add(UICamera.mHit);
											goto IL_31B;
										}
										goto IL_31B;
									}
									UICamera.mHits.Sort((UICamera.DepthEntry r1, UICamera.DepthEntry r2) => r2.depth.CompareTo(r1.depth));
									for (int k = 0; k < UICamera.mHits.size; k++)
									{
										if (UICamera.IsVisible(ref UICamera.mHits.buffer[k]))
										{
											UICamera.lastHit = UICamera.mHits[k].hit;
											UICamera.mRayHitObject = UICamera.mHits[k].go;
											UICamera.lastWorldRay = ray;
											UICamera.lastWorldPosition = UICamera.mHits[k].point;
											UICamera.mHits.Clear();
											return true;
										}
									}
									UICamera.mHits.Clear();
								}
								else if (num2 == 1)
								{
									GameObject gameObject2 = UICamera.mRayHits[0].collider.gameObject;
									UIWidget component2 = gameObject2.GetComponent<UIWidget>();
									if (component2 != null)
									{
										if (!component2.isVisible)
										{
											goto IL_851;
										}
										if (component2.hitCheck != null && !component2.hitCheck(UICamera.mRayHits[0].point))
										{
											goto IL_851;
										}
									}
									else
									{
										UIRect uIRect2 = NGUITools.FindInParents<UIRect>(gameObject2);
										if (uIRect2 != null && uIRect2.finalAlpha < 0.001f)
										{
											goto IL_851;
										}
									}
									if (UICamera.IsVisible(UICamera.mRayHits[0].point, UICamera.mRayHits[0].collider.gameObject))
									{
										UICamera.lastHit = UICamera.mRayHits[0];
										UICamera.lastWorldRay = ray;
										UICamera.lastWorldPosition = UICamera.mRayHits[0].point;
										UICamera.mRayHitObject = UICamera.lastHit.collider.gameObject;
										return true;
									}
								}
							}
							else if (uICamera.eventType == UICamera.EventType.World_2D)
							{
								if (UICamera.m2DPlane.Raycast(ray, out num))
								{
									Vector3 point = ray.GetPoint(num);
									Collider2D collider2D = Physics2D.OverlapPoint(point, layerMask);
									if (collider2D)
									{
										UICamera.lastWorldPosition = point;
										UICamera.mRayHitObject = collider2D.gameObject;
										if (!uICamera.eventsGoToColliders)
										{
											Rigidbody2D rigidbody2D = UICamera.FindRootRigidbody2D(UICamera.mRayHitObject.transform);
											if (rigidbody2D != null)
											{
												UICamera.mRayHitObject = rigidbody2D.gameObject;
											}
										}
										return true;
									}
								}
							}
							else if (uICamera.eventType == UICamera.EventType.UI_2D)
							{
								if (UICamera.m2DPlane.Raycast(ray, out num))
								{
									UICamera.lastWorldPosition = ray.GetPoint(num);
									if (UICamera.mOverlap == null)
									{
										UICamera.mOverlap = new Collider2D[50];
									}
									int num3 = Physics2D.OverlapPointNonAlloc(UICamera.lastWorldPosition, UICamera.mOverlap, layerMask);
									if (num3 > 1)
									{
										int l = 0;
										while (l < num3)
										{
											GameObject gameObject3 = UICamera.mOverlap[l].gameObject;
											UIWidget component3 = gameObject3.GetComponent<UIWidget>();
											if (component3 != null)
											{
												if (component3.isVisible)
												{
													if (component3.hitCheck == null || component3.hitCheck(UICamera.lastWorldPosition))
													{
														goto IL_6A9;
													}
												}
											}
											else
											{
												UIRect uIRect3 = NGUITools.FindInParents<UIRect>(gameObject3);
												if (!(uIRect3 != null) || uIRect3.finalAlpha >= 0.001f)
												{
													goto IL_6A9;
												}
											}
											IL_6F8:
											l++;
											continue;
											IL_6A9:
											UICamera.mHit.depth = NGUITools.CalculateRaycastDepth(gameObject3);
											if (UICamera.mHit.depth != 2147483647)
											{
												UICamera.mHit.go = gameObject3;
												UICamera.mHit.point = UICamera.lastWorldPosition;
												UICamera.mHits.Add(UICamera.mHit);
												goto IL_6F8;
											}
											goto IL_6F8;
										}
										UICamera.mHits.Sort((UICamera.DepthEntry r1, UICamera.DepthEntry r2) => r2.depth.CompareTo(r1.depth));
										for (int m = 0; m < UICamera.mHits.size; m++)
										{
											if (UICamera.IsVisible(ref UICamera.mHits.buffer[m]))
											{
												UICamera.mRayHitObject = UICamera.mHits[m].go;
												UICamera.mHits.Clear();
												return true;
											}
										}
										UICamera.mHits.Clear();
									}
									else if (num3 == 1)
									{
										GameObject gameObject4 = UICamera.mOverlap[0].gameObject;
										UIWidget component4 = gameObject4.GetComponent<UIWidget>();
										if (component4 != null)
										{
											if (!component4.isVisible)
											{
												goto IL_851;
											}
											if (component4.hitCheck != null && !component4.hitCheck(UICamera.lastWorldPosition))
											{
												goto IL_851;
											}
										}
										else
										{
											UIRect uIRect4 = NGUITools.FindInParents<UIRect>(gameObject4);
											if (uIRect4 != null && uIRect4.finalAlpha < 0.001f)
											{
												goto IL_851;
											}
										}
										if (UICamera.IsVisible(UICamera.lastWorldPosition, gameObject4))
										{
											UICamera.mRayHitObject = gameObject4;
											return true;
										}
									}
								}
							}
						}
					}
				}
			}
			IL_851:;
		}
		return false;
	}

	private static bool IsVisible(Vector3 worldPoint, GameObject go)
	{
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(go);
		while (uIPanel != null)
		{
			if (!uIPanel.IsVisible(worldPoint))
			{
				return false;
			}
			uIPanel = uIPanel.parentPanel;
		}
		return true;
	}

	private static bool IsVisible(ref UICamera.DepthEntry de)
	{
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(de.go);
		while (uIPanel != null)
		{
			if (!uIPanel.IsVisible(de.point))
			{
				return false;
			}
			uIPanel = uIPanel.parentPanel;
		}
		return true;
	}

	public static bool IsHighlighted(GameObject go)
	{
		return UICamera.hoveredObject == go;
	}

	public static UICamera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		for (int i = 0; i < UICamera.list.size; i++)
		{
			UICamera uICamera = UICamera.list.buffer[i];
			Camera cachedCamera = uICamera.cachedCamera;
			if (cachedCamera != null && (cachedCamera.cullingMask & num) != 0)
			{
				return uICamera;
			}
		}
		return null;
	}

	private static int GetDirection(KeyCode up, KeyCode down)
	{
		if (UICamera.GetKeyDown(up))
		{
			UICamera.currentKey = up;
			return 1;
		}
		if (UICamera.GetKeyDown(down))
		{
			UICamera.currentKey = down;
			return -1;
		}
		return 0;
	}

	private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (UICamera.GetKeyDown(up0))
		{
			UICamera.currentKey = up0;
			return 1;
		}
		if (UICamera.GetKeyDown(up1))
		{
			UICamera.currentKey = up1;
			return 1;
		}
		if (UICamera.GetKeyDown(down0))
		{
			UICamera.currentKey = down0;
			return -1;
		}
		if (UICamera.GetKeyDown(down1))
		{
			UICamera.currentKey = down1;
			return -1;
		}
		return 0;
	}

	private static int GetDirection(string axis)
	{
		float time = RealTime.time;
		if (UICamera.mNextEvent < time && !string.IsNullOrEmpty(axis))
		{
			float num = UICamera.GetAxis(axis);
			if (num > 0.75f)
			{
				UICamera.currentKey = KeyCode.JoystickButton0;
				UICamera.mNextEvent = time + 0.25f;
				return 1;
			}
			if (num < -0.75f)
			{
				UICamera.currentKey = KeyCode.JoystickButton0;
				UICamera.mNextEvent = time + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	public static void Notify(GameObject go, string funcName, object obj)
	{
		if (UICamera.mNotifying > 10)
		{
			return;
		}
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller && UIPopupList.isOpen && UIPopupList.current.source == go && UIPopupList.isOpen)
		{
			go = UIPopupList.current.gameObject;
		}
		if (go && go.activeInHierarchy)
		{
			UICamera.mNotifying++;
			go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			if (UICamera.mGenericHandler != null && UICamera.mGenericHandler != go)
			{
				UICamera.mGenericHandler.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			}
			UICamera.mNotifying--;
		}
	}

	private void Awake()
	{
		UICamera.mWidth = Screen.width;
		UICamera.mHeight = Screen.height;
		UICamera.currentScheme = UICamera.ControlScheme.Touch;
		UICamera.mMouse[0].pos = Input.mousePosition;
		for (int i = 1; i < 3; i++)
		{
			UICamera.mMouse[i].pos = UICamera.mMouse[0].pos;
			UICamera.mMouse[i].lastPos = UICamera.mMouse[0].pos;
		}
		UICamera.mLastPos = UICamera.mMouse[0].pos;
	}

	private void OnEnable()
	{
		UICamera.list.Add(this);
		BetterList<UICamera> arg_2D_0 = UICamera.list;
		if (UICamera.<>f__mg$cache0 == null)
		{
			UICamera.<>f__mg$cache0 = new BetterList<UICamera>.CompareFunc(UICamera.CompareFunc);
		}
		arg_2D_0.Sort(UICamera.<>f__mg$cache0);
	}

	private void OnDisable()
	{
		UICamera.list.Remove(this);
	}

	private void Start()
	{
		BetterList<UICamera> arg_22_0 = UICamera.list;
		if (UICamera.<>f__mg$cache1 == null)
		{
			UICamera.<>f__mg$cache1 = new BetterList<UICamera>.CompareFunc(UICamera.CompareFunc);
		}
		arg_22_0.Sort(UICamera.<>f__mg$cache1);
		if (this.eventType != UICamera.EventType.World_3D && this.cachedCamera.transparencySortMode != TransparencySortMode.Orthographic)
		{
			this.cachedCamera.transparencySortMode = TransparencySortMode.Orthographic;
		}
		if (Application.isPlaying)
		{
			if (UICamera.fallThrough == null)
			{
				UIRoot uIRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
				UICamera.fallThrough = ((!(uIRoot != null)) ? base.gameObject : uIRoot.gameObject);
			}
			this.cachedCamera.eventMask = 0;
		}
	}

	private void Update()
	{
		if (UICamera.ignoreAllEvents)
		{
			return;
		}
		if (!this.handlesEvents)
		{
			return;
		}
		if (this.processEventsIn == UICamera.ProcessEventsIn.Update)
		{
			this.ProcessEvents();
		}
	}

	private void LateUpdate()
	{
		if (!this.handlesEvents)
		{
			return;
		}
		if (this.processEventsIn == UICamera.ProcessEventsIn.LateUpdate)
		{
			this.ProcessEvents();
		}
		int width = Screen.width;
		int height = Screen.height;
		if (width != UICamera.mWidth || height != UICamera.mHeight)
		{
			UICamera.mWidth = width;
			UICamera.mHeight = height;
			UIRoot.Broadcast("UpdateAnchors");
			if (UICamera.onScreenResize != null)
			{
				UICamera.onScreenResize();
			}
		}
	}

	private void ProcessEvents()
	{
		UICamera.current = this;
		NGUIDebug.debugRaycast = this.debug;
		if (this.useTouch)
		{
			this.ProcessTouches();
		}
		else if (this.useMouse)
		{
			this.ProcessMouse();
		}
		if (UICamera.onCustomInput != null)
		{
			UICamera.onCustomInput();
		}
		if ((this.useKeyboard || this.useController) && !UICamera.disableController && !UICamera.ignoreControllerInput)
		{
			this.ProcessOthers();
		}
		if (this.useMouse && UICamera.mHover != null)
		{
			float num = string.IsNullOrEmpty(this.scrollAxisName) ? 0f : UICamera.GetAxis(this.scrollAxisName);
			if (num != 0f)
			{
				if (UICamera.onScroll != null)
				{
					UICamera.onScroll(UICamera.mHover, num);
				}
				UICamera.Notify(UICamera.mHover, "OnScroll", num);
			}
			if (UICamera.currentScheme == UICamera.ControlScheme.Mouse && UICamera.showTooltips && UICamera.mTooltipTime != 0f && !UIPopupList.isOpen && UICamera.mMouse[0].dragged == null && (UICamera.mTooltipTime < RealTime.time || UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift)))
			{
				UICamera.currentTouch = UICamera.mMouse[0];
				UICamera.currentTouchID = -1;
				UICamera.ShowTooltip(UICamera.mHover);
			}
		}
		if (UICamera.mTooltip != null && !NGUITools.GetActive(UICamera.mTooltip))
		{
			UICamera.ShowTooltip(null);
		}
		UICamera.current = null;
		UICamera.currentTouchID = -100;
	}

	public void ProcessMouse()
	{
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < 3; i++)
		{
			if (Input.GetMouseButtonDown(i))
			{
				UICamera.currentKey = KeyCode.Mouse0 + i;
				flag2 = true;
				flag = true;
			}
			else if (Input.GetMouseButton(i))
			{
				UICamera.currentKey = KeyCode.Mouse0 + i;
				flag = true;
			}
		}
		if (UICamera.currentScheme == UICamera.ControlScheme.Touch && UICamera.activeTouches.Count > 0)
		{
			return;
		}
		UICamera.currentTouch = UICamera.mMouse[0];
		Vector2 vector = Input.mousePosition;
		if (UICamera.currentTouch.ignoreDelta == 0)
		{
			UICamera.currentTouch.delta = vector - UICamera.currentTouch.pos;
		}
		else
		{
			UICamera.currentTouch.ignoreDelta--;
			UICamera.currentTouch.delta.x = 0f;
			UICamera.currentTouch.delta.y = 0f;
		}
		float sqrMagnitude = UICamera.currentTouch.delta.sqrMagnitude;
		UICamera.currentTouch.pos = vector;
		UICamera.mLastPos = vector;
		bool flag3 = false;
		if (UICamera.currentScheme != UICamera.ControlScheme.Mouse)
		{
			if (sqrMagnitude < 0.001f)
			{
				return;
			}
			UICamera.currentKey = KeyCode.Mouse0;
			flag3 = true;
		}
		else if (sqrMagnitude > 0.001f)
		{
			flag3 = true;
		}
		for (int j = 1; j < 3; j++)
		{
			UICamera.mMouse[j].pos = UICamera.currentTouch.pos;
			UICamera.mMouse[j].delta = UICamera.currentTouch.delta;
		}
		if (flag || flag3 || this.mNextRaycast < RealTime.time)
		{
			this.mNextRaycast = RealTime.time + 0.02f;
			UICamera.Raycast(UICamera.currentTouch);
			if (flag)
			{
				flag3 = true;
				for (int k = 0; k < 3; k++)
				{
					UICamera.mMouse[k].current = UICamera.currentTouch.current;
				}
			}
			else if (UICamera.mMouse[0].current != UICamera.currentTouch.current)
			{
				UICamera.currentKey = KeyCode.Mouse0;
				flag3 = true;
				for (int l = 0; l < 3; l++)
				{
					UICamera.mMouse[l].current = UICamera.currentTouch.current;
				}
			}
		}
		bool flag4 = UICamera.currentTouch.last != UICamera.currentTouch.current;
		bool flag5 = UICamera.currentTouch.pressed != null;
		if (!flag5 && flag3)
		{
			UICamera.hoveredObject = UICamera.currentTouch.current;
		}
		UICamera.currentTouchID = -1;
		if (flag4)
		{
			UICamera.currentKey = KeyCode.Mouse0;
		}
		if (!flag && flag3 && (!this.stickyTooltip || flag4))
		{
			if (UICamera.mTooltipTime != 0f)
			{
				UICamera.mTooltipTime = Time.unscaledTime + this.tooltipDelay;
			}
			else if (UICamera.mTooltip != null)
			{
				UICamera.ShowTooltip(null);
			}
		}
		if (flag3 && UICamera.onMouseMove != null)
		{
			UICamera.onMouseMove(UICamera.currentTouch.delta);
			UICamera.currentTouch = null;
		}
		if (flag4 && (flag2 || (flag5 && !flag)))
		{
			UICamera.hoveredObject = null;
		}
		for (int m = 0; m < 3; m++)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(m);
			bool mouseButtonUp = Input.GetMouseButtonUp(m);
			if (mouseButtonDown || mouseButtonUp)
			{
				UICamera.currentKey = KeyCode.Mouse0 + m;
			}
			UICamera.currentTouch = UICamera.mMouse[m];
			UICamera.currentTouchID = -1 - m;
			UICamera.currentKey = KeyCode.Mouse0 + m;
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
				UICamera.currentTouch.pressTime = RealTime.time;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			this.ProcessTouch(mouseButtonDown, mouseButtonUp);
		}
		if (!flag && flag4)
		{
			UICamera.currentTouch = UICamera.mMouse[0];
			UICamera.mTooltipTime = Time.unscaledTime + this.tooltipDelay;
			UICamera.currentTouchID = -1;
			UICamera.currentKey = KeyCode.Mouse0;
			UICamera.hoveredObject = UICamera.currentTouch.current;
		}
		UICamera.currentTouch = null;
		UICamera.mMouse[0].last = UICamera.mMouse[0].current;
		for (int n = 1; n < 3; n++)
		{
			UICamera.mMouse[n].last = UICamera.mMouse[0].last;
		}
	}

	public void ProcessTouches()
	{
		int num = (UICamera.GetInputTouchCount != null) ? UICamera.GetInputTouchCount() : Input.touchCount;
		for (int i = 0; i < num; i++)
		{
			TouchPhase phase;
			int fingerId;
			Vector2 position;
			int tapCount;
			if (UICamera.GetInputTouch == null)
			{
				UnityEngine.Touch touch = Input.GetTouch(i);
				phase = touch.phase;
				fingerId = touch.fingerId;
				position = touch.position;
				tapCount = touch.tapCount;
			}
			else
			{
				UICamera.Touch touch2 = UICamera.GetInputTouch(i);
				phase = touch2.phase;
				fingerId = touch2.fingerId;
				position = touch2.position;
				tapCount = touch2.tapCount;
			}
			UICamera.currentTouchID = ((!this.allowMultiTouch) ? 1 : fingerId);
			UICamera.currentTouch = UICamera.GetTouch(UICamera.currentTouchID, true);
			bool flag = phase == TouchPhase.Began || UICamera.currentTouch.touchBegan;
			bool flag2 = phase == TouchPhase.Canceled || phase == TouchPhase.Ended;
			UICamera.currentTouch.delta = position - UICamera.currentTouch.pos;
			UICamera.currentTouch.pos = position;
			UICamera.currentKey = KeyCode.None;
			UICamera.Raycast(UICamera.currentTouch);
			if (flag)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			if (tapCount > 1)
			{
				UICamera.currentTouch.clickTime = RealTime.time;
			}
			this.ProcessTouch(flag, flag2);
			if (flag2)
			{
				UICamera.RemoveTouch(UICamera.currentTouchID);
			}
			UICamera.currentTouch.touchBegan = false;
			UICamera.currentTouch.last = null;
			UICamera.currentTouch = null;
			if (!this.allowMultiTouch)
			{
				break;
			}
		}
		if (num == 0)
		{
			if (UICamera.mUsingTouchEvents)
			{
				UICamera.mUsingTouchEvents = false;
				return;
			}
			if (this.useMouse)
			{
				this.ProcessMouse();
			}
		}
		else
		{
			UICamera.mUsingTouchEvents = true;
		}
	}

	private void ProcessFakeTouches()
	{
		bool mouseButtonDown = Input.GetMouseButtonDown(0);
		bool mouseButtonUp = Input.GetMouseButtonUp(0);
		bool mouseButton = Input.GetMouseButton(0);
		if (mouseButtonDown || mouseButtonUp || mouseButton)
		{
			UICamera.currentTouchID = 1;
			UICamera.currentTouch = UICamera.mMouse[0];
			UICamera.currentTouch.touchBegan = mouseButtonDown;
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressTime = RealTime.time;
				UICamera.activeTouches.Add(UICamera.currentTouch);
			}
			Vector2 vector = Input.mousePosition;
			UICamera.currentTouch.delta = vector - UICamera.currentTouch.pos;
			UICamera.currentTouch.pos = vector;
			UICamera.Raycast(UICamera.currentTouch);
			if (mouseButtonDown)
			{
				UICamera.currentTouch.pressedCam = UICamera.currentCamera;
			}
			else if (UICamera.currentTouch.pressed != null)
			{
				UICamera.currentCamera = UICamera.currentTouch.pressedCam;
			}
			UICamera.currentKey = KeyCode.None;
			this.ProcessTouch(mouseButtonDown, mouseButtonUp);
			if (mouseButtonUp)
			{
				UICamera.activeTouches.Remove(UICamera.currentTouch);
			}
			UICamera.currentTouch.last = null;
			UICamera.currentTouch = null;
		}
	}

	public void ProcessOthers()
	{
		UICamera.currentTouchID = -100;
		UICamera.currentTouch = UICamera.controller;
		bool flag = false;
		bool flag2 = false;
		if (this.submitKey0 != KeyCode.None && UICamera.GetKeyDown(this.submitKey0))
		{
			UICamera.currentKey = this.submitKey0;
			flag = true;
		}
		else if (this.submitKey1 != KeyCode.None && UICamera.GetKeyDown(this.submitKey1))
		{
			UICamera.currentKey = this.submitKey1;
			flag = true;
		}
		else if ((this.submitKey0 == KeyCode.Return || this.submitKey1 == KeyCode.Return) && UICamera.GetKeyDown(KeyCode.KeypadEnter))
		{
			UICamera.currentKey = this.submitKey0;
			flag = true;
		}
		if (this.submitKey0 != KeyCode.None && UICamera.GetKeyUp(this.submitKey0))
		{
			UICamera.currentKey = this.submitKey0;
			flag2 = true;
		}
		else if (this.submitKey1 != KeyCode.None && UICamera.GetKeyUp(this.submitKey1))
		{
			UICamera.currentKey = this.submitKey1;
			flag2 = true;
		}
		else if ((this.submitKey0 == KeyCode.Return || this.submitKey1 == KeyCode.Return) && UICamera.GetKeyUp(KeyCode.KeypadEnter))
		{
			UICamera.currentKey = this.submitKey0;
			flag2 = true;
		}
		if (flag)
		{
			UICamera.currentTouch.pressTime = RealTime.time;
		}
		if ((flag || flag2) && UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			UICamera.currentTouch.current = UICamera.controllerNavigationObject;
			this.ProcessTouch(flag, flag2);
			UICamera.currentTouch.last = UICamera.currentTouch.current;
		}
		KeyCode keyCode = KeyCode.None;
		if (this.useController && !UICamera.ignoreControllerInput)
		{
			if (!UICamera.disableController && UICamera.currentScheme == UICamera.ControlScheme.Controller && (UICamera.currentTouch.current == null || !UICamera.currentTouch.current.activeInHierarchy))
			{
				UICamera.currentTouch.current = UICamera.controllerNavigationObject;
			}
			if (!string.IsNullOrEmpty(this.verticalAxisName))
			{
				int direction = UICamera.GetDirection(this.verticalAxisName);
				if (direction != 0)
				{
					UICamera.ShowTooltip(null);
					UICamera.currentScheme = UICamera.ControlScheme.Controller;
					UICamera.currentTouch.current = UICamera.controllerNavigationObject;
					if (UICamera.currentTouch.current != null)
					{
						keyCode = ((direction <= 0) ? KeyCode.DownArrow : KeyCode.UpArrow);
						if (UICamera.onNavigate != null)
						{
							UICamera.onNavigate(UICamera.currentTouch.current, keyCode);
						}
						UICamera.Notify(UICamera.currentTouch.current, "OnNavigate", keyCode);
					}
				}
			}
			if (!string.IsNullOrEmpty(this.horizontalAxisName))
			{
				int direction2 = UICamera.GetDirection(this.horizontalAxisName);
				if (direction2 != 0)
				{
					UICamera.ShowTooltip(null);
					UICamera.currentScheme = UICamera.ControlScheme.Controller;
					UICamera.currentTouch.current = UICamera.controllerNavigationObject;
					if (UICamera.currentTouch.current != null)
					{
						keyCode = ((direction2 <= 0) ? KeyCode.LeftArrow : KeyCode.RightArrow);
						if (UICamera.onNavigate != null)
						{
							UICamera.onNavigate(UICamera.currentTouch.current, keyCode);
						}
						UICamera.Notify(UICamera.currentTouch.current, "OnNavigate", keyCode);
					}
				}
			}
			float num = string.IsNullOrEmpty(this.horizontalPanAxisName) ? 0f : UICamera.GetAxis(this.horizontalPanAxisName);
			float num2 = string.IsNullOrEmpty(this.verticalPanAxisName) ? 0f : UICamera.GetAxis(this.verticalPanAxisName);
			if (num != 0f || num2 != 0f)
			{
				UICamera.ShowTooltip(null);
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.currentTouch.current = UICamera.controllerNavigationObject;
				if (UICamera.currentTouch.current != null)
				{
					Vector2 vector = new Vector2(num, num2);
					vector *= Time.unscaledDeltaTime;
					if (UICamera.onPan != null)
					{
						UICamera.onPan(UICamera.currentTouch.current, vector);
					}
					UICamera.Notify(UICamera.currentTouch.current, "OnPan", vector);
				}
			}
		}
		if ((UICamera.GetAnyKeyDown == null) ? Input.anyKeyDown : UICamera.GetAnyKeyDown())
		{
			int i = 0;
			int num3 = NGUITools.keys.Length;
			while (i < num3)
			{
				KeyCode keyCode2 = NGUITools.keys[i];
				if (keyCode != keyCode2)
				{
					if (UICamera.GetKeyDown(keyCode2))
					{
						if (this.useKeyboard || keyCode2 >= KeyCode.Mouse0)
						{
							if ((this.useController && !UICamera.ignoreControllerInput) || keyCode2 < KeyCode.JoystickButton0)
							{
								if (this.useMouse || keyCode2 < KeyCode.Mouse0 || keyCode2 > KeyCode.Mouse6)
								{
									UICamera.currentKey = keyCode2;
									if (UICamera.onKey != null)
									{
										UICamera.onKey(UICamera.currentTouch.current, keyCode2);
									}
									UICamera.Notify(UICamera.currentTouch.current, "OnKey", keyCode2);
								}
							}
						}
					}
				}
				i++;
			}
		}
		UICamera.currentTouch = null;
	}

	private void ProcessPress(bool pressed, float click, float drag)
	{
		if (pressed)
		{
			if (UICamera.mTooltip != null)
			{
				UICamera.ShowTooltip(null);
			}
			UICamera.mTooltipTime = Time.unscaledTime + this.tooltipDelay;
			UICamera.currentTouch.pressStarted = true;
			if (UICamera.onPress != null && UICamera.currentTouch.pressed)
			{
				UICamera.onPress(UICamera.currentTouch.pressed, false);
			}
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			if (UICamera.currentScheme == UICamera.ControlScheme.Mouse && UICamera.hoveredObject == null && UICamera.currentTouch.current != null)
			{
				UICamera.hoveredObject = UICamera.currentTouch.current;
			}
			UICamera.currentTouch.pressed = UICamera.currentTouch.current;
			UICamera.currentTouch.dragged = UICamera.currentTouch.current;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			UICamera.currentTouch.totalDelta = Vector2.zero;
			UICamera.currentTouch.dragStarted = false;
			if (UICamera.onPress != null && UICamera.currentTouch.pressed)
			{
				UICamera.onPress(UICamera.currentTouch.pressed, true);
			}
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", true);
			if (UICamera.mSelected != UICamera.currentTouch.pressed)
			{
				UICamera.mInputFocus = false;
				if (UICamera.mSelected)
				{
					UICamera.Notify(UICamera.mSelected, "OnSelect", false);
					if (UICamera.onSelect != null)
					{
						UICamera.onSelect(UICamera.mSelected, false);
					}
				}
				UICamera.mSelected = UICamera.currentTouch.pressed;
				if (UICamera.currentTouch.pressed != null)
				{
					UIKeyNavigation component = UICamera.currentTouch.pressed.GetComponent<UIKeyNavigation>();
					if (component != null)
					{
						UICamera.controller.current = UICamera.currentTouch.pressed;
					}
				}
				if (UICamera.mSelected)
				{
					UICamera.mInputFocus = (UICamera.mSelected.activeInHierarchy && UICamera.mSelected.GetComponent<UIInput>() != null);
					if (UICamera.onSelect != null)
					{
						UICamera.onSelect(UICamera.mSelected, true);
					}
					UICamera.Notify(UICamera.mSelected, "OnSelect", true);
				}
			}
		}
		else if (UICamera.currentTouch.pressed != null && (UICamera.currentTouch.delta.sqrMagnitude != 0f || UICamera.currentTouch.current != UICamera.currentTouch.last))
		{
			UICamera.currentTouch.totalDelta += UICamera.currentTouch.delta;
			float sqrMagnitude = UICamera.currentTouch.totalDelta.sqrMagnitude;
			bool flag = false;
			if (!UICamera.currentTouch.dragStarted && UICamera.currentTouch.last != UICamera.currentTouch.current)
			{
				UICamera.currentTouch.dragStarted = true;
				UICamera.currentTouch.delta = UICamera.currentTouch.totalDelta;
				UICamera.isDragging = true;
				if (UICamera.onDragStart != null)
				{
					UICamera.onDragStart(UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDragStart", null);
				if (UICamera.onDragOver != null)
				{
					UICamera.onDragOver(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.last, "OnDragOver", UICamera.currentTouch.dragged);
				UICamera.isDragging = false;
			}
			else if (!UICamera.currentTouch.dragStarted && drag < sqrMagnitude)
			{
				flag = true;
				UICamera.currentTouch.dragStarted = true;
				UICamera.currentTouch.delta = UICamera.currentTouch.totalDelta;
			}
			if (UICamera.currentTouch.dragStarted)
			{
				if (UICamera.mTooltip != null)
				{
					UICamera.ShowTooltip(null);
				}
				UICamera.isDragging = true;
				bool flag2 = UICamera.currentTouch.clickNotification == UICamera.ClickNotification.None;
				if (flag)
				{
					if (UICamera.onDragStart != null)
					{
						UICamera.onDragStart(UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.dragged, "OnDragStart", null);
					if (UICamera.onDragOver != null)
					{
						UICamera.onDragOver(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.current, "OnDragOver", UICamera.currentTouch.dragged);
				}
				else if (UICamera.currentTouch.last != UICamera.currentTouch.current)
				{
					if (UICamera.onDragOut != null)
					{
						UICamera.onDragOut(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.last, "OnDragOut", UICamera.currentTouch.dragged);
					if (UICamera.onDragOver != null)
					{
						UICamera.onDragOver(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
					}
					UICamera.Notify(UICamera.currentTouch.current, "OnDragOver", UICamera.currentTouch.dragged);
				}
				if (UICamera.onDrag != null)
				{
					UICamera.onDrag(UICamera.currentTouch.dragged, UICamera.currentTouch.delta);
				}
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDrag", UICamera.currentTouch.delta);
				UICamera.currentTouch.last = UICamera.currentTouch.current;
				UICamera.isDragging = false;
				if (flag2)
				{
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				}
				else if (UICamera.currentTouch.clickNotification == UICamera.ClickNotification.BasedOnDelta && click < sqrMagnitude)
				{
					UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				}
			}
		}
	}

	private void ProcessRelease(bool isMouse, float drag)
	{
		if (UICamera.currentTouch == null)
		{
			return;
		}
		UICamera.currentTouch.pressStarted = false;
		if (UICamera.currentTouch.pressed != null)
		{
			if (UICamera.currentTouch.dragStarted)
			{
				if (UICamera.onDragOut != null)
				{
					UICamera.onDragOut(UICamera.currentTouch.last, UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.last, "OnDragOut", UICamera.currentTouch.dragged);
				if (UICamera.onDragEnd != null)
				{
					UICamera.onDragEnd(UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.dragged, "OnDragEnd", null);
			}
			if (UICamera.onPress != null)
			{
				UICamera.onPress(UICamera.currentTouch.pressed, false);
			}
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			if (isMouse)
			{
				bool flag = this.HasCollider(UICamera.currentTouch.pressed);
				if (flag)
				{
					if (UICamera.mHover == UICamera.currentTouch.current)
					{
						if (UICamera.onHover != null)
						{
							UICamera.onHover(UICamera.currentTouch.current, true);
						}
						UICamera.Notify(UICamera.currentTouch.current, "OnHover", true);
					}
					else
					{
						UICamera.hoveredObject = UICamera.currentTouch.current;
					}
				}
			}
			if (UICamera.currentTouch.dragged == UICamera.currentTouch.current || (UICamera.currentScheme != UICamera.ControlScheme.Controller && UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && UICamera.currentTouch.totalDelta.sqrMagnitude < drag))
			{
				if (UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && UICamera.currentTouch.pressed == UICamera.currentTouch.current)
				{
					UICamera.ShowTooltip(null);
					float time = RealTime.time;
					if (UICamera.onClick != null)
					{
						UICamera.onClick(UICamera.currentTouch.pressed);
					}
					UICamera.Notify(UICamera.currentTouch.pressed, "OnClick", null);
					if (UICamera.currentTouch.clickTime + 0.35f > time)
					{
						if (UICamera.onDoubleClick != null)
						{
							UICamera.onDoubleClick(UICamera.currentTouch.pressed);
						}
						UICamera.Notify(UICamera.currentTouch.pressed, "OnDoubleClick", null);
					}
					UICamera.currentTouch.clickTime = time;
				}
			}
			else if (UICamera.currentTouch.dragStarted)
			{
				if (UICamera.onDrop != null)
				{
					UICamera.onDrop(UICamera.currentTouch.current, UICamera.currentTouch.dragged);
				}
				UICamera.Notify(UICamera.currentTouch.current, "OnDrop", UICamera.currentTouch.dragged);
			}
		}
		UICamera.currentTouch.dragStarted = false;
		UICamera.currentTouch.pressed = null;
		UICamera.currentTouch.dragged = null;
	}

	private bool HasCollider(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		Collider component = go.GetComponent<Collider>();
		if (component != null)
		{
			return component.enabled;
		}
		Collider2D component2 = go.GetComponent<Collider2D>();
		return component2 != null && component2.enabled;
	}

	public void ProcessTouch(bool pressed, bool released)
	{
		if (released)
		{
			UICamera.mTooltipTime = 0f;
		}
		bool flag = UICamera.currentScheme == UICamera.ControlScheme.Mouse;
		float num = (!flag) ? this.touchDragThreshold : this.mouseDragThreshold;
		float num2 = (!flag) ? this.touchClickThreshold : this.mouseClickThreshold;
		num *= num;
		num2 *= num2;
		if (UICamera.currentTouch.pressed != null)
		{
			if (released)
			{
				this.ProcessRelease(flag, num);
			}
			this.ProcessPress(pressed, num2, num);
			if (this.tooltipDelay != 0f && UICamera.currentTouch.deltaTime > this.tooltipDelay && UICamera.currentTouch.pressed == UICamera.currentTouch.current && UICamera.mTooltipTime != 0f && !UICamera.currentTouch.dragStarted)
			{
				UICamera.mTooltipTime = 0f;
				UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
				if (this.longPressTooltip)
				{
					UICamera.ShowTooltip(UICamera.currentTouch.pressed);
				}
				UICamera.Notify(UICamera.currentTouch.current, "OnLongPress", null);
			}
		}
		else if (flag || pressed || released)
		{
			this.ProcessPress(pressed, num2, num);
			if (released)
			{
				this.ProcessRelease(flag, num);
			}
		}
	}

	public static void CancelNextTooltip()
	{
		UICamera.mTooltipTime = 0f;
	}

	public static bool ShowTooltip(GameObject go)
	{
		if (UICamera.mTooltip != go)
		{
			if (UICamera.mTooltip != null)
			{
				if (UICamera.onTooltip != null)
				{
					UICamera.onTooltip(UICamera.mTooltip, false);
				}
				UICamera.Notify(UICamera.mTooltip, "OnTooltip", false);
			}
			UICamera.mTooltip = go;
			UICamera.mTooltipTime = 0f;
			if (UICamera.mTooltip != null)
			{
				if (UICamera.onTooltip != null)
				{
					UICamera.onTooltip(UICamera.mTooltip, true);
				}
				UICamera.Notify(UICamera.mTooltip, "OnTooltip", true);
			}
			return true;
		}
		return false;
	}

	public static bool HideTooltip()
	{
		return UICamera.ShowTooltip(null);
	}
}
