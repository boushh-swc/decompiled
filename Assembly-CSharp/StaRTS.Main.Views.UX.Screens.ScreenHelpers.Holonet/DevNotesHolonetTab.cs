using StaRTS.Main.Controllers.Holonet;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Holonet
{
	public class DevNotesHolonetTab : AbstractHolonetTab
	{
		private const string NOTES_TABLE = "NotesTable";

		private const string NOTES_ITEM = "NotesItem";

		private const string NOTES_ITEM_THUMB_SPRITE = "SpriteNotesThumbnailFrame";

		private const string NOTES_ITEM_LABEL_TITLE = "NotesItemLabelTitle";

		private const string NOTES_ITEM_LABEL_BODY = "NotesItemLabelBody";

		private const string NOTES_ITEM_BTN_ACTION = "NotesItemBtnAction";

		private const string NOTES_ITEM_BTN_ACTION_LABEL = "NotesItemBtnActionLabel";

		private const string NOTES_ITEM_BACK_COLLIDER = "NotesBackCollider";

		private const string NOTES_THUMB_TEXTURE = "NotesThumbTexture";

		private const string NOTES_THUMB_SPRITE = "SpriteNotesThumbnailFrame";

		private const string IMG_TAG = "[img]";

		private const string IMAGE_TOKEN = "IMAGE_TOKEN";

		private const int BUFFER_BETWEEN_IMAGES = 115;

		private List<UXLabel> newLabels = new List<UXLabel>();

		private UXTable notesTable;

		public DevNotesHolonetTab(HolonetScreen screen, HolonetControllerType holonetControllerType) : base(screen, holonetControllerType)
		{
			base.InitializeTab("NotesTab", "hn_devnotes_tab");
			this.notesTable = screen.GetElement<UXTable>("NotesTable");
			this.notesTable.SetTemplateItem("NotesItem");
			List<DevNoteEntryVO> devNotes = Service.HolonetController.DevNotesController.DevNotes;
			int i = 0;
			int count = devNotes.Count;
			while (i < count)
			{
				DevNoteEntryVO devNoteEntryVO = devNotes[i];
				UXElement item = this.notesTable.CloneTemplateItem(devNoteEntryVO.Uid);
				this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemLabelTitle").Text = this.lang.Get(devNoteEntryVO.TitleText, new object[0]);
				UXButton subElement = this.notesTable.GetSubElement<UXButton>(devNoteEntryVO.Uid, "NotesItemBtnAction");
				UXLabel subElement2 = this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemBtnActionLabel");
				base.PrepareButton(devNoteEntryVO, 1, subElement, subElement2);
				this.notesTable.GetSubElement<UXButton>(devNoteEntryVO.Uid, "NotesBackCollider").OnClicked = new UXButtonClickedDelegate(this.ClickedLink);
				UXTexture subElement3 = this.notesTable.GetSubElement<UXTexture>(devNoteEntryVO.Uid, "NotesThumbTexture");
				UXSprite subElement4 = this.notesTable.GetSubElement<UXSprite>(devNoteEntryVO.Uid, "SpriteNotesThumbnailFrame");
				string text = this.lang.Get(devNoteEntryVO.BodyText, new object[0]);
				List<string> list = new List<string>();
				List<UXTexture> list2 = new List<UXTexture>();
				MiscElementsManager miscElementsManager = Service.UXController.MiscElementsManager;
				if (text.Contains("src="))
				{
					string[] separator = new string[]
					{
						"[img]"
					};
					string[] array = text.Split(separator, StringSplitOptions.None);
					int j = 0;
					int num = array.Length;
					while (j < num)
					{
						list.Add(array[j]);
						j++;
					}
					this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemLabelBody").Text = string.Empty;
				}
				else
				{
					this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemLabelBody").Text = text;
				}
				int k = 0;
				int count2 = list.Count;
				while (k < count2)
				{
					if (list[k].Contains("src="))
					{
						UXTexture uXTexture = miscElementsManager.CloneUXTexture(subElement3, "newImage #" + k, subElement3.Root.transform.parent.gameObject);
						string[] array2 = list[k].Split(new char[]
						{
							'='
						});
						string assetName = array2[1];
						uXTexture.Visible = true;
						uXTexture.LoadTexture(assetName);
						list2.Add(uXTexture);
						string oldValue = "[img]" + list[k] + "[img]";
						text = text.Replace(oldValue, "IMAGE_TOKEN");
					}
					k++;
				}
				if (text.Contains("IMAGE_TOKEN"))
				{
					string[] separator2 = new string[]
					{
						"IMAGE_TOKEN"
					};
					string[] array3 = text.Split(separator2, StringSplitOptions.None);
					int l = 0;
					int num2 = array3.Length;
					while (l < num2)
					{
						UXLabel uXLabel = miscElementsManager.CloneUXLabel(this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemLabelBody"), "thisLabel #" + l, subElement3.Root.transform.parent.gameObject);
						uXLabel.LocalPosition = new Vector2(this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemLabelBody").LocalPosition.x, this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemLabelBody").LocalPosition.y);
						uXLabel.Text = array3[l];
						if (list2.Count > 0)
						{
							list2[0].LocalPosition = new Vector2(uXLabel.LocalPosition.x - 115f * uXLabel.UXCamera.Camera.transform.localScale.x, uXLabel.LocalPosition.y - uXLabel.Height);
							list2.Remove(list2[0]);
						}
						UXButton uXButton = miscElementsManager.CloneUXButton(this.notesTable.GetSubElement<UXButton>(devNoteEntryVO.Uid, "NotesBackCollider"), "thisButton #" + l, subElement3.Root.transform.parent.gameObject);
						uXButton.LocalPosition = uXLabel.LocalPosition;
						uXButton.GetUIWidget.SetAnchor(uXLabel.GetUIWidget.transform);
						uXButton.OnClicked = new UXButtonClickedDelegate(this.ClickedLink);
						this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemLabelBody").LocalPosition = new Vector2(this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemLabelBody").LocalPosition.x, this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemLabelBody").LocalPosition.y - uXLabel.Height);
						this.newLabels.Add(uXLabel);
						l++;
					}
				}
				else
				{
					this.newLabels.Add(this.notesTable.GetSubElement<UXLabel>(devNoteEntryVO.Uid, "NotesItemLabelBody"));
				}
				if (!string.IsNullOrEmpty(devNoteEntryVO.Image))
				{
					subElement3.Visible = true;
					subElement4.Visible = true;
					subElement3.LoadTexture(devNoteEntryVO.Image);
				}
				else
				{
					subElement3.Visible = false;
					subElement4.Visible = false;
				}
				this.notesTable.AddItem(item, this.notesTable.Count);
				i++;
			}
		}

		private void ClickedLink(UXButton btn)
		{
			int i = 0;
			int count = this.newLabels.Count;
			while (i < count)
			{
				string getURLAtPosition = this.newLabels[i].GetURLAtPosition;
				if (getURLAtPosition != null)
				{
					GameUtils.OpenURL(getURLAtPosition);
				}
				i++;
			}
		}

		public override void OnDestroyTab()
		{
			if (this.notesTable != null)
			{
				this.notesTable.Clear();
				this.notesTable = null;
			}
		}

		protected override void SetVisibleByTabButton(UXCheckbox button, bool selected)
		{
			base.SetVisibleByTabButton(button, selected);
			if (selected)
			{
				this.eventManager.SendEvent(EventId.HolonetDevNotes, "tab");
			}
		}

		public override void OnTabOpen()
		{
			base.OnTabOpen();
			this.notesTable.RepositionItemsFrameDelayed();
		}

		protected override void FeaturedButton1Clicked(UXButton button)
		{
			DevNoteEntryVO devNoteEntryVO = (DevNoteEntryVO)button.Tag;
			Service.HolonetController.HandleCallToActionButton(devNoteEntryVO.Btn1Action, devNoteEntryVO.Btn1Data, devNoteEntryVO.Uid);
			base.SendCallToActionBI(devNoteEntryVO.Btn1Action, devNoteEntryVO.Uid, EventId.HolonetDevNotes);
		}

		protected override void FeaturedButton2Clicked(UXButton button)
		{
			DevNoteEntryVO devNoteEntryVO = (DevNoteEntryVO)button.Tag;
			Service.HolonetController.HandleCallToActionButton(devNoteEntryVO.Btn2Action, devNoteEntryVO.Btn2Data, devNoteEntryVO.Uid);
			base.SendCallToActionBI(devNoteEntryVO.Btn2Action, devNoteEntryVO.Uid, EventId.HolonetDevNotes);
		}

		public override string GetBITabName()
		{
			return "dev_notes";
		}
	}
}
