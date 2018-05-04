using StaRTS.Assets;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class EquipmentUnlockedCelebrationScreen : ScreenBase, IViewFrameTimeObserver
	{
		private const int CENTER_THRESHOLD = 7;

		private const float OUTLINE_WIDTH = 0.0008f;

		private const float FAR_Y = 300f;

		private const string BUTTON_CONTINUE = "ButtonPrimaryAction";

		private const string LABEL_EQUIPMENT_NAME = "LabelEquipmentName";

		private const string LABEL_EQUIPMENT_UNLOCK_TITLE = "LabelEquipmentText";

		private const string LABEL_INSTRUCTIONS = "LabelInstructions";

		private const string SPRITE_INSTRUCTIONS = "SpriteInstructions";

		private const string UNLOCK_EQUIPMENT_TITLE = "UNLOCK_EQUIPMENT_TITLE";

		private const string UNLOCK_INSTRUCTIONS = "UNLOCK_INSTRUCTIONS";

		protected EquipmentVO subjectVO;

		private GameObject rig;

		private GameObject subject;

		private AssetHandle rigHandle;

		private AssetHandle subjectHandle;

		private List<Material> outLineMatList = new List<Material>();

		private bool fadingOutSubject;

		protected UXButton buttonContinue;

		private UXGrid itemGrid;

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public EquipmentUnlockedCelebrationScreen(EquipmentVO vo) : base("gui_equipment_unlocked")
		{
			this.subjectVO = vo;
		}

		protected override void OnScreenLoaded()
		{
			Service.UXController.HUD.Visible = false;
			this.InitButtons();
			this.SetUIText();
			this.LoadFx();
			UXSprite element = base.GetElement<UXSprite>("SpriteInstructions");
			ArmoryNode head = Service.BuildingLookupController.ArmoryNodeList.Head;
			if (head != null)
			{
				BuildingTypeVO buildingType = head.BuildingComp.BuildingType;
				ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(buildingType, element, false);
				projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
				ProjectorUtils.GenerateProjector(projectorConfig);
			}
			this.fadingOutSubject = false;
		}

		public override void OnDestroyElement()
		{
			if (this.itemGrid != null)
			{
				this.itemGrid.Clear();
				this.itemGrid = null;
			}
			base.OnDestroyElement();
			if (this.subject != null)
			{
				UnityEngine.Object.Destroy(this.subject);
				this.subject = null;
			}
			if (this.rig != null)
			{
				UnityEngine.Object.Destroy(this.rig);
				this.rig = null;
			}
			int i = 0;
			int count = this.outLineMatList.Count;
			while (i < count)
			{
				Material material = this.outLineMatList[i];
				if (material != null)
				{
					UnityUtils.DestroyMaterial(material);
				}
				i++;
			}
			this.outLineMatList.Clear();
			if (this.subjectHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.subjectHandle);
				this.subjectHandle = AssetHandle.Invalid;
			}
			if (this.rigHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.rigHandle);
				this.rigHandle = AssetHandle.Invalid;
			}
			if (this.fadingOutSubject)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		private void InitButtons()
		{
			this.buttonContinue = base.GetElement<UXButton>("ButtonPrimaryAction");
			this.buttonContinue.OnClicked = new UXButtonClickedDelegate(this.OnButtonContinueClicked);
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnButtonContinueClicked);
			base.CurrentBackButton = this.buttonContinue;
			base.AllowFUEBackButton = true;
			Service.UserInputInhibitor.AddToAllow(this.buttonContinue);
		}

		private void SetUIText()
		{
			base.GetElement<UXLabel>("LabelEquipmentName").Text = LangUtils.GetEquipmentDisplayName(this.subjectVO);
			base.GetElement<UXLabel>("LabelEquipmentText").Text = this.lang.Get("UNLOCK_EQUIPMENT_TITLE", new object[0]);
			base.GetElement<UXLabel>("LabelInstructions").Text = this.lang.Get("UNLOCK_INSTRUCTIONS", new object[0]);
		}

		private void LoadFx()
		{
			Service.AssetManager.Load(ref this.rigHandle, "gui_celebrationunlockrig", new AssetSuccessDelegate(this.ShowFx), null, null);
		}

		private void ShowFx(object asset, object cookie)
		{
			this.rig = (GameObject)asset;
			this.rig = UnityEngine.Object.Instantiate<GameObject>(this.rig);
			this.rig.transform.position = new Vector3(0f, 300f, 0f);
			IGeometryVO geometryVO = ProjectorUtils.DetermineVOForEquipment(this.subjectVO);
			Service.AssetManager.Load(ref this.subjectHandle, geometryVO.IconAssetName, new AssetSuccessDelegate(this.ArrangeRig), null, null);
		}

		private void ArrangeRig(object asset, object cookie)
		{
			this.subject = (GameObject)asset;
			GameObject gameObject = UnityUtils.FindGameObject(this.rig, "content_scaler");
			GameObject gameObject2 = UnityUtils.FindGameObject(this.rig, "CelebrationCamera");
			GameObject gameObject3 = UnityUtils.FindGameObject(this.rig, "BuildingDisplayRig");
			string shaderName = "Outline_Unlit";
			Shader shader = Service.AssetManager.Shaders.GetShader(shaderName);
			if (this.subject != null && gameObject != null)
			{
				this.subject.transform.parent = gameObject.transform;
				this.subject.transform.localPosition = this.subjectVO.IconUnlockPosition;
				this.subject.transform.localEulerAngles = this.subjectVO.IconUnlockRotation;
				this.subject.transform.localScale = this.subjectVO.IconUnlockScale;
				AssetMeshDataMonoBehaviour component = this.subject.GetComponent<AssetMeshDataMonoBehaviour>();
				if (component == null)
				{
					return;
				}
				int i = 0;
				int count = component.SelectableGameObjects.Count;
				while (i < count)
				{
					Renderer component2 = component.SelectableGameObjects[i].GetComponent<Renderer>();
					if (component2 != null)
					{
						Material material = UnityUtils.EnsureMaterialCopy(component2);
						this.outLineMatList.Add(material);
						if (material != null)
						{
							material.shader = shader;
							material.SetColor("_OutlineColor", FXUtils.SELECTION_OUTLINE_COLOR);
							material.SetFloat("_Outline", 0.0008f);
						}
					}
					i++;
				}
				if (component.ShadowGameObject != null)
				{
					component.ShadowGameObject.SetActive(false);
				}
			}
			if (gameObject2 != null)
			{
				gameObject2.GetComponent<Camera>().depth = 1f;
			}
			if (gameObject3 != null)
			{
				Animator component3 = gameObject3.GetComponent<Animator>();
				if (component3)
				{
					component3.enabled = true;
					component3.SetTrigger("Show");
				}
			}
			Service.EventManager.SendEvent(EventId.EquipmentUnlockCelebrationPlayed, null);
		}

		private void OnButtonContinueClicked(UXButton button)
		{
			this.Close(true);
		}

		public override void Close(object modalResult)
		{
			base.Close(modalResult);
			if (this.WantTransitions)
			{
				this.FadeOutSubject();
			}
		}

		private void FadeOutSubject()
		{
			if (this.fadingOutSubject || this.outLineMatList.Count == 0)
			{
				return;
			}
			Shader shader = Service.AssetManager.Shaders.GetShader("UnlitTexture_Fade");
			int i = 0;
			int count = this.outLineMatList.Count;
			while (i < count)
			{
				Material material = this.outLineMatList[i];
				if (material != null)
				{
					material.shader = shader;
				}
				i++;
			}
			this.fadingOutSubject = true;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void OnViewFrameTime(float dt)
		{
			float alpha = base.GetAlpha();
			if (alpha > 0f)
			{
				int i = 0;
				int count = this.outLineMatList.Count;
				while (i < count)
				{
					Material material = this.outLineMatList[i];
					if (material != null)
					{
						material.color = new Color(1f, 1f, 1f, alpha);
					}
					i++;
				}
			}
			else
			{
				this.fadingOutSubject = false;
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}
	}
}
