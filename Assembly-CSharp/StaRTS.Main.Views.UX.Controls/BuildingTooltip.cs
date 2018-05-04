using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Controls
{
	public class BuildingTooltip : IViewFrameTimeObserver
	{
		private const float SHARPNESS_FACTOR = 4f;

		private const string TITLE_LABEL = "LabelName";

		private const string LEVEL_LABEL = "LabelLevel";

		private const string BUBBLE_LABEL = "Label";

		private const string BUBBLE_SPRITE = "SpriteBkg";

		private const string TIME_LABEL = "LabelTime";

		private const string PROGRESS_ELEMENT = "Pbar";

		private const string ICON_ELEMENT = "SpriteImage";

		private const string SELECTED_GROUP = "Selected";

		private UXLabel titleLabel;

		private UXLabel levelLabel;

		private UXLabel bubbleLabel;

		private UXSprite bubbleSprite;

		private UXLabel timeLabel;

		private UXSlider progressSlider;

		private UXSprite iconSprite;

		private GeometryProjector iconGeometry;

		private UXElement selectedGroup;

		private readonly Color redToolTipColor = new Color(0.737f, 0.074f, 0.074f);

		private float lastTimeLeft;

		private float lastTimeTotal;

		public UXElement TooltipElement
		{
			get;
			private set;
		}

		public BuildingTooltipKind Kind
		{
			get;
			private set;
		}

		public BuildingTooltip(UXFactory factory, UXElement tooltipElement, string subElementPrefix, string parentName, BuildingTooltipKind kind)
		{
			this.TooltipElement = tooltipElement;
			this.Kind = kind;
			this.lastTimeLeft = 0f;
			this.lastTimeTotal = 0f;
			string name = UXUtils.FormatAppendedName(subElementPrefix + "LabelName", parentName);
			this.titleLabel = factory.GetElement<UXLabel>(name);
			string name2 = UXUtils.FormatAppendedName(subElementPrefix + "LabelLevel", parentName);
			this.levelLabel = factory.GetElement<UXLabel>(name2);
			string name3 = UXUtils.FormatAppendedName(subElementPrefix + "Label", parentName);
			this.bubbleLabel = factory.GetOptionalElement<UXLabel>(name3);
			string name4 = UXUtils.FormatAppendedName(subElementPrefix + "SpriteBkg", parentName);
			this.bubbleSprite = factory.GetOptionalElement<UXSprite>(name4);
			string name5 = UXUtils.FormatAppendedName(subElementPrefix + "PbarLabelTime", parentName);
			this.timeLabel = factory.GetOptionalElement<UXLabel>(name5);
			string name6 = UXUtils.FormatAppendedName(subElementPrefix + "Pbar", parentName);
			this.progressSlider = factory.GetOptionalElement<UXSlider>(name6);
			string name7 = UXUtils.FormatAppendedName(subElementPrefix + "SpriteImage", parentName);
			this.iconSprite = factory.GetOptionalElement<UXSprite>(name7);
			string name8 = UXUtils.FormatAppendedName(subElementPrefix + "Selected", parentName);
			this.selectedGroup = factory.GetOptionalElement<UXElement>(name8);
			this.SetupBGBasedOnKind();
		}

		private void SetupBGBasedOnKind()
		{
			if (this.Kind == BuildingTooltipKind.HQBubble || this.Kind == BuildingTooltipKind.ShardUpgradeBubble)
			{
				this.bubbleSprite.Color = this.redToolTipColor;
			}
		}

		public void DestroyTooltip()
		{
			this.DestroyIconGeometry();
			this.EnableViewTimeObserving(false);
			Service.UXController.MiscElementsManager.DestroyBuildingTooltip(this);
		}

		private void DestroyIconGeometry()
		{
			if (this.iconGeometry != null)
			{
				this.iconGeometry.Destroy();
				this.iconGeometry = null;
			}
		}

		public void SetTitle(string title)
		{
			this.titleLabel.Text = title;
		}

		public void SetLevel(BuildingTypeVO building)
		{
			this.levelLabel.Text = string.Empty;
		}

		public void SetBubbleText(string bubbleText)
		{
			if (this.bubbleLabel != null)
			{
				this.bubbleLabel.Visible = !string.IsNullOrEmpty(bubbleText);
				this.bubbleLabel.Text = bubbleText;
				Vector4 border = this.bubbleSprite.Border;
				this.bubbleSprite.Width = this.bubbleLabel.TextWidth + border.x + border.z;
			}
		}

		public void SetIconAsset(IUpgradeableVO iconAsset)
		{
			if (this.iconSprite != null)
			{
				bool flag = iconAsset != null;
				this.iconSprite.Visible = flag;
				this.DestroyIconGeometry();
				if (flag)
				{
					ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(iconAsset, this.iconSprite);
					Service.EventManager.SendEvent(EventId.ButtonCreated, new GeometryTag(iconAsset, projectorConfig, Service.CurrentPlayer.ActiveArmory));
					projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
					projectorConfig.Sharpness = 4f;
					this.iconGeometry = ProjectorUtils.GenerateProjector(projectorConfig);
				}
			}
		}

		public void SetTime(int timeLeft)
		{
			if (this.timeLabel != null)
			{
				if (timeLeft < 0)
				{
					this.timeLabel.Visible = false;
				}
				else
				{
					this.timeLabel.Visible = true;
					this.timeLabel.Text = GameUtils.GetTimeLabelFromSeconds(timeLeft);
				}
			}
		}

		public void SetProgress(int timeLeft, int timeTotal)
		{
			if (this.progressSlider != null)
			{
				if (timeLeft <= 0 || timeTotal <= 0)
				{
					this.progressSlider.Visible = false;
					this.EnableViewTimeObserving(false);
					this.lastTimeLeft = 0f;
					this.lastTimeTotal = 0f;
				}
				else
				{
					this.progressSlider.Visible = true;
					this.EnableViewTimeObserving(true);
					this.lastTimeLeft = (float)timeLeft;
					this.lastTimeTotal = (float)timeTotal;
					this.InternalSetProgress();
				}
			}
		}

		private void InternalSetProgress()
		{
			this.progressSlider.Value = MathUtils.NormalizeRange(this.lastTimeTotal - this.lastTimeLeft, 0f, this.lastTimeTotal);
		}

		private void EnableViewTimeObserving(bool enable)
		{
			if (enable)
			{
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
			else
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		public void OnViewFrameTime(float dt)
		{
			this.lastTimeLeft -= dt;
			if (this.lastTimeLeft <= 0f)
			{
				this.lastTimeLeft = 0f;
				this.EnableViewTimeObserving(false);
			}
			this.InternalSetProgress();
		}

		public void SetSelected(bool selected)
		{
			if (this.selectedGroup != null)
			{
				this.selectedGroup.Visible = false;
			}
		}
	}
}
