using System;
using System.Collections.Generic;
using System.Linq;

namespace SwrveUnity.Messaging
{
	public class SwrveConditions
	{
		public enum TriggerOperatorType
		{
			AND = 0,
			EQUALS = 1
		}

		private const string OP_KEY = "op";

		private const string OP_EQ_KEY = "eq";

		private const string OP_AND_KEY = "and";

		private const string KEY_KEY = "key";

		private const string VALUE_KEY = "value";

		private const string ARGS_KEY = "args";

		private string key;

		private SwrveConditions.TriggerOperatorType? op;

		private string value;

		private List<SwrveConditions> args;

		private SwrveConditions(SwrveConditions.TriggerOperatorType? op)
		{
			this.op = op;
		}

		private SwrveConditions(SwrveConditions.TriggerOperatorType? op, string key, string value) : this(op)
		{
			this.key = key;
			this.value = value;
		}

		private SwrveConditions(SwrveConditions.TriggerOperatorType? op, List<SwrveConditions> args) : this(op)
		{
			this.args = args;
		}

		public string GetKey()
		{
			return this.key;
		}

		public SwrveConditions.TriggerOperatorType? GetOp()
		{
			return this.op;
		}

		public string GetValue()
		{
			return this.value;
		}

		public List<SwrveConditions> GetArgs()
		{
			return this.args;
		}

		private bool isEmpty()
		{
			SwrveConditions.TriggerOperatorType? triggerOperatorType = this.op;
			return !triggerOperatorType.HasValue;
		}

		private bool matchesEquals(IDictionary<string, string> payload)
		{
			return this.op == SwrveConditions.TriggerOperatorType.EQUALS && payload.ContainsKey(this.key) && string.Equals(payload[this.key], this.value, StringComparison.OrdinalIgnoreCase);
		}

		private bool matchesAll(IDictionary<string, string> payload)
		{
			return this.op == SwrveConditions.TriggerOperatorType.AND && this.args.All((SwrveConditions cond) => cond.Matches(payload));
		}

		public bool Matches(IDictionary<string, string> payload)
		{
			return this.isEmpty() || (payload != null && (this.matchesEquals(payload) || this.matchesAll(payload)));
		}

		public static SwrveConditions LoadFromJson(IDictionary<string, object> json, bool isRoot)
		{
			if (json.Keys.Count == 0)
			{
				if (isRoot)
				{
					return new SwrveConditions(null);
				}
				return null;
			}
			else
			{
				string a = (string)json["op"];
				if (a == "eq")
				{
					string text = (string)json["key"];
					string text2 = (string)json["value"];
					if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
					{
						return null;
					}
					return new SwrveConditions(new SwrveConditions.TriggerOperatorType?(SwrveConditions.TriggerOperatorType.EQUALS), text, text2);
				}
				else
				{
					if (!isRoot || !(a == "and"))
					{
						return null;
					}
					IList<object> list = (IList<object>)json["args"];
					List<SwrveConditions> list2 = new List<SwrveConditions>();
					IEnumerator<object> enumerator = list.GetEnumerator();
					while (enumerator.MoveNext())
					{
						SwrveConditions swrveConditions = SwrveConditions.LoadFromJson((Dictionary<string, object>)enumerator.Current, false);
						if (swrveConditions == null)
						{
							return null;
						}
						list2.Add(swrveConditions);
					}
					if (list2.Count == 0)
					{
						return null;
					}
					return new SwrveConditions(new SwrveConditions.TriggerOperatorType?(SwrveConditions.TriggerOperatorType.AND), list2);
				}
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Conditions{key='",
				this.key,
				'\'',
				", op='",
				this.op,
				'\'',
				", value='",
				this.value,
				'\'',
				", args=",
				this.args,
				'}'
			});
		}
	}
}
