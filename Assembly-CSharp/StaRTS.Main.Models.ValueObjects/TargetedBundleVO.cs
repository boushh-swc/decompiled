using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TargetedBundleVO : IValueObject
	{
		public List<AudienceCondition> AudienceConditions;

		public List<string> LinkedIAPs;

		public List<string> RewardUIDs;

		public List<string> Groups;

		public static int COLUMN_uid
		{
			get;
			private set;
		}

		public static int COLUMN_rewards
		{
			get;
			private set;
		}

		public static int COLUMN_title
		{
			get;
			private set;
		}

		public static int COLUMN_description
		{
			get;
			private set;
		}

		public static int COLUMN_confirmationString
		{
			get;
			private set;
		}

		public static int COLUMN_groups
		{
			get;
			private set;
		}

		public static int COLUMN_discount
		{
			get;
			private set;
		}

		public static int COLUMN_audienceConditions
		{
			get;
			private set;
		}

		public static int COLUMN_duration
		{
			get;
			private set;
		}

		public static int COLUMN_startDate
		{
			get;
			private set;
		}

		public static int COLUMN_endDate
		{
			get;
			private set;
		}

		public static int COLUMN_expirationCooldown
		{
			get;
			private set;
		}

		public static int COLUMN_repurchaseCooldown
		{
			get;
			private set;
		}

		public static int COLUMN_globalCooldown
		{
			get;
			private set;
		}

		public static int COLUMN_groupCooldown
		{
			get;
			private set;
		}

		public static int COLUMN_maxPurchases
		{
			get;
			private set;
		}

		public static int COLUMN_linkedPack
		{
			get;
			private set;
		}

		public static int COLUMN_character1Image
		{
			get;
			private set;
		}

		public static int COLUMN_character2Image
		{
			get;
			private set;
		}

		public static int COLUMN_iconImage
		{
			get;
			private set;
		}

		public static int COLUMN_iconString
		{
			get;
			private set;
		}

		public static int COLUMN_offerImage
		{
			get;
			private set;
		}

		public static int COLUMN_autoPop
		{
			get;
			private set;
		}

		public static int COLUMN_priority
		{
			get;
			private set;
		}

		public static int COLUMN_cost
		{
			get;
			private set;
		}

		public static int COLUMN_ignoreCooldown
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string Reward
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string ConfirmationString
		{
			get;
			set;
		}

		public int Discount
		{
			get;
			set;
		}

		public int Duration
		{
			get;
			set;
		}

		public DateTime StartTime
		{
			get;
			set;
		}

		public DateTime EndTime
		{
			get;
			set;
		}

		public int ExpirationCooldown
		{
			get;
			set;
		}

		public int RepurchaseCooldown
		{
			get;
			set;
		}

		public int GlobalCooldown
		{
			get;
			set;
		}

		public int GroupCooldown
		{
			get;
			set;
		}

		public int MaxPurchases
		{
			get;
			set;
		}

		public string Character1Image
		{
			get;
			set;
		}

		public string Character2Image
		{
			get;
			set;
		}

		public string IconImage
		{
			get;
			set;
		}

		public string IconString
		{
			get;
			set;
		}

		public string OfferImage
		{
			get;
			set;
		}

		public bool AutoPop
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			set;
		}

		public string[] Cost
		{
			get;
			set;
		}

		public bool IgnoreCooldown
		{
			get;
			set;
		}

		public override string ToString()
		{
			return this.Uid;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Title = row.TryGetString(TargetedBundleVO.COLUMN_title);
			this.Description = row.TryGetString(TargetedBundleVO.COLUMN_description);
			this.ConfirmationString = row.TryGetString(TargetedBundleVO.COLUMN_confirmationString);
			this.Discount = row.TryGetInt(TargetedBundleVO.COLUMN_discount);
			this.Duration = row.TryGetInt(TargetedBundleVO.COLUMN_duration);
			string text = row.TryGetString(TargetedBundleVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				this.StartTime = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
			}
			else
			{
				this.StartTime = DateTime.MinValue;
				Service.Logger.Warn("TargetedBundle VO Start Date Not Specified");
			}
			string text2 = row.TryGetString(TargetedBundleVO.COLUMN_endDate);
			if (!string.IsNullOrEmpty(text2))
			{
				this.EndTime = DateTime.ParseExact(text2, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
			}
			else
			{
				this.EndTime = DateTime.MaxValue;
				Service.Logger.Warn("TargetedBundle VO End Date Not Specified");
			}
			this.ExpirationCooldown = row.TryGetInt(TargetedBundleVO.COLUMN_expirationCooldown);
			this.RepurchaseCooldown = row.TryGetInt(TargetedBundleVO.COLUMN_repurchaseCooldown);
			this.GlobalCooldown = row.TryGetInt(TargetedBundleVO.COLUMN_globalCooldown);
			this.GroupCooldown = row.TryGetInt(TargetedBundleVO.COLUMN_groupCooldown);
			this.MaxPurchases = row.TryGetInt(TargetedBundleVO.COLUMN_maxPurchases);
			this.Character1Image = row.TryGetString(TargetedBundleVO.COLUMN_character1Image);
			this.Character2Image = row.TryGetString(TargetedBundleVO.COLUMN_character2Image);
			this.IconImage = row.TryGetString(TargetedBundleVO.COLUMN_iconImage);
			this.IconString = row.TryGetString(TargetedBundleVO.COLUMN_iconString);
			this.OfferImage = row.TryGetString(TargetedBundleVO.COLUMN_offerImage);
			this.AutoPop = row.TryGetBool(TargetedBundleVO.COLUMN_autoPop);
			this.Priority = row.TryGetInt(TargetedBundleVO.COLUMN_priority);
			this.Cost = row.TryGetStringArray(TargetedBundleVO.COLUMN_cost);
			this.IgnoreCooldown = row.TryGetBool(TargetedBundleVO.COLUMN_ignoreCooldown);
			this.AudienceConditions = new List<AudienceCondition>();
			string[] array = row.TryGetStringArray(TargetedBundleVO.COLUMN_audienceConditions);
			if (array != null)
			{
				int i = 0;
				int num = array.Length;
				while (i < num)
				{
					this.AudienceConditions.Add(new AudienceCondition(array[i]));
					i++;
				}
			}
			this.Groups = new List<string>();
			string[] array2 = row.TryGetStringArray(TargetedBundleVO.COLUMN_groups);
			if (array2 != null)
			{
				int j = 0;
				int num2 = array2.Length;
				while (j < num2)
				{
					this.Groups.Add(array2[j]);
					j++;
				}
			}
			this.LinkedIAPs = new List<string>();
			string[] array3 = row.TryGetStringArray(TargetedBundleVO.COLUMN_linkedPack);
			if (array3 != null)
			{
				int k = 0;
				int num3 = array3.Length;
				while (k < num3)
				{
					this.LinkedIAPs.Add(array3[k]);
					k++;
				}
			}
			this.RewardUIDs = new List<string>();
			string[] array4 = row.TryGetStringArray(TargetedBundleVO.COLUMN_rewards);
			if (array4 != null)
			{
				int l = 0;
				int num4 = array4.Length;
				while (l < num4)
				{
					this.RewardUIDs.Add(array4[l]);
					l++;
				}
			}
		}
	}
}
