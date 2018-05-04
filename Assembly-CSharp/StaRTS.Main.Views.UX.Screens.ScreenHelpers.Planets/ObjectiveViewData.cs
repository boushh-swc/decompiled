using StaRTS.Main.Models.Player.Objectives;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets
{
	public class ObjectiveViewData
	{
		public ObjectiveProgress Objective;

		public UXLabel TitleLabel;

		public UXLabel StatusLabel;

		public UXLabel ExpiredLabel;

		public UXSprite RadialProgress;

		public UXSlider ProgressSlider;

		public UXButton BtnSupplyCrate;

		public UXSprite SpriteCheckmark;

		public UXSprite SpritePreview;

		public UXSprite SpriteSupplyCrate;

		public UXSprite SpriteObjectiveIcon;

		public UXElement ObjectiveBgComplete;

		public UXElement ObjectiveBgActive;

		public UXElement ObjectiveBgCollected;

		public UXElement ObjectiveBgExpired;

		public UXSprite SpriteObjectiveExpired;

		public GeometryProjector GeoControlCrate;

		public GeometryProjector GeoControlIcon;

		public UXElement SpecailObjectiveFx;

		public UXElement ObjectiveContainer;

		public UXElement ObjectiveContainerLEI;
	}
}
