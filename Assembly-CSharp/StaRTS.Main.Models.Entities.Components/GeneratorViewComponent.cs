using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Views;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.World;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.Entities.Components
{
	public class GeneratorViewComponent : ComponentBase
	{
		private const string TOOLTIP_NAME = "GeneratorCollectLabel";

		private const float TOOLTIP_SHOW_DURATION = 2f;

		private const float TIMER_INTERVAL_SEC = 0.01f;

		private const float HEIGHT_RISE = 1.5f;

		private GameObjectViewComponent viewComp;

		private CollectButton collectButton;

		private UXLabel textLabel;

		private uint textTimerId;

		private float timerDt;

		private TooltipHelper tooltipHelper;

		private static readonly Color CREDITS_COLOR = new Color(0.9372549f, 0.9843137f, 0f);

		private static readonly Color MATERIALS_COLOR = new Color(0.482352942f, 0.831372559f, 0.996078432f);

		private static readonly Color CONTRABAND_COLOR = new Color(0.819607854f, 0.184313729f, 0.419607848f);

		public bool Enabled
		{
			get;
			private set;
		}

		public GeneratorViewComponent(Entity entity)
		{
			this.viewComp = entity.Get<GameObjectViewComponent>();
			this.collectButton = new CollectButton(entity);
			UXController uXController = Service.UXController;
			this.textLabel = uXController.MiscElementsManager.CreateCollectionLabel("GeneratorCollectLabel", uXController.WorldAnchor);
			this.textLabel.Visible = false;
			this.textTimerId = 0u;
		}

		public void SetEnabled(bool enable)
		{
			this.Enabled = enable;
		}

		public void ShowCollectButton(bool show)
		{
			if (this.Enabled)
			{
				this.collectButton.Visible = show;
			}
		}

		public void ShowAmountCollectedText(int amount, CurrencyType currencyType)
		{
			this.textLabel.Visible = true;
			this.textLabel.Text = Service.Lang.Get("PLUS", new object[]
			{
				amount
			});
			switch (currencyType)
			{
			case CurrencyType.Credits:
				this.textLabel.TextColor = GeneratorViewComponent.CREDITS_COLOR;
				break;
			case CurrencyType.Materials:
				this.textLabel.TextColor = GeneratorViewComponent.MATERIALS_COLOR;
				break;
			case CurrencyType.Contraband:
				this.textLabel.TextColor = GeneratorViewComponent.CONTRABAND_COLOR;
				break;
			}
			this.KillTextTimer();
			if (this.tooltipHelper == null)
			{
				this.tooltipHelper = new TooltipHelper();
				this.tooltipHelper.SetupElements(this.viewComp, this.textLabel, 0f, true, false);
			}
			this.timerDt = 0f;
			this.textTimerId = Service.ViewTimerManager.CreateViewTimer(0.01f, true, new TimerDelegate(this.OnTextTimer), null);
		}

		private void OnTextTimer(uint id, object cookie)
		{
			this.timerDt += 0.01f;
			if (this.timerDt >= 2f || this.tooltipHelper == null)
			{
				this.textLabel.Visible = false;
				this.KillTextTimer();
			}
			else
			{
				float gameObjectHeight = this.viewComp.GameObjectHeight;
				float extraHeightOffGround = Easing.CubicEaseOut(this.timerDt, gameObjectHeight, gameObjectHeight + 1.5f, 2f);
				this.tooltipHelper.UpdateLocation(extraHeightOffGround, false);
			}
		}

		private void KillTextTimer()
		{
			if (this.textTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.textTimerId);
				this.textTimerId = 0u;
			}
		}

		public override void OnRemove()
		{
			if (this.collectButton != null)
			{
				this.collectButton.Destroy();
				this.collectButton = null;
			}
			this.KillTextTimer();
			if (this.textLabel != null)
			{
				Service.UXController.MiscElementsManager.DestroyMiscElement(this.textLabel);
				this.textLabel = null;
			}
			this.tooltipHelper = null;
		}
	}
}
