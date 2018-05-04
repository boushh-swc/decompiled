using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class GraphRequest : MenuBase
	{
		private string apiQuery = string.Empty;

		private Texture2D profilePic;

		protected override void GetGui()
		{
			bool enabled = GUI.enabled;
			GUI.enabled = (enabled && FB.IsLoggedIn);
			if (base.Button("Basic Request - Me"))
			{
				FB.API("/me", HttpMethod.GET, new FacebookDelegate<IGraphResult>(base.HandleResult), null);
			}
			if (base.Button("Retrieve Profile Photo"))
			{
				FB.API("/me/picture", HttpMethod.GET, new FacebookDelegate<IGraphResult>(this.ProfilePhotoCallback), null);
			}
			if (base.Button("Take and Upload screenshot"))
			{
				base.StartCoroutine(this.TakeScreenshot());
			}
			base.LabelAndTextField("Request", ref this.apiQuery);
			if (base.Button("Custom Request"))
			{
				FB.API(this.apiQuery, HttpMethod.GET, new FacebookDelegate<IGraphResult>(base.HandleResult), null);
			}
			if (this.profilePic != null)
			{
				GUILayout.Box(this.profilePic, new GUILayoutOption[0]);
			}
			GUI.enabled = enabled;
		}

		private void ProfilePhotoCallback(IGraphResult result)
		{
			if (string.IsNullOrEmpty(result.Error) && result.Texture != null)
			{
				this.profilePic = result.Texture;
			}
			base.HandleResult(result);
		}

		[DebuggerHidden]
		private IEnumerator TakeScreenshot()
		{
			yield return new WaitForEndOfFrame();
			int width = Screen.width;
			int height = Screen.height;
			Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
			texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
			texture2D.Apply();
			byte[] contents = texture2D.EncodeToPNG();
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddBinaryData("image", contents, "InteractiveConsole.png");
			wWWForm.AddField("message", "herp derp.  I did a thing!  Did I do this right?");
			FB.API("me/photos", HttpMethod.POST, new FacebookDelegate<IGraphResult>(base.HandleResult), wWWForm);
			yield break;
		}
	}
}
