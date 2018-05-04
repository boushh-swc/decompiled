using StaRTS.Assets;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Utils.MetaData
{
	public class Catalog
	{
		public delegate void CatalogDelegate(bool success, string file);

		private Dictionary<string, Sheet> sheets;

		private Dictionary<string, AssetHandle> assetHandles;

		private List<string> pendingPatches;

		private Dictionary<string, KeyValuePair<JoeFile, Catalog.CatalogDelegate>> pendingPatchesData;

		public Catalog()
		{
			this.sheets = new Dictionary<string, Sheet>();
			this.assetHandles = new Dictionary<string, AssetHandle>();
			this.pendingPatches = new List<string>();
			this.pendingPatchesData = new Dictionary<string, KeyValuePair<JoeFile, Catalog.CatalogDelegate>>();
		}

		public Sheet GetSheet(string sheetName)
		{
			return (!this.sheets.ContainsKey(sheetName)) ? null : this.sheets[sheetName];
		}

		public void PatchData(string catalogFile, Catalog.CatalogDelegate completeCallback)
		{
			catalogFile = catalogFile.Replace(".json", ".json.joe");
			int num = catalogFile.LastIndexOf("/");
			string assetName = catalogFile.Substring(num + 1);
			string assetPath = catalogFile.Substring(0, num);
			AssetManager assetManager = Service.AssetManager;
			assetManager.AddJoeFileToManifest(assetName, assetPath);
			assetManager.RegisterPreloadableAsset(assetName);
			object cookie = new KeyValuePair<string, Catalog.CatalogDelegate>(catalogFile, completeCallback);
			AssetHandle value = AssetHandle.Invalid;
			assetManager.Load(ref value, assetName, new AssetSuccessDelegate(this.AssetSuccess), new AssetFailureDelegate(this.AssetFailure), cookie);
			this.assetHandles.Add(catalogFile, value);
			this.pendingPatches.Add(catalogFile);
		}

		private void AssetSuccess(object asset, object cookie)
		{
			byte[] binaryContents = Service.AssetManager.GetBinaryContents(asset);
			JoeFile joe = new JoeFile(binaryContents);
			this.ProcessJoe(joe, cookie);
		}

		private void AssetFailure(object cookie)
		{
			this.ProcessJoe(null, cookie);
		}

		private void ProcessJoe(JoeFile joe, object cookie)
		{
			KeyValuePair<string, Catalog.CatalogDelegate> keyValuePair = (KeyValuePair<string, Catalog.CatalogDelegate>)cookie;
			string key = keyValuePair.Key;
			Catalog.CatalogDelegate value = keyValuePair.Value;
			Service.AssetManager.Unload(this.assetHandles[key]);
			this.assetHandles.Remove(key);
			this.ProcessPendingPatch(key, joe, value);
		}

		private void ProcessPendingPatch(string catalogFile, JoeFile joe, Catalog.CatalogDelegate completeCallback)
		{
			if (joe != null)
			{
				if (this.pendingPatches.Count > 0 && this.pendingPatches[0] != catalogFile)
				{
					this.pendingPatchesData[catalogFile] = new KeyValuePair<JoeFile, Catalog.CatalogDelegate>(joe, completeCallback);
					return;
				}
				this.ParseCatalog(joe);
			}
			this.pendingPatches.Remove(catalogFile);
			this.pendingPatchesData.Remove(catalogFile);
			if (completeCallback != null)
			{
				catalogFile = catalogFile.Replace(".json.joe", ".json");
				completeCallback(joe != null, catalogFile);
			}
			this.ProcessPendingPatches();
		}

		private void ProcessPendingPatches()
		{
			if (this.pendingPatches.Count > 0 && this.pendingPatchesData.ContainsKey(this.pendingPatches[0]))
			{
				string text = this.pendingPatches[0];
				JoeFile key = this.pendingPatchesData[text].Key;
				Catalog.CatalogDelegate value = this.pendingPatchesData[text].Value;
				this.ProcessPendingPatch(text, key, value);
			}
		}

		public void ParseCatalog(JoeFile joe)
		{
			Sheet[] allSheets = joe.GetAllSheets();
			if (allSheets == null)
			{
				return;
			}
			int i = 0;
			int num = allSheets.Length;
			while (i < num)
			{
				Sheet sheet = allSheets[i];
				string sheetName = sheet.SheetName;
				if (this.sheets.ContainsKey(sheetName))
				{
					this.sheets[sheetName].PatchRows(sheet);
				}
				else
				{
					this.sheets.Add(sheetName, sheet);
				}
				i++;
			}
		}
	}
}
