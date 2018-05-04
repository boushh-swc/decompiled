using StaRTS.Main.Models;
using StaRTS.Utils.Pooling;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StaRTS.Utils.MeshCombiner
{
	public class MeshCombiner
	{
		private HashSet<Renderer> renderers;

		private GameObject containerObject;

		private GameObjectPool meshCombinerGameObjectPool;

		private bool isDisabled;

		[CompilerGenerated]
		private static GameObjectPool.CreatePoolObjectDelegate <>f__mg$cache0;

		[CompilerGenerated]
		private static GameObjectPool.DestroyPoolObjectDelegate <>f__mg$cache1;

		[CompilerGenerated]
		private static GameObjectPool.ActivatePoolObjectDelegate <>f__mg$cache2;

		[CompilerGenerated]
		private static GameObjectPool.DeactivatePoolObjectDelegate <>f__mg$cache3;

		public MeshCombiner(GameObjectPool meshCombinerGameObjectPool, GameObject parentObject, string containerObjectName)
		{
			this.isDisabled = GameConstants.MESH_COMBINE_DISABLED;
			this.meshCombinerGameObjectPool = meshCombinerGameObjectPool;
			if (parentObject != null)
			{
				this.containerObject = UnityUtils.CreateChildGameObject(containerObjectName, parentObject);
			}
			else
			{
				this.containerObject = new GameObject(containerObjectName);
			}
		}

		private static GameObject CreateMeshCombinerGameObject(GameObjectPool objectPool)
		{
			GameObject gameObject = new GameObject();
			MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
			Mesh sharedMesh = UnityUtils.CreateMesh();
			meshFilter.sharedMesh = sharedMesh;
			return gameObject;
		}

		private static void DestroyMeshCombinerGameObject(GameObject gameObject)
		{
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			UnityUtils.DestroyMesh(component.sharedMesh);
			UnityEngine.Object.Destroy(gameObject);
		}

		private static void ActivateMeshCombinerGameObject(GameObject gameObject)
		{
			gameObject.SetActive(true);
		}

		private static void DeactivateMeshCombinerGameObject(GameObject gameObject)
		{
			gameObject.GetComponent<MeshFilter>().sharedMesh.Clear(false);
			gameObject.name = "DeactivatedMeshCombinerGameObject";
			gameObject.GetComponent<Renderer>().sharedMaterial = null;
			gameObject.SetActive(false);
		}

		public static GameObjectPool CreateMeshCombinerObjectPool()
		{
			if (MeshCombiner.<>f__mg$cache0 == null)
			{
				MeshCombiner.<>f__mg$cache0 = new GameObjectPool.CreatePoolObjectDelegate(MeshCombiner.CreateMeshCombinerGameObject);
			}
			GameObjectPool.CreatePoolObjectDelegate arg_74_0 = MeshCombiner.<>f__mg$cache0;
			if (MeshCombiner.<>f__mg$cache1 == null)
			{
				MeshCombiner.<>f__mg$cache1 = new GameObjectPool.DestroyPoolObjectDelegate(MeshCombiner.DestroyMeshCombinerGameObject);
			}
			GameObjectPool.DestroyPoolObjectDelegate arg_74_1 = MeshCombiner.<>f__mg$cache1;
			if (MeshCombiner.<>f__mg$cache2 == null)
			{
				MeshCombiner.<>f__mg$cache2 = new GameObjectPool.ActivatePoolObjectDelegate(MeshCombiner.ActivateMeshCombinerGameObject);
			}
			GameObjectPool.ActivatePoolObjectDelegate arg_74_2 = MeshCombiner.<>f__mg$cache2;
			if (MeshCombiner.<>f__mg$cache3 == null)
			{
				MeshCombiner.<>f__mg$cache3 = new GameObjectPool.DeactivatePoolObjectDelegate(MeshCombiner.DeactivateMeshCombinerGameObject);
			}
			return new GameObjectPool(arg_74_0, arg_74_1, arg_74_2, MeshCombiner.<>f__mg$cache3);
		}

		public void CombineMeshes(HashSet<Renderer> combineRenderers)
		{
			if (this.isDisabled)
			{
				return;
			}
			if (combineRenderers == null)
			{
				throw new NullReferenceException("Rederers cannot be null.");
			}
			if (this.IsMeshCombined())
			{
				this.UncombineMesh();
			}
			this.renderers = combineRenderers;
			this.CombineMeshes();
		}

		private void CombineMeshes()
		{
			List<Renderer> list = null;
			Dictionary<Material, List<Renderer>> dictionary = new Dictionary<Material, List<Renderer>>();
			foreach (Renderer current in this.renderers)
			{
				if (current.enabled)
				{
					Material[] sharedMaterials = current.sharedMaterials;
					for (int i = 0; i < sharedMaterials.Length; i++)
					{
						Material material = sharedMaterials[i];
						if (material != null)
						{
							List<Renderer> list2;
							if (!dictionary.TryGetValue(material, out list2))
							{
								list2 = new List<Renderer>();
								dictionary[material] = list2;
							}
							list2.Add(current);
						}
					}
					current.enabled = false;
				}
				else
				{
					if (list == null)
					{
						list = new List<Renderer>();
					}
					list.Add(current);
				}
			}
			if (list != null)
			{
				int count = list.Count;
				for (int j = 0; j < count; j++)
				{
					this.renderers.Remove(list[j]);
				}
				list.Clear();
				list = null;
			}
			foreach (KeyValuePair<Material, List<Renderer>> current2 in dictionary)
			{
				GameObject fromPool = this.meshCombinerGameObjectPool.GetFromPool();
				UnityUtils.ChangeGameObjectParent(fromPool, this.containerObject);
				fromPool.name = current2.Key.name;
				MeshFilter component = fromPool.GetComponent<MeshFilter>();
				fromPool.GetComponent<Renderer>().sharedMaterial = current2.Key;
				List<Renderer> value = current2.Value;
				int count2 = value.Count;
				CombineInstance[] array = new CombineInstance[count2];
				for (int k = 0; k < count2; k++)
				{
					MeshFilter component2 = value[k].GetComponent<MeshFilter>();
					UnityUtils.SetCombineInstance(ref array[k], component2.sharedMesh, component2.transform.localToWorldMatrix);
				}
				component.sharedMesh.CombineMeshes(array, true, true);
			}
		}

		public void UncombineMesh()
		{
			if (this.isDisabled)
			{
				return;
			}
			if (this.IsMeshCombined())
			{
				GameObject[] children = UnityUtils.GetChildren(this.containerObject);
				int i = 0;
				int num = children.Length;
				while (i < num)
				{
					children[i].transform.parent = null;
					this.meshCombinerGameObjectPool.ReturnToPool(children[i]);
					i++;
				}
				foreach (Renderer current in this.renderers)
				{
					if (current != null)
					{
						current.enabled = true;
					}
				}
				this.renderers = null;
			}
		}

		private bool IsMeshCombined()
		{
			return this.renderers != null;
		}

		public bool IsRendererCombined(Renderer renderer)
		{
			return this.IsMeshCombined() && this.renderers.Contains(renderer);
		}
	}
}
