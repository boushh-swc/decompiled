using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class ProfanityController
	{
		private const string META_RESERVED_UID = "reserved";

		private const string META_EN_UID = "en";

		private List<string> profaneWordsLoose;

		private List<string> profaneWordsStrict;

		private List<string> reservedWords;

		public static readonly char[] SPECIAL_CHARS = new char[]
		{
			' ',
			'_',
			'.',
			'-',
			':',
			','
		};

		public ProfanityController()
		{
			Service.ProfanityController = this;
			this.profaneWordsLoose = new List<string>();
			this.profaneWordsStrict = new List<string>();
			this.reservedWords = new List<string>();
			string b = Service.Lang.ExtractLanguageFromLocale();
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (ProfanityVO current in staticDataController.GetAll<ProfanityVO>())
			{
				string[] words = current.Words;
				if (current.Uid == "reserved")
				{
					this.reservedWords.AddRange(words);
				}
				else if (current.Uid == b || current.Uid == "en")
				{
					this.profaneWordsLoose.AddRange(words);
				}
				else
				{
					this.profaneWordsStrict.AddRange(words);
				}
			}
			for (int i = 0; i < this.profaneWordsLoose.Count; i++)
			{
				this.profaneWordsLoose[i] = this.profaneWordsLoose[i].ToLower();
			}
			for (int j = 0; j < this.profaneWordsStrict.Count; j++)
			{
				this.profaneWordsStrict[j] = this.profaneWordsStrict[j].ToLower();
			}
			staticDataController.Unload<ProfanityVO>();
		}

		public bool IsValid(string input, bool checkReservedWords)
		{
			string text = input.ToLower();
			if (this.profaneWordsStrict.Contains(text))
			{
				return false;
			}
			string text2 = text;
			int i = 0;
			int num = ProfanityController.SPECIAL_CHARS.Length;
			while (i < num)
			{
				text2 = text2.Replace(ProfanityController.SPECIAL_CHARS[i].ToString(), string.Empty);
				i++;
			}
			int j = 0;
			int count = this.profaneWordsLoose.Count;
			while (j < count)
			{
				if (text.Contains(this.profaneWordsLoose[j]) || text2.Contains(this.profaneWordsLoose[j]))
				{
					return false;
				}
				j++;
			}
			if (checkReservedWords)
			{
				int k = 0;
				int count2 = this.reservedWords.Count;
				while (k < count2)
				{
					if (text.Contains(this.reservedWords[k]) || text2.Contains(this.reservedWords[k]))
					{
						return false;
					}
					k++;
				}
			}
			return true;
		}
	}
}
