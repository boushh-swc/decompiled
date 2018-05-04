using StaRTS.Main.Models;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class ValueObjectController
	{
		private Dictionary<string, List<StrIntPair>> strIntPairLists;

		private Dictionary<string, SequencePair> sequencePairs;

		public ValueObjectController()
		{
			Service.ValueObjectController = this;
			this.strIntPairLists = new Dictionary<string, List<StrIntPair>>();
			this.sequencePairs = new Dictionary<string, SequencePair>();
		}

		public List<StrIntPair> GetStrIntPairs(string uid, string blob)
		{
			if (string.IsNullOrEmpty(blob))
			{
				return null;
			}
			if (this.strIntPairLists.ContainsKey(blob))
			{
				return this.strIntPairLists[blob];
			}
			List<StrIntPair> list = this.ParseBlob(uid, blob);
			this.strIntPairLists.Add(blob, list);
			return list;
		}

		public SequencePair GetGunSequences(string uid, string rawSequence)
		{
			if (string.IsNullOrEmpty(rawSequence))
			{
				Service.Logger.Warn("Blank gunSequence in " + uid);
				return this.GetDefaultGunSequence(uid);
			}
			if (this.sequencePairs.ContainsKey(rawSequence))
			{
				return this.sequencePairs[rawSequence];
			}
			string[] array = rawSequence.Split(new char[]
			{
				','
			});
			int num = array.Length;
			if (num == 0)
			{
				Service.Logger.WarnFormat("Empty gunSequence in {0} '{1}'", new object[]
				{
					uid,
					rawSequence
				});
				return this.GetDefaultGunSequence(uid);
			}
			int[] array2 = new int[num];
			for (int i = 0; i < num; i++)
			{
				int num2;
				if (!int.TryParse(array[i], out num2))
				{
					Service.Logger.WarnFormat("Illegal gunSequence value in {0} '{1}'", new object[]
					{
						uid,
						rawSequence
					});
					return this.GetDefaultGunSequence(uid);
				}
				array2[i] = num2;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int j = 0; j < num; j++)
			{
				int num3 = array2[j];
				if (dictionary.ContainsKey(num3))
				{
					Dictionary<int, int> dictionary2;
					int key;
					(dictionary2 = dictionary)[key = num3] = dictionary2[key] + 1;
				}
				else
				{
					dictionary[num3] = 1;
				}
			}
			SequencePair sequencePair = new SequencePair(array2, dictionary);
			this.sequencePairs.Add(rawSequence, sequencePair);
			return sequencePair;
		}

		private SequencePair GetDefaultGunSequence(string uid)
		{
			return this.GetGunSequences(uid, "1");
		}

		private List<StrIntPair> ParseBlob(string uid, string blob)
		{
			List<StrIntPair> result = null;
			List<StrIntPair> list = null;
			StrIntState strIntState = StrIntState.StrStart;
			int num = -1;
			string strKey = null;
			int i = 0;
			int length = blob.Length;
			while (i < length)
			{
				char c = blob[i];
				bool flag = c == '"';
				switch (strIntState)
				{
				case StrIntState.StrStart:
					if (flag)
					{
						num = i + 1;
						strIntState++;
					}
					else if (c != ',' && !char.IsWhiteSpace(c))
					{
						this.AddError(ref list, "Missing initial quote");
						num = i;
						strIntState++;
					}
					break;
				case StrIntState.StrEnd:
					if (flag)
					{
						if (i == num)
						{
							this.AddError(ref list, "Empty string inside");
						}
						strKey = blob.Substring(num, i - num);
						num = -1;
						strIntState++;
					}
					else if (c == ':')
					{
						this.AddError(ref list, "Missing final quote");
						strKey = blob.Substring(num, i - num);
						num = -1;
						strIntState++;
					}
					break;
				case StrIntState.IntStart:
					if (char.IsDigit(c) || (num == -1 && c == '-'))
					{
						num = i;
						strIntState++;
					}
					else if (c != ':' && !char.IsWhiteSpace(c))
					{
						this.AddError(ref list, "Unsupported delimiter");
					}
					break;
				case StrIntState.IntEnd:
					if (!char.IsDigit(c))
					{
						string s = blob.Substring(num, i - num);
						this.AddPair(ref result, strKey, int.Parse(s));
						num = -1;
						strIntState = StrIntState.StrStart;
						if (flag)
						{
							if (i == length - 1)
							{
								this.AddError(ref list, "Extra quote");
							}
							else
							{
								this.AddError(ref list, "Missing comma");
								num = i + 1;
								strIntState++;
							}
						}
					}
					break;
				}
				i++;
			}
			if (strIntState != StrIntState.StrStart)
			{
				if (strIntState != StrIntState.IntEnd)
				{
					this.AddError(ref list, "String-int mismatch");
				}
				else
				{
					string s2 = blob.Substring(num);
					this.AddPair(ref result, strKey, int.Parse(s2));
				}
			}
			if (list != null)
			{
				string text = string.Format("Formatting errors in {0} '{1}'", uid, blob);
				int j = 0;
				int count = list.Count;
				while (j < count)
				{
					StrIntPair strIntPair = list[j];
					text += string.Format("; {0} ({1})", strIntPair.StrKey, strIntPair.IntVal);
					j++;
				}
				Service.Logger.Warn(text);
			}
			return result;
		}

		private void AddPair(ref List<StrIntPair> list, string strKey, int intVal)
		{
			if (list == null)
			{
				list = new List<StrIntPair>();
			}
			list.Add(new StrIntPair(strKey, intVal));
		}

		private void AddError(ref List<StrIntPair> errors, string error)
		{
			if (errors == null)
			{
				errors = new List<StrIntPair>();
			}
			else
			{
				int i = 0;
				int count = errors.Count;
				while (i < count)
				{
					StrIntPair strIntPair = errors[i];
					if (strIntPair.StrKey == error)
					{
						strIntPair.IntVal++;
						return;
					}
					i++;
				}
			}
			errors.Add(new StrIntPair(error, 1));
		}
	}
}
