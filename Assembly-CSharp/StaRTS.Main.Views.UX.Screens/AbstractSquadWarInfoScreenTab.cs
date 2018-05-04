using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class AbstractSquadWarInfoScreenTab
	{
		protected const int TEXTURE_ARG = 0;

		protected const int TITLE_ARG = 1;

		protected const int LABEL_ARG = 1;

		protected const int BODY_ARG = 2;

		protected const char DELIMTER1 = '|';

		protected const char DELIMTER2 = ',';

		protected SquadWarInfoScreen parent;

		public UXCheckbox TabButton;

		public UXSprite TabHighlight;

		protected UXElement topGroup;

		public AbstractSquadWarInfoScreenTab(SquadWarInfoScreen parent, UXCheckbox tabButton, UXElement topGroup)
		{
			this.parent = parent;
			this.TabButton = tabButton;
			this.topGroup = topGroup;
			tabButton.OnSelected = new UXCheckboxSelectedDelegate(this.TabButtonOnSelect);
		}

		public virtual void ShowContents(bool showNotHide)
		{
			this.topGroup.Visible = showNotHide;
		}

		protected void TabButtonOnSelect(UXCheckbox button, bool selected)
		{
			this.ShowContents(selected);
			this.parent.OnTabShown(this);
		}

		protected void PopulateBgTexture(string textureUid, string textureName)
		{
			TextureVO optional = Service.StaticDataController.GetOptional<TextureVO>(textureUid);
			if (optional == null)
			{
				Service.Logger.WarnFormat("Cannot find texture with uid {0}", new object[]
				{
					textureUid
				});
				return;
			}
			this.parent.GetElement<UXTexture>(textureName).LoadTexture(optional.AssetName);
		}

		protected void PopulateGrid(string[] rawSplit, string gridName, string itemName, string itemTextureName, string itemTitleName, string itemBodyName)
		{
			UXGrid element = this.parent.GetElement<UXGrid>(gridName);
			element.SetTemplateItem(itemName);
			int i = 1;
			int num = rawSplit.Length;
			while (i < num)
			{
				string itemUid = i.ToString();
				UXElement item = element.CloneTemplateItem(itemUid);
				element.AddItem(item, i - 1);
				string[] array = rawSplit[i].Split(new char[]
				{
					','
				});
				UXTexture subElement = element.GetSubElement<UXTexture>(itemUid, itemTextureName);
				TextureVO optional = Service.StaticDataController.GetOptional<TextureVO>(array[0]);
				if (optional == null)
				{
					Service.Logger.WarnFormat("Cannot find texture with uid {0}", new object[]
					{
						array[0]
					});
				}
				else
				{
					subElement.LoadTexture(optional.AssetName);
				}
				Lang lang = Service.Lang;
				element.GetSubElement<UXLabel>(itemUid, itemTitleName).Text = lang.Get(array[1], new object[0]);
				element.GetSubElement<UXLabel>(itemUid, itemBodyName).Text = lang.Get(array[2], new object[0]);
				i++;
			}
			element.RepositionItems();
		}
	}
}
