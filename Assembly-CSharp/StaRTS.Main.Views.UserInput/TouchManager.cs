using System;
using UnityEngine;

namespace StaRTS.Main.Views.UserInput
{
	public class TouchManager : UserInputManager
	{
		private const int MAX_FINGERS = 2;

		public TouchManager() : base(2)
		{
		}

		public override void OnUpdate()
		{
			if (this.inited && this.enabled)
			{
				this.HandleTouchChanges();
			}
		}

		private void HandleTouchChanges()
		{
			int i = 0;
			int num = Input.touches.Length;
			while (i < num)
			{
				Touch touch = Input.touches[i];
				int fingerId = touch.fingerId;
				int num2 = this.FindTouch(fingerId);
				switch (touch.phase)
				{
				case TouchPhase.Began:
					if (num2 < 0)
					{
						num2 = this.FindTouch(-1);
						if (num2 >= 0)
						{
							Vector2 position = touch.position;
							UserInputLayer userInputLayer = UserInputLayer.InternalNone;
							GameObject gameObject = null;
							Vector3 zero = Vector3.zero;
							if (base.Raycast(UserInputLayer.UX, position, ref gameObject, ref zero))
							{
								userInputLayer = UserInputLayer.UX;
							}
							else if (base.Raycast(UserInputLayer.World, position, ref gameObject, ref zero))
							{
								userInputLayer = ((!(gameObject == null)) ? UserInputLayer.World : UserInputLayer.InternalLowest);
							}
							this.fingerIds[num2] = fingerId;
							this.lastIsPressed[num2] = true;
							this.lastScreenPosition[num2] = position;
							this.lastLayer[num2] = userInputLayer;
							base.SetLastGameObject(num2, gameObject);
							UserInputLayer lowestLayer = base.GetLowestLayer(userInputLayer);
							base.SendOnPress(num2, gameObject, position, zero, lowestLayer);
						}
					}
					break;
				case TouchPhase.Moved:
				case TouchPhase.Stationary:
					if (num2 >= 0)
					{
						Vector2 position2 = touch.position;
						GameObject target = null;
						Vector3 zero2 = Vector3.zero;
						base.Raycast(UserInputLayer.InternalLowest, position2, ref target, ref zero2);
						target = base.GetLastGameObject(num2);
						this.lastScreenPosition[num2] = position2;
						UserInputLayer lowestLayer2 = base.GetLowestLayer(this.lastLayer[num2]);
						base.SendOnDrag(num2, target, position2, zero2, lowestLayer2);
					}
					break;
				case TouchPhase.Ended:
				case TouchPhase.Canceled:
					if (num2 >= 0)
					{
						UserInputLayer lowestLayer3 = base.GetLowestLayer(this.lastLayer[num2]);
						base.ResetTouch(num2);
						base.SendOnRelease(num2, lowestLayer3);
					}
					break;
				}
				i++;
			}
		}

		private int FindTouch(int fingerId)
		{
			for (int i = 0; i < 2; i++)
			{
				if (this.fingerIds[i] == fingerId)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
