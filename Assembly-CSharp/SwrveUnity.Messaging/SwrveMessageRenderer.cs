using System;
using UnityEngine;

namespace SwrveUnity.Messaging
{
	public class SwrveMessageRenderer
	{
		protected static readonly Color ButtonPressedColor = new Color(0.5f, 0.5f, 0.5f);

		protected static Texture2D blankTexture;

		protected static Rect WholeScreen = default(Rect);

		public static ISwrveMessageAnimator Animator;

		protected static Texture2D GetBlankTexture()
		{
			if (SwrveMessageRenderer.blankTexture == null)
			{
				SwrveMessageRenderer.blankTexture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
				SwrveMessageRenderer.blankTexture.SetPixel(0, 0, Color.white);
				SwrveMessageRenderer.blankTexture.SetPixel(1, 0, Color.white);
				SwrveMessageRenderer.blankTexture.SetPixel(0, 1, Color.white);
				SwrveMessageRenderer.blankTexture.SetPixel(1, 1, Color.white);
				SwrveMessageRenderer.blankTexture.Apply(false, true);
			}
			return SwrveMessageRenderer.blankTexture;
		}

		public static void InitMessage(SwrveMessageFormat format, SwrveOrientation deviceOrientation)
		{
			if (SwrveMessageRenderer.Animator != null)
			{
				SwrveMessageRenderer.Animator.InitMessage(format);
			}
			else
			{
				format.Init(deviceOrientation);
				format.InitAnimation(new Point(0, 0), new Point(0, 0));
			}
		}

		public static void AnimateMessage(SwrveMessageFormat format)
		{
			if (SwrveMessageRenderer.Animator != null)
			{
				SwrveMessageRenderer.Animator.AnimateMessage(format);
			}
		}

		public static void DrawMessage(SwrveMessageFormat format, int centerx, int centery)
		{
			if (SwrveMessageRenderer.Animator != null)
			{
				SwrveMessageRenderer.AnimateMessage(format);
			}
			if (format.BackgroundColor.HasValue && SwrveMessageRenderer.GetBlankTexture() != null)
			{
				Color value = format.BackgroundColor.Value;
				value.a *= format.Message.BackgroundAlpha;
				GUI.color = value;
				SwrveMessageRenderer.WholeScreen.width = (float)Screen.width;
				SwrveMessageRenderer.WholeScreen.height = (float)Screen.height;
				GUI.DrawTexture(SwrveMessageRenderer.WholeScreen, SwrveMessageRenderer.blankTexture, ScaleMode.StretchToFill, true, 10f);
				GUI.color = Color.white;
			}
			bool rotate = format.Rotate;
			if (rotate)
			{
				Vector2 pivotPoint = new Vector2((float)centerx, (float)centery);
				GUIUtility.RotateAroundPivot(90f, pivotPoint);
			}
			float num = format.Scale * format.Message.AnimationScale;
			GUI.color = Color.white;
			for (int i = 0; i < format.Images.Count; i++)
			{
				SwrveImage swrveImage = format.Images[i];
				if (swrveImage.Texture != null)
				{
					float num2 = num * swrveImage.AnimationScale;
					Point centeredPosition = swrveImage.GetCenteredPosition((float)swrveImage.Texture.width, (float)swrveImage.Texture.height, num2, num);
					centeredPosition.X += centerx;
					centeredPosition.Y += centery;
					swrveImage.Rect.x = (float)centeredPosition.X;
					swrveImage.Rect.y = (float)centeredPosition.Y;
					swrveImage.Rect.width = (float)swrveImage.Texture.width * num2;
					swrveImage.Rect.height = (float)swrveImage.Texture.height * num2;
					GUI.DrawTexture(swrveImage.Rect, swrveImage.Texture, ScaleMode.StretchToFill, true, 10f);
				}
				else
				{
					GUI.Box(swrveImage.Rect, swrveImage.File);
				}
			}
			for (int j = 0; j < format.Buttons.Count; j++)
			{
				SwrveButton swrveButton = format.Buttons[j];
				if (swrveButton.Texture != null)
				{
					float num3 = num * swrveButton.AnimationScale;
					Point centeredPosition2 = swrveButton.GetCenteredPosition((float)swrveButton.Texture.width, (float)swrveButton.Texture.height, num3, num);
					swrveButton.Rect.x = (float)(centeredPosition2.X + centerx);
					swrveButton.Rect.y = (float)(centeredPosition2.Y + centery);
					swrveButton.Rect.width = (float)swrveButton.Texture.width * num3;
					swrveButton.Rect.height = (float)swrveButton.Texture.height * num3;
					if (rotate)
					{
						Point center = swrveButton.GetCenter((float)swrveButton.Texture.width, (float)swrveButton.Texture.height, num3);
						swrveButton.PointerRect.x = (float)centerx - (float)swrveButton.Position.Y * num + (float)center.Y;
						swrveButton.PointerRect.y = (float)centery + (float)swrveButton.Position.X * num + (float)center.X;
						swrveButton.PointerRect.width = swrveButton.Rect.height;
						swrveButton.PointerRect.height = swrveButton.Rect.width;
					}
					else
					{
						swrveButton.PointerRect = swrveButton.Rect;
					}
					if (SwrveMessageRenderer.Animator != null)
					{
						SwrveMessageRenderer.Animator.AnimateButtonPressed(swrveButton);
					}
					else
					{
						GUI.color = ((!swrveButton.Pressed) ? Color.white : SwrveMessageRenderer.ButtonPressedColor);
					}
					GUI.DrawTexture(swrveButton.Rect, swrveButton.Texture, ScaleMode.StretchToFill, true, 10f);
				}
				else
				{
					GUI.Box(swrveButton.Rect, swrveButton.Image);
				}
				GUI.color = Color.white;
			}
			if ((SwrveMessageRenderer.Animator == null && format.Closing) || (SwrveMessageRenderer.Animator != null && SwrveMessageRenderer.Animator.IsMessageDismissed(format)))
			{
				format.Dismissed = true;
				format.UnloadAssets();
			}
		}
	}
}
