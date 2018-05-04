using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Controls
{
	public class SliderControl
	{
		private const string LABEL_X = "LabelpBar{0}";

		private const string LABEL_X_CURRENT = "LabelpBar{0}Current";

		private const string LABEL_X_NEXT = "LabelpBar{0}Next";

		private const string SLIDER_X_CURRENT = "pBarCurrent{0}";

		private const string SLIDER_X_NEXT = "pBarNext{0}";

		private const string BACKGROUND = "SpritepBarBkg{0}";

		private const string STORAGE = "Storage";

		private const string TURRET = "Turret";

		public UXSlider CurrentSlider
		{
			get;
			private set;
		}

		public UXSlider NextSlider
		{
			get;
			private set;
		}

		public UXSprite Background
		{
			get;
			private set;
		}

		public UXLabel DescLabel
		{
			get;
			private set;
		}

		public UXLabel CurrentLabel
		{
			get;
			private set;
		}

		public UXLabel NextLabel
		{
			get;
			private set;
		}

		public SliderControl(UXFactory factory, string num, bool forStorage, bool forTurret, bool forUpgrade)
		{
			this.CurrentSlider = factory.GetElement<UXSlider>(this.GetFormattedName("pBarCurrent{0}", num, forStorage, forTurret));
			this.NextSlider = factory.GetElement<UXSlider>(this.GetFormattedName("pBarNext{0}", num, forStorage, forTurret));
			this.Background = factory.GetElement<UXSprite>(this.GetFormattedName("SpritepBarBkg{0}", num, forStorage, forTurret));
			this.DescLabel = factory.GetElement<UXLabel>(this.GetFormattedName("LabelpBar{0}", num, forStorage, forTurret));
			this.CurrentLabel = factory.GetElement<UXLabel>(this.GetFormattedName("LabelpBar{0}Current", num, forStorage, forTurret));
			this.NextLabel = factory.GetElement<UXLabel>(this.GetFormattedName("LabelpBar{0}Next", num, forStorage, forTurret));
			if (!forUpgrade)
			{
				this.NextSlider.Visible = false;
				this.NextLabel.Visible = false;
			}
		}

		private string GetFormattedName(string elementName, string num, bool forStorage, bool forTurret)
		{
			if (forStorage)
			{
				elementName += "Storage";
			}
			else if (forTurret)
			{
				elementName += "Turret";
			}
			return string.Format(elementName, num);
		}

		public void HideAll()
		{
			this.CurrentSlider.Visible = false;
			this.NextSlider.Visible = false;
			this.Background.Visible = false;
			this.DescLabel.Visible = false;
			this.CurrentLabel.Visible = false;
			this.NextLabel.Visible = false;
		}
	}
}
