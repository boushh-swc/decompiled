using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Controls
{
	public class JewelControl
	{
		public const string JEWEL_ID_CLANS = "Clans";

		public const string JEWEL_ID_LOG = "Log";

		public const string JEWEL_ID_LEADERBOARD = "Leaderboard";

		public const string JEWEL_ID_SETTINGS = "Settings";

		public const string JEWEL_ID_STORE = "Store";

		public const string JEWEL_ID_LEI = "StoreSpecial";

		public const string JEWEL_ID_BATTLE = "Battle";

		public const string JEWEL_ID_INVITES = "Invites";

		public const string JEWEL_ID_GALAXY = "Galaxy";

		public const string JEWEL_ID_HOLONET = "NewsHolo";

		public const string JEWEL_ID_WAR = "War";

		public const string JEWEL_ID_WAR_PREP = "Prep";

		public const string JEWEL_ID_PERK = "Perks";

		public const string JEWEL_ID_ACT_PERK = "ActCardPerks";

		public const string JEWEL_ID_UP_PERK = "UpCardPerks";

		public const string JEWEL_ID_CHAT = "Chat";

		public const string JEWEL_ID_SOCIAL_CHAT = "SocialChat";

		public const string JEWEL_ID_SOCIAL_WAR_LOG = "SocialWarLog";

		public const string JEWEL_ID_SOCIAL_MESSAGES = "Messages";

		public const string JEWEL_ID_SOCIAL_REQUESTS = "Requests";

		public const string JEWEL_ID_SOCIAL_REPLAYS = "Replays";

		public const string JEWEL_ID_SOCIAL_UPDATES = "Updates";

		public const string JEWEL_ID_SOCIAL_VIDEOS = "Videos";

		public const string JEWEL_ID_INVENTORY_CRATES = "Crates";

		public const string JEWEL_ID_INVENTORY_BUILDINGS = "Buildings";

		public const string JEWEL_ID_INVENTORY_TROOPS = "Troops";

		public const string JEWEL_ID_INVENTORY_CURRENCY = "Currency";

		public const string CONTAINER_PREFIX = "ContainerJewel";

		private const string LABEL_PREFIX = "LabelMessageCount";

		private const string LABEL_PREFIX_ALT = "LabelMesageCount";

		private const string SPRITE_PREFIX = "SpriteJewel";

		private const string GRADIENT_PREFIX = "SpriteJewelGradient";

		private const string ICON_PREFIX = "TextureJewel";

		private const string TIMER_PREFIX = "LabelTimer";

		private const int MAX_JEWEL_AMOUNT = 99;

		private const string JEWEL_NOTIFICATION_STRING = "!";

		private bool useMaxJewelAmount;

		private bool showCount;

		private UXElement container;

		private UXLabel label;

		private Animator jewelAnimator;

		private UXSprite jewelSprite;

		private UXSprite gradientSprite;

		private UXTexture iconSprite;

		private List<uint> activeViewTimers;

		private UXLabel jewelTimer;

		public int EndTime
		{
			get;
			set;
		}

		public int Value
		{
			set
			{
				this.container.Visible = (value > 0);
				string text;
				if (!this.showCount && value > 0)
				{
					text = "!";
				}
				else if (this.useMaxJewelAmount && value > 99)
				{
					text = 99.ToString() + "+";
				}
				else
				{
					text = value.ToString();
				}
				this.label.Text = text;
			}
		}

		public string Text
		{
			set
			{
				this.container.Visible = !string.IsNullOrEmpty(value);
				this.label.Text = value;
			}
		}

		public UXLabel TimerLabel
		{
			get
			{
				return this.jewelTimer;
			}
		}

		public string Color
		{
			set
			{
				if (this.jewelSprite == null)
				{
					Service.Logger.WarnFormat("No base sprite to set color of", new object[0]);
				}
				else
				{
					this.jewelSprite.Color = FXUtils.ConvertHexStringToColorObject(value);
				}
			}
		}

		public string GradientColor
		{
			set
			{
				if (this.gradientSprite == null)
				{
					Service.Logger.WarnFormat("No gradient sprite to set color of", new object[0]);
				}
				else
				{
					this.gradientSprite.Color = FXUtils.ConvertHexStringToColorObject(value);
				}
			}
		}

		public string Icon
		{
			set
			{
				if (this.iconSprite == null)
				{
					Service.Logger.WarnFormat("No icon sprite to set", new object[0]);
				}
				else
				{
					this.iconSprite.LoadTexture(value);
				}
			}
		}

		private JewelControl(UXElement c, UXLabel l, Animator a, UXSprite s, UXSprite g, UXTexture i, UXLabel t, bool useMaxJewelAmount, bool showCount, bool shouldHide)
		{
			this.container = c;
			this.label = l;
			this.jewelAnimator = a;
			this.jewelSprite = s;
			this.gradientSprite = g;
			this.iconSprite = i;
			this.jewelTimer = t;
			this.useMaxJewelAmount = useMaxJewelAmount;
			this.showCount = showCount;
			if (shouldHide)
			{
				this.Value = 0;
			}
			this.activeViewTimers = new List<uint>();
		}

		public static JewelControl Create(UXFactory uxFactory, string jewelName, string parentNameSuffix, bool showCount, bool shouldHide)
		{
			bool flag = jewelName == "Clans";
			string text = "ContainerJewel" + jewelName;
			UXElement optionalElement = uxFactory.GetOptionalElement<UXElement>(text);
			UXLabel optionalElement2 = uxFactory.GetOptionalElement<UXLabel>("LabelMessageCount" + jewelName);
			Animator a = null;
			if (optionalElement != null)
			{
				a = optionalElement.Root.GetComponent<Animator>();
			}
			else
			{
				Service.Logger.WarnFormat("No UI element found: " + text, new object[0]);
			}
			UXSprite optionalElement3 = uxFactory.GetOptionalElement<UXSprite>("SpriteJewel" + jewelName);
			UXTexture optionalElement4 = uxFactory.GetOptionalElement<UXTexture>("TextureJewel" + jewelName);
			if (string.IsNullOrEmpty(parentNameSuffix))
			{
				parentNameSuffix = jewelName;
			}
			UXLabel optionalElement5 = uxFactory.GetOptionalElement<UXLabel>("LabelTimer" + parentNameSuffix);
			UXSprite optionalElement6 = uxFactory.GetOptionalElement<UXSprite>("SpriteJewelGradient" + parentNameSuffix);
			if (optionalElement2 == null)
			{
				optionalElement2 = uxFactory.GetOptionalElement<UXLabel>("LabelMesageCount" + jewelName);
			}
			return (optionalElement != null && optionalElement2 != null) ? new JewelControl(optionalElement, optionalElement2, a, optionalElement3, optionalElement6, optionalElement4, optionalElement5, flag, showCount, shouldHide) : null;
		}

		public static JewelControl Create(UXFactory uxFactory, string name, string parentName)
		{
			return JewelControl.Create(uxFactory, name, parentName, true, true);
		}

		public static JewelControl Create(UXFactory uxFactory, string name)
		{
			return JewelControl.Create(uxFactory, name, null, true, true);
		}

		public static JewelControl CreateSticker(UXFactory uxFactory, string name, string parentName)
		{
			return JewelControl.Create(uxFactory, name, parentName, true, false);
		}

		public void SetAnimParamBool(string paramName, bool value)
		{
			if (this.jewelAnimator == null)
			{
				Service.Logger.WarnFormat("No animator on jewel control to set parameter", new object[0]);
				return;
			}
			if (this.jewelAnimator.isActiveAndEnabled)
			{
				this.jewelAnimator.SetBool(paramName, value);
			}
			else
			{
				KeyValuePair<string, bool> keyValuePair = new KeyValuePair<string, bool>(paramName, value);
				this.activeViewTimers.Add(Service.ViewTimerManager.CreateViewTimer(0.1f, false, new TimerDelegate(this.OnAnimReadyCheck), keyValuePair));
			}
		}

		public void Cleanup()
		{
			foreach (uint current in this.activeViewTimers)
			{
				Service.ViewTimerManager.KillViewTimer(current);
			}
		}

		private void OnAnimReadyCheck(uint id, object cookie)
		{
			KeyValuePair<string, bool> keyValuePair = (KeyValuePair<string, bool>)cookie;
			this.SetAnimParamBool(keyValuePair.Key, keyValuePair.Value);
			this.activeViewTimers.Remove(id);
		}
	}
}
