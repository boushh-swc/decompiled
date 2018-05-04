using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class TestImpressionAnalytics : GameAnalytics
	{
		private string _testName = string.Empty;

		private int _shardNumber;

		private bool _applied;

		public string TestName
		{
			get
			{
				return this._testName;
			}
		}

		public int ShardNumber
		{
			get
			{
				return this._shardNumber;
			}
		}

		public bool IsApplied
		{
			get
			{
				return this._applied;
			}
		}

		public TestImpressionAnalytics(string testName, int shardNumber, bool isApplied)
		{
			this._testName = testName;
			this._shardNumber = shardNumber;
			this._applied = isApplied;
		}

		public override string GetSwrveEvent()
		{
			return "test_impression";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["test_name"] = this._testName;
			dictionary["shard_number"] = this._shardNumber;
			dictionary["applied"] = ((!this._applied) ? 0 : 1);
			return dictionary;
		}
	}
}
