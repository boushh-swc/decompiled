using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Utils.Json
{
	public class JsonParser
	{
		private readonly string json;

		private readonly StringBuilder sb = new StringBuilder();

		private int index;

		private int jsonLength = -1;

		private int nextBackslash = -1;

		private JsonTokens lookAheadToken;

		private bool internStrings;

		private static readonly JsonTokens[] jsonTokenMap;

		private static List<Dictionary<string, object>> pool;

		public JsonParser(string json) : this(json, 0, false)
		{
		}

		public JsonParser(string json, int startFrom) : this(json, startFrom, false)
		{
		}

		public JsonParser(string json, int startFrom, bool internStrings)
		{
			this.json = json;
			this.jsonLength = ((json != null) ? json.Length : 0);
			this.index = startFrom;
			this.internStrings = internStrings;
			this.nextBackslash = 0;
			this.FindNextBackslash();
		}

		static JsonParser()
		{
			JsonParser.jsonTokenMap = new JsonTokens[256];
			JsonParser.pool = new List<Dictionary<string, object>>();
			for (int i = 0; i < 256; i++)
			{
				JsonParser.jsonTokenMap[i] = JsonTokens.None;
			}
			JsonParser.jsonTokenMap[123] = JsonTokens.ObjectOpen;
			JsonParser.jsonTokenMap[125] = JsonTokens.ObjectClose;
			JsonParser.jsonTokenMap[91] = JsonTokens.ArrayOpen;
			JsonParser.jsonTokenMap[93] = JsonTokens.ArrayClose;
			JsonParser.jsonTokenMap[44] = JsonTokens.Comma;
			JsonParser.jsonTokenMap[34] = JsonTokens.String;
			JsonParser.jsonTokenMap[48] = JsonTokens.Number;
			JsonParser.jsonTokenMap[49] = JsonTokens.Number;
			JsonParser.jsonTokenMap[50] = JsonTokens.Number;
			JsonParser.jsonTokenMap[51] = JsonTokens.Number;
			JsonParser.jsonTokenMap[52] = JsonTokens.Number;
			JsonParser.jsonTokenMap[53] = JsonTokens.Number;
			JsonParser.jsonTokenMap[54] = JsonTokens.Number;
			JsonParser.jsonTokenMap[55] = JsonTokens.Number;
			JsonParser.jsonTokenMap[56] = JsonTokens.Number;
			JsonParser.jsonTokenMap[57] = JsonTokens.Number;
			JsonParser.jsonTokenMap[45] = JsonTokens.Number;
			JsonParser.jsonTokenMap[43] = JsonTokens.Number;
			JsonParser.jsonTokenMap[46] = JsonTokens.Number;
			JsonParser.jsonTokenMap[58] = JsonTokens.Colon;
			JsonParser.jsonTokenMap[102] = JsonTokens.False;
			JsonParser.jsonTokenMap[116] = JsonTokens.WordFirst;
			JsonParser.jsonTokenMap[110] = JsonTokens.Null;
		}

		public static void StaticReset()
		{
			JsonParser.pool.Clear();
		}

		public object Parse()
		{
			return (this.jsonLength != 0) ? this.ParseValue() : null;
		}

		private Dictionary<string, object> ParseObject()
		{
			Dictionary<string, object> dictionary;
			if (JsonParser.pool.Count == 0)
			{
				dictionary = new Dictionary<string, object>();
			}
			else
			{
				int num = JsonParser.pool.Count - 1;
				dictionary = JsonParser.pool[num];
				JsonParser.pool.RemoveAt(num);
			}
			this.lookAheadToken = JsonTokens.None;
			while (true)
			{
				JsonTokens jsonTokens = this.LookAhead();
				if (jsonTokens == JsonTokens.Comma)
				{
					this.lookAheadToken = JsonTokens.None;
				}
				else
				{
					if (jsonTokens == JsonTokens.ObjectClose)
					{
						break;
					}
					string key = this.ParseString();
					JsonTokens jsonTokens2 = (this.lookAheadToken == JsonTokens.None) ? this.NextTokenCore() : this.lookAheadToken;
					this.lookAheadToken = JsonTokens.None;
					if (jsonTokens2 != JsonTokens.Colon)
					{
						goto Block_5;
					}
					object value = this.ParseValue();
					dictionary[key] = value;
				}
			}
			this.lookAheadToken = JsonTokens.None;
			goto IL_CC;
			Block_5:
			dictionary.Clear();
			IL_CC:
			if (dictionary.Count == 0)
			{
				JsonParser.pool.Add(dictionary);
				dictionary = null;
			}
			return dictionary;
		}

		private List<object> ParseArray()
		{
			List<object> list = new List<object>();
			this.lookAheadToken = JsonTokens.None;
			while (true)
			{
				JsonTokens jsonTokens = this.LookAhead();
				if (jsonTokens == JsonTokens.Comma)
				{
					this.lookAheadToken = JsonTokens.None;
				}
				else
				{
					if (jsonTokens == JsonTokens.ArrayClose)
					{
						break;
					}
					list.Add(this.ParseValue());
				}
			}
			this.lookAheadToken = JsonTokens.None;
			list.Capacity = list.Count;
			return list;
		}

		private object ParseValue()
		{
			JsonTokens jsonTokens = this.LookAhead();
			switch (jsonTokens)
			{
			case JsonTokens.ObjectOpen:
				return this.ParseObject();
			case JsonTokens.ObjectClose:
			case JsonTokens.ArrayClose:
			case JsonTokens.Colon:
			case JsonTokens.Comma:
				IL_2F:
				switch (jsonTokens)
				{
				case JsonTokens.WordFirst:
					this.lookAheadToken = JsonTokens.None;
					return true;
				case JsonTokens.False:
					this.lookAheadToken = JsonTokens.None;
					return false;
				case JsonTokens.Null:
					this.lookAheadToken = JsonTokens.None;
					return null;
				default:
					return null;
				}
				break;
			case JsonTokens.ArrayOpen:
				return this.ParseArray();
			case JsonTokens.String:
				return this.ParseString();
			case JsonTokens.Number:
				return this.ParseNumber();
			}
			goto IL_2F;
		}

		private void FindNextBackslash()
		{
			if (this.nextBackslash >= 0)
			{
				this.nextBackslash = this.json.IndexOf('\\', this.index);
			}
		}

		private string NewString(string s)
		{
			return (!this.internStrings) ? s : string.Intern(s);
		}

		private string ParseString()
		{
			this.lookAheadToken = JsonTokens.None;
			int num = this.json.IndexOf('"', this.index);
			if (num < 0)
			{
				return null;
			}
			if (this.nextBackslash < 0 || this.nextBackslash > num)
			{
				string s = this.json.Substring(this.index, num - this.index);
				this.index = num + 1;
				return this.NewString(s);
			}
			this.sb.Length = 0;
			int num2 = -1;
			while (this.index < this.jsonLength)
			{
				char c = this.json[this.index++];
				if (c == '"')
				{
					if (num2 != -1)
					{
						if (this.sb.Length == 0)
						{
							this.FindNextBackslash();
							return this.NewString(this.json.Substring(num2, this.index - num2 - 1));
						}
						this.sb.Append(this.json.Substring(num2, this.index - num2 - 1));
					}
					this.FindNextBackslash();
					return this.NewString(this.sb.ToString());
				}
				if (c != '\\')
				{
					if (num2 == -1)
					{
						num2 = this.index - 1;
					}
				}
				else
				{
					if (this.index == this.jsonLength)
					{
						break;
					}
					if (num2 != -1)
					{
						this.sb.Append(this.json.Substring(num2, this.index - num2 - 1));
						num2 = -1;
					}
					char c2 = this.json[this.index++];
					switch (c2)
					{
					case 'n':
						this.sb.Append('\n');
						continue;
					case 'o':
					case 'p':
					case 'q':
					case 's':
						IL_1BD:
						if (c2 == '"')
						{
							this.sb.Append('"');
							continue;
						}
						if (c2 == '/')
						{
							this.sb.Append('/');
							continue;
						}
						if (c2 == '\\')
						{
							this.sb.Append('\\');
							continue;
						}
						if (c2 == 'b')
						{
							this.sb.Append('\b');
							continue;
						}
						if (c2 != 'f')
						{
							continue;
						}
						this.sb.Append('\f');
						continue;
					case 'r':
						this.sb.Append('\r');
						continue;
					case 't':
						this.sb.Append('\t');
						continue;
					case 'u':
					{
						if (this.jsonLength - this.index < 4)
						{
							continue;
						}
						uint num3 = this.ParseUnicode(this.json[this.index], this.json[this.index + 1], this.json[this.index + 2], this.json[this.index + 3]);
						this.sb.Append((char)num3);
						this.index += 4;
						continue;
					}
					}
					goto IL_1BD;
				}
			}
			this.FindNextBackslash();
			return null;
		}

		private uint ParseSingleChar(char c1, uint multipliyer)
		{
			uint result = 0u;
			if (c1 >= '0' && c1 <= '9')
			{
				result = (uint)(c1 - '0') * multipliyer;
			}
			else if (c1 >= 'A' && c1 <= 'F')
			{
				result = (uint)(c1 - 'A' + '\n') * multipliyer;
			}
			else if (c1 >= 'a' && c1 <= 'f')
			{
				result = (uint)(c1 - 'a' + '\n') * multipliyer;
			}
			return result;
		}

		private uint ParseUnicode(char c1, char c2, char c3, char c4)
		{
			uint num = this.ParseSingleChar(c1, 4096u);
			uint num2 = this.ParseSingleChar(c2, 256u);
			uint num3 = this.ParseSingleChar(c3, 16u);
			uint num4 = this.ParseSingleChar(c4, 1u);
			return num + num2 + num3 + num4;
		}

		private string ParseNumber()
		{
			this.lookAheadToken = JsonTokens.None;
			int num = this.index - 1;
			char c = this.json[this.index];
			while ((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
			{
				if (++this.index == this.jsonLength)
				{
					return null;
				}
				c = this.json[this.index];
			}
			return this.NewString(this.json.Substring(num, this.index - num));
		}

		private JsonTokens LookAhead()
		{
			if (this.lookAheadToken != JsonTokens.None)
			{
				return this.lookAheadToken;
			}
			return this.lookAheadToken = this.NextTokenCore();
		}

		private JsonTokens NextTokenCore()
		{
			char c;
			for (c = this.json[this.index]; c <= ' '; c = this.json[this.index])
			{
				if (++this.index == this.jsonLength)
				{
					return JsonTokens.None;
				}
			}
			this.index++;
			if (c >= 'Ā')
			{
				return JsonTokens.None;
			}
			JsonTokens jsonTokens = JsonParser.jsonTokenMap[(int)c];
			if (jsonTokens >= JsonTokens.WordFirst)
			{
				do
				{
					c = this.json[this.index];
				}
				while (c >= 'a' && c <= 'z' && ++this.index < this.jsonLength);
			}
			return jsonTokens;
		}
	}
}
