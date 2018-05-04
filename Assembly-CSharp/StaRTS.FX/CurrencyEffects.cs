using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.FX
{
	public class CurrencyEffects
	{
		public const string SETUP_TYPE_LOOTING = "setupTypeLooting";

		public const string SETUP_TYPE_COLLECTION = "setupTypeCollection";

		private const string FX_LOOT_CREDITS_ACTIVE = "effect24";

		private const string FX_LOOT_CREDITS_PASSIVE = "effect25";

		private const string FX_LOOT_MATERIALS_ACTIVE = "effect26";

		private const string FX_LOOT_MATERIALS_PASSIVE = "effect27";

		private const string FX_LOOT_CONTRABAND_ACTIVE = "fx_contralooting_active";

		private const string FX_LOOT_CONTRABAND_PASSIVE = "fx_contralooting_passive";

		private const string PARTICLE_SYSTEM_ICON = "icon";

		private const int NUM_ICONS_PER_HIT = 1;

		private const int MAX_NUM_ICONS_PER_EMITTER = 10;

		private GameObject[] loadedAssets;

		private Dictionary<Entity, CurrencyEffectData> effects;

		private StaticDataController sdc;

		private string currentSetupType;

		public CurrencyEffects()
		{
			Service.CurrencyEffects = this;
			this.sdc = Service.StaticDataController;
			this.effects = new Dictionary<Entity, CurrencyEffectData>();
		}

		public void InitializeEffects(string setupType)
		{
			int num = 6;
			this.loadedAssets = new GameObject[num];
			for (int i = 0; i < num; i++)
			{
				this.loadedAssets[i] = null;
			}
			List<string> list = new List<string>();
			List<object> list2 = new List<object>();
			List<AssetHandle> list3 = new List<AssetHandle>();
			if (setupType == "setupTypeCollection")
			{
				list.Add(this.GetAssetType("effect24").AssetName);
				list.Add(this.GetAssetType("effect26").AssetName);
				list.Add(this.GetAssetType("fx_contralooting_active").AssetName);
			}
			else
			{
				if (!(setupType == "setupTypeLooting"))
				{
					throw new Exception(setupType + " is not a valid setup type");
				}
				list.Add(this.GetAssetType("effect25").AssetName);
				list.Add(this.GetAssetType("effect27").AssetName);
				list.Add(this.GetAssetType("fx_contralooting_passive").AssetName);
			}
			list2.Add(CurrencyType.Credits);
			list2.Add(CurrencyType.Materials);
			list2.Add(CurrencyType.Contraband);
			list3.Add(AssetHandle.Invalid);
			list3.Add(AssetHandle.Invalid);
			list3.Add(AssetHandle.Invalid);
			Service.AssetManager.MultiLoad(list3, list, new AssetSuccessDelegate(this.OnEffectLoaded), null, list2, new AssetsCompleteDelegate(this.OnAllEffectsLoaded), setupType);
		}

		private void OnEffectLoaded(object asset, object cookie)
		{
			this.loadedAssets[(int)((CurrencyType)cookie)] = (asset as GameObject);
		}

		private void OnAllEffectsLoaded(object cookie)
		{
			this.currentSetupType = cookie.ToString();
			this.PlaceEffects();
		}

		public void PlaceEffects()
		{
			this.Cleanup();
			EntityController entityController = Service.EntityController;
			NodeList<LootNode> nodeList = entityController.GetNodeList<LootNode>();
			LootNode lootNode = nodeList.Head;
			while (lootNode != null)
			{
				BuildingTypeVO buildingType = lootNode.BuildingComp.BuildingType;
				if (this.currentSetupType == "setupTypeCollection")
				{
					if (buildingType.Type == BuildingType.Resource)
					{
						goto IL_7C;
					}
				}
				else if (!(this.currentSetupType == "setupTypeLooting") || buildingType.IsLootable)
				{
					goto IL_7C;
				}
				IL_114:
				lootNode = lootNode.Next;
				continue;
				IL_7C:
				Bounds gameObjectBounds = UnityUtils.GetGameObjectBounds(lootNode.View.MainGameObject);
				Vector3 position = lootNode.View.MainTransform.position;
				position.y = gameObjectBounds.center.y;
				CurrencyType currency = buildingType.Currency;
				if (buildingType.Type == BuildingType.HQ || currency == CurrencyType.None)
				{
					this.CreateEffect(lootNode.Entity, CurrencyType.Credits, position);
					this.CreateEffect(lootNode.Entity, CurrencyType.Materials, position);
					this.CreateEffect(lootNode.Entity, CurrencyType.Contraband, position);
					goto IL_114;
				}
				this.CreateEffect(lootNode.Entity, currency, position);
				goto IL_114;
			}
		}

		private void CreateEffect(Entity building, CurrencyType currencyType, Vector3 fxPosition)
		{
			if (this.loadedAssets[(int)currencyType] == null)
			{
				Service.Logger.Error("Cannot create effect for " + currencyType);
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.loadedAssets[(int)currencyType]);
			gameObject.transform.position = fxPosition;
			gameObject.SetActive(true);
			if (!this.effects.ContainsKey(building))
			{
				this.effects[building] = new CurrencyEffectData();
			}
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				ParticleSystem component = gameObject.transform.GetChild(i).GetComponent<ParticleSystem>();
				if (component.name == "icon")
				{
					component.main.maxParticles = 10;
				}
				this.effects[building].Add(currencyType, component);
			}
		}

		public void PlayEffect(Entity building, CurrencyType currency, int amountGained)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currency != CurrencyType.Credits)
			{
				if (currency != CurrencyType.Materials)
				{
					if (currency == CurrencyType.Contraband)
					{
						amountGained = currentPlayer.MaxContrabandAmount - currentPlayer.CurrentContrabandAmount - amountGained;
					}
				}
				else
				{
					amountGained = currentPlayer.MaxMaterialsAmount - currentPlayer.CurrentMaterialsAmount - amountGained;
				}
			}
			else
			{
				amountGained = currentPlayer.MaxCreditsAmount - currentPlayer.CurrentCreditsAmount - amountGained;
			}
			if (amountGained <= 0)
			{
				return;
			}
			if (this.effects.ContainsKey(building))
			{
				List<ParticleSystem> list = this.effects[building].Get(currency);
				if (list != null)
				{
					int i = 0;
					int count = list.Count;
					while (i < count)
					{
						if (list[i].name == "icon")
						{
							list[i].Emit(1);
						}
						else
						{
							list[i].Play();
						}
						i++;
					}
				}
			}
		}

		public void TransferEffects(Entity oldBuilding, Entity newBuilding)
		{
			if (this.effects != null && this.effects.ContainsKey(oldBuilding))
			{
				this.effects.Add(newBuilding, this.effects[oldBuilding]);
				this.effects.Remove(oldBuilding);
			}
		}

		public void Cleanup()
		{
			if (this.effects == null)
			{
				return;
			}
			foreach (KeyValuePair<Entity, CurrencyEffectData> current in this.effects)
			{
				current.Value.Destroy();
			}
			this.effects.Clear();
		}

		public List<IAssetVO> GetEffectAssetTypes(string setupType)
		{
			List<IAssetVO> list = new List<IAssetVO>();
			if (setupType == "setupTypeCollection")
			{
				list.Add(this.GetAssetType("effect24"));
				list.Add(this.GetAssetType("effect26"));
				list.Add(this.GetAssetType("fx_contralooting_active"));
			}
			else if (setupType == "setupTypeLooting")
			{
				list.Add(this.GetAssetType("effect25"));
				list.Add(this.GetAssetType("effect27"));
				list.Add(this.GetAssetType("fx_contralooting_passive"));
			}
			else
			{
				Service.Logger.WarnFormat("{0} is not a valid setup type.", new object[]
				{
					setupType
				});
			}
			return list;
		}

		private IAssetVO GetAssetType(string uid)
		{
			return this.sdc.Get<EffectsTypeVO>(uid);
		}
	}
}
