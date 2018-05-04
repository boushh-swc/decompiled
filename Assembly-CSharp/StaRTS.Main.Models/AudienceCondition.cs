using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models
{
	public class AudienceCondition
	{
		private const char DELIMITER = ':';

		private const string INVALID = "Invalid";

		public string ConditionType = "Invalid";

		public string ConditionValue = "Invalid";

		public AudienceCondition(string source)
		{
			string[] array = source.Split(new char[]
			{
				':'
			});
			if (array.Length < 2)
			{
				Service.Logger.WarnFormat("Could not define AudienceCondition from {0}", new object[]
				{
					source
				});
				return;
			}
			this.ConditionType = array[0];
			this.ConditionValue = array[1];
		}
	}
}
