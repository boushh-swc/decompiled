using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class EpisodePointsHelpScreen : ClosableScreen
	{
		private const string LABEL_TITLE = "LabelTitle";

		private const string LABEL_BODY = "LabelEventPointsBody";

		private const string PVP_STRING_UID = "gui_event_points_help_pvp";

		private const string RAID_STRING_UID = "gui_event_points_help_raid";

		private const string OBJECTIVES_STRING_UID = "gui_event_points_help_objectives";

		private const string CONFLICTS_STRING_UID = "gui_event_points_help_conflicts";

		private const string PVP_TEXTURE_UID = "gui_event_points_help_pvp_icon";

		private const string RAID_TEXTURE_UID = "gui_event_points_help_raid_icon";

		private const string OBJECTIVES_TEXTURE_UID = "gui_event_points_help_objectives_icon";

		private const string CONFLICTS_TEXTURE_UID = "gui_event_points_help_conflicts_icon";

		private const string TITLE_TEXT = "gui_event_points_help_title";

		private const string HEADER_TEXT = "gui_event_points_help_subtitle";

		private const string BODY_TEXT = "gui_event_points_help_body";

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public EpisodePointsHelpScreen() : base("gui_event_points_help")
		{
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			this.InitLabels();
		}

		private void SetupCategory(string labelName, string stringUid, string textureName, string textureUid, int[] epList)
		{
			object[] args = Array.ConvertAll<int, object>(epList, (int item) => item);
			base.GetElement<UXLabel>(labelName).Text = this.lang.Get(stringUid, args);
			TextureVO optional = Service.StaticDataController.GetOptional<TextureVO>(textureUid);
			UXTexture element = base.GetElement<UXTexture>(textureName);
			element.LoadTexture(optional.AssetName);
		}

		private void InitLabels()
		{
			Lang lang = Service.Lang;
			base.GetElement<UXLabel>("LabelTitle").Text = lang.Get("gui_event_points_help_title", new object[0]);
			base.GetElement<UXLabel>("LabelEventPointsBody").Text = lang.Get("gui_event_points_help_body", new object[0]);
		}
	}
}
