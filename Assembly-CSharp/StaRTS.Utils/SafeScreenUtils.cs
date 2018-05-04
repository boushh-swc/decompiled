using System;
using UnityEngine;

namespace StaRTS.Utils
{
	public static class SafeScreenUtils
	{
		private const string WIDGET_SAFE_AREA = "WidgetSafeArea";

		private const int IPX_OFFSET_L = 53;

		private const int IPX_OFFSET_T = 0;

		private const int IPX_OFFSET_R = -53;

		private const int IPX_OFFSET_B = 0;

		private static Rect invalidRect = new Rect(1f, 1f, 1f, 1f);

		private static float forceXOffset = 0f;

		private static float forceYOffset = 0f;

		private static float forceWidthOffset = 0f;

		private static float forceHeightOffset = 0f;

		private static bool useFixedAnchors = true;

		private static Rect safeRect = new Rect(1f, 1f, 1f, 1f);

		public static void PrepAssetForScreenSize(object asset)
		{
			GameObject gameObject = asset as GameObject;
			if (gameObject == null)
			{
				return;
			}
			GameObject gameObject2 = UnityUtils.FindGameObject(gameObject, "WidgetSafeArea");
			if (gameObject2 == null)
			{
				return;
			}
			UIWidget component = gameObject2.GetComponent<UIWidget>();
			Rect safeArea = SafeScreenUtils.GetSafeArea();
			if (safeArea.width < (float)Screen.width || safeArea.height < (float)Screen.height || safeArea.x > 0f || safeArea.y > 0f)
			{
				if (SafeScreenUtils.useFixedAnchors)
				{
					component.leftAnchor.absolute = 53;
					component.topAnchor.absolute = 0;
					component.rightAnchor.absolute = -53;
					component.bottomAnchor.absolute = 0;
				}
				else
				{
					component.leftAnchor.absolute = (int)safeArea.x;
					component.topAnchor.absolute = (int)safeArea.y;
					component.rightAnchor.absolute = (int)(safeArea.width - (float)Screen.width);
					component.bottomAnchor.absolute = (int)(safeArea.height - (float)Screen.height);
				}
			}
		}

		private static Rect GetSafeArea()
		{
			if (SafeScreenUtils.safeRect != SafeScreenUtils.invalidRect)
			{
				return SafeScreenUtils.safeRect;
			}
			float num = 0f;
			float num2 = 0f;
			float num3 = (float)Screen.width;
			float num4 = (float)Screen.height;
			SafeScreenUtils.safeRect = new Rect(num + SafeScreenUtils.forceXOffset, num2 + SafeScreenUtils.forceYOffset, num3 + SafeScreenUtils.forceWidthOffset, num4 + SafeScreenUtils.forceHeightOffset);
			return SafeScreenUtils.safeRect;
		}

		public static string GetSafeRectString()
		{
			Rect safeArea = SafeScreenUtils.GetSafeArea();
			return string.Format("x:{0} y:{1} w:{2} h:{3}", new object[]
			{
				safeArea.x.ToString("F1"),
				safeArea.y.ToString("F1"),
				safeArea.width.ToString("F1"),
				safeArea.height.ToString("F1")
			});
		}

		public static string GetScreenDimensionsString()
		{
			Rect rect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
			return string.Format("x:{0} y:{1} w:{2} h:{3}", new object[]
			{
				rect.x.ToString("F1"),
				rect.y.ToString("F1"),
				rect.width.ToString("F1"),
				rect.height.ToString("F1")
			});
		}

		public static string ResetRect()
		{
			SafeScreenUtils.safeRect = SafeScreenUtils.invalidRect;
			SafeScreenUtils.forceXOffset = 0f;
			SafeScreenUtils.forceYOffset = 0f;
			SafeScreenUtils.forceWidthOffset = 0f;
			SafeScreenUtils.forceHeightOffset = 0f;
			return SafeScreenUtils.GetSafeRectString();
		}

		public static string SetOffset(string key, float newValue)
		{
			SafeScreenUtils.safeRect = SafeScreenUtils.invalidRect;
			key = key.ToLower();
			if (key != null)
			{
				if (!(key == "x"))
				{
					if (!(key == "y"))
					{
						if (!(key == "w"))
						{
							if (key == "h")
							{
								SafeScreenUtils.forceHeightOffset = newValue - (float)Screen.height;
							}
						}
						else
						{
							SafeScreenUtils.forceWidthOffset = newValue - (float)Screen.width;
						}
					}
					else
					{
						SafeScreenUtils.forceYOffset = newValue;
					}
				}
				else
				{
					SafeScreenUtils.forceXOffset = newValue;
				}
			}
			return SafeScreenUtils.GetSafeRectString();
		}

		public static bool ToggleUseFixedAnchors()
		{
			SafeScreenUtils.useFixedAnchors = !SafeScreenUtils.useFixedAnchors;
			return SafeScreenUtils.useFixedAnchors;
		}
	}
}
