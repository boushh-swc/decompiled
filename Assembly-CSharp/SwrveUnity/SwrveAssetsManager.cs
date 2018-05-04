using SwrveUnity.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace SwrveUnity
{
	public class SwrveAssetsManager : ISwrveAssetsManager
	{
		private MonoBehaviour Container;

		private string SwrveTemporaryPath;

		public string CdnImages
		{
			get;
			set;
		}

		public string CdnFonts
		{
			get;
			set;
		}

		public HashSet<string> AssetsOnDisk
		{
			get;
			set;
		}

		public SwrveAssetsManager(MonoBehaviour container, string swrveTemporaryPath)
		{
			this.Container = container;
			this.SwrveTemporaryPath = swrveTemporaryPath;
			this.AssetsOnDisk = new HashSet<string>();
		}

		[DebuggerHidden]
		public IEnumerator DownloadAssets(HashSet<SwrveAssetsQueueItem> assetsQueue, Action callBack)
		{
			yield return this.StartTask("SwrveAssetsManager.DownloadAssetQueue", this.DownloadAssetQueue(assetsQueue));
			if (callBack != null)
			{
				callBack();
			}
			this.TaskFinished("SwrveAssetsManager.DownloadAssets");
			yield break;
		}

		[DebuggerHidden]
		private IEnumerator DownloadAssetQueue(HashSet<SwrveAssetsQueueItem> assetsQueue)
		{
			IEnumerator<SwrveAssetsQueueItem> enumerator = assetsQueue.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SwrveAssetsQueueItem current = enumerator.Current;
				if (!this.CheckAsset(current.Name))
				{
					yield return this.StartTask("SwrveAssetsManager.DownloadAsset", this.DownloadAsset(current));
				}
				else
				{
					this.AssetsOnDisk.Add(current.Name);
				}
			}
			this.TaskFinished("SwrveAssetsManager.DownloadAssetQueue");
			yield break;
		}

		[DebuggerHidden]
		protected virtual IEnumerator DownloadAsset(SwrveAssetsQueueItem item)
		{
			string str = (!item.IsImage) ? this.CdnFonts : this.CdnImages;
			string text = str + item.Name;
			SwrveLog.Log("Downloading asset: " + text);
			WWW wWW = new WWW(text);
			yield return wWW;
			WwwDeducedError wwwDeducedError = UnityWwwHelper.DeduceWwwError(wWW);
			if (wWW != null && wwwDeducedError == WwwDeducedError.NoError && wWW.isDone)
			{
				if (item.IsImage)
				{
					this.SaveImageAsset(item, wWW);
				}
				else
				{
					this.SaveBinaryAsset(item, wWW);
				}
			}
			this.TaskFinished("SwrveAssetsManager.DownloadAsset");
			yield break;
		}

		private bool CheckAsset(string fileName)
		{
			return CrossPlatformFile.Exists(this.GetTemporaryPathFileName(fileName));
		}

		private string GetTemporaryPathFileName(string fileName)
		{
			return Path.Combine(this.SwrveTemporaryPath, fileName);
		}

		protected virtual void SaveImageAsset(SwrveAssetsQueueItem item, WWW www)
		{
			Texture2D texture = www.texture;
			if (texture != null)
			{
				byte[] bytes = www.bytes;
				string text = SwrveHelper.sha1(bytes);
				if (text == item.Digest)
				{
					byte[] bytes2 = texture.EncodeToPNG();
					string temporaryPathFileName = this.GetTemporaryPathFileName(item.Name);
					SwrveLog.Log("Saving to " + temporaryPathFileName);
					CrossPlatformFile.SaveBytes(temporaryPathFileName, bytes2);
					UnityEngine.Object.Destroy(texture);
					this.AssetsOnDisk.Add(item.Name);
				}
				else
				{
					SwrveLog.Log("Error downloading image assetItem:" + item.Name + ". Did not match digest:" + text);
				}
			}
		}

		protected virtual void SaveBinaryAsset(SwrveAssetsQueueItem item, WWW www)
		{
			byte[] bytes = www.bytes;
			string text = SwrveHelper.sha1(bytes);
			if (text == item.Digest)
			{
				string temporaryPathFileName = this.GetTemporaryPathFileName(item.Name);
				SwrveLog.Log("Saving to " + temporaryPathFileName);
				CrossPlatformFile.SaveBytes(temporaryPathFileName, bytes);
				this.AssetsOnDisk.Add(item.Name);
			}
			else
			{
				SwrveLog.Log("Error downloading binary assetItem:" + item.Name + ". Did not match digest:" + text);
			}
		}

		protected virtual Coroutine StartTask(string tag, IEnumerator task)
		{
			return this.Container.StartCoroutine(task);
		}

		protected virtual void TaskFinished(string tag)
		{
		}
	}
}
