using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class StructuresTabHelper : AbstractTabHelper
	{
		private const string FILTER_PANEL = "FilterPanel";

		private const string FILTER_GRID = "FilterGrid";

		private const string FILTER_TEMPLATE = "FilterTemplate";

		private const string FILTER_BUTTON = "BtnFilter";

		private const string FILTER_BUTTON_LABEL = "LabelBtnFilter";

		public StructuresTabHelper() : base("FilterPanel", "FilterGrid", "FilterTemplate", "BtnFilter", "LabelBtnFilter")
		{
		}
	}
}
