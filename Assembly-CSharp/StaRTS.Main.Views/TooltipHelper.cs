using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views
{
	public class TooltipHelper
	{
		private const float BASE_HEIGHT_OFFSET = 1f;

		private const float SCALE_FACTOR = 300f;

		private UXElement tooltipElement;

		private GameObjectViewComponent attachmentView;

		private float heightOffGround;

		private uint fadeTimerId;

		private Action onFadeComplete;

		private Vector3 lastViewLocation;

		private Vector3 lastCameraLocation;

		private bool enabled;

		public int Slot
		{
			get;
			set;
		}

		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
				this.ResetLast();
				if (this.tooltipElement != null)
				{
					this.tooltipElement.Visible = this.enabled;
					if (this.enabled)
					{
						this.UpdateLocation(false);
					}
				}
			}
		}

		public TooltipHelper()
		{
			this.enabled = true;
			this.SetTooltipElement(null);
			this.heightOffGround = 0f;
			this.fadeTimerId = 0u;
			this.Slot = -1;
		}

		private void SetTooltipElement(UXElement tooltip)
		{
			this.tooltipElement = tooltip;
			this.ResetLast();
		}

		private void ResetLast()
		{
			this.lastViewLocation = Vector3.zero;
			this.lastCameraLocation = Vector3.zero;
		}

		public bool SetupElements(GameObjectViewComponent viewComp, UXElement element, float extraHeightOffset, bool addHeight, bool checkOverlap)
		{
			this.attachmentView = viewComp;
			this.SetTooltipElement(element);
			this.tooltipElement.WidgetDepth = -1;
			this.heightOffGround = 1f + this.attachmentView.TooltipHeightOffset + extraHeightOffset;
			if (addHeight)
			{
				this.heightOffGround += viewComp.GameObjectHeight;
			}
			return this.UpdateLocation(checkOverlap);
		}

		public bool UpdateLocation(bool checkOverlap)
		{
			return this.tooltipElement == null || !this.enabled || this.UpdateLocation(0f, checkOverlap);
		}

		public static bool WouldOverlapAnotherTooltip(GameObjectViewComponent view)
		{
			if (view.MainTransform == null)
			{
				return true;
			}
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			Vector3 position = view.MainTransform.position;
			position.y = 1f + view.TooltipHeightOffset + view.GameObjectHeight;
			Vector3 vector = mainCamera.WorldPositionToScreenPoint(position);
			return vector.z <= 0f || Service.BuildingTooltipController.UpdateTooltipScreenSlot(null, vector.x, vector.y);
		}

		public bool UpdateLocation(float extraHeightOffGround, bool checkOverlap)
		{
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			Vector3 vector = mainCamera.CurrentCameraPosition - mainCamera.CurrentCameraShakeOffset;
			Vector3 vector2 = (!(this.attachmentView.MainTransform == null)) ? this.attachmentView.MainTransform.position : this.lastViewLocation;
			bool flag = vector != this.lastCameraLocation;
			bool flag2 = vector2 != this.lastViewLocation;
			if (extraHeightOffGround == 0f && !flag && !flag2)
			{
				return false;
			}
			this.lastCameraLocation = vector;
			this.lastViewLocation = vector2;
			vector2.y = this.heightOffGround + extraHeightOffGround;
			Vector3 vector3 = mainCamera.WorldPositionToScreenPoint(vector2);
			if (checkOverlap && Service.BuildingTooltipController.UpdateTooltipScreenSlot(this, vector3.x, vector3.y))
			{
				return true;
			}
			bool flag3 = vector3.z > 0f;
			this.tooltipElement.Visible = flag3;
			if (flag3)
			{
				this.tooltipElement.LocalPosition = new Vector3(vector3.x, vector3.y, 0f);
				if (flag)
				{
					float magnitude = (vector2 - vector).magnitude;
					if (magnitude > 0f)
					{
						float d = 300f / magnitude;
						this.tooltipElement.Root.transform.localScale = Vector3.one * d;
					}
				}
			}
			return false;
		}

		public virtual void TeardownElements(bool clearOverlap)
		{
			if (clearOverlap)
			{
				Service.BuildingTooltipController.ClearTooltipScreenSlot(this);
			}
			this.KillFadeTimer();
			this.SetTooltipElement(null);
		}

		public void GoAwayIn(float seconds, Action onFadeComplete)
		{
			if (seconds <= 0f)
			{
				if (onFadeComplete != null)
				{
					onFadeComplete();
				}
			}
			else
			{
				this.KillFadeTimer();
				this.fadeTimerId = Service.ViewTimerManager.CreateViewTimer(seconds, false, new TimerDelegate(this.OnFadeTimer), onFadeComplete);
			}
		}

		public bool HasFadeTimer()
		{
			return this.fadeTimerId != 0u;
		}

		private void KillFadeTimer()
		{
			if (this.HasFadeTimer())
			{
				Service.ViewTimerManager.KillViewTimer(this.fadeTimerId);
				this.fadeTimerId = 0u;
			}
		}

		private void OnFadeTimer(uint id, object cookie)
		{
			if (id == this.fadeTimerId)
			{
				this.fadeTimerId = 0u;
				Action action = (Action)cookie;
				if (action != null)
				{
					action();
				}
			}
		}
	}
}
