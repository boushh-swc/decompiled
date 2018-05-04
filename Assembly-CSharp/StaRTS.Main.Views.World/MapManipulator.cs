using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UserInput;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class MapManipulator : IUserInputObserver, IEventObserver, IViewPhysicsTimeObserver
	{
		private const float MAP_CAMERA_MIN_Y_ULTRA = 0f;

		private const float MAP_CAMERA_MAX_Y_ULTRA = 700f;

		private const float MAP_LOOKAT_SUPER_EARLY_DIFF = 0f;

		private const float MAP_ZOOM_SUPER_EARLY_DIFF = 50f;

		private const float MAP_ZOOM_TOP_DOWN_DIFF = 200f;

		private const float MAP_RADIUS_ULTRA = 300f;

		private const float MAP_CAMERA_MIN_X_ULTRA = -1000f;

		private const float MAP_CAMERA_MAX_X_ULTRA = 300f;

		private const float MAP_CAMERA_MIN_Z_ULTRA = -1000f;

		private const float MAP_CAMERA_MAX_Z_ULTRA = 300f;

		private const float SUPER_ZOOM_FACTOR = 1.5f;

		private const float ZOOM_FACTOR = 1200f;

		private const float SCROLL_ZOOM_FACTOR = 20f;

		private const float AUTO_PAN_FACTOR = 2.5f;

		private const float AUTO_PAN_THRESHOLD = 0.2f;

		private const float AUTO_PAN_MINIMUM = 0.05f;

		private const float ZOOM_IN_OUT_AMOUNT = 150f;

		private const float SLOW_AUTO_ZOOM_THRESHOLD = 5f;

		private const float SCROLL_END_TIMEOUT = 0.1f;

		private const float VANTAGE_SWITCH_TIME = 0.5f;

		private const float AUTOPAN_THRESH = 107.100006f;

		private MainCamera camera;

		private bool switchingVantage;

		private uint switchingVantageTimer;

		private int anchorFingerId;

		private Vector2 anchorScreenPosition;

		private Vector2 anchorLastScreenPosition;

		private Vector3 anchorGroundPosition;

		private Vector3 anchorLastGroundPosition;

		private int pinchFingerId;

		private Vector2 pinchScreenPosition;

		private Vector2 pinchLastScreenPosition;

		private Vector3 pinchGroundPosition;

		private Vector3 pinchLastGroundPosition;

		private EasingDirection easingDirection;

		private Entity liftedBuilding;

		private uint scrollTimer;

		private bool scrollImmediate;

		private Vector3 mapNearLEarly;

		private Vector3 mapNearREarly;

		private Vector3 mapFarLEarly;

		private Vector3 mapFarREarly;

		private Vector3 mapNearLSuper;

		private Vector3 mapNearRSuper;

		private Vector3 mapFarLSuper;

		private Vector3 mapFarRSuper;

		private Vector3[] earlyCorners;

		private Vector3[] superCorners;

		private float mapLowYEarly;

		private float mapHighYEarly;

		private float mapHighYEarlyTopDown;

		private float mapLowYSuper;

		private float mapHighYSuper;

		private float mapHighYSuperTopDown;

		private Vector3[] screenCorners;

		private UnityEngine.Object screenLock = new UnityEngine.Object();

		public float YToHypotenuse
		{
			get;
			set;
		}

		public Vector3[] CornerLocators
		{
			set
			{
				this.mapNearLSuper = value[0];
				this.mapNearRSuper = value[1];
				this.mapFarLSuper = value[2];
				this.mapFarRSuper = value[3];
				Vector3 normalized = (this.mapFarRSuper - this.mapNearLSuper).normalized;
				Vector3 normalized2 = (this.mapFarLSuper - this.mapNearRSuper).normalized;
				this.mapNearLEarly = this.mapNearLSuper + normalized * 0f;
				this.mapNearREarly = this.mapNearRSuper + normalized2 * 0f;
				this.mapFarLEarly = this.mapFarLSuper - normalized2 * 0f;
				this.mapFarREarly = this.mapFarRSuper - normalized * 0f;
				this.AssertValidEdge(ref this.mapNearLSuper, this.mapFarLSuper);
				this.AssertValidEdge(ref this.mapFarLSuper, this.mapFarRSuper);
				this.AssertValidEdge(ref this.mapFarRSuper, this.mapNearRSuper);
				this.AssertValidEdge(ref this.mapNearRSuper, this.mapNearLSuper);
				this.AssertValidEdge(ref this.mapNearLEarly, this.mapFarLEarly);
				this.AssertValidEdge(ref this.mapFarLEarly, this.mapFarREarly);
				this.AssertValidEdge(ref this.mapFarREarly, this.mapNearREarly);
				this.AssertValidEdge(ref this.mapNearREarly, this.mapNearLEarly);
				this.superCorners = new Vector3[4];
				this.superCorners[0] = this.mapNearLSuper;
				this.superCorners[1] = this.mapFarLSuper;
				this.superCorners[2] = this.mapFarRSuper;
				this.superCorners[3] = this.mapNearRSuper;
				this.earlyCorners = new Vector3[4];
				this.earlyCorners[0] = this.mapNearLEarly;
				this.earlyCorners[1] = this.mapFarLEarly;
				this.earlyCorners[2] = this.mapFarREarly;
				this.earlyCorners[3] = this.mapNearREarly;
				this.mapLowYSuper = 50f;
				float magnitude = (this.mapNearLSuper - this.mapFarRSuper).magnitude;
				this.mapHighYSuper = 1.5f * magnitude * (float)Screen.height / (float)Screen.width;
				this.mapLowYEarly = this.mapLowYSuper + 50f;
				this.mapHighYEarly = this.mapHighYSuper - 50f;
				this.mapHighYSuperTopDown = this.mapHighYSuper + 200f;
				this.mapHighYEarlyTopDown = this.mapHighYEarly + 200f;
			}
		}

		public MapManipulator(float yToHypotenuse, Vector3[] cornerLocators)
		{
			this.camera = Service.CameraManager.MainCamera;
			this.camera.SetCameraFeel(CameraFeel.Medium);
			this.switchingVantage = false;
			this.switchingVantageTimer = 0u;
			this.anchorFingerId = -1;
			this.anchorScreenPosition = -Vector2.one;
			this.anchorLastScreenPosition = -Vector2.one;
			this.anchorGroundPosition = Vector3.zero;
			this.anchorLastGroundPosition = Vector3.zero;
			this.pinchFingerId = -1;
			this.pinchScreenPosition = -Vector2.one;
			this.pinchLastScreenPosition = -Vector2.one;
			this.pinchGroundPosition = Vector3.zero;
			this.pinchLastGroundPosition = Vector3.zero;
			this.easingDirection = new EasingDirection();
			this.liftedBuilding = null;
			this.scrollTimer = 0u;
			this.scrollImmediate = false;
			this.YToHypotenuse = yToHypotenuse;
			this.CornerLocators = cornerLocators;
			this.screenCorners = new Vector3[4];
			this.screenCorners[0] = new Vector3(0f, 0f, 0f);
			this.screenCorners[1] = new Vector3((float)Screen.width, 0f, 0f);
			this.screenCorners[2] = new Vector3(0f, (float)Screen.height, 0f);
			this.screenCorners[3] = new Vector3((float)Screen.width, (float)Screen.height, 0f);
			this.camera.SetupDefaults(0f, 700f, -1000f, 300f, -1000f, 300f, this.camera.CurrentCameraAnchor, this.camera.CurrentLookatAnchor);
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.UserLiftedBuilding, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.UserLoweredBuilding, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ScreenSizeChanged, EventPriority.Default);
		}

		private void AssertValidEdge(ref Vector3 edge0, Vector3 edge1)
		{
			if (edge1.x == edge0.x)
			{
				Service.Logger.Error(string.Concat(new object[]
				{
					"Invalid map edge: ",
					edge0,
					",",
					edge1
				}));
			}
			if (edge0.y != 0f)
			{
				Service.Logger.Warn("Corner locator not on ground: " + edge0);
				edge0.y = 0f;
			}
		}

		private void EnableUserInput()
		{
			Service.UserInputManager.RegisterObserver(this, UserInputLayer.InternalLowest);
			Service.ViewTimeEngine.RegisterPhysicsTimeObserver(this);
		}

		private void DisableUserInput()
		{
			Service.UserInputManager.UnregisterObserver(this, UserInputLayer.InternalLowest);
			Service.ViewTimeEngine.UnregisterPhysicsTimeObserver(this);
		}

		public void OnVantageSwitch(bool topDown)
		{
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			if (this.switchingVantage)
			{
				viewTimerManager.KillViewTimer(this.switchingVantageTimer);
			}
			else
			{
				this.switchingVantage = true;
			}
			this.switchingVantageTimer = viewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.OnVantageSwitchTimer), null);
			if (topDown)
			{
				this.mapHighYSuper = this.mapHighYSuperTopDown;
				this.mapHighYEarly = this.mapHighYEarlyTopDown;
			}
			else
			{
				this.mapHighYSuper = this.mapHighYSuperTopDown - 200f;
				this.mapHighYEarly = this.mapHighYEarlyTopDown - 200f;
			}
		}

		private void OnVantageSwitchTimer(uint id, object cookie)
		{
			this.switchingVantage = false;
		}

		public EatResponse OnPress(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			if (this.switchingVantage)
			{
				return EatResponse.Eaten;
			}
			MapFinger mapFinger = this.ChooseFinger(id);
			if (mapFinger != MapFinger.Anchor)
			{
				if (mapFinger != MapFinger.Pinch)
				{
					return EatResponse.NotEaten;
				}
				this.pinchScreenPosition = screenPosition;
				this.pinchLastScreenPosition = this.pinchScreenPosition;
				this.pinchGroundPosition = groundPosition;
				this.pinchLastGroundPosition = this.pinchGroundPosition;
			}
			else
			{
				this.anchorScreenPosition = screenPosition;
				this.anchorLastScreenPosition = this.anchorScreenPosition;
				this.anchorGroundPosition = groundPosition;
				this.anchorLastGroundPosition = this.anchorGroundPosition;
			}
			this.camera.StopMoving();
			if (this.scrollTimer != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.scrollTimer);
				this.scrollTimer = 0u;
				this.scrollImmediate = false;
			}
			this.camera.SetCameraFeel(CameraFeel.Fast);
			this.HandleAnchorAndPinch();
			Service.EventManager.SendEvent(EventId.UserStartedCameraMove, null);
			return EatResponse.Eaten;
		}

		public EatResponse OnDrag(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			MapFinger mapFinger = this.TranslateToFinger(id);
			if (mapFinger != MapFinger.Anchor)
			{
				if (mapFinger != MapFinger.Pinch)
				{
					return EatResponse.NotEaten;
				}
				this.pinchScreenPosition = screenPosition;
				this.pinchGroundPosition = groundPosition;
			}
			else
			{
				this.anchorScreenPosition = screenPosition;
				this.anchorGroundPosition = groundPosition;
			}
			this.HandleAnchorAndPinch();
			return EatResponse.Eaten;
		}

		public EatResponse OnRelease(int id)
		{
			MapFinger mapFinger = this.TranslateToFinger(id);
			if (mapFinger != MapFinger.Anchor)
			{
				if (mapFinger != MapFinger.Pinch)
				{
					return EatResponse.NotEaten;
				}
				this.AbandonPinch();
			}
			else
			{
				this.AbandonAnchor();
			}
			this.HandleAnchorAndPinch();
			if (this.anchorFingerId < 0)
			{
				this.EnsureWithinBounds();
			}
			return EatResponse.NotEaten;
		}

		public EatResponse OnScroll(float delta, Vector2 screenPosition)
		{
			this.camera.StopAddonBehaviors();
			this.camera.SetCameraFeel(CameraFeel.Fast);
			if (this.CameraZoom(delta * 20f, this.scrollImmediate))
			{
				this.scrollImmediate = true;
			}
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			if (this.scrollTimer != 0u)
			{
				viewTimerManager.KillViewTimer(this.scrollTimer);
			}
			this.scrollTimer = viewTimerManager.CreateViewTimer(0.1f, false, new TimerDelegate(this.OnScrollTimer), null);
			Service.EventManager.SendEvent(EventId.UserStartedCameraMove, null);
			return EatResponse.Eaten;
		}

		private void OnScrollTimer(uint id, object cookie)
		{
			if (id == this.scrollTimer)
			{
				this.scrollTimer = 0u;
				this.scrollImmediate = false;
				this.EnsureWithinBounds();
			}
		}

		private MapFinger ChooseFinger(int id)
		{
			if (this.anchorFingerId < 0 || this.anchorFingerId == id)
			{
				this.anchorFingerId = id;
				return MapFinger.Anchor;
			}
			if (this.pinchFingerId < 0 || this.pinchFingerId == id)
			{
				this.pinchFingerId = id;
				return MapFinger.Pinch;
			}
			return MapFinger.Unknown;
		}

		private MapFinger TranslateToFinger(int id)
		{
			if (this.anchorFingerId == id)
			{
				return MapFinger.Anchor;
			}
			if (this.pinchFingerId == id)
			{
				return MapFinger.Pinch;
			}
			return MapFinger.Unknown;
		}

		private void EnsureWithinBounds()
		{
			Vector3 zero = Vector3.zero;
			this.RestrictCameraWithinEarlySet(ref zero);
			this.camera.Pan(zero, false);
			Vector3 currentCameraAnchor = this.camera.CurrentCameraAnchor;
			float num = 0f;
			if (currentCameraAnchor.y < this.mapLowYEarly)
			{
				num = this.mapLowYEarly - currentCameraAnchor.y;
			}
			else if (currentCameraAnchor.y > this.mapHighYEarly)
			{
				num = this.mapHighYEarly - currentCameraAnchor.y;
			}
			if (num != 0f)
			{
				this.CameraZoom(-num * this.YToHypotenuse, false);
				if (Mathf.Abs(num) > 5f)
				{
					this.camera.SetCameraFeel(CameraFeel.Medium);
				}
			}
		}

		private void AbandonAnchor()
		{
			this.anchorFingerId = this.pinchFingerId;
			this.anchorScreenPosition = this.pinchScreenPosition;
			this.AbandonPinch();
		}

		private bool IsNonBaseCameraState(IState currentState)
		{
			return currentState is GalaxyState || currentState is WarBoardState;
		}

		private bool IsCurrentStateANonBaseCameraState()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			return this.IsNonBaseCameraState(currentState);
		}

		private void AbandonPinch()
		{
			this.pinchFingerId = -1;
			this.pinchScreenPosition = -Vector2.one;
			this.pinchGroundPosition = Vector3.zero;
			if (this.anchorFingerId >= 0)
			{
				this.camera.GetGroundPosition(this.anchorScreenPosition, ref this.anchorGroundPosition);
			}
			else
			{
				if (this.IsCurrentStateANonBaseCameraState())
				{
					return;
				}
				Vector3 amount = this.easingDirection.CalculateAndReset();
				this.camera.Pan(amount, false);
				this.anchorGroundPosition = Vector3.zero;
			}
			this.anchorLastGroundPosition = this.anchorGroundPosition;
		}

		private void HandleAnchorAndPinch()
		{
			if (this.pinchFingerId >= 0)
			{
				this.HandleDrag();
				this.HandleZoom();
			}
			else if (this.anchorFingerId >= 0)
			{
				this.HandleDrag();
			}
			this.anchorLastScreenPosition = this.anchorScreenPosition;
			this.pinchLastScreenPosition = this.pinchScreenPosition;
		}

		private void HandleDrag()
		{
			Vector3 b;
			Vector3 a;
			if (this.pinchFingerId >= 0)
			{
				b = (this.anchorGroundPosition + this.pinchGroundPosition) * 0.25f;
				a = (this.anchorLastGroundPosition + this.pinchLastGroundPosition) * 0.25f;
			}
			else
			{
				b = this.anchorGroundPosition;
				a = this.anchorLastGroundPosition;
			}
			Vector3 amount = a - b;
			this.RestrictCameraWithinSuperAdjust(ref amount);
			this.camera.Pan(amount, true);
			this.easingDirection.OnDrag(amount);
		}

		private void RestrictCameraWithinEarlyAdjust(ref Vector3 amount)
		{
			if (amount.x == 0f && amount.z == 0f)
			{
				return;
			}
			int i = 0;
			int num = this.screenCorners.Length;
			while (i < num)
			{
				Vector3 zero = Vector3.zero;
				this.camera.GetGroundPosition(this.screenCorners[i], ref zero);
				this.RestrictPointWithinEarlyAdjust(zero, ref amount);
				i++;
			}
		}

		private void RestrictCameraWithinSuperAdjust(ref Vector3 amount)
		{
			if (amount.x == 0f && amount.z == 0f)
			{
				return;
			}
			int i = 0;
			int num = this.screenCorners.Length;
			while (i < num)
			{
				Vector3 zero = Vector3.zero;
				this.camera.GetGroundPosition(this.screenCorners[i], ref zero);
				this.RestrictPointWithinSuperAdjust(zero, ref amount);
				i++;
			}
		}

		private void RestrictCameraWithinEarlySet(ref Vector3 amount)
		{
			int i = 0;
			int num = this.screenCorners.Length;
			while (i < num)
			{
				Vector3 vector = Vector3.zero;
				this.camera.GetGroundPosition(this.screenCorners[i], ref vector);
				vector += this.camera.CurrentLookatAnchor - this.camera.CurrentLookatPosition;
				this.RestrictPointWithinEarlySet(vector, ref amount);
				i++;
			}
		}

		private void RestrictCameraWithinSuperSet(ref Vector3 amount, bool immediate)
		{
			int i = 0;
			int num = this.screenCorners.Length;
			while (i < num)
			{
				Vector3 vector = Vector3.zero;
				this.camera.GetGroundPosition(this.screenCorners[i], ref vector);
				if (!immediate)
				{
					vector += this.camera.CurrentLookatAnchor - this.camera.CurrentLookatPosition;
				}
				this.RestrictPointWithinSuperSet(vector, ref amount);
				i++;
			}
		}

		private void RestrictPointWithinEarlyAdjust(Vector3 p, ref Vector3 amount)
		{
			int num = 0;
			int num2 = 0;
			while (num2 < 4 && num < 2)
			{
				num += this.IntersectHelperAdjust(p, ref amount, this.earlyCorners, num2);
				num2++;
			}
		}

		private void RestrictPointWithinSuperAdjust(Vector3 p, ref Vector3 amount)
		{
			int num = 0;
			int num2 = 0;
			while (num2 < 4 && num < 2)
			{
				num += this.IntersectHelperAdjust(p, ref amount, this.superCorners, num2);
				num2++;
			}
		}

		private void RestrictPointWithinEarlySet(Vector3 p, ref Vector3 amount)
		{
			this.IntersectHelperSet(p, ref amount, this.mapNearLEarly, this.mapFarLEarly);
			this.IntersectHelperSet(p, ref amount, this.mapFarLEarly, this.mapFarREarly);
			this.IntersectHelperSet(p, ref amount, this.mapFarREarly, this.mapNearREarly);
			this.IntersectHelperSet(p, ref amount, this.mapNearREarly, this.mapNearLEarly);
		}

		private void RestrictPointWithinSuperSet(Vector3 p, ref Vector3 amount)
		{
			this.IntersectHelperSet(p, ref amount, this.mapNearLSuper, this.mapFarLSuper);
			this.IntersectHelperSet(p, ref amount, this.mapFarLSuper, this.mapFarRSuper);
			this.IntersectHelperSet(p, ref amount, this.mapFarRSuper, this.mapNearRSuper);
			this.IntersectHelperSet(p, ref amount, this.mapNearRSuper, this.mapNearLSuper);
		}

		private int IntersectHelperAdjust(Vector3 curPosition, ref Vector3 amount, Vector3[] corners, int corner)
		{
			Vector3 a = curPosition + amount;
			Vector3 b = corners[corner];
			Vector3 vector = corners[(corner + 1) % 4];
			if (Vector3.Cross(a - b, vector - b).y > 0f)
			{
				Vector3 vector2 = curPosition - b;
				Vector3 vector3 = vector - b;
				float num = vector3.z / vector3.x;
				float num2 = vector2.z - num * vector2.x;
				float num3 = num * amount.x - amount.z;
				float num4;
				if (num3 == 0f)
				{
					num4 = 0.5f;
				}
				else
				{
					float num5 = num2 / num3;
					num4 = (vector2.x + num5 * amount.x) / vector3.x;
					num4 = (num4 - 0.5f) * 2f + 0.5f;
					if (num4 < 0f)
					{
						num4 = 0f;
					}
					else if (num4 > 1f)
					{
						num4 = 1f;
					}
				}
				Vector3 a2 = corners[(corner + 3) % 4] - b;
				Vector3 a3 = corners[(corner + 2) % 4] - vector;
				amount = a2 * (1f - num4) + a3 * num4;
				vector2 = a - b;
				num2 = vector2.z - num * vector2.x;
				num3 = num * amount.x - amount.z;
				if (num3 == 0f)
				{
					amount = Vector3.zero;
				}
				else
				{
					float d = num2 / num3;
					a += amount * d;
					amount = a - curPosition;
				}
				return 1;
			}
			return 0;
		}

		private void IntersectHelperSet(Vector3 curPosition, ref Vector3 amount, Vector3 edge0, Vector3 edge1)
		{
			Vector3 a = curPosition + amount;
			if (Vector3.Cross(a - edge0, edge1 - edge0).y > 0f)
			{
				Vector3 vector = curPosition - edge0;
				Vector3 value = edge1 - edge0;
				float num = value.z / value.x;
				if (amount.sqrMagnitude != 0f)
				{
					value = Vector3.Normalize(value) * amount.magnitude;
				}
				amount.x += value.z;
				amount.z += -value.x;
				float num2 = vector.z - num * vector.x;
				float num3 = num * amount.x - amount.z;
				amount *= ((num3 != 0f) ? (num2 / num3) : 0f);
			}
		}

		private void HandleZoom()
		{
			float num = this.pinchLastScreenPosition.x - this.anchorLastScreenPosition.x;
			float num2 = this.pinchLastScreenPosition.y - this.anchorLastScreenPosition.y;
			float num3 = Mathf.Sqrt(num * num + num2 * num2);
			float num4 = this.pinchScreenPosition.x - this.anchorScreenPosition.x;
			float num5 = this.pinchScreenPosition.y - this.anchorScreenPosition.y;
			float num6 = Mathf.Sqrt(num4 * num4 + num5 * num5);
			float zoomAmount = (num6 - num3) * 1200f / (float)Screen.height;
			this.CameraZoom(zoomAmount, true);
		}

		private void CameraZoomToAbsolute(float absoluteValue, bool immediate)
		{
			Vector3 b = (!immediate) ? this.camera.CurrentCameraAnchor : this.camera.CurrentCameraPosition;
			Vector3 a = (!immediate) ? this.camera.CurrentLookatAnchor : this.camera.CurrentLookatPosition;
			Vector3 vector = Vector3.Normalize(a - b);
			float y = b.y;
			float num = absoluteValue * (this.mapHighYEarly - this.mapLowYEarly) + this.mapLowYEarly;
			float zoomAmount = (y - num) / -vector.y;
			this.CameraZoom(zoomAmount, immediate);
		}

		private bool CameraZoom(float zoomAmount, bool immediate)
		{
			Vector3 vector = (!immediate) ? this.camera.CurrentCameraAnchor : this.camera.CurrentCameraPosition;
			Vector3 a = (!immediate) ? this.camera.CurrentLookatAnchor : this.camera.CurrentLookatPosition;
			Vector3 vector2 = vector;
			Vector3 a2 = Vector3.Normalize(a - vector2);
			Vector3 vector3 = vector2 + a2 * zoomAmount;
			float num = 0f;
			if (vector3.y < this.mapLowYSuper)
			{
				num = this.mapLowYSuper - vector3.y;
			}
			else if (vector3.y > this.mapHighYSuper)
			{
				num = this.mapHighYSuper - vector3.y;
			}
			if (num != 0f)
			{
				vector3 -= a2 * num * this.YToHypotenuse;
			}
			if (vector3 != vector2)
			{
				if (immediate)
				{
					this.camera.PositionCamera(vector3);
					this.camera.GetGroundPosition(this.anchorScreenPosition, ref this.anchorGroundPosition);
					this.camera.GetGroundPosition(this.pinchScreenPosition, ref this.pinchGroundPosition);
					this.camera.GetGroundPosition(this.anchorLastScreenPosition, ref this.anchorLastGroundPosition);
					this.camera.GetGroundPosition(this.pinchLastScreenPosition, ref this.pinchLastGroundPosition);
				}
				else
				{
					this.camera.AnchorCamera(vector3);
				}
				Vector3 zero = Vector3.zero;
				this.RestrictCameraWithinSuperSet(ref zero, immediate);
				this.camera.Pan(zero, immediate);
				return zero.sqrMagnitude != 0f;
			}
			return false;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.GameStateChanged)
			{
				if (id != EventId.UserLiftedBuilding)
				{
					if (id != EventId.UserLoweredBuilding)
					{
						if (id == EventId.ScreenSizeChanged)
						{
							Vector2 vector = (Vector2)cookie;
							this.OnScreenSizeChanged((int)vector.x, (int)vector.y);
						}
					}
					else
					{
						this.liftedBuilding = null;
					}
				}
				else
				{
					this.liftedBuilding = (cookie as Entity);
				}
			}
			else if (this.IsCurrentStateANonBaseCameraState())
			{
				this.DisableUserInput();
			}
			else
			{
				this.EnableUserInput();
			}
			return EatResponse.NotEaten;
		}

		public void OnViewPhysicsTime(float dt)
		{
			if (this.liftedBuilding != null)
			{
				this.AutoPanToLiftedBuilding();
			}
			else if (this.camera.IsStillMoving())
			{
				this.EnsureWithinBounds();
			}
		}

		public void ResetCameraPositionImmediatly()
		{
			this.camera.GroundOffset = 0f;
			this.camera.ResetAndStopRotation();
			this.camera.ResetHarness(Vector3.zero);
			this.camera.ResetClipPlanes();
			this.camera.PositionCamera(this.camera.DefCameraLocation);
			this.camera.PositionLookat(this.camera.DefLookatLocation);
			this.ZoomTo(1f, true);
		}

		public void PanToLocation(Vector3 worldLocation)
		{
			this.camera.StopAddonBehaviors();
			Vector3 currentLookatAnchor = this.camera.CurrentLookatAnchor;
			Vector3 amount = worldLocation - currentLookatAnchor;
			amount.y = 0f;
			this.camera.Pan(amount, false);
		}

		public void ZoomIn(bool immediate)
		{
			this.camera.StopAddonBehaviors();
			this.camera.SetCameraFeel(CameraFeel.Medium);
			this.CameraZoom(150f, immediate);
		}

		public void ZoomOut(bool immediate)
		{
			this.camera.StopAddonBehaviors();
			this.camera.SetCameraFeel(CameraFeel.Medium);
			this.CameraZoom(-150f, immediate);
		}

		public void ZoomTo(float amount, bool immediate)
		{
			this.camera.StopAddonBehaviors();
			this.camera.SetCameraFeel(CameraFeel.Medium);
			this.CameraZoomToAbsolute(amount, immediate);
		}

		private void AutoPanToLiftedBuilding()
		{
			if (this.liftedBuilding == null || this.anchorFingerId >= 0)
			{
				return;
			}
			GameObjectViewComponent gameObjectViewComponent = this.liftedBuilding.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent == null)
			{
				return;
			}
			Vector3 position = gameObjectViewComponent.MainTransform.position;
			position.y = 0f;
			if (Mathf.Abs(position.x) + Mathf.Abs(position.z) > 107.100006f)
			{
				return;
			}
			Vector3 vector = this.camera.WorldPositionToScreenPoint(position);
			float num = (float)Screen.width;
			float num2 = (float)Screen.height;
			if (vector.x < 0f)
			{
				vector.x = 0f;
			}
			else if (vector.x > num)
			{
				vector.x = num;
			}
			if (vector.y < 0f)
			{
				vector.y = 0f;
			}
			else if (vector.y > num2)
			{
				vector.y = num2;
			}
			float num3 = (num <= num2) ? num2 : num;
			float num4 = num3 * 0.2f;
			bool flag = false;
			Vector3 vector2 = vector;
			float num5;
			float x;
			if (vector2.x < (num5 = num4))
			{
				vector2.x = num5;
				flag = true;
			}
			else if (vector2.x > (x = num - num5))
			{
				vector2.x = x;
				flag = true;
			}
			float num6;
			float y;
			if (vector2.y < (num6 = num4))
			{
				vector2.y = num6;
				flag = true;
			}
			else if (vector2.y > (y = num2 - num6))
			{
				vector2.y = y;
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			Vector3 zero = Vector3.zero;
			this.camera.GetGroundPosition(vector2, ref zero);
			Vector3 vector3 = Vector3.Normalize(position - zero);
			float num7 = (vector - vector2).sqrMagnitude;
			float num8 = num4 * num4;
			if (num7 > num8)
			{
				num7 = num8;
			}
			float num9 = 2.5f * num7 / num8;
			if (num9 < 0.05f)
			{
				return;
			}
			vector3 *= num9;
			this.RestrictCameraWithinEarlyAdjust(ref vector3);
			this.camera.StopAddonBehaviors();
			this.camera.Pan(vector3, false);
		}

		public void OnScreenSizeChanged(int width, int height)
		{
			object obj = this.screenLock;
			lock (obj)
			{
				this.AbandonAnchor();
				this.AbandonPinch();
				PlanetView view = Service.WorldInitializer.View;
				if (view != null)
				{
					view.ComputeCornerLocators();
				}
				this.screenCorners[0] = new Vector3(0f, 0f, 0f);
				this.screenCorners[1] = new Vector3((float)width, 0f, 0f);
				this.screenCorners[2] = new Vector3(0f, (float)height, 0f);
				this.screenCorners[3] = new Vector3((float)width, (float)height, 0f);
				float magnitude = (this.mapNearLSuper - this.mapFarRSuper).magnitude;
				float num = (float)Screen.height / (float)Screen.width;
				if (num > 1f)
				{
					num = 1f / num;
				}
				this.mapHighYSuper = 1.5f * magnitude * num;
				this.mapLowYEarly = this.mapLowYSuper + 50f;
				this.mapHighYEarly = this.mapHighYSuper;
				this.mapHighYSuperTopDown = this.mapHighYSuper + 200f;
				this.mapHighYEarlyTopDown = this.mapHighYEarly + 200f;
				IState currentState = Service.GameStateMachine.CurrentState;
				if (currentState is EditBaseState || currentState is WarBaseEditorState)
				{
					this.mapHighYSuper = this.mapHighYSuperTopDown;
					this.mapHighYEarly = this.mapHighYEarlyTopDown;
				}
				if (!this.IsNonBaseCameraState(currentState))
				{
					this.ZoomTo(0.7f, false);
					Vector3 zero = Vector3.zero;
					this.PanToLocation(zero);
				}
			}
		}
	}
}
