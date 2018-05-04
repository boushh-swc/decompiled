using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Holonet;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Holonet
{
	public class TransmissionsHolonetTab : AbstractHolonetTab
	{
		private const string RESEARCH_TITLE = "hn_transmission_research_title";

		private const string RESEARCH_BODY = "hn_transmission_research_body";

		private const string PCUPGRADE_TITLE = "hn_transmission_pcupgrade_title";

		private const string PCUPGRADE_BODY = "hn_transmission_pcupgrade_body";

		private const string HQUPGRADE_TITLE = "hn_transmission_hqupgrade_title";

		private const string HQUPGRADE_BODY = "hn_transmission_hqupgrade_body";

		private const string CONFLICT_TITLE = "hn_transmission_conflict_title";

		private const string CONFLICT_BODY = "hn_transmission_conflict_body";

		private const string SQUAD_CHANGE_TITLE = "hn_squad_application_accepted_title";

		private const string SQUAD_CHANGE_BODY = "hn_squad_application_accepted_body";

		private const string TRANSMISSIONS_LOG_TABLE = "TransmissionsLogTable";

		private const string TRANSMISSION_TEMPLATE_ITEM = "TransmissionsLogItem";

		private const string TRANSMISSION_ITEM_IMAGE = "TransmissionsLogThumbTexture";

		private const string TRANSMISSION_ITEM_TITLE = "TransmissionsLogItemLabelTitle";

		private const string TRANSMISSION_ITEM_BODY = "TransmissionsLogItemLabelBody";

		private const string TRANSMISSION_ITEM_BODY_NO_BTN = "TransmissionsLogItemLabelBodyNoBtn";

		private const string TRANSMISSIONS_ITEM_BTN_ACTION = "TransmissionsItemBtnAction";

		private const string TRANSMISSIONS_ITEM_BTN_ACTION_LABEL = "TransmissionsItemBtnActionLabel";

		private UXTable table;

		public TransmissionsHolonetTab(HolonetScreen holoScreen, HolonetControllerType holonetControllerType) : base(holoScreen, holonetControllerType)
		{
			base.InitializeTab("TransmissionsLogTab", "hn_transmissions_tab");
			List<TransmissionVO> transmissions = Service.HolonetController.TransmissionsController.Transmissions;
			int count = transmissions.Count;
			this.table = holoScreen.GetElement<UXTable>("TransmissionsLogTable");
			this.table.SetTemplateItem("TransmissionsLogItem");
			string title = string.Empty;
			string body = string.Empty;
			for (int i = 0; i < count; i++)
			{
				bool flag = false;
				TransmissionVO transmissionVO = transmissions[i];
				switch (transmissionVO.Type)
				{
				case TransmissionType.Research:
					title = this.lang.Get("hn_transmission_research_title", new object[]
					{
						this.GetResearchDisplayName(transmissionVO.TransData)
					});
					body = this.lang.Get("hn_transmission_research_body", new object[]
					{
						this.GetResearchDisplayName(transmissionVO.TransData),
						transmissionVO.Btn1Data
					});
					flag = true;
					break;
				case TransmissionType.HqUpgrade:
					title = this.lang.Get("hn_transmission_hqupgrade_title", new object[0]);
					body = this.lang.Get("hn_transmission_hqupgrade_body", new object[]
					{
						transmissionVO.Btn1Data
					});
					flag = true;
					break;
				case TransmissionType.NavigationCenterUpgrade:
					title = this.lang.Get("hn_transmission_pcupgrade_title", new object[0]);
					body = this.lang.Get("hn_transmission_pcupgrade_body", new object[]
					{
						transmissionVO.Btn1Data
					});
					flag = true;
					break;
				case TransmissionType.Conflict:
				{
					string text = string.Empty;
					string text2 = string.Empty;
					StaticDataController staticDataController = Service.StaticDataController;
					TournamentVO optional = staticDataController.GetOptional<TournamentVO>(transmissionVO.TransData);
					TournamentTierVO optional2 = staticDataController.GetOptional<TournamentTierVO>(transmissionVO.Btn1Data);
					if (optional != null && optional2 != null)
					{
						text = LangUtils.GetPlanetDisplayName(optional.PlanetId);
						text2 = this.lang.Get(optional2.RankName, new object[0]) + " " + this.lang.Get(optional2.Division, new object[0]);
					}
					title = this.lang.Get("hn_transmission_conflict_title", new object[0]);
					body = this.lang.Get("hn_transmission_conflict_body", new object[]
					{
						text,
						text2
					});
					flag = true;
					break;
				}
				case TransmissionType.WarPreparation:
					title = this.lang.Get(LangUtils.AppendPlayerFactionToKey("transm_war_prep_title"), new object[0]);
					body = this.lang.Get(LangUtils.AppendPlayerFactionToKey("transm_war_prep_body"), new object[0]);
					flag = true;
					break;
				case TransmissionType.WarStart:
					title = this.lang.Get(LangUtils.AppendPlayerFactionToKey("transm_war_start_title"), new object[0]);
					body = this.lang.Get(LangUtils.AppendPlayerFactionToKey("transm_war_start_body"), new object[0]);
					flag = true;
					break;
				case TransmissionType.WarEnded:
					title = this.lang.Get(LangUtils.AppendPlayerFactionToKey("transm_war_end_title"), new object[0]);
					body = this.lang.Get(LangUtils.AppendPlayerFactionToKey("transm_war_end_body"), new object[0]);
					flag = true;
					break;
				case TransmissionType.SquadChange:
				{
					object[] array = (transmissionVO.TransData ?? string.Empty).Split(new char[]
					{
						'\\'
					});
					if (array.Length > 1)
					{
						title = this.lang.Get("hn_squad_application_accepted_title", new object[]
						{
							array[1]
						});
						body = this.lang.Get("hn_squad_application_accepted_body", array);
					}
					flag = true;
					break;
				}
				case TransmissionType.GuildLevelUp:
					title = this.lang.Get("hn_perks_squad_level_up_title", new object[0]);
					body = this.lang.Get("hn_perks_squad_level_up_body", new object[]
					{
						transmissionVO.SquadLevel
					});
					flag = true;
					break;
				case TransmissionType.DailyCrateReward:
				{
					title = this.lang.Get("hn_daily_crate_reward_title", new object[0]);
					CrateVO crateVO = Service.StaticDataController.Get<CrateVO>(transmissionVO.CrateId);
					body = this.lang.Get("hn_daily_crate_reward_body", new object[]
					{
						LangUtils.GetCrateDisplayName(crateVO)
					});
					flag = true;
					break;
				}
				}
				if (flag)
				{
					this.AddCustomTransmission(transmissionVO, title, body, i);
				}
				else
				{
					this.AddGenericTransmission(transmissionVO, i);
				}
			}
		}

		private string GetResearchDisplayName(string uid)
		{
			string result = string.Empty;
			if (!string.IsNullOrEmpty(uid))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				TroopTypeVO optional = staticDataController.GetOptional<TroopTypeVO>(uid);
				if (optional != null)
				{
					result = LangUtils.GetTroopDisplayName(optional);
				}
				else
				{
					SpecialAttackTypeVO optional2 = staticDataController.GetOptional<SpecialAttackTypeVO>(uid);
					if (optional2 != null)
					{
						result = LangUtils.GetStarshipDisplayName(optional2);
					}
					else
					{
						EquipmentVO optional3 = staticDataController.GetOptional<EquipmentVO>(uid);
						if (optional3 != null)
						{
							result = LangUtils.GetEquipmentDisplayName(optional3);
						}
					}
				}
			}
			return result;
		}

		private void AddCustomTransmission(TransmissionVO vo, string title, string body, int order)
		{
			string uid = vo.Uid;
			if (!string.IsNullOrEmpty(uid))
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				string serverTransmissionMessageImage = GameUtils.GetServerTransmissionMessageImage(currentPlayer.Faction, currentPlayer.Planet.Uid);
				UXElement item = this.table.CloneTemplateItem(uid);
				this.table.GetSubElement<UXTexture>(uid, "TransmissionsLogThumbTexture").LoadTexture(serverTransmissionMessageImage);
				this.table.GetSubElement<UXLabel>(uid, "TransmissionsLogItemLabelTitle").Text = title;
				this.SetupTransmissionBody(this.table, vo, body);
				this.table.AddItem(item, order);
			}
		}

		private void AddGenericTransmission(TransmissionVO vo, int order)
		{
			string uid = vo.Uid;
			UXElement item = this.table.CloneTemplateItem(uid);
			this.table.GetSubElement<UXTexture>(uid, "TransmissionsLogThumbTexture").LoadTexture(vo.Image);
			this.table.GetSubElement<UXLabel>(uid, "TransmissionsLogItemLabelTitle").Text = this.lang.Get(vo.TitleText, new object[0]);
			string body = this.lang.Get(vo.BodyText, new object[0]);
			this.SetupTransmissionBody(this.table, vo, body);
			this.table.AddItem(item, order);
		}

		private void SetupTransmissionBody(UXTable table, TransmissionVO vo, string body)
		{
			UXLabel subElement = table.GetSubElement<UXLabel>(vo.Uid, "TransmissionsLogItemLabelBody");
			UXLabel subElement2 = table.GetSubElement<UXLabel>(vo.Uid, "TransmissionsLogItemLabelBodyNoBtn");
			if (string.IsNullOrEmpty(vo.Btn1))
			{
				subElement2.Text = body;
				subElement2.Visible = true;
				subElement.Text = string.Empty;
				subElement.Visible = false;
			}
			else
			{
				subElement2.Text = string.Empty;
				subElement2.Visible = false;
				subElement.Text = body;
				subElement.Visible = true;
			}
			UXButton subElement3 = table.GetSubElement<UXButton>(vo.Uid, "TransmissionsItemBtnAction");
			UXLabel subElement4 = table.GetSubElement<UXLabel>(vo.Uid, "TransmissionsItemBtnActionLabel");
			base.PrepareButton(vo, 1, subElement3, subElement4);
		}

		public override void OnDestroyTab()
		{
			if (this.table != null)
			{
				this.table.Clear();
				this.table = null;
			}
		}

		protected override void SetVisibleByTabButton(UXCheckbox button, bool selected)
		{
			base.SetVisibleByTabButton(button, selected);
			if (selected)
			{
				EventManager eventManager = Service.EventManager;
				eventManager.SendEvent(EventId.HolonetTransmissionLog, null);
			}
		}

		protected override void FeaturedButton1Clicked(UXButton button)
		{
			TransmissionVO transmissionVO = (TransmissionVO)button.Tag;
			Service.HolonetController.HandleCallToActionButton(transmissionVO.Btn1Action, transmissionVO.Btn1Data, transmissionVO.Uid);
			base.SendCallToActionBI(transmissionVO.Btn1Action, transmissionVO.Uid, EventId.HolonetTransmissionLog);
		}

		protected override void FeaturedButton2Clicked(UXButton button)
		{
			TransmissionVO transmissionVO = (TransmissionVO)button.Tag;
			Service.HolonetController.HandleCallToActionButton(transmissionVO.Btn2Action, transmissionVO.Btn2Data, transmissionVO.Uid);
			base.SendCallToActionBI(transmissionVO.Btn2Action, transmissionVO.Uid, EventId.HolonetTransmissionLog);
		}

		public override string GetBITabName()
		{
			return "transmission_log";
		}
	}
}
