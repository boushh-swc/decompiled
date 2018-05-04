using AhoCorasick;
using StaRTS.Utils.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Midcore.Chat.Filter
{
	public class SwearFilter
	{
		private const string PUNCTUATION_REGEX = "[\\.,!\"@#\\$%\\^&\\*?<>/]";

		private Trie trie;

		private Logger logger;

		private string[] words;

		private string wordReplacement;

		public SwearFilter(string[] words, string wordReplacement, Logger logger)
		{
			this.words = words;
			this.wordReplacement = wordReplacement;
			this.logger = logger;
			this.BuildTrie();
		}

		public void BuildTrie()
		{
			if (this.trie != null)
			{
				this.logger.Error("Trie should only be built once!");
			}
			this.trie = new Trie();
			if (this.words != null)
			{
				this.trie.Add(this.words);
				this.logger.DebugFormat("Added {0} censored words", new object[]
				{
					this.words.Length
				});
				this.trie.Build();
			}
		}

		public void ResetTrie()
		{
			this.trie = null;
		}

		public string Filter(string message)
		{
			if (this.trie == null)
			{
				this.logger.Error("Trie is null, build it first!");
				return message;
			}
			if (string.IsNullOrEmpty(message))
			{
				this.logger.Warn("Message is null or blank, nothing to filter!");
				return message;
			}
			string[] array = message.Split(new char[]
			{
				' '
			});
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			int num = array.Length;
			while (i < num)
			{
				string text = array[i];
				Regex regex = new Regex("[\\.,!\"@#\\$%\\^&\\*?<>/]");
				string text2 = regex.Replace(text, string.Empty);
				text2 = text2.ToLower();
				bool flag = false;
				using (IEnumerator<string> enumerator = this.trie.Find(text2).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						string current = enumerator.Current;
						flag = true;
					}
				}
				stringBuilder.Append((!flag) ? (text + " ") : this.wordReplacement);
				i++;
			}
			stringBuilder.Remove(stringBuilder.Length - 1, 1);
			return stringBuilder.ToString();
		}
	}
}
