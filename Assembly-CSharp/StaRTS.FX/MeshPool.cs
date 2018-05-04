using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.FX
{
	public class MeshPool
	{
		private const int INITIAL_POOL_SIZE = 10;

		private GameObject meshProto;

		private Stack<GameObject> meshes;

		public MeshPool(GameObject meshSeed)
		{
			this.meshes = new Stack<GameObject>(10);
			this.meshProto = meshSeed;
		}

		private void InstantiateMeshes()
		{
			for (uint num = 0u; num < 10u; num += 1u)
			{
				GameObject mesh = UnityEngine.Object.Instantiate<GameObject>(this.meshProto);
				this.ReturnToPool(mesh);
			}
		}

		public GameObject GetMesh()
		{
			if (this.meshProto == null)
			{
				return null;
			}
			if (this.meshes.Count == 0)
			{
				this.InstantiateMeshes();
			}
			GameObject gameObject = this.meshes.Pop();
			gameObject.SetActive(true);
			return gameObject;
		}

		public void ReturnToPool(GameObject mesh)
		{
			mesh.SetActive(false);
			this.meshes.Push(mesh);
		}

		public void Destroy()
		{
			this.meshProto = null;
			while (this.meshes.Count > 0)
			{
				GameObject obj = this.meshes.Pop();
				UnityEngine.Object.Destroy(obj);
			}
			this.meshes.Clear();
		}
	}
}
