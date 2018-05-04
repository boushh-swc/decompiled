using SwrveUnity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SwrveUnity.Messaging
{
	public class SwrveMessage : SwrveBaseMessage
	{
		public string Name;

		public int Priority = 9999;

		public List<SwrveMessageFormat> Formats;

		public Point Position = new Point(0, 0);

		public Point TargetPosition = new Point(0, 0);

		public float BackgroundAlpha = 1f;

		public float AnimationScale = 1f;

		private ISwrveAssetsManager SwrveAssetsManager;

		private SwrveMessage(ISwrveAssetsManager swrveAssetsManager, SwrveMessagesCampaign campaign)
		{
			this.SwrveAssetsManager = swrveAssetsManager;
			this.Campaign = campaign;
			this.Formats = new List<SwrveMessageFormat>();
		}

		public SwrveMessageFormat GetFormat(SwrveOrientation orientation)
		{
			IEnumerator<SwrveMessageFormat> enumerator = this.Formats.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Orientation == orientation)
				{
					return enumerator.Current;
				}
			}
			return null;
		}

		public static SwrveMessage LoadFromJSON(ISwrveAssetsManager swrveAssetsManager, SwrveMessagesCampaign campaign, Dictionary<string, object> messageData, Color? defaultBackgroundColor)
		{
			SwrveMessage swrveMessage = new SwrveMessage(swrveAssetsManager, campaign);
			swrveMessage.Id = MiniJsonHelper.GetInt(messageData, "id");
			swrveMessage.Name = (string)messageData["name"];
			if (messageData.ContainsKey("priority"))
			{
				swrveMessage.Priority = MiniJsonHelper.GetInt(messageData, "priority");
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)messageData["template"];
			IList<object> list = (List<object>)dictionary["formats"];
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				Dictionary<string, object> messageFormatData = (Dictionary<string, object>)list[i];
				SwrveMessageFormat item = SwrveMessageFormat.LoadFromJSON(swrveAssetsManager, swrveMessage, messageFormatData, defaultBackgroundColor);
				swrveMessage.Formats.Add(item);
				i++;
			}
			return swrveMessage;
		}

		public bool SupportsOrientation(SwrveOrientation orientation)
		{
			return orientation == SwrveOrientation.Both || this.GetFormat(orientation) != null;
		}

		public HashSet<SwrveAssetsQueueItem> SetOfAssets()
		{
			HashSet<SwrveAssetsQueueItem> hashSet = new HashSet<SwrveAssetsQueueItem>();
			for (int i = 0; i < this.Formats.Count; i++)
			{
				SwrveMessageFormat swrveMessageFormat = this.Formats[i];
				for (int j = 0; j < swrveMessageFormat.Images.Count; j++)
				{
					SwrveImage swrveImage = swrveMessageFormat.Images[j];
					if (!string.IsNullOrEmpty(swrveImage.File))
					{
						hashSet.Add(new SwrveAssetsQueueItem(swrveImage.File, swrveImage.File, true));
					}
				}
				for (int k = 0; k < swrveMessageFormat.Buttons.Count; k++)
				{
					SwrveButton swrveButton = swrveMessageFormat.Buttons[k];
					if (!string.IsNullOrEmpty(swrveButton.Image))
					{
						hashSet.Add(new SwrveAssetsQueueItem(swrveButton.Image, swrveButton.Image, true));
					}
				}
			}
			return hashSet;
		}

		public bool IsDownloaded()
		{
			HashSet<SwrveAssetsQueueItem> source = this.SetOfAssets();
			return source.All((SwrveAssetsQueueItem asset) => this.SwrveAssetsManager.AssetsOnDisk.Contains(asset.Name));
		}

		public override string GetBaseFormattedMessageType()
		{
			return "Message";
		}
	}
}
