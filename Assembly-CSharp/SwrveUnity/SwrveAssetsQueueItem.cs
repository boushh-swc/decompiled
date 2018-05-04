using System;

namespace SwrveUnity
{
	public class SwrveAssetsQueueItem
	{
		public string Name
		{
			get;
			private set;
		}

		public string Digest
		{
			get;
			private set;
		}

		public bool IsImage
		{
			get;
			private set;
		}

		public SwrveAssetsQueueItem(string name, string digest, bool isImage)
		{
			this.Name = name;
			this.Digest = digest;
			this.IsImage = isImage;
		}

		public override bool Equals(object obj)
		{
			SwrveAssetsQueueItem swrveAssetsQueueItem = obj as SwrveAssetsQueueItem;
			return swrveAssetsQueueItem != null && swrveAssetsQueueItem.Name == this.Name && swrveAssetsQueueItem.Digest == this.Digest && swrveAssetsQueueItem.IsImage == this.IsImage;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + this.Name.GetHashCode();
			num = num * 23 + this.Digest.GetHashCode();
			return num * 23 + this.IsImage.GetHashCode();
		}
	}
}
