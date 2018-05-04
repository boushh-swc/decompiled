using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public class MainCamera : WorldCamera, IViewPhysicsTimeObserver
	{
		private const float SPRING_MASS_FAST = 1f;

		private const float SPRING_K_FAST = 180f;

		private const float SPRING_MASS_MEDIUM = 10f;

		private const float SPRING_K_MEDIUM = 120f;

		private const float SPRING_MASS_SLOW = 150f;

		private const float SPRING_K_SLOW = 20f;

		private GameObject camera;

		private DampedSpring physicalCamera;

		private DampedSpring lookatPoint;

		private DampedSpring rotationSpring;

		private float cameraMinY;

		private float cameraMaxY;

		private float cameraMinX;

		private float cameraMaxX;

		private float cameraMinZ;

		private float cameraMaxZ;

		private Vector3 defCameraLocation;

		private Vector3 defLookatLocation;

		public GameObject GameObj;

		private float distanceFromEyeToScreen;

		private bool focusing;

		private Vector3 groundFocusPosition;

		private Vector3 screenFocusPosition;

		private bool isMoving;

		private Vector3 nextHarnessPos;

		private bool dirtyHarness;

		private float defaultNearClipPlane;

		private float defaultFarClipPlane;

		public Vector3 RotateAboutPoint
		{
			get;
			set;
		}

		public Vector3 DefCameraLocation
		{
			get
			{
				return this.defCameraLocation;
			}
		}

		public Vector3 DefLookatLocation
		{
			get
			{
				return this.defLookatLocation;
			}
		}

		public DampedSpring RotationSpring
		{
			get
			{
				return this.rotationSpring;
			}
		}

		public GameObject MainPosCameraHarness
		{
			get;
			private set;
		}

		public GameObject MainRotCameraHarness
		{
			get;
			private set;
		}

		public Vector3 CurrentCameraAnchor
		{
			get
			{
				return this.physicalCamera.Anchor;
			}
		}

		public Vector3 CurrentLookatAnchor
		{
			get
			{
				return this.lookatPoint.Anchor;
			}
		}

		public Vector3 CurrentCameraPosition
		{
			get
			{
				return this.physicalCamera.Position;
			}
		}

		public Vector3 CurrentLookatPosition
		{
			get
			{
				return this.lookatPoint.Position;
			}
		}

		public Vector3 CurrentCameraShakeOffset
		{
			get;
			set;
		}

		public MainCamera()
		{
			this.unityCamera = Camera.main;
			this.camera = ((!(this.unityCamera == null)) ? this.unityCamera.gameObject : null);
			if (this.camera == null)
			{
				throw new Exception("Unable to find the Main Camera");
			}
			this.GameObj = this.camera;
			this.unityCamera.eventMask = 0;
			this.defaultNearClipPlane = this.unityCamera.nearClipPlane;
			this.defaultFarClipPlane = this.unityCamera.farClipPlane;
			this.physicalCamera = new DampedSpring(1f, 180f);
			this.lookatPoint = new DampedSpring(1f, 180f);
			this.rotationSpring = new DampedSpring(10f, 120f);
			base.GroundOffset = 0f;
			this.SetupDefaults(-1E+10f, 1E+10f, -1E+10f, 1E+10f, -1E+10f, 1E+10f, Vector3.zero, Vector3.forward);
			this.PositionCamera(this.camera.transform.localPosition);
			this.PositionLookat(Vector3.zero);
			this.OnScreenSizeChange();
			this.isMoving = false;
			this.focusing = false;
			this.groundFocusPosition = Vector3.zero;
			this.screenFocusPosition = Vector3.zero;
			this.MainRotCameraHarness = new GameObject();
			this.MainRotCameraHarness.name = "Rotate Camera Harness";
			this.MainPosCameraHarness = GameObject.Find("Main Camera Harness");
			this.MainPosCameraHarness.transform.parent = this.MainRotCameraHarness.transform;
			this.MainPosCameraHarness.transform.position = Vector3.zero;
			this.nextHarnessPos = Vector3.zero;
			this.dirtyHarness = false;
			Service.ViewTimeEngine.RegisterPhysicsTimeObserver(this);
		}

		public MainCamera(string cameraName)
		{
			this.unityCamera = Camera.main;
			this.camera = ((!(this.unityCamera == null)) ? this.unityCamera.gameObject : null);
			if (this.camera == null)
			{
				throw new Exception("Unable to find the Main Camera");
			}
			this.GameObj = this.camera;
			this.GameObj.name = cameraName;
			this.unityCamera.eventMask = 0;
			this.defaultNearClipPlane = this.unityCamera.nearClipPlane;
			this.defaultFarClipPlane = this.unityCamera.farClipPlane;
			this.physicalCamera = new DampedSpring(1f, 180f);
			this.lookatPoint = new DampedSpring(1f, 180f);
			this.rotationSpring = new DampedSpring(10f, 120f);
			base.GroundOffset = 0f;
			this.SetupDefaults(-1E+10f, 1E+10f, -1E+10f, 1E+10f, -1E+10f, 1E+10f, Vector3.zero, Vector3.forward);
			this.PositionCamera(this.camera.transform.localPosition);
			this.PositionLookat(Vector3.zero);
			this.OnScreenSizeChange();
			this.isMoving = false;
			this.focusing = false;
			this.groundFocusPosition = Vector3.zero;
			this.screenFocusPosition = Vector3.zero;
			Service.ViewTimeEngine.RegisterPhysicsTimeObserver(this);
		}

		public void UpdateHarnessPosition(Vector3 harnessPos)
		{
			this.dirtyHarness = true;
			this.nextHarnessPos = harnessPos;
		}

		public void SetHarnessPosition(Vector3 harnessPos)
		{
			this.dirtyHarness = false;
			this.MainPosCameraHarness.transform.localPosition = harnessPos;
		}

		public void SetLookAtPositionImmediately(Vector3 lookAt, Quaternion offset)
		{
			this.camera.transform.LookAt(lookAt);
			this.camera.transform.rotation = offset * this.camera.transform.rotation;
		}

		public void ResetHarness(Vector3 lookAt)
		{
			this.MainRotCameraHarness.transform.position = Vector3.zero;
			this.MainRotCameraHarness.transform.rotation = Quaternion.identity;
			this.UpdateHarnessPosition(Vector3.zero);
			this.MainPosCameraHarness.transform.rotation = Quaternion.identity;
			this.MainPosCameraHarness.transform.position = Vector3.zero;
			this.PositionLookat(lookAt);
		}

		public void ForceCameraMoveFinish()
		{
			this.physicalCamera.StopMoving();
			this.lookatPoint.StopMoving();
			this.camera.transform.localPosition = this.physicalCamera.Position;
			this.camera.transform.LookAt(this.lookatPoint.Position);
		}

		public void ResetAndStopRotation()
		{
			this.rotationSpring.Anchor = Vector3.zero;
			this.rotationSpring.Position = Vector3.zero;
			this.rotationSpring.StopMoving();
		}

		public void SetRotationFeel(CameraFeel feel)
		{
			float m;
			float k;
			if (feel != CameraFeel.Slow)
			{
				if (feel != CameraFeel.Medium)
				{
					m = 1f;
					k = 180f;
				}
				else
				{
					m = 10f;
					k = 120f;
				}
			}
			else
			{
				m = 150f;
				k = 20f;
			}
			this.rotationSpring.SetSpring(m, k);
		}

		public void SetRotationHarnessPosition(Vector3 pos)
		{
			this.MainRotCameraHarness.transform.position = pos;
		}

		public void UpdateRotationAnchor(float rotation)
		{
			this.rotationSpring.Anchor = new Vector3(this.rotationSpring.Anchor.x + rotation, 0f, 0f);
		}

		public void UpdateRotationImmediatelyTo(float rotation)
		{
			this.MainRotCameraHarness.transform.RotateAround(this.RotateAboutPoint, Vector3.up, rotation - this.rotationSpring.Position.x);
			Vector3 position = new Vector3(rotation, this.rotationSpring.Position.y, 0f);
			this.rotationSpring.Position = position;
			this.rotationSpring.StopMoving();
		}

		public void UpdateRotationImmediatelyBy(float rotation)
		{
			this.MainRotCameraHarness.transform.RotateAround(this.RotateAboutPoint, Vector3.up, rotation);
			Vector3 position = new Vector3(rotation + this.rotationSpring.Position.x, this.rotationSpring.Position.y, 0f);
			this.rotationSpring.Position = position;
			this.rotationSpring.StopMoving();
		}

		public void SetupDefaults(float cameraMinY, float cameraMaxY, float cameraMinX, float cameraMaxX, float cameraMinZ, float cameraMaxZ, Vector3 defCameraLocation, Vector3 defLookatLocation)
		{
			this.cameraMinY = cameraMinY;
			this.cameraMaxY = cameraMaxY;
			this.cameraMinX = cameraMinX;
			this.cameraMaxX = cameraMaxX;
			this.cameraMinZ = cameraMinZ;
			this.cameraMaxZ = cameraMaxZ;
			this.defCameraLocation = defCameraLocation;
			this.defLookatLocation = defLookatLocation;
		}

		public void SetCameraFeel(CameraFeel feel)
		{
			float m;
			float k;
			if (feel != CameraFeel.Slow)
			{
				if (feel != CameraFeel.Medium)
				{
					m = 1f;
					k = 180f;
				}
				else
				{
					m = 10f;
					k = 120f;
				}
			}
			else
			{
				m = 150f;
				k = 20f;
			}
			this.physicalCamera.SetSpring(m, k);
			this.lookatPoint.SetSpring(m, k);
		}

		public void OnScreenSizeChange()
		{
			this.distanceFromEyeToScreen = CameraUtils.CalculateDistanceFromEyeToScreen(this.unityCamera);
		}

		private void CheckBounds()
		{
			Vector3 position = this.physicalCamera.Position;
			if (this.cameraMinY <= position.y && position.y <= this.cameraMaxY && this.cameraMinX <= position.x && position.x <= this.cameraMaxX && this.cameraMinZ <= position.z && position.z <= this.cameraMaxZ)
			{
				return;
			}
			Service.Logger.Warn("Camera reached ultra bounds: " + position);
			this.PositionCamera(this.defCameraLocation);
			this.PositionLookat(this.defLookatLocation);
			this.StopMoving();
		}

		public void AnchorCamera(Vector3 point)
		{
			this.physicalCamera.Anchor = point;
		}

		public void AnchorLookat(Vector3 point)
		{
			this.lookatPoint.Anchor = point;
		}

		public void PositionCamera(Vector3 point)
		{
			this.StopAddonBehaviors();
			this.physicalCamera.Anchor = point;
			this.physicalCamera.Position = point;
		}

		public void PositionLookat(Vector3 point)
		{
			this.StopAddonBehaviors();
			this.lookatPoint.Anchor = point;
			this.lookatPoint.Position = point;
		}

		public void GetLookatBoardCell(out int cx, out int cz)
		{
			Vector3 anchor = this.lookatPoint.Anchor;
			float x = anchor.x;
			float z = anchor.z;
			Units.SnapWorldToGridX(ref x);
			Units.SnapWorldToGridZ(ref z);
			cx = Units.WorldToBoardX(x);
			cz = Units.WorldToBoardZ(z);
		}

		public void KeepFocus(Vector3 focus)
		{
			Vector3 vector = base.WorldPositionToScreenPoint(focus);
			if (vector.x < 0f || vector.x >= (float)Screen.width || vector.y < 0f || vector.y >= (float)Screen.height)
			{
				return;
			}
			this.focusing = true;
			this.groundFocusPosition = focus;
			this.groundFocusPosition.y = 0f;
			this.screenFocusPosition = vector;
			Vector3 b = this.ScreenPointToGroundAnchor(this.screenFocusPosition);
			Vector3 b2 = this.groundFocusPosition - b;
			this.physicalCamera.Anchor += b2;
			this.lookatPoint.Anchor += b2;
		}

		private void AdjustFocus()
		{
			Vector3 zero = Vector3.zero;
			this.GetGroundPosition(this.screenFocusPosition, ref zero);
			Vector3 b = this.groundFocusPosition - zero;
			this.physicalCamera.Position += b;
			this.lookatPoint.Position += b;
		}

		public void SetFov(float fov)
		{
			if (this.unityCamera.fieldOfView != fov)
			{
				this.unityCamera.fieldOfView = fov;
				this.OnScreenSizeChange();
			}
		}

		public void ResetClipPlanes()
		{
			this.SetClipPlanes(this.defaultNearClipPlane, this.defaultFarClipPlane);
		}

		public void SetClipPlanes(float near, float far)
		{
			this.unityCamera.nearClipPlane = near;
			this.unityCamera.farClipPlane = far;
		}

		public void StopAddonBehaviors()
		{
			if (this.focusing)
			{
				this.groundFocusPosition = Vector3.zero;
				this.screenFocusPosition = Vector3.zero;
				this.focusing = false;
			}
		}

		public void StopMoving()
		{
			this.StopAddonBehaviors();
			this.physicalCamera.StopMoving();
			this.lookatPoint.StopMoving();
		}

		public void Pan(Vector3 amount, bool immediate)
		{
			this.physicalCamera.Anchor = this.physicalCamera.Anchor + amount;
			this.lookatPoint.Anchor = this.lookatPoint.Anchor + amount;
			if (immediate)
			{
				this.StopAddonBehaviors();
				this.physicalCamera.Position = this.physicalCamera.Anchor;
				this.lookatPoint.Position = this.lookatPoint.Anchor;
			}
		}

		public bool IsStillMoving()
		{
			return this.physicalCamera.IsStillMoving() || this.lookatPoint.IsStillMoving();
		}

		public bool IsStillRotating()
		{
			return this.rotationSpring.IsStillMoving();
		}

		public void OnViewPhysicsTime(float dt)
		{
			bool flag = this.isMoving;
			bool flag2 = this.IsStillRotating();
			this.isMoving = this.IsStillMoving();
			if (!this.isMoving && !flag2 && !this.dirtyHarness)
			{
				if (flag && Service.EventManager != null)
				{
					Service.EventManager.SendEvent(EventId.CameraFinishedMoving, null);
				}
				if (this.focusing)
				{
					this.StopAddonBehaviors();
				}
			}
			else if (flag2)
			{
				this.rotationSpring.Move(dt);
				this.MainRotCameraHarness.transform.RotateAround(this.RotateAboutPoint, Vector3.up, this.rotationSpring.Velocity.x * dt);
			}
			else
			{
				this.physicalCamera.Move(dt);
				this.lookatPoint.Move(dt);
				this.CheckBounds();
				Vector3 vector = this.camera.transform.localPosition - this.physicalCamera.Position;
				this.camera.transform.localPosition = this.physicalCamera.Position;
				this.camera.transform.LookAt(this.lookatPoint.Position);
				if (this.focusing)
				{
					if (vector.sqrMagnitude <= 1E-08f)
					{
						this.StopAddonBehaviors();
					}
					else
					{
						this.AdjustFocus();
					}
				}
			}
			if (this.dirtyHarness)
			{
				this.dirtyHarness = false;
				this.MainPosCameraHarness.transform.localPosition = this.nextHarnessPos;
			}
		}

		private Vector3 ScreenPointToGroundAnchor(Vector3 screenPoint)
		{
			Transform transform = this.camera.transform;
			Vector3 localPosition = transform.localPosition;
			Quaternion rotation = transform.rotation;
			transform.localPosition = this.physicalCamera.Anchor;
			transform.LookAt(this.lookatPoint.Anchor);
			Vector3 zero = Vector3.zero;
			Vector3 anchor = this.physicalCamera.Anchor;
			CameraUtils.GetGroundPositionHelper(this.unityCamera, screenPoint, anchor, this.distanceFromEyeToScreen, base.GroundOffset, ref zero);
			transform.localPosition = localPosition;
			transform.rotation = rotation;
			return zero;
		}

		public override bool GetGroundPosition(Vector3 screenPosition, ref Vector3 groundPosition)
		{
			Vector3 rayOrigin = (base.GroundOffset != 0f) ? this.unityCamera.transform.position : this.physicalCamera.Position;
			return CameraUtils.GetGroundPositionHelper(this.unityCamera, screenPosition, rayOrigin, this.distanceFromEyeToScreen, base.GroundOffset, ref groundPosition);
		}

		private float DistanceToOrigin(Vector3 cameraEye)
		{
			Vector3 vector = cameraEye;
			Vector3 vector2 = Vector3.Normalize(Vector3.zero - cameraEye);
			return -vector.y / vector2.y;
		}
	}
}
