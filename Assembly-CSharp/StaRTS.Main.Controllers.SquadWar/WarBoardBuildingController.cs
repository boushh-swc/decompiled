using StaRTS.Assets;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Main.Views.UserInput;
using StaRTS.Main.Views.UX.Squads;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.SquadWar
{
	public class WarBoardBuildingController : IUserInputObserver, IViewFrameTimeObserver
	{
		private const string HOLO_DISTANCE = "_Distance";

		private const string HOLO_OPACITY = "_Opacity";

		private const float HOLO_DISTANCE_VALUE = 139.5f;

		private const float HOLO_OPACITY_DIM_VALUE = 0.3f;

		private const float MIN_SELECTION_X_ROTATION = -7.7f;

		private const float MAX_SELECTION_X_ROTATION = 1.9f;

		private const float FACTORY_OUTPOST_ROTATION_SPEED = 0.5f;

		private const string GAMEOBJECT_PARENT_PREFIX = "WarBoardBuilding_";

		private const int FINGER_ID = 0;

		private const float TIME_TO_SELECT = 1f;

		private Dictionary<SquadWarBoardBuilding, SquadWarParticipantState> participantBuildingsPlayer;

		private Dictionary<SquadWarBoardBuilding, SquadWarParticipantState> participantBuildingsOpponent;

		private List<AssetHandle> assetHandles;

		private List<Material> unsharedMaterials;

		private string empireHQId;

		private string rebelHQId;

		private string smugglerHQId;

		private GameObject pressedBuilding;

		private uint pressTimerId;

		private Vector2 pressScreenPosition;

		private GameObject selectedBuilding;

		private OutlinedAsset outline;

		private bool dragged;

		public FactionType currentFaction
		{
			get;
			private set;
		}

		public SquadWarSquadType currentSquadType
		{
			get;
			private set;
		}

		public WarBoardBuildingController()
		{
			Service.WarBoardBuildingController = this;
			this.participantBuildingsPlayer = new Dictionary<SquadWarBoardBuilding, SquadWarParticipantState>();
			this.participantBuildingsOpponent = new Dictionary<SquadWarBoardBuilding, SquadWarParticipantState>();
			this.unsharedMaterials = new List<Material>();
		}

		public void ShowBuildings(List<Transform> warBuildingLocators)
		{
			if (this.assetHandles == null)
			{
				this.assetHandles = new List<AssetHandle>();
			}
			if (this.outline == null)
			{
				this.outline = new OutlinedAsset();
			}
			this.pressedBuilding = null;
			this.pressTimerId = 0u;
			this.pressScreenPosition = Vector2.zero;
			this.selectedBuilding = null;
			this.dragged = false;
			this.CacheHQBuildingIds();
			BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
			AssetManager assetManager = Service.AssetManager;
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarSquadData squadData = warManager.GetSquadData(SquadWarSquadType.PLAYER_SQUAD);
			this.AddBuildingsForParticipants(squadData, true, buildingUpgradeCatalog, assetManager, warBuildingLocators);
			SquadWarSquadData squadData2 = warManager.GetSquadData(SquadWarSquadType.OPPONENT_SQUAD);
			this.AddBuildingsForParticipants(squadData2, false, buildingUpgradeCatalog, assetManager, warBuildingLocators);
			Service.UserInputManager.RegisterObserver(this, UserInputLayer.World);
		}

		public void DestroyBuildings()
		{
			if (this.participantBuildingsPlayer != null)
			{
				foreach (KeyValuePair<SquadWarBoardBuilding, SquadWarParticipantState> current in this.participantBuildingsPlayer)
				{
					current.Key.Destroy();
				}
				this.participantBuildingsPlayer.Clear();
			}
			if (this.participantBuildingsOpponent != null)
			{
				foreach (KeyValuePair<SquadWarBoardBuilding, SquadWarParticipantState> current2 in this.participantBuildingsOpponent)
				{
					current2.Key.Destroy();
				}
				this.participantBuildingsOpponent.Clear();
			}
			if (this.assetHandles != null)
			{
				AssetManager assetManager = Service.AssetManager;
				int i = 0;
				int count = this.assetHandles.Count;
				while (i < count)
				{
					assetManager.Unload(this.assetHandles[i]);
					i++;
				}
				this.assetHandles.Clear();
			}
			if (this.unsharedMaterials != null)
			{
				int j = 0;
				int count2 = this.unsharedMaterials.Count;
				while (j < count2)
				{
					UnityUtils.DestroyMaterial(this.unsharedMaterials[j]);
					j++;
				}
				this.unsharedMaterials.Clear();
			}
			if (this.outline != null)
			{
				this.outline.Cleanup();
			}
			this.ResetPressedBuilding();
			this.selectedBuilding = null;
			Service.UserInputManager.UnregisterObserver(this, UserInputLayer.World);
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		public SquadWarParticipantState GetParticipantState(GameObject building)
		{
			Dictionary<SquadWarBoardBuilding, SquadWarParticipantState> currentParticipantBuildingList = this.GetCurrentParticipantBuildingList();
			foreach (KeyValuePair<SquadWarBoardBuilding, SquadWarParticipantState> current in currentParticipantBuildingList)
			{
				if (current.Key.Building == building)
				{
					return current.Value;
				}
			}
			return null;
		}

		public void DisableBuilding(string squadMemberId)
		{
			this.DisableBuilding(this.FindBuildingForParticipant(squadMemberId));
		}

		public void DisableBuilding(GameObject building)
		{
			if (building != null)
			{
				Renderer componentInChildren = building.GetComponentInChildren<Renderer>();
				if (componentInChildren != null)
				{
					componentInChildren.sharedMaterial.SetFloat("_Opacity", 0.3f);
				}
			}
		}

		public GameObject SelectBuilding(string squadMemberId)
		{
			GameObject gameObject = this.FindBuildingForParticipant(squadMemberId);
			if (gameObject != null)
			{
				this.SelectBuilding(gameObject);
			}
			return gameObject;
		}

		public void DeselectBuilding()
		{
			if (this.selectedBuilding != null)
			{
				this.DeselectSelectedBuilding();
			}
		}

		private Dictionary<SquadWarBoardBuilding, SquadWarParticipantState> GetCurrentParticipantBuildingList()
		{
			if (this.currentSquadType == SquadWarSquadType.PLAYER_SQUAD)
			{
				return this.participantBuildingsPlayer;
			}
			return this.participantBuildingsOpponent;
		}

		public GameObject FindBuildingForParticipant(string squadMemberId)
		{
			Dictionary<SquadWarBoardBuilding, SquadWarParticipantState> currentParticipantBuildingList = this.GetCurrentParticipantBuildingList();
			foreach (KeyValuePair<SquadWarBoardBuilding, SquadWarParticipantState> current in currentParticipantBuildingList)
			{
				if (current.Value.SquadMemberId == squadMemberId)
				{
					return current.Key.Building;
				}
			}
			return null;
		}

		private void AddBuildingsForParticipants(SquadWarSquadData squadData, bool isForPlayerSquad, BuildingUpgradeCatalog catalog, AssetManager assetManager, List<Transform> warBuildingLocators)
		{
			List<SquadWarParticipantState> list = new List<SquadWarParticipantState>(squadData.Participants);
			list.Sort(new Comparison<SquadWarParticipantState>(this.SortParticipantsAsc));
			string upgradeGroup = (squadData.Faction != FactionType.Empire) ? this.rebelHQId : this.empireHQId;
			bool isEmpire = squadData.Faction == FactionType.Empire;
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				BuildingTypeVO byLevel = catalog.GetByLevel(upgradeGroup, list[i].HQLevel);
				if (byLevel != null)
				{
					this.AddBuildingForParticipant(list[i], byLevel, isForPlayerSquad, isEmpire, i, assetManager, warBuildingLocators[i]);
				}
				i++;
			}
		}

		private void AddBuildingForParticipant(SquadWarParticipantState participantState, BuildingTypeVO buildingVO, bool isForPlayerSquad, bool isEmpire, int index, AssetManager assetManager, Transform locationTransform)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "WarBoardBuilding_" + participantState.SquadMemberName;
			SquadWarBoardBuilding key = new SquadWarBoardBuilding(participantState, gameObject, isEmpire);
			if (isForPlayerSquad)
			{
				this.participantBuildingsPlayer.Add(key, participantState);
			}
			else
			{
				this.participantBuildingsOpponent.Add(key, participantState);
			}
			this.LoadAsset(gameObject, locationTransform, buildingVO, assetManager);
		}

		public void ShowWarBuildings(SquadWarSquadType squadType, bool deselectSelectedBuilding)
		{
			bool flag = squadType == SquadWarSquadType.PLAYER_SQUAD;
			Dictionary<SquadWarBoardBuilding, SquadWarParticipantState> dictionary = (!flag) ? this.participantBuildingsOpponent : this.participantBuildingsPlayer;
			if (dictionary != null)
			{
				foreach (KeyValuePair<SquadWarBoardBuilding, SquadWarParticipantState> current in this.participantBuildingsPlayer)
				{
					current.Key.ToggleVisibility(flag);
				}
				foreach (KeyValuePair<SquadWarBoardBuilding, SquadWarParticipantState> current2 in this.participantBuildingsOpponent)
				{
					current2.Key.ToggleVisibility(!flag);
				}
				this.currentSquadType = squadType;
				if (deselectSelectedBuilding)
				{
					this.DeselectSelectedBuilding();
				}
			}
		}

		public void UpdateVisiblity()
		{
			bool flag = this.currentSquadType == SquadWarSquadType.PLAYER_SQUAD;
			Dictionary<SquadWarBoardBuilding, SquadWarParticipantState> dictionary = (!flag) ? this.participantBuildingsOpponent : this.participantBuildingsPlayer;
			foreach (KeyValuePair<SquadWarBoardBuilding, SquadWarParticipantState> current in dictionary)
			{
				if (current.Key != null && current.Key.PlayerInfo != null)
				{
					current.Key.PlayerInfo.UpdateVisibility();
				}
			}
		}

		private void LoadAsset(GameObject parentObject, Transform locator, BuildingTypeVO buildingVO, AssetManager assetManager)
		{
			Transform transform = parentObject.transform;
			transform.parent = locator;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			BoxCollider boxCollider = parentObject.AddComponent<BoxCollider>();
			float x = Units.BoardToWorldZ(buildingVO.SizeY);
			float num = Units.BoardToWorldX(buildingVO.SizeX);
			boxCollider.size = new Vector3(x, num, num);
			AssetHandle item = AssetHandle.Invalid;
			assetManager.Load(ref item, buildingVO.AssetName, new AssetSuccessDelegate(this.OnAssetLoaded), null, transform);
			this.assetHandles.Add(item);
		}

		private void OnAssetLoaded(object asset, object cookie)
		{
			GameObject gameObject = (GameObject)asset;
			Transform parent = (Transform)cookie;
			Transform transform = gameObject.transform;
			transform.parent = parent;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.Euler(0f, -135f, 0f);
			transform.localScale = Vector3.one;
			this.CreateNonSharedMaterials(gameObject);
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				Renderer renderer = componentsInChildren[i];
				if (renderer.sharedMaterial != null)
				{
					Shader shader = Service.AssetManager.Shaders.GetShader("PL_2Color_Mask_HoloBldg");
					renderer.sharedMaterial.shader = shader;
				}
				i++;
			}
			AssetMeshDataMonoBehaviour component = gameObject.GetComponent<AssetMeshDataMonoBehaviour>();
			if (component != null)
			{
				component.ShadowGameObject.SetActive(false);
			}
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		private void CreateNonSharedMaterials(GameObject assetGameObject)
		{
			MeshRenderer[] componentsInChildren = assetGameObject.GetComponentsInChildren<MeshRenderer>(true);
			SkinnedMeshRenderer[] componentsInChildren2 = assetGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			Dictionary<Material, Material> dictionary = new Dictionary<Material, Material>();
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				if (dictionary.ContainsKey(componentsInChildren[i].sharedMaterial))
				{
					componentsInChildren[i].sharedMaterial = dictionary[componentsInChildren[i].sharedMaterial];
				}
				else
				{
					Material sharedMaterial = componentsInChildren[i].sharedMaterial;
					Material material = UnityUtils.CreateMaterial(sharedMaterial.shader);
					material.mainTexture = sharedMaterial.mainTexture;
					material.mainTextureOffset = sharedMaterial.mainTextureOffset;
					material.mainTextureScale = sharedMaterial.mainTextureScale;
					dictionary.Add(componentsInChildren[i].sharedMaterial, material);
					componentsInChildren[i].material = material;
					this.unsharedMaterials.Add(material);
				}
				i++;
			}
			int j = 0;
			int num2 = componentsInChildren2.Length;
			while (j < num2)
			{
				if (dictionary.ContainsKey(componentsInChildren2[j].sharedMaterial))
				{
					componentsInChildren2[j].sharedMaterial = dictionary[componentsInChildren2[j].sharedMaterial];
				}
				else
				{
					Material material2 = UnityUtils.CreateMaterial(componentsInChildren2[j].sharedMaterial.shader);
					material2.CopyPropertiesFromMaterial(componentsInChildren2[j].sharedMaterial);
					dictionary.Add(componentsInChildren2[j].sharedMaterial, material2);
					componentsInChildren2[j].material = material2;
					this.unsharedMaterials.Add(material2);
				}
				j++;
			}
			dictionary.Clear();
		}

		private int SortParticipantsAsc(SquadWarParticipantState a, SquadWarParticipantState b)
		{
			return a.HQLevel - b.HQLevel;
		}

		private int SortBuffBases(SquadWarBuffBaseData a, SquadWarBuffBaseData b)
		{
			return a.BaseLevel - b.BaseLevel;
		}

		private void CacheHQBuildingIds()
		{
			if (this.HaveCachedBuildingIds())
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (BuildingTypeVO current in staticDataController.GetAll<BuildingTypeVO>())
			{
				if (this.HaveCachedBuildingIds())
				{
					break;
				}
				if (current.Type == BuildingType.HQ && current.BuildingRequirement != null)
				{
					if (current.Faction == FactionType.Empire)
					{
						this.empireHQId = current.BuildingID;
					}
					else if (current.Faction == FactionType.Rebel)
					{
						this.rebelHQId = current.BuildingID;
					}
					else if (current.Faction == FactionType.Smuggler)
					{
						this.smugglerHQId = current.BuildingID;
					}
				}
			}
		}

		private bool HaveCachedBuildingIds()
		{
			return this.empireHQId != null && this.rebelHQId != null && this.smugglerHQId != null;
		}

		private void DeselectSelectedBuilding()
		{
			GameObject cookie = this.selectedBuilding;
			this.selectedBuilding = null;
			this.outline.RemoveOutline();
			Service.EventManager.SendEvent(EventId.WarBoardBuildingDeselected, cookie);
		}

		private bool ShouldParticipantBuildingBeDisabled(SquadWarParticipantState data)
		{
			return data.TurnsLeft <= 0 && data.VictoryPointsLeft <= 0;
		}

		public void SelectBuilding(GameObject building)
		{
			Quaternion rotationTarget = Service.WarBoardViewController.GetRotationTarget(building.transform);
			Quaternion planetRotation = Service.WarBoardViewController.GetPlanetRotation();
			float num = rotationTarget.eulerAngles.x - planetRotation.eulerAngles.x;
			if (num < -7.7f || num > 1.9f)
			{
				return;
			}
			SquadWarParticipantState participantState = this.GetParticipantState(building);
			if (participantState == null)
			{
				return;
			}
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarStatusType currentStatus = warManager.GetCurrentStatus();
			if (currentStatus == SquadWarStatusType.PhaseCooldown)
			{
				return;
			}
			this.selectedBuilding = building;
			Transform transform = building.transform;
			if (transform.childCount > 0)
			{
				GameObject gameObject = transform.GetChild(0).gameObject;
				this.outline.Init(gameObject, "PL_2Color_Mask_HoloBldg_Outline");
			}
			if (participantState != null)
			{
				Service.EventManager.SendEvent(EventId.WarBoardParticipantBuildingSelected, building);
			}
		}

		private void OnBuildingPressedTimer(uint id, object cookie)
		{
			if (id == this.pressTimerId && this.pressedBuilding != null)
			{
				if (this.pressedBuilding != this.selectedBuilding)
				{
					this.DeselectSelectedBuilding();
					this.SelectBuilding(this.pressedBuilding);
				}
				this.pressTimerId = 0u;
				this.pressedBuilding = null;
			}
		}

		private void OnBuildingReleased()
		{
			if (this.pressedBuilding == null)
			{
				this.DeselectSelectedBuilding();
			}
			else
			{
				if (this.selectedBuilding != null && this.pressedBuilding != this.selectedBuilding)
				{
					this.DeselectSelectedBuilding();
				}
				this.SelectBuilding(this.pressedBuilding);
			}
			this.ResetPressedBuilding();
		}

		private void ResetPressedBuilding()
		{
			if (this.pressTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.pressTimerId);
				this.pressTimerId = 0u;
			}
			this.pressedBuilding = null;
		}

		public EatResponse OnPress(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			if (id != 0)
			{
				return EatResponse.NotEaten;
			}
			if (target != null)
			{
				this.pressedBuilding = target;
				this.pressTimerId = Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.OnBuildingPressedTimer), null);
			}
			this.pressScreenPosition = screenPosition;
			this.dragged = false;
			return EatResponse.NotEaten;
		}

		public EatResponse OnDrag(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			if (id != 0)
			{
				this.ResetPressedBuilding();
				return EatResponse.NotEaten;
			}
			if (!this.dragged && CameraUtils.HasDragged(screenPosition, this.pressScreenPosition))
			{
				this.dragged = true;
				this.ResetPressedBuilding();
			}
			return EatResponse.NotEaten;
		}

		public EatResponse OnRelease(int id)
		{
			if (id != 0)
			{
				return EatResponse.NotEaten;
			}
			if (!this.dragged)
			{
				this.OnBuildingReleased();
			}
			return EatResponse.NotEaten;
		}

		public EatResponse OnScroll(float delta, Vector2 screenPosition)
		{
			return EatResponse.NotEaten;
		}

		public void OnViewFrameTime(float dt)
		{
			this.UpdateVisiblity();
		}
	}
}
