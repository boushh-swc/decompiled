using StaRTS.Main.Models;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class TroopTooltipHelper
	{
		private Dictionary<UXButton, string> buttonTooltipTextMap;

		public void RegisterButtonTooltip(UXButton button, IUpgradeableVO vo, BattleEntry battle)
		{
			string tooltipText = null;
			if (vo is SpecialAttackTypeVO)
			{
				tooltipText = LangUtils.GetStarshipDisplayName((SpecialAttackTypeVO)vo);
			}
			else if (vo is TroopTypeVO)
			{
				tooltipText = LangUtils.GetTroopDisplayName((TroopTypeVO)vo);
			}
			GeometryTag geometryTag = new GeometryTag(vo, tooltipText, battle);
			Service.EventManager.SendEvent(EventId.TooltipCreated, geometryTag);
			if (geometryTag.tooltipText != null)
			{
				this.RegisterButtonTooltip(button, geometryTag.tooltipText);
			}
		}

		public void RegisterButtonTooltip(UXButton button, string localizedText)
		{
			if (this.buttonTooltipTextMap == null)
			{
				this.buttonTooltipTextMap = new Dictionary<UXButton, string>();
			}
			this.buttonTooltipTextMap.Add(button, localizedText);
			button.OnPressed = new UXButtonPressedDelegate(this.OnTooltipButtonPressed);
			button.OnReleased = new UXButtonReleasedDelegate(this.OnTooltipButtonReleased);
		}

		private void OnTooltipButtonPressed(UXButton button)
		{
			string localizedText = this.buttonTooltipTextMap[button];
			Service.UXController.MiscElementsManager.ShowTroopTooltip(button, localizedText);
		}

		private void OnTooltipButtonReleased(UXButton button)
		{
			Service.UXController.MiscElementsManager.HideTroopTooltip();
		}

		public void Destroy()
		{
			if (this.buttonTooltipTextMap != null)
			{
				foreach (KeyValuePair<UXButton, string> current in this.buttonTooltipTextMap)
				{
					UXButton key = current.Key;
					key.OnPressed = null;
					key.OnReleased = null;
				}
				this.buttonTooltipTextMap = null;
			}
		}
	}
}
