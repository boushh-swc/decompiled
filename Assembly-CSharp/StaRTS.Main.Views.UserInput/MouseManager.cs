using System;
using UnityEngine;

namespace StaRTS.Main.Views.UserInput
{
	public class MouseManager : UserInputManager
	{
		private const int MAX_FINGERS = 1;

		private const float SCROLL_THRESHOLD = 0.03f;

		private const float ZOOM_FACTOR = 4f;

		private float curScroll;

		public MouseManager() : base(1)
		{
		}

		public override void OnUpdate()
		{
			if (this.inited && this.enabled)
			{
				this.HandleMouseChangesPrimaryButton();
				this.HandleMouseChangesSecondaryButton();
				this.HandleScrollWheel();
			}
		}

		private void HandleMouseChangesPrimaryButton()
		{
			bool mouseButton = Input.GetMouseButton(0);
			Vector3 mousePosition = Input.mousePosition;
			Vector2 vector = new Vector2(mousePosition.x, mousePosition.y);
			UserInputLayer userInputLayer = UserInputLayer.InternalNone;
			GameObject gameObject = null;
			Vector3 zero = Vector3.zero;
			bool flag = this.lastIsPressed[0] != mouseButton;
			bool flag2 = this.lastScreenPosition[0] != vector;
			if (mouseButton || flag2)
			{
				if (mouseButton && !flag)
				{
					if (this.lastLayer[0] != UserInputLayer.UX)
					{
						base.Raycast(UserInputLayer.World, vector, ref gameObject, ref zero);
					}
					userInputLayer = this.lastLayer[0];
					gameObject = base.GetLastGameObject(0);
				}
				else if (base.Raycast(UserInputLayer.UX, vector, ref gameObject, ref zero))
				{
					userInputLayer = UserInputLayer.UX;
				}
				else if (base.Raycast(UserInputLayer.World, vector, ref gameObject, ref zero))
				{
					userInputLayer = ((!(gameObject == null)) ? UserInputLayer.World : UserInputLayer.InternalLowest);
				}
			}
			else
			{
				userInputLayer = this.lastLayer[0];
			}
			this.lastIsPressed[0] = mouseButton;
			this.lastScreenPosition[0] = vector;
			this.lastLayer[0] = userInputLayer;
			base.SetLastGameObject(0, gameObject);
			UserInputLayer lowestLayer = base.GetLowestLayer(userInputLayer);
			if (flag)
			{
				if (mouseButton)
				{
					base.SendOnPress(0, gameObject, vector, zero, lowestLayer);
				}
				else
				{
					base.SendOnRelease(0, lowestLayer);
				}
			}
			if (mouseButton)
			{
				base.SendOnDrag(0, gameObject, vector, zero, lowestLayer);
			}
		}

		private void HandleMouseChangesSecondaryButton()
		{
			if (!Input.GetMouseButton(1))
			{
				this.isPressed2ndMouseBut = false;
				this.lastScreenPos2ndMouseBut = Vector2.zero;
				return;
			}
			Vector3 mousePosition = Input.mousePosition;
			Vector2 vector = new Vector2(mousePosition.x, mousePosition.y);
			if (!this.isPressed2ndMouseBut)
			{
				this.isPressed2ndMouseBut = true;
				this.lastScreenPos2ndMouseBut = vector;
				return;
			}
			float num = (vector.y - this.lastScreenPos2ndMouseBut.y) / (float)Screen.height;
			this.lastScreenPos2ndMouseBut = vector;
			if (num == 0f)
			{
				return;
			}
			base.SendOnScroll(num * 4f, vector, UserInputLayer.InternalLowest);
		}

		private void HandleScrollWheel()
		{
			if (!this.lastIsPressed[0] && this.lastLayer[0] != UserInputLayer.UX && this.lastScreenPosition[0].x >= 0f && this.lastScreenPosition[0].x < (float)Screen.width && this.lastScreenPosition[0].y >= 0f && this.lastScreenPosition[0].y < (float)Screen.height)
			{
				float axis = Input.GetAxis("Mouse ScrollWheel");
				if (axis != 0f)
				{
					float num = (float)((axis <= 0f) ? -1 : 1);
					this.curScroll += axis;
					if (num * this.curScroll >= 0.03f)
					{
						this.curScroll = 0f;
						base.SendOnScroll(axis, this.lastScreenPosition[0], UserInputLayer.InternalLowest);
					}
				}
			}
		}
	}
}
