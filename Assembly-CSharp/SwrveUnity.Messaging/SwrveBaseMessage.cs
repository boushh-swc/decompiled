using System;

namespace SwrveUnity.Messaging
{
	public abstract class SwrveBaseMessage
	{
		public int Id;

		public SwrveBaseCampaign Campaign;

		public string GetBaseMessageType()
		{
			return this.GetBaseFormattedMessageType().ToLower();
		}

		public abstract string GetBaseFormattedMessageType();

		public string GetEventPrefix()
		{
			return string.Concat(new string[]
			{
				"Swrve.",
				this.GetBaseFormattedMessageType(),
				"s.",
				this.GetBaseMessageType(),
				"_"
			});
		}
	}
}
